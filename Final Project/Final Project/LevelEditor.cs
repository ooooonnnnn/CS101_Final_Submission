namespace Final_Project;
using System.Linq;

public class LevelEditor : SceneWithBoard
{
	//level creation tool
	
	protected override void Initialize()
	{
		boardState = new BoardState(boardWidth, boardHeight);
		//screen initialization
		Drawing.Initialize(boardState);
		baseMessage = $"({InputHandler.MarkDot}): check if solvable. ";
		currentMessage = baseMessage;
		//updates everything on screen 
		Drawing.Draw(boardState);
		gameCursorX = 0;
		gameCursorY = 0;
		Drawing.UpdateCursor(gameCursorX, gameCursorY);
		Console.CursorVisible = true;
		Drawing.UpdateMessage(baseMessage,msgLeft,msgTop);
	}

	private bool SaveLevel(string levelName)
	{
		//saves the boardstate into a new file called levelname
		if (levelName.Length == 0) return false;
		StreamWriter sw = null;
		try
		{
			sw = File.CreateText($"{levelName}.txt");
		}
		catch (Exception e)
		{
			if (sw != null)
			{
				sw.Close();
			}
			return false;
		}
		
		for (int i = 0; i < boardHeight; i++)
		{
			for (int j = 0; j < boardWidth; j++)
			{
				char charToWrite = boardState.Cells[i, j] == CellState.Black ? '1' : '0';
				sw.Write(charToWrite);
			}

			//for the last line, skip writing the '\n'
			if (i == boardHeight - 1)
			{
				break;
			}

			sw.Write("\n");
		}

		sw.Close();

		return true;
	}

	public override void Action2()
	{
		AnyKeyButArrow();//this will turn off soft marking
		
		if (!isSolved)
		{
			isSolved = CheckSolution();
		}
		else
		{
			//take user input for level name and save it
			Console.Clear();
			Console.Write("New level name: ");
			string? levelName;
			while(true)
			{
				levelName = Console.ReadLine();
				if (SaveLevel(levelName))
				{
					break;
				}
			}
			//finish scene
			sceneFinished = true;
		}
	}

	protected override void EditorBoardChangeNotice()
	{
		isSolved = false;
		Drawing.UpdateMessage(currentMessage, msgLeft, msgTop);
	}

	CellState[,] attemptSolution; // used in CheckSolution and SolveLine
	protected override bool CheckSolution()
	{
		//check whether the user provided drawing is solvable given the clues that define it
		//create clues
		solution = boardState.Cells;
		CalculateClues();
		
		//solve line by line until done
		attemptSolution = new CellState[boardHeight, boardWidth];
		bool changed = true;
		do
		{
			changed = false;
			//solve rows
			for (int i = 0; i < boardHeight; i++)
			{
				CellState[] newLineSolution = SolveLine(i, 1);
				//check if the solution step changed anything
				for (int j = 0; j < boardWidth; j++)
				{
					if (attemptSolution[i, j] != newLineSolution[j] && newLineSolution[j] != CellState.Unknown)
					{
						attemptSolution[i, j] = newLineSolution[j];
						changed = true;
					}
				}
			}

			//solve columns
			for (int i = 0; i < boardWidth; i++)
			{
				CellState[] newLineSolution = SolveLine(i, 0);
				//check if the solution step changed anything
				for (int j = 0; j < boardHeight; j++)
				{
					if (attemptSolution[j, i] != newLineSolution[j] && newLineSolution[j] != CellState.Unknown)
					{
						attemptSolution[j, i] = newLineSolution[j];
						changed = true;
					}
				}
			}
		} while (changed);
		
		//check if the solution is identical to the board state
		bool solvable = CellStateUtilities.BlacksMatch(attemptSolution,boardState.Cells);
		UpdateMessageSolvable(solvable);
		
		return solvable;
	}

	private void UpdateMessageSolvable(bool solvable)
	{
		//update message
		if (!solvable)
		{
			Drawing.UpdateMessage(baseMessage + "Not Solvable.", msgLeft, msgTop);
		}
		else
		{
			Drawing.UpdateMessage($"Solvable. Press ({InputHandler.MarkDot}) to finish editing. ", msgLeft, msgTop);
		}
	}

