namespace Final_Project;

public class LevelEditor : SceneWithBoard
{
	//level creation tool
	//parameters
	private int boardWidth, boardHeight;
	private BoardState boardState;
	private int gameCursorX, gameCursorY;
	private bool editing = true; //will be set to false when finished editing

	public LevelEditor(int width, int height)
	{
		//initialize input
		InputHandler.scene = this;
		
		//initialize parameters
		boardWidth = width;
		boardHeight = height;
		boardState = new BoardState(width, height, new int[height][], new int[width][]);

		//initialize graphic
		Drawing.Initialize(boardState);
		Drawing.Draw(boardState);
		Drawing.UpdateCursor(gameCursorX,gameCursorY);
		
		//take user input
		while (true)
		{
			InputHandler.Input(Console.ReadKey(true));
		}
		
		//save level or give up
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

	public override void MoveCursor(Direction direction)
	{
		switch (direction)
		{
			case Direction.Up:
				if (gameCursorY > 0) gameCursorY--;
				break;
			case Direction.Down:
				if (gameCursorY < boardHeight - 1) gameCursorY++;
				break;
			case Direction.Left:
				if (gameCursorX > 0) gameCursorX--;
				break;
			case Direction.Right:
				if (gameCursorX < boardWidth - 1) gameCursorX++;
				break;
		}
		
		Drawing.UpdateCursor(gameCursorX,gameCursorY);
	}


	public override void Action1()
	{
		
	}

	public override void Action2()
	{
		
	}

	public override void ActionShift1()
	{
		
	}

	public override void ActionShift2()
	{
		
	}

	public override void AnyKeyButArrow()
	{
		
	}
}