namespace Final_Project;

using System.IO;

public static class SceneManager
{
	//Runs the game, switches between scenes: game, level editor, menu
	//each scene has it's own flag
	private static SceneFlag nextSceneFlag = SceneFlag.startMenu;
	private enum SceneFlag
	{
		startMenu = 0,
		game = 1,
		tutorial = 2,
		levelEditorMenu = 3,
		editor = 4,
		levelSelect = 5
	}
	
	//menus write the selected option to this variable 
	public static Menu.MenuOption selectedOption;
	
	//to load a specific level from text file
	private static string levelPath;
	//all level names
	private static string[] levels;
	
	//to create a level editor with board width and height
	public static int editorWidth = 10, editorHeight = 10;
	
	//Option lists for menus
	private static List<Menu.MenuOption> mainOpts = new List<Menu.MenuOption>()
	{
		new Menu.MenuOption("Level Select"),
		new Menu.MenuOption("Tutorial"),
		new Menu.MenuOption("Level Editor"),
		new Menu.MenuOption("Quit")
	};

	private static List<Menu.MenuOption> levelEditorOpts = new List<Menu.MenuOption>()
	{
		new ($"Board Width: {editorWidth}",
			dir =>
			{
				if (dir == Direction.Right && editorWidth < 100) editorWidth += 5;
				else if (dir == Direction.Left && editorWidth > 5) editorWidth -= 5;
				levelEditorOpts[0].text = $"Board Width: {editorWidth}";
			}),
		new ($"Board Height: {editorHeight}",
			dir =>
			{
				if (dir == Direction.Right && editorHeight < 100) editorHeight += 5;
				else if (dir == Direction.Left && editorHeight > 5) editorHeight -= 5;
				levelEditorOpts[1].text = $"Board Height: {editorHeight}";
			}),
		new Menu.MenuOption("Start"),
		
		new Menu.MenuOption("Back")
	};

	private static List<Menu.MenuOption> tutorialOpts = new List<Menu.MenuOption>()
	{
		new Menu.MenuOption("Back")
	};
	
	public static void Main()
	{
		//main scene loop
		
		//get all level names
		string basePath =
			"C:\\Users\\USER\\Documents\\GitHub\\CS101_Final_Submission\\Final Project\\Final Project\\bin\\Debug\\net9.0";
		string[] files = Directory.GetFiles(basePath, "*.txt");
		levels = new string[files.Length];
		for (int i = 0; i < files.Length; i++)
		{
			string s = files[i];
			levels[i] = s.Remove(s.Length - 4).Remove(0,basePath.Length + 1);
		}
		
		//prepare console window
		// Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
		
		//used inside the menu loop
		string title;
		Menu menu;
		List<Menu.MenuOption> levelSelectOpts;
		//-------------------------------------main game loop: switch between scenes
		while (true)
		{
			switch (nextSceneFlag)
			{
				case SceneFlag.startMenu:
					title = "Sh'chor Uptor\n" +
					        "q to select";
					menu = new Menu(title,mainOpts);
					menu.StartScene();//this assigns a value to selectedOption
					switch (selectedOption.text)
					{
						case "Level Select":
							nextSceneFlag = SceneFlag.levelSelect;
							break;
						case "Tutorial":
							nextSceneFlag = SceneFlag.tutorial;
							break;
						case "Level Editor":
							nextSceneFlag = SceneFlag.levelEditorMenu;
							break;
						case "Quit":
							//stop application
							return;
					}
					break;
				
				case SceneFlag.tutorial:
					title = $"OBJECTIVE: \n" +
					        $"-----------------------------------------------------------\n" +
					        $"Use the clues to decipher the picture!\n" +
					        $"Each row and column have a series of clues that define consecutive runs of black cells in the final picture,\n" +
					        $"with at least one space between them.\n" +
					        $"You may use dots to mark cells you know must be empty, but it isn't required for solving the puzzle. \n" +
					        $"\n" +
					        $"EXAMPLE: \n" +
					        $"\n" +
					        $"       2 1 2\n     2 1 1 1 2\n    ._________.\n 1 1| :@: :@: |\n 1 1| :@: :@: |<- Two runs of 1 cell" +
					        $"\n   1| : :@: : |\n 1 1|@: : : :@|\n   5|@:@:@:@:@|" +
					        $"<- One run of 5 cells\n" +
					        $"       ^ \n" +
					        $"       ^A run of 2 cells, followed by a run of 1 cell \n" +
					        $"\n" +
					        $"CONTROLS: \n" +
					        $"-----------------------------------------------------------\n" +
					        $"Arrow keys to move cursor\n" +
					        $"({InputHandler.MarkBlack}): mark cell black, ({InputHandler.MarkDot}): mark cell with dot\n" +
					        $"(Left Shift + {InputHandler.MarkBlack}/{InputHandler.MarkDot}): initiate continuous marking, and cancel with and key.";
					menu = new Menu(title,tutorialOpts);
					menu.StartScene();//this assigns a value to selectedOption
					switch (selectedOption.text)
					{
						case "Back":
							nextSceneFlag = SceneFlag.startMenu;
							break;
					}
					break;
				
				case SceneFlag.levelSelect:
					levelSelectOpts = new List<Menu.MenuOption>();
					foreach (string level in levels)
					{
						levelSelectOpts.Add(new Menu.MenuOption(level));
					}

					title = "Choose Level: ";
					menu = new Menu(title, levelSelectOpts);
					menu.StartScene();
					nextSceneFlag = SceneFlag.game;
					levelPath = selectedOption.text + ".txt";
					break;
				
				case SceneFlag.game:
					Game game = new Game();
					game.levelPath = levelPath;
					game.StartScene();
					nextSceneFlag = SceneFlag.startMenu;
					break;
				
				case SceneFlag.levelEditorMenu:
					title = "Choose board size: (right/left arrows)";
					menu = new Menu(title, levelEditorOpts);
					menu.StartScene();
					switch (selectedOption.text)
					{
						case "Start":
							nextSceneFlag = SceneFlag.editor;
							break;
						case "Back":
							nextSceneFlag = SceneFlag.startMenu;
							break;
					}
					break;
				
				case SceneFlag.editor:
					LevelEditor levelEditor = new LevelEditor();
					levelEditor.boardWidth = editorWidth;
					levelEditor.boardHeight = editorHeight;
					levelEditor.StartScene();
					// levelEditor.RunBoardMenu();
					nextSceneFlag = SceneFlag.startMenu;
					break;
			}
		}
	}
}