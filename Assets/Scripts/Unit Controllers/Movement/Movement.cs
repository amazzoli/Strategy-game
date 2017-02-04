using UnityEngine;


/// <summary>
/// Movement abstract class for the deployment and the in game movement
/// </summary>
public abstract class Movement {

	public bool movConstructed;
	protected Transform armyT;
	protected Vector3 startPosition, startForward;

	/// <summary> Returns if the army can be moved at the click </summary>
	public abstract bool IsAllowed ();

	/// <summary> Initialzes the movement parameters at the mouse click </summary>
	public abstract void InitMovement ();

	/// <summary> Updates the army position during the movement coroutine </summary>
	public abstract void UpdateMovement ();

	/// <summary> Stops the movement </summary>
	public abstract void StopMovement ();

	/// <summary> Returns the movement left after the movement </summary>
	public abstract float GetMovementLeft ();

	/// <summary> Reset the position to the initial position </summary>
	public abstract void ResetMovement ();
}
