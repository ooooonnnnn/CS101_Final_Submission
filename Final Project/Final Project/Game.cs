namespace Final_Project;

public class Game : SceneWithBoard
{
	//Main game logic
	
	// //Cursor position when ingame
	// public static int GameCursorX = 0, GameCursorY = 0; //first cell is 0
	//
	// public static BoardState boardState;
	
	// //Soft marking
	// public static bool SoftMarking = false;
	// //only cells with the first state are changed into the second state
	// private static (CellState,CellState) SoftMarkingMode = (CellState.Unknown,CellState.Unknown);
	//
	// //Flag for ending the game
	// public static bool isSolved = false;

    protected override void Initialize()
    {
	    ReadLevelFile(levelPath);
	    boardState = new BoardState(BoardWidth, BoardHeight, RowClues, ColumnClues);
	    //screen initialization
	    Drawing.Initialize(boardState);
	    //updates everything on screen 
	    Drawing.Draw(boardState);
	    Drawing.UpdateCursor(GameCursorX, GameCursorY);
	    Console.CursorVisible = true;
    }

    protected override void OnUpdateCell()
    {
	    CheckSolution();
    }
    
	public override void Action2()
	{
		if (SoftMarking)
			SoftMarking = false;
		else
			UpdateCell(CellState.Dot);
	}
	
	public override void ActionShift2()
    {
	    StartSoftMarking(CellState.Dot);
    }

	private void ReadLevelFile(string path)
    {
	    /* reads level solution from file (written with LevelEditor class)
	     writes into Solution, RowClues, ColumnClues, BoardWidth, BoardHeight
	     */
	    StreamReader sr = File.OpenText(path);
	    string str = sr.ReadToEnd();
	    //temporary list for dynamically reading from file
	    List<List<CellState>> list = new List<List<CellState>>();
	    list.Add(new List<CellState>());
	    //Start tracking board width and height
	    BoardWidth = 0;
	    bool trackWidth = true;
	    BoardHeight = 1;
	    foreach (char c in str)
	    {
		    if (trackWidth) BoardWidth++;
		    switch (c)
		    {
			    case '1':
				    list[list.Count - 1].Add(CellState.Black);
				    break;
			    case '0':
				    list[list.Count - 1].Add(CellState.Unknown);
				    break;
			    case '\n':
				    if (trackWidth)
				    {
					    BoardWidth--;
					    trackWidth = false;
				    }
				    BoardHeight++;
				    list.Add(new List<CellState>());
				    break;
		    }
	    }
	    
	    //Convert from List<List<CellState>> to CellState[,] and assign to Solution
	    Solution = new CellState[BoardWidth, BoardWidth];
	    for (int i = 0; i < BoardWidth; i++)
	    {
		    for (int j = 0; j < BoardHeight; j++)
		    {
			    Solution[i, j] = list[j][i];
		    }
	    }
	    
	    //Calculate Clues
	    //calculate column clues
	    ColumnClues = new int[BoardWidth][];
	    for (int i = 0; i < BoardWidth; i++)
	    {
		    List<int> newClues = new(); //running list of this columns clues (from end to beginning)
		    int currentClue = 0; //keeps track of the latest parsed clue
		    for (int j = BoardHeight-1; j >= 0; j--)
		    {
			    if (Solution[i,j] == CellState.Black)
			    {
				    currentClue++;
			    }
			    else if (currentClue > 0)
			    {
				    newClues.Add(currentClue);
				    currentClue = 0;
			    }
		    }
		    if (currentClue > 0)
		    {
			    newClues.Add(currentClue);
		    }
		    ColumnClues[i] = newClues.ToArray();
	    }
	    
	    //calculate row clues
	    RowClues = new int[BoardHeight][];
	    for (int i = 0; i < BoardHeight; i++)
	    {
		    List<int> newClues = new(); //running list of this row's clues (from end to beginning)
		    int currentClue = 0; //keeps track of the latest parsed clue
		    for (int j = BoardWidth-1; j >= 0; j--)
		    {
			    if (Solution[j,i] == CellState.Black)
			    {
				    currentClue++;
			    }
			    else if (currentClue > 0)
			    {
				    newClues.Add(currentClue);
				    currentClue = 0;
			    }
		    }
		    if (currentClue > 0)
		    {
			    newClues.Add(currentClue);
		    }
		    RowClues[i] = newClues.ToArray();
	    }
    }

    protected override bool CheckSolution()
    {
	    for (int i = 0; i < BoardWidth; i++)
	    {
		    for (int j = 0; j < BoardHeight; j++)
		    {
			    if (boardState.Cells[i,j] == CellState.Black ^ Solution[i,j] == CellState.Black)
			    {
				    return false;
			    }
		    }
	    }
	    isSolved = true;
	    sceneFinished = true;//this is temporary. I should end a state where the puzzle is solved but the scene is still ongoing
	    return true;
    }

    // private static void UpdateScreen()
    // {
	   //  //updates everything on screen 
	   //  Drawing.Draw(boardState);
	   //  Drawing.UpdateCursor(GameCursorX, GameCursorY);
    // }
}
