using System.Text;
using System.Text.Unicode;

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
	    boardState = new BoardState(boardWidth, boardHeight, rowClues, columnClues);
	    //screen initialization
	    Drawing.Initialize(boardState);
	    message = "";
	    //updates everything on screen 
	    Drawing.Draw(boardState);
	    Drawing.UpdateCursor(gameCursorX, gameCursorY);
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
	    sr.Close();
	    //temporary list for dynamically reading from file
	    List<List<CellState>> list = new List<List<CellState>>();
	    list.Add(new List<CellState>());
	    //Start tracking board width and height
	    boardWidth = 0;
	    bool trackWidth = true;
	    boardHeight = 1;
	    foreach (char c in str)
	    {
		    if (trackWidth) boardWidth++;
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
					    boardWidth--;
					    trackWidth = false;
				    }
				    boardHeight++;
				    list.Add(new List<CellState>());
				    break;
			    default: //handles unexpected chars such as \r
				    if (trackWidth)
				    {
					    boardWidth--;
				    }
				    break;
		    }
	    }
	    
	    //Convert from List<List<CellState>> to CellState[,] and assign to Solution
	    solution = new CellState[boardHeight, boardWidth];
	    for (int i = 0; i < boardHeight; i++)
	    {
		    for (int j = 0; j < boardWidth; j++)
		    {
			    solution[i, j] = list[i][j];
		    }
	    }
	    
	    //Calculate Clues
	    CalculateClues();
    }
	
	protected override bool CheckSolution()
    {
	    for (int i = 0; i < boardHeight; i++)
	    {
		    for (int j = 0; j < boardWidth; j++)
		    {
			    if (boardState.Cells[i,j] == CellState.Black ^ solution[i,j] == CellState.Black)
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
