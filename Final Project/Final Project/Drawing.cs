namespace Final_Project;
using System;
using System.Linq;

public static class Drawing
{
	//This class handles drawing stuff on the screen
	//The following symbols are used
	public static string black = "@",
		empty = " ",
		hLine = "_",
		hLineBold = "=",
		vLine = ":",
		vLineBold = "|",
		corner = ".",
		dot = "`",
		rightArrow = ">";
	//settings for drawing the board
	private static int cellWidth = 3; //external width of each cell (if 3 than internal width is 2)
	private static int boardWidth, boardHeight;
	private static int clueRowHeight, clueColWidth; //How much space is reserved for the clues on the top and left of the board
	private static int maxRowClues; //how many cells need to be drawn on the left for clues?

	public static void Initialize(BoardState boardState)
	{
		//imports the board size from boardState and calculates the space needed for the clues
		boardWidth = boardState.Width;
		boardHeight = boardState.Height;

		clueRowHeight = boardState.ColumnClues.Max(c => c != null ? c.Length : 0); //one row per clue
		maxRowClues = boardState.RowClues.Max(c => c != null ? c.Length : 0); //how many clues are on the left, max?
		clueColWidth = maxRowClues * cellWidth; //one cell per clue
	}
	
	public static void Draw(BoardState boardState)
	{
		Console.Clear();
		//draw the column clues on top
		// Console.BackgroundColor = ConsoleColor.White;
		// Console.ForegroundColor = ConsoleColor.Black;
		for (int i = 0; i < clueRowHeight; i++)
		{
			//make room for left clues
			Console.Write(StrRepeat(empty, clueColWidth));
			
			//looking for clues with index clueRowHeight-1-i. in places there aren't, print an empty cell
			for (int j = 0; j < boardWidth; j++)
			{
				//check if this clue is in the same column as the highlighted cell
				bool highlight = j == Game.gameCursorX;
				string clue;
				try
				{
					clue = boardState.ColumnClues[j][clueRowHeight - 1 - i].ToString();
					PrintClueCell(clue, highlight, false);
				}
				catch
				{
					//empty cell
					PrintClueCell("", highlight, false);
				}
			}
			Console.WriteLine();//newline
		}
		
		//draw first line of the board
		//first make room for left clues
		Console.Write(StrRepeat(empty, clueColWidth));
		Console.WriteLine(corner + StrRepeat(hLine, cellWidth * boardWidth - 1) + corner);
		
		//draw board line by line. draw clues first
		for (int i = 0; i < boardHeight; i++)
		{
			bool highlight = i == Game.gameCursorY;
			//draw clues
			//look for clues with index maxRowClues - 1 - j
			for (int j = 0; j < maxRowClues; j++)
			{
				try
				{
					string clue = boardState.RowClues[i][maxRowClues - 1 - j].ToString();
					PrintClueCell(clue, highlight, true);
				}
				catch
				{
					PrintClueCell("", highlight, true);
				}
			}
			
			//each line looks like |  :..:  :@@:  |  :  :  :  :  | ... where "  " is unknown, ".." is dot and "@@" is black
			//in every 5th-1 line excluding the last one, unknown spaces look like "__"
			for (int j = 0; j < boardWidth; j++)
			{
				//draw "|" or ":"
				if (j % 5== 0) // every 5th column
				{
					Console.Write(vLineBold);
				}
				else // every other column
				{
					Console.Write(vLine);
				}
				
				//draw the cell content
				switch (boardState.Cells[j,i])
				{
					case CellState.Unknown:
						if ((i + 1) % 5 == 0) Console.Write(StrRepeat(hLine, cellWidth - 1));
						else Console.Write(StrRepeat(empty, cellWidth - 1));
						break;
					case CellState.Dot:
						Console.Write(StrRepeat(dot, cellWidth - 1));
						break;
					case CellState.Black:
						Console.Write(StrRepeat(black, cellWidth - 1));
						break;
				}
			}
			//end with |
			Console.WriteLine(vLineBold);
			
		}
		
		/* board example
			    1          
		     1  1>>1  1  1 
		   .______________.
	       |  :..:..:..:..|
     _____2|@@:@@:..:  :  | the middle cell in this row is highlighted, this affects the lines between the clues
	       |  :..:  :..:..| some cells are dark "@@" some dotted ".." and some unknown "  "
	   1  1|@@:..:  :  :@@|
		  3|  :@@:@@:@@:  |
		   |______________|
		*/
	}

