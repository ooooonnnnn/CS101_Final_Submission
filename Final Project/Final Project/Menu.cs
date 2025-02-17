using System.Linq;

namespace Final_Project;

public class Menu : Scene
{
	//class for menu display and functionality
	
	private List<MenuOption> menuOptions;
	private string title;
	private int numTitleRows; //how much room should be saved for the title
	
	private int highlightedOption;
	private int page = 0; //for menus that don't fit in a single screen
	private int numPages;
	private int maxOptsPerPage;
	private int numOpts;

	public bool updateGraphicsFlag; //to enable other classes to define menu options that update graphics that aren't just moving the highlight 
	
	
	public Menu(string title, List<MenuOption> options)
	{
		//Initialize menu
		menuOptions = options;
		Console.CursorVisible = false;
		this.title = title;
		numOpts = menuOptions.Count;
		
		//calculate number of pages and add page-change-options if necessary
		numTitleRows = title.Count(c => c == '\n') + 2;
		maxOptsPerPage = (Console.BufferHeight - numTitleRows) / 2 - 1;
		numPages = (int)Math.Ceiling((double)numOpts / maxOptsPerPage);
		if (numPages > 1)
		{
			for (int i = 0; i < numPages; i++)
			{
				int index = (maxOptsPerPage + 1) * i + maxOptsPerPage;
				if (index > menuOptions.Count) index = options.Count;
				MenuOption.RightLeftDelegate func = dir =>
				{
					WritePage(
						dir == Direction.Right ? page + 1 :
						dir == Direction.Left ? page - 1 : -1);
				};
				menuOptions.Insert(index, new MenuOption($"\u2af7 Page {i+1} \u2af8",func));
			}
			
			numOpts = menuOptions.Count;
		}
		
		//---------------------------display--------------------------------
		WritePage(page);
	}

	protected override void OnSceneEnd()
	{
		SceneManager.selectedOption = menuOptions[highlightedOption];
	}

	private void WritePage(int newPage)
	{
		//changes the page, resets cursor to top, and highlights first option in page
		if (newPage < 0 || newPage >= numPages) return;//ignore non existent pages
		page = newPage;
		
		int firstOptIndex = newPage * (maxOptsPerPage + 1);
		highlightedOption = firstOptIndex;
		
		Console.Clear();
		Console.WriteLine(title + "\n");
		for (int i = firstOptIndex; i < menuOptions.Count && i < firstOptIndex + maxOptsPerPage + 1; i++)
		{ 
			WriteOption(i, i - firstOptIndex);
		}
	}
	
	private void WriteOption(int optionNum, int positionOnScreen)
	{
		//updates the graphic for the option in menuOptions with index optionNum

		if (!Drawing.SafeSetCursorPosition(0, positionOnScreen * 2 + numTitleRows)) return;
		// Console.SetCursorPosition(0,optionNum*2 + numTitleRows);
		
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

		int firstOpt = page * (maxOptsPerPage + 1);
		int lastOpt = Math.Min(firstOpt + maxOptsPerPage, numOpts - 1);
		switch (direction)
		{
			case Direction.Up:
				highlightedOption = highlightedOption > firstOpt ? highlightedOption - 1 : firstOpt;
				break;
			case Direction.Down:
				highlightedOption = highlightedOption < lastOpt ? highlightedOption + 1 : lastOpt;
				break;
			default:
				menuOptions[highlightedOption].RightLeftFunc(direction);
				break;
		}
		
		//update graphics
		if (direction == Direction.Up || direction == Direction.Down || updateGraphicsFlag)
		{
			updateGraphicsFlag = false;
			WriteOption(prevHighlight, prevHighlight - firstOpt);
			WriteOption(highlightedOption, highlightedOption - firstOpt);
		}
	}

	public override void Action1()
	{
		sceneFinished = true;
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
}


