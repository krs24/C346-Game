using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for any moving object in the game. Player and Enemy are children of MovingObject.
/// Inherits from MonoBehaviour 
/// </summary>
public abstract class MovingObject : MonoBehaviour
{
	/// <summary>
	/// The time the object will take to move. Unit is seconds. 
	/// </summary>
	public float moveTime = 0.1f;
	
	/// <summary>
	/// Collision checking layer.
	/// </summary>
	public LayerMask blockingLayer;
		
	/// <summary>
	/// 2D collider used to detect collisions. 
	/// </summary>		
	private BoxCollider2D boxCollider; 		
	
	/// <summary>
	/// 2D rigid body used for movement of 2D objects. 
	/// </summary>	
	private Rigidbody2D rb2D;				
	
	/// <summary>
	/// Increases movement efficiency
	/// </summary>
	private float inverseMoveTime;
		
	/// <summary>
	/// Initialization function for a MovingObject. Takes care of data initialization.
	/// </summary>
	protected virtual void Start ()
	{
		//Get reference to this object's BoxCollider2D
		boxCollider = GetComponent <BoxCollider2D> ();
			
		//Get reference to this object's Rigidbody2D
		rb2D = GetComponent <Rigidbody2D> ();
			
		//By storing the reciprocal of the move time we can use it 
		//by multiplying instead of dividing. More efficient.
		inverseMoveTime = 1f / moveTime;
	}
		
	/// <summary>
	/// Movement function to control any moving objects movements. 
	/// </summary>
	/// <param name="xDir">
	/// X direction the object is trying to move.
	/// </param>
	/// <param name="yDir">
	/// Y direction the object is trying to move.
	/// </param>
	/// <param name="hit">
	/// An output if a collision is detected. 
	/// </param>
	/// <returns>
	/// Returns a boolean to control movement. True if move is possible, false otherwise.
	/// </returns>
	protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
	{
		//Use objects current position as starting position.
		Vector2 start = transform.position;
			
		// Calculate end position based on parameters.
		Vector2 end = start + new Vector2 (xDir, yDir);
			
		//Disable the boxCollider so that linecast doesn't cause the object to collide with itself.
		boxCollider.enabled = false;
			
		//Cast a line from start point to end point checking collision on blockingLayer.
		hit = Physics2D.Linecast (start, end, blockingLayer);
			
		//Re-enable boxCollider after linecast.
		boxCollider.enabled = true;
			
		//Check if anything was hit and if the object hit is still active.
		if(hit.transform == null && gameObject.activeInHierarchy)
		{
			//If nothing was hit, start SmoothMovement with end as destination
			StartCoroutine (SmoothMovement (end));
				
			//Succesful Move
			return true;
		}
			
		//Something was hit. Move was unsuccesful.
		return false;
	}
		
	/// <summary>
	/// Coroutine to help move units from one space to the next.
	/// </summary>
	/// <param name="end">
	/// The end point for the unit's movement. Generally one away tile in some direction.
	/// </param>
	/// <returns>
	/// An IEnumerator to help with movement delay.
	/// </returns>

	protected IEnumerator SmoothMovement (Vector3 end)
	{
		//Calculate the remaining distance to move. 
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			
		//Move the remaining distance
		while(sqrRemainingDistance > float.Epsilon)
		{
			//Find a new position proportionally closer to the end, based on the moveTime
			Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
				
			//Move object to calculated position.
			rb2D.MovePosition (newPostion);
				
			//Recalculate the remaining distance after moving.
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
				
			//Return and loop until sqrRemainingDistance is close enough to zero to end the function
			yield return null;
		}
	}
		
	/// <summary>
	/// Attempts to move the unit in the specified directon. Checks for any collisions in the 
	/// given direction. If a collision is detected it gets the type of object being collided with
	/// and takes appropriate action.
	/// </summary>
	/// <param name="xDir">
	/// X direction the object is trying to move.
	/// </param>
	/// <param name="yDir">
	/// Y direction the object is trying to move.
	/// </param>
	protected virtual void AttemptMove <T> (int xDir, int yDir)
		where T : Component
	{
		RaycastHit2D hit;							//Stores whatever linecast hits.
		bool canMove = Move (xDir, yDir, out hit);	//True if object can move.
	
		//If linecst doesn't hit anything return.
		if(hit.transform == null) 					
			return;
			
		//Get reference to the object that was hit
		T hitComponent = hit.transform.GetComponent <T> ();
			
		//MovingObject has hit an item it can interact with so perform proper action
		if(!canMove && hitComponent != null && gameObject.activeInHierarchy)
			OnCantMove (hitComponent);
	}
		
	/// <summary>
	/// If the unit can't move calls the appropriate action method from the proper unit. 
	/// </summary>
	protected abstract void OnCantMove <T> (T component)
		where T : Component;
}