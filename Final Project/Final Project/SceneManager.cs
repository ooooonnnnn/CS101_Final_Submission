namespace Final_Project;

public static class SceneManager
{
	//Runs the game, switches between scenes: game, level editor, menu
	//each scene has it's own flag
	private static int nextSceneFlag = 0;
	private const int flagStartMenu = 0, flagGame = 1, flagTutorial = 2, flagLevelEditorMenu = 3, flagEditor = 4;
	
	//menus write to this variable the selected option
	public static Menu.MenuOption selectedOption;
	
	//to load a specific level
	private static string levelPath;
	
	//to create a level editor with board width and height
	public static int editorWidth = 10, editorHeight = 10;
	
	//Option lists for menus
	private static List<Menu.MenuOption> mainOpts = new List<Menu.MenuOption>()
	{
		new Menu.MenuOption("Test Level"),
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
	
	public static void Main()
	{
		//main scene loop
		
		//used inside the while for menus
		string title;
		Menu menu;
		while (true)
		{
			switch (nextSceneFlag)
			{
				case flagStartMenu:
					title = "Sh'chor Uptor\n" +
					        "q to select";
					menu = new Menu(title,mainOpts);
					menu.StartScene();//this assigns a value to selectedOption
					switch (selectedOption.text)
					{
						case "Test Level":
							nextSceneFlag = flagGame;
							levelPath = "test2.txt";
							break;
						case "Level Editor":
							nextSceneFlag = flagLevelEditorMenu;
							break;
						case "Quit":
							//stop application
							return;
					}
					break;
				
				case flagGame:
					Game game = new Game();
					game.levelPath = levelPath;
					game.StartScene();
					nextSceneFlag = flagStartMenu;
					break;
				
				case flagLevelEditorMenu:
					title = "Choose board size: (right/left arrows)";
					menu = new Menu(title, levelEditorOpts);
					menu.StartScene();
					switch (selectedOption.text)
					{
						case "Start":
							nextSceneFlag = flagEditor;
							break;
						case "Back":
							nextSceneFlag = flagStartMenu;
							break;
					}
					break;
				
				case flagEditor:
					LevelEditor levelEditor = new LevelEditor();
					levelEditor.boardWidth = editorWidth;
					levelEditor.boardHeight = editorHeight;
					levelEditor.StartScene();
					// levelEditor.RunBoardMenu();
					nextSceneFlag = flagStartMenu;
					break;
			}
		}
	}
}