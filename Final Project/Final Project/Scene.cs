using System.ComponentModel.Design.Serialization;

namespace Final_Project;

public enum Direction
{
	Up, Down, Left, Right
}

public abstract class Scene
{
	 // interactive scene in the game - either menu, game, or level editor
	 protected bool sceneFinished = false;
	
	public void StartScene() // starts scene functionality
	{
		//initialization
		Initialize();
		InputHandler.scene = this;

		while (!sceneFinished)
		{
			InputHandler.Input(Console.ReadKey(true));
		}
		OnSceneEnd();
	}

	protected virtual void Initialize(){}
	protected virtual void OnSceneEnd(){}
	
	public abstract void MoveCursor(Direction direction);

	public abstract void Action1();

	public virtual void Action2(){}

	public virtual void ActionShift1(){}

	public virtual void ActionShift2(){}

	public virtual void AnyKeyButArrow(){}
}