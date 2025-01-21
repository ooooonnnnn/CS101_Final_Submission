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
		message = $"({InputHandler.MarkDot}): check if solvable. ";
		//updates everything on screen 
		Drawing.Draw(boardState);
		Drawing.UpdateCursor(SceneWithBoard.gameCursorX, SceneWithBoard.gameCursorY);
		Console.CursorVisible = true;
		Drawing.UpdateMessage(message,msgLeft,msgTop);
	}

	public void SaveLevel(string levelName)
	{
		//saves the boardstate into a new file called levelname
		StreamWriter sw = File.CreateText($"{levelName}.txt");
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
			do
			{
				levelName = Console.ReadLine();
			} while (levelName.Length == 0);
			SaveLevel(levelName);
			//finish scene
			sceneFinished = true;
		}
	}

	
	CellState[,] attemptSolution; // used in CheckSolution and SolveLine
	protected override bool CheckSolution()
	{
		bool solvable;
		
		//create clues
		solution = boardState.Cells;
		CalculateClues();
		
		//solve line by line until done
		attemptSolution = new CellState[boardHeight, boardWidth];
		bool changed = false;
		while (!changed)
		{
			changed = false;
			//solve rows
			for (int i = 0; i < boardHeight; i++)
			{
				CellState[] newLineSolution = SolveLine(i, 1);
				//check if the solution step changed anything
				for (int j = 0; j < boardWidth; j++)
				{
					if (attemptSolution[i,j] != newLineSolution[j])
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
					if (attemptSolution[j, i] != newLineSolution[j])
					{
						attemptSolution[j, i] = newLineSolution[j];
						changed = true;
					}
				}
			}
		}
		
		//check if the solution is identical to the board state
		solvable = attemptSolution == boardState.Cells;
		
		if (!solvable)
		{
			Drawing.UpdateMessage(message + "Not Solvable.", msgLeft, msgTop);
		}
		else
		{
			Drawing.UpdateMessage($"Solvable. Press ({InputHandler.MarkDot}) to finish editing. ", msgLeft, msgTop);
		}
		return solvable;
	}

	private CellState[] SolveLine(int lineIndex, int dimension)
	{
		//given the current state of the solution, and a row or column defined by lineIndex and dimension,
		//return the best solution for this line. for example, if the clue is {3}, and the current solution is
		//{|  |  |  |&&|  |}, it will return {|``|  |&&|&&|  |}.
		//works by intersecting all possible solutions for this line with both the clue and the current state of the solution

		//initialization
		int lineLength = boardState.Cells.GetLength(dimension);
		CellState[] currentLineSolution = new CellState[lineLength];
		CellState[] newLineSolution = new CellState[lineLength];
		for (int i = 0; i < lineLength; i++)
		{
			currentLineSolution[i] = dimension == 0 ? attemptSolution[i, lineIndex] : attemptSolution[lineIndex, i];
		}
		int[] clue = dimension == 0 ? columnClues[lineIndex] : rowClues[lineIndex];
		//return trivial solution
		if (clue.Length == 0)
		{
			for (int i = 0; i < newLineSolution.Length; i++)
			{
				newLineSolution[i] = CellState.Dot;
			}

			return newLineSolution;
		}
		
		//all possible solutions for the clue
		List<CellState[]> solutions = new List<CellState[]>();
		int[] spaces = new int[clue.Length]; // the number of spaces before each run of blacks in the solution
		bool moreSolutions;
		do
		{
			//the solution defined by the current state of spaces is valid
			CellState[] solutionToAdd = new CellState[lineLength];
			int index = 0;
			for (int i = 0; i < spaces.Length; i++)
			{
				for (int j = 0; j < spaces[i]; j++)
				{
					solutionToAdd[index + j] = CellState.Unknown;
				}
				index += spaces[i];

				for (int j = 0; j < clue[i]; j++)
				{
					solutionToAdd[index + j] = CellState.Black;
				}
				index += clue[i];
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
					nextSpaces[i] = 0;
				}
			}
		} while (moreSolutions);
		
		//remove solutions that don't fit the current state of the solution
		solutions.RemoveAll(sol => sol.Zip(currentLineSolution).Any(pairCells =>
			(pairCells.First != CellState.Black) && (pairCells.Second == CellState.Black)));
		
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

		CellState[] seed = new CellState[lineLength];//used as a seed for the intersection aggergate
		for (int i = 0; i < lineLength; i++) seed[i] = CellState.Black;
		CellState[] blacks = solutions.Aggregate(seed, IntersectBlack);
		for (int i = 0; i < lineLength; i++) seed[i] = CellState.Dot;
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