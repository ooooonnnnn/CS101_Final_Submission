namespace Final_Project;

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
				char charToWrite = boardState.Cells[j, i] == CellState.Black ? '1' : '0';
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

	protected override bool CheckSolution()
	{
		bool solvable = true;
		
		//create clues
		//solve line by line until done
		//check if the solution is identical to the board state
		
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
	
	
}