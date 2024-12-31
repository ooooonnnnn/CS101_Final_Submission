namespace Final_Project;

public class LevelEditor : SceneWithBoard
{
	//level creation tool
	//parameters
	private bool editing = true; //will be set to false when finished editing
	
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

	public void Start()
	{
		//TEST
		int[,] test = { { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 0, 0, 0, 0, 0 } };
		StreamWriter sw = File.CreateText("testfile.txt");
		for (int i = 0; i < test.GetLength(0); i++)
		{
			for (int j = 0; j < test.GetLength(1); j++)
			{
				sw.Write(test[i, j].ToString());
			}

			if (i == test.GetLength(0) - 1)
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
		CheckSolution();
	}

	protected override bool CheckSolution()
	{
		bool solvable = false;
		/*
			solving logic here
		*/
		if (solvable)
		{
			sceneFinished = true;
		}
		return false;
	}
}