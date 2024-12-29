using System.ComponentModel.Design.Serialization;

namespace Final_Project;

public enum Direction
{
	Up, Down, Left, Right
}

public abstract class Scene
{
	//interactive scene in the game - either menu, game, or editor
	// protected bool sceneFinished = false;
	// protected bool cursorVisible;
	//
	// public void StartScene() // starts scene functionality
	// {
	// 	//initialization
	// 	InputHandler.scene = this;
	// 	
	// 	while (!sceneFinished)
	// 	{
	// 		InputHandler.Input(Console.ReadKey(true));
	// 	}
	// } 
	
	public abstract void MoveCursor(Direction direction);

	public abstract void Action1();

	public virtual void Action2(){}

	public virtual void ActionShift1(){}

	public virtual void ActionShift2(){}

	public virtual void AnyKeyButArrow(){}
}