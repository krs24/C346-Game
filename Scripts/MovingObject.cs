using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{
	public float moveTime = 0.1f;			//Time it will take object to move, in seconds.
	public LayerMask blockingLayer;			//Layer on which collision will be checked.
		
		
	private BoxCollider2D boxCollider; 		//All MovingObjects get a BoxCollider2D.
	private Rigidbody2D rb2D;				//All MovingObjects get a Rigidbody2D.
	private float inverseMoveTime;			//Used to make movement more efficient.
		
	// Initialization function for MovingObject	
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
		
		
	//Move returns true if it is able to move and false if not. 
	//Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
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
		
		
	//Co-routine for moving units from one space to next.
	//Takes a parameter end to specify where to move to.
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
		
	//Takes a generic parameter T to specify the type of component we expect our unit to interact with.
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
		
	//OnCantMove will be overriden by functions in the inheriting classes.
	protected abstract void OnCantMove <T> (T component)
		where T : Component;
}