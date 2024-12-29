using System.Linq;

namespace Final_Project;

public class Menu : Scene
{
	//class for menu display and functionality
	
	private List<MenuOption> menuOptions;
	private int numTitleRows; //how much room should be saved for the title
	
	private int highlightedOption;
	private bool clickFlag = false; //will become true when option is selected
	
	public Menu(string title, List<MenuOption> options)
	{
		//Initialize InputHandler
		InputHandler.scene = this;
		
		//Initialize menu
		menuOptions = options;
		
		//display
		Console.Clear();
		Console.WriteLine(title + "\n");
		numTitleRows = title.Count(c => c == '\n') + 2;
		for (int i = 0; i < menuOptions.Count; i++)
		{
			WriteOption(i);
		}
	}

	public MenuOption RunMenu()
	{
		//starts menu functionality. returns the number of the selected option. Hide the cursor for the duration
		Console.CursorVisible = false;
		while (!clickFlag)
		{
			InputHandler.Input(Console.ReadKey(true));
		}
		Console.CursorVisible = true;
		return menuOptions[highlightedOption];
	}

	private void WriteOption(int optionNum)
	{
		//updates the graphic for the option in menuOptions with index optionNum
		
		Console.SetCursorPosition(0,optionNum*2 + numTitleRows);
		
		//first clear the line
		Console.BackgroundColor = ConsoleColor.Black;
		Console.WriteLine(Drawing.StrRepeat(" ",Console.BufferWidth));
		Console.CursorTop--;
		
		//highlighted options are black on white
		if (optionNum == highlightedOption)
		{
			Console.BackgroundColor = ConsoleColor.White;
			Console.ForegroundColor = ConsoleColor.Black;
		}
		
		//then write the new line
		Console.WriteLine(menuOptions[optionNum].text);
		
		Console.BackgroundColor = ConsoleColor.Black;
		Console.ForegroundColor = ConsoleColor.White;
	}

	public override void MoveCursor(Direction direction)
	{
		//changes the highlighted option
		//update highlightedOption
		int prevHighlight = highlightedOption;
		switch (direction)
		{
			case Direction.Up:
				highlightedOption = highlightedOption > 0 ? highlightedOption - 1 : 0;
				break;
			case Direction.Down:
				highlightedOption = highlightedOption < (menuOptions.Count - 1)
					? highlightedOption + 1
					: (menuOptions.Count - 1);
				break;
			case Direction.Left:
			case Direction.Right:
				menuOptions[highlightedOption].RightLeftFunc(direction);
				break;
		}
		
		//update graphics
		WriteOption(prevHighlight);
		WriteOption(highlightedOption);
	}

	public override void Action1()
	{
		clickFlag = true;
	}

	public class MenuOption
	{
		public string text;
		public delegate void RightLeftDelegate(Direction direction);
		public RightLeftDelegate RightLeftFunc;
		
		public MenuOption(string text, RightLeftDelegate func)
		{
			this.text = text;
			RightLeftFunc = func;
		}
		
		public MenuOption(string text)
		{
			this.text = text;
			RightLeftFunc = d => { };
		}
	}
	
	//Unimplemented functions - irrelevant
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


