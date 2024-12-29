namespace Final_Project;

public static class InputHandler
{
	//This class handles input
	
	//Reference to the scene that instantiates this (game/menu/level editor)
	public static Scene scene;
	
	//Input settings:
	private const ConsoleKey MoveUp = ConsoleKey.UpArrow;
	private const ConsoleKey MoveDown = ConsoleKey.DownArrow;
	private const ConsoleKey MoveLeft = ConsoleKey.LeftArrow;
	private const ConsoleKey MoveRight = ConsoleKey.RightArrow;
	private const ConsoleKey MarkBlack = ConsoleKey.Q;
	private const ConsoleKey MarkDot = ConsoleKey.S; //not w anymore
	
	public static void Input(ConsoleKeyInfo keyInfo)
	{
		/* Scene calls this function repeatedly
		 arrow keys move the cursor on the board
		q to mark a cell black, w to mark a cell with a dot. marking a marked cell (with the same mark) clears the cell instead
		shift-q and shift-w to begin "soft" marking - every cell you move to will be marked if it's empty according to this logic:
		only marking actions that are identical to the initial one are repeated, ex. empty->black, black->dot, black->empty 
		any key other than arrow keys to end "soft" marking
			*/

		ConsoleKey key = keyInfo.Key;
		
		//handle movement keys
		switch (key)
		{
			case MoveRight:
				scene.MoveCursor(Direction.Right);
				return;
			case MoveLeft:
				scene.MoveCursor(Direction.Left);
				return;
			case MoveUp:
				scene.MoveCursor(Direction.Up);
				return;
			case MoveDown:
				scene.MoveCursor(Direction.Down);
				return;
		}
		
		//handle q w and shift
		ConsoleModifiers mods = keyInfo.Modifiers;
		if ((mods & ConsoleModifiers.Shift) != 0)
		{
			switch (key)
			{
				case MarkBlack:
					scene.ActionShift1();
					return;
				case MarkDot:
					scene.ActionShift2();
					return;
			}
		}
		else
		{
			switch (key)
			{
				case MarkBlack:
					scene.Action1();
					return;
				case MarkDot:
					scene.Action2();
					return;
			}
		}
		
		//handle any key
		if ((mods & ConsoleModifiers.Shift) == 0 && NotArrowKey(key))
		{
			scene.AnyKeyButArrow();
		}
		
		// if (_scene.SoftMarking && NotArrowKey(key))
		// {
		// 	_scene.ToggleSoftMarking(false);
		// }
		// else if (key == MarkBlack && ((mods & ConsoleModifiers.Shift) != 0)) //shift-q
		// {
		// 	_scene.ToggleSoftMarking(true, CellState.Black);
		// }
		// else if (key == MarkDot && (mods & ConsoleModifiers.Shift) != 0) //shift-w
		// {
		// 	_scene.ToggleSoftMarking(true, CellState.Dot);
		// }
		// else if (key == MarkBlack)
		// {
		// 	_scene.UpdateCell(CellState.Black);
		// }
		// else if (key == MarkDot)
		// {
		// 	_scene.UpdateCell(CellState.Dot);
		// }
		
	}

	private static bool NotArrowKey(ConsoleKey key)
	{
		return key != ConsoleKey.LeftArrow && key != ConsoleKey.RightArrow && key != ConsoleKey.DownArrow &&
		       key != ConsoleKey.UpArrow;
	}
	
	private static void UpdateTapHold(ref ConsoleModifiers input, ConsoleModifiers target, ref bool tap, ref bool hold)
	{
		//takes input of modifier keys, compared to target, and updates tap and hold bools accordingly
		//first instance that modifier key is pressed gives tap = true, hold = true
		//subsequent instances give tap = false, hold = true

		if ((input & target) != 0)
		{
			tap = true;
			if (hold)
			{
				tap = false;
			}
			hold = true;
		}
		else
		{
			tap = false;
			hold = false;
		}
	}
	
}