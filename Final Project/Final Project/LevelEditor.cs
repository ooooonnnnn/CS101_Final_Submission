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
		//updates everything on screen 
		Drawing.Draw(boardState);
		Drawing.UpdateCursor(SceneWithBoard.gameCursorX, SceneWithBoard.gameCursorY);
		Console.CursorVisible = true;
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

	protected override bool CheckSolution()
	{
		//throw new NotImplementedException();
		return false;
	}
}