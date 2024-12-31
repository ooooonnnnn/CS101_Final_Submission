namespace Final_Project;

public class BoardState
{
	//everything about the board: size, cell markings, clues and clue satisfaction
	public int Width, Height;
	public CellState[,] Cells; //marking state of the cells in the board
	public int[][] RowClues, ColumnClues;
	/* the clues that are drawn above the rows and columns
	   the first entry of each sub-array of clues is the right-most/bottom-most one. ex. if the column clues are [[1 2],[1 2 3]] the clues
	   will look like
	      3
	   2  2
	   1  1
	*/
	
	public BoardState(int width, int height, int[][] rowClues, int[][] columnClues)
	{
		Constructor(width, height, rowClues, columnClues);
	}
	
	public BoardState(int width, int height)
	{
		Constructor(width, height, new int[width][], new int[height][]);
	}

	private void Constructor(int width, int height, int[][] rowClues, int[][] columnClues)
	{
		//check that both width and height are multiples of 5
		if (!(width > 0 && (width % 5) == 0) || !(height > 0 && (height % 5) == 0))
		{
			throw new ArgumentException("Board width and height must be positive multiples of 5.");
		}
		//check that board isn't bigger than 100X100
		if (width > 100 || height > 100)
		{
			throw new ArgumentException("Maximum board size is 100X100.");
		}
		
		Width = width;
		Height = height;
		Cells = new CellState[width, height];
		RowClues = rowClues;
		ColumnClues = columnClues;
	}

}

public enum CellState
{
	Unknown, Dot, Black
}

public static class Extensions
{
	public static string ToString(this CellState cellState)
	{
		switch (cellState)
		{
			case CellState.Unknown: return "Unknown";
			case CellState.Dot: return "Dot";
			case CellState.Black: return "Black";
		}

		return "";
	}
}