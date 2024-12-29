namespace Final_Project;

public abstract class SceneWithBoard : Scene
{
	//base class for game and editor scenes
	
	//Game parameters
	public int BoardWidth = 5, BoardHeight = 5; //must always be a multiple of 5 to keep the game readable, board can be rectangular. must be at least 5
	protected int[][] RowClues, ColumnClues; //These are meant for board initialization only
	protected CellState[,] Solution;
	
	//Cursor position when ingame
	public static int GameCursorX = 0, GameCursorY = 0; //first cell is 0
	
	public static BoardState boardState;
	
	//Soft marking
	protected bool SoftMarking = false;
	//only cells with the first state are changed into the second state
	protected (CellState,CellState) SoftMarkingMode = (CellState.Unknown,CellState.Unknown);
	
	//Flag for ending the game 
	protected bool isSolved = false;

	public void RunBoardScene()
	{
		//screen initialization
		Drawing.Initialize(boardState);
		//updates everything on screen 
		Drawing.Draw(boardState);
		Drawing.UpdateCursor(GameCursorX, GameCursorY);
		
		// Game loop
		while (true)
		{
			//take user input
			InputHandler.Input(Console.ReadKey(true));
			//solution is checked automatically from UpdateCell
			if (isSolved)
				return;
		}
		
	}

	protected abstract bool CheckSolution();
	
	protected void UpdateCell(CellState inputState)
	{
		/*sets the highlighted state according to its' current state and input state
		 if they are the same, the new state is unknown,
		 else, the new state is the input state
		 */
		//soft mark only updates cells with SoftMarkingMode.Item1
		if (SoftMarking && boardState.Cells[GameCursorX,GameCursorY] != SoftMarkingMode.Item1)
		{
			return;
		}
	    
		CellState current = boardState.Cells[GameCursorX, GameCursorY];
		boardState.Cells[GameCursorX, GameCursorY] = current == inputState ? CellState.Unknown : inputState;
		Drawing.UpdateBoardCell(boardState.Cells[GameCursorX,GameCursorY]);
	    
		//call for some function (in Game this is check solution)
		OnUpdateCell();

	}

	protected virtual void OnUpdateCell(){}

	public override void MoveCursor(Direction dir)
	{
		//moves game cursor and applies soft marking if on
		int prevx = GameCursorX, prevy = GameCursorY;
		switch (dir)
		{
			case Direction.Up:
				if (GameCursorY > 0) GameCursorY--;
				break;
			case Direction.Down:
				if (GameCursorY < BoardHeight - 1) GameCursorY++;
				break;
			case Direction.Left:
				if (GameCursorX > 0) GameCursorX--;
				break;
			case Direction.Right:
				if (GameCursorX < BoardWidth - 1) GameCursorX++;
				break;
		}

		Drawing.UpdateCursor(GameCursorX,GameCursorY, prevx, prevy);
	    
		if (SoftMarking)
		{
			UpdateCell(SoftMarkingMode.Item2);
		}
	}

	protected void StartSoftMarking(CellState targetState)
	{
		SoftMarking = true; 
		SoftMarkingMode.Item1 = boardState.Cells[GameCursorX, GameCursorY]; 
		SoftMarkingMode.Item2 = targetState == SoftMarkingMode.Item1 ? CellState.Unknown : targetState; 
		UpdateCell(targetState);
	}
	
	public override void Action1()
	{
		if (SoftMarking)
			SoftMarking = false;
		else
			UpdateCell(CellState.Black);
	}
	
	public override void ActionShift1()
	{
		StartSoftMarking(CellState.Black);
	}
	
	public override void AnyKeyButArrow()
	{
		SoftMarking = false;
	}
	
}