	private static void PrintClueCell(string clue, bool highlight, bool isLeft)
	{
		//helper function for printing cells of clues. appearance is different for clues pertaining to the highlighted board cell
		//left clues look different from top clues
		if (highlight)
		{
			if (isLeft)
			{
				Console.Write(StrRepeat(hLine, cellWidth - clue.Length) + clue);
			}
			else
			{
				if (clue == "")
				{
					Console.Write(StrRepeat(empty,cellWidth));
				}
				else
				{
					Console.Write(StrRepeat(rightArrow, cellWidth - clue.Length) + clue);
				}
			}
		}
		else
		{
			Console.Write(StrRepeat(empty, cellWidth - clue.Length) + clue);
		}
	}

	public static void UpdateBoardCell(CellState newState)
	{
		//updates the contents of the current board cell without the borders
		string newStringBase = ""; //this will be repeated in the cell
		// Console.BackgroundColor = ConsoleColor.White;
		// Console.ForegroundColor = ConsoleColor.Black;
		switch (newState)
		{
			case CellState.Unknown:
				//if the cell row is a multiple of 5 minus 1, unknown looks like "__"
				if ((Game.gameCursorY + 1) % 5 == 0)
				{
					newStringBase = hLine;
				}
				else newStringBase = empty;
				break;
			case CellState.Dot:
				newStringBase = dot;
				break;
			case CellState.Black:
				// Console.BackgroundColor = ConsoleColor.Black;
				newStringBase = black;
				break;
		}

		int length = cellWidth - 1;
		Console.Write(StrRepeat(newStringBase, length));
		// Console.BackgroundColor = ConsoleColor.White;
		Console.CursorLeft -= length;
	}

	public static void UpdateCursor(int column, int row, int prevCol, int prevRow)
	{
		//places the cursor in a cell with coordinates column and row and updates relevant graphics
		
		//update clue highlights
		//hide the cursor before moving it around all crazy style
		Console.CursorVisible = false;
		//de-highlight column clues
		for (int i = 0; i < clueRowHeight; i++)
		{
			Console.SetCursorPosition(clueColWidth + prevCol*cellWidth, i);
			string clue;
			try
			{
				clue = Game.boardState.ColumnClues[prevCol][clueRowHeight - 1 - i].ToString();
				PrintClueCell(clue, false, false);
			}
			catch
			{
				//empty cell
				PrintClueCell("", false, false);
			}
		}
		//de-highlight row clues
		for (int i = 0; i < maxRowClues; i++)
		{
			Console.SetCursorPosition(i*cellWidth, clueRowHeight + 1 + prevRow);
			string clue;
			try
			{
				clue = Game.boardState.RowClues[prevRow][maxRowClues - 1 - i].ToString();
				PrintClueCell(clue, false, true);
			}
			catch
			{
				//empty cell
				PrintClueCell("", false, true);
			}
		}
		//highlight column clues
		for (int i = 0; i < clueRowHeight; i++)
		{
			Console.SetCursorPosition(clueColWidth + column*cellWidth, i);
			string clue;
			try
			{
				clue = Game.boardState.ColumnClues[column][clueRowHeight - 1 - i].ToString();
				PrintClueCell(clue, true, false);
			}
			catch
			{
				//empty cell
				PrintClueCell("", true, false);
			}
		}
		//highlight row clues
		for (int i = 0; i < maxRowClues; i++)
		{
			Console.SetCursorPosition(i*cellWidth, clueRowHeight + 1 + row);
			string clue;
			try
			{
				clue = Game.boardState.RowClues[row][maxRowClues - 1 - i].ToString();
				PrintClueCell(clue, true, true);
			}
			catch
			{
				//empty cell
				PrintClueCell("", true, true);
			}
		}
		
		//put the cursor in the right cell
		UpdateCursor(column, row);
		
		//reveal the cursor
		Console.CursorVisible = true;
	}
	
	public static void UpdateCursor(int column, int row)
	{
		//places the cursor in a cell with coordinates column and row WITHOUT UPDATING graphics
		Console.SetCursorPosition(1 + clueColWidth + cellWidth * column, 1 + clueRowHeight + row);
	}

	public static void UpdateMessage(string message, int msgLeft, int msgTop)
	{
		//updates the text of a message shown next to the game board
		
		//record cursor state to restore later
		bool csrVis = Console.CursorVisible;
		int csrLft = Console.CursorLeft;
		int csrTop = Console.CursorTop;

		Console.SetCursorPosition(clueColWidth + cellWidth * boardWidth + msgLeft,
			clueRowHeight + msgTop);
		Console.Write(message);
		//clear the rest of the line
		Console.Write(StrRepeat(" ",Console.BufferWidth - Console.CursorLeft));
		
		//restore cursor visibility and position
		Console.CursorVisible = csrVis;
		Console.CursorLeft = csrLft;
		Console.CursorTop = csrTop;
	}
	
	public static string StrRepeat(string str, int times)
	{
		//function for repeating strings 
		string result = "";
		for (int i = 0; i < times; i++)
		{
			result += str;
		}

		return result;
	}
}