	private CellState[] SolveLine(int lineIndex, int dimension)
	{
		//given the current state of the solution, and a row or column defined by lineIndex and dimension,
		//return the best solution for this line. for example, if the clue is {3}, and the current solution is
		//{|  |  |  |&&|  |}, it will return {|``|  |&&|&&|  |}.
		//works by intersecting all possible solutions for this line with both the clue and the current state of the solution

		//------------initialization
		int lineLength = boardState.Cells.GetLength(dimension);
		CellState[] currentLineSolution = new CellState[lineLength];
		for (int i = 0; i < lineLength; i++)
		{
			currentLineSolution[i] = dimension == 0 ? attemptSolution[i, lineIndex] : attemptSolution[lineIndex, i];
		}
		CellState[] newLineSolution = new CellState[lineLength];
		int[] clue = dimension == 0 ? columnClues[lineIndex] : rowClues[lineIndex];
		
		//------------return trivial solution
		if (clue.Length == 0)
		{
			return newLineSolution.Select(_ => CellState.Dot).ToArray();
		}
		
		//------------all possible solutions for the clue
		HashSet<CellState[]> solutions = new HashSet<CellState[]>();
		int[] spaces = (new int[clue.Length]).Select((_, i) => i > 0 ? 1 : 0).ToArray(); // the number of spaces before each run of blacks in the solution
		bool moreSolutions;
		do
		{
			//the solution defined by the current state of spaces is valid
			//construct the solution from the spaces
			CellState[] solutionToAdd = (new CellState[lineLength]).Select(_ => CellState.Dot).ToArray();
			int index = 0;
			for (int i = 0; i < spaces.Length; i++)
			{
				index += spaces[i];

				for (int j = 0; j < clue[clue.Length - 1 - i]; j++)
				{
					solutionToAdd[index + j] = CellState.Black;
				}
				index += clue[clue.Length - 1 - i];
			}
			solutions.Add(solutionToAdd);
			
			//try finding another set of spaces
			moreSolutions = false;
			int[] nextSpaces = new int[spaces.Length];
			spaces.CopyTo(nextSpaces,0);
			for (int i = spaces.Length - 1; i >= 0; i--)
			{
				nextSpaces[i]++;
				if (nextSpaces.Sum() + clue.Sum() <= lineLength)
				{
					moreSolutions = true;
					nextSpaces.CopyTo(spaces, 0);
					break;
				}
				else
				{
					nextSpaces[i] = i > 0 ? 1 : 0;
				}
			}
		} while (moreSolutions);
		
		//remove solutions that don't fit the current state of the solution
		solutions.RemoveWhere(sol => sol.Zip(currentLineSolution).Any(pairCells =>
			CellStateUtilities.UnequalAndNotUnkown(pairCells.First, pairCells.Second)));
		
		//intersect all remaining solutions to find certain black cells and certain dot cells
		CellState[] IntersectBlack(CellState[] a,CellState[] b)
		{
			return IntersectGeneral(a, b, CellState.Black);
		}
		CellState[] IntersectDot(CellState[] a, CellState[] b)
		{
			return IntersectGeneral(a, b, CellState.Dot);
		}
		CellState[] IntersectGeneral(CellState[] a, CellState[] b, CellState target)
		{
			//returns a CellState[] where each entry equals target if it also equals target in both a and b
			int length = a.Length;
			CellState[] result = new CellState[length];
			for (int i = 0; i < length; i++)
			{
				if (a[i] == target && b[i] == target)
				{
					result[i] = target;
				}
			}
			return result;
		}

		CellState[] seed = (new CellState[lineLength]).Select(_ => CellState.Black).ToArray();//used as a seed for the intersection aggergate
		CellState[] blacks = solutions.Aggregate(seed, IntersectBlack);
		seed = (new CellState[lineLength]).Select(_ => CellState.Dot).ToArray();
		CellState[] dots = solutions.Aggregate(seed, IntersectDot);
		for (int i = 0; i < lineLength; i++)
		{
			if (blacks[i] == CellState.Black)
			{
				newLineSolution[i] = CellState.Black;
			}

			if (dots[i] == CellState.Dot)
			{
				newLineSolution[i] = CellState.Dot;
			}
		}
		
		return newLineSolution;
	}
	
	
}