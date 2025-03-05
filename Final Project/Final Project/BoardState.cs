namespace Final_Project;

public class BoardState
{
	//everything about the board: size, cell markings, clues and clue satisfaction
	public int width, height;
	public CellState[,] cells; //marking state of the cells in the board
	public int[][] rowClues, columnClues;
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
		
		this.width = width;
		this.height = height;
		cells = new CellState[height, width];
		this.rowClues = rowClues;
		this.columnClues = columnClues;
	}

}

public enum CellState
{
	Unknown, Dot, Black
}

public static class CellStateUtilities
{
	public static string ToString(CellState cellState)
	{
		switch (cellState)
		{
			case CellState.Unknown: return "Unknown";
			case CellState.Dot: return "Dot";
			case CellState.Black: return "Black";
		}

		return "";
	}

	public static bool UnequalAndNotUnkown(CellState a, CellState b)
	{
		return a != b && a != CellState.Unknown && b != CellState.Unknown;
	}

	public static bool BlacksMatch(CellState[,] cellStates, CellState[,] other)
	{
		//returns true if all black cells correspond in the two CellState[,]'s
		
		//check equal sizes
		int numRows = cellStates.GetLength(0);
		int numCols = cellStates.GetLength(1);
		if (numRows != other.GetLength(0) || numCols != other.GetLength(1))
		{
			throw new ArgumentException("both arrays must have the same size");
		}

		for (int i = 0; i < numRows; i++)
		{
			for (int j = 0; j < numCols; j++)
			{
				if (cellStates[i,j] == CellState.Black ^ other[i,j] == CellState.Black)
				{
					return false;
				}
			}
		}

		return true;
	}
}