using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// This the main controller script for Enemies in the game. Inherits from MovingObject 
/// </summary>
public class Enemy : MovingObject
{
	/// <summary>
	/// Amount of health to subtract from player.
	/// </summary>
	public int playerDamage; 							
	
	/// <summary>
	/// Stores the first enemy attack sound.
	/// </summary>
	public AudioClip attackSound1;						
	
	/// <summary>
	/// Stores the second enemy attack sound.
	/// </summary>
	public AudioClip attackSound2;					
	
	/// <summary>
	/// Stores the enemy's current health.
	/// </summary>
	public int hp;										//Enemy health
	
	/// <summary>
	/// Stores the first player attack sound.
	/// </summary>
	public AudioClip chopSound1;
	
	/// <summary>
	/// Stores the second player attack sound.
	/// </summary>
	public AudioClip chopSound2;
	
	/// <summary>
	/// Stores special loot to be dropped upon enemy death.
	/// </summary>
	public GameObject[] enemyLoot;
	
	/// <summary>
	/// Stores the player object's current position in the world. 
	/// </summary>
    public Vector3 playerPos;
	
	/// <summary>
	/// Stores a reference to enemy's Animator component.
	/// </summary>
    private Animator animator;							
	
	/// <summary>
	/// Transform of the player object's position. Enemies attempt to move toward the player each turn.
	/// </summary>
	private Transform target;						
	
	/// <summary>
	/// Enemies move one turn for every two player turns. Stores whether it is the enemy's turn or not.
	/// </summary>
	private bool skipMove;						
		
	/// <summary>
	/// Overrides the base class' Start method. 
	/// </summary>
	protected override void Start ()
	{
		//Add this enemy to list of enemies
		GameManager.instance.AddEnemyToList (this);		
			
		//Get reference to the attached Animator.
		animator = GetComponent<Animator> ();
			
		//Store reference to Player's transform.
		target = GameObject.FindGameObjectWithTag ("Player").transform;
			
		//Call MovingObject's start.
		base.Start ();
	}
		
		
	/// <summary>
	/// Overrides the base class' AttemptMove to provide enemy specific functionality.
	/// </summary>
	/// <param name="xDir">
	/// X direction the object is trying to move.
	/// </param>
	/// <param name="yDir">
	/// Y direction the object is trying to move.
	/// </param>
	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		//Check if skipMove is true, if so set it to false and skip this turn.
		if(skipMove)
		{
			skipMove = false;
			return;	
		}
			
		//Call MovingObject's AttemptMove.
		base.AttemptMove <T> (xDir, yDir);
			
		//Enemy is done moving
		skipMove = true;
	}
		
		
	/// <summary>
	/// Moves the enemy toward the player each enemy turn. 
	/// </summary>
	public void MoveEnemy ()
	{
		//Declare variables for X and Y axis move directions, these range from -1 to 1.
		//These values allow us to choose between the cardinal directions: up, down, left and right.
		int xDir = 0;
		int yDir = 0;
			
		//If the difference in positions is less than Epsilon
		if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
				
			//If the y coordinate of the target's (player) position is greater 
			//than the y coordinate of this enemy's position set y direction to 1 
			// (to move up). If not, set it to -1 (to move down).
			yDir = target.position.y > transform.position.y ? 1 : -1;
			
		//If the difference in positions is greater than Epsilon
		else
			//Check if target x position is greater than enemy's x position, if so 
			//set x direction to 1 (move right), if not set to -1 (move left).
			xDir = target.position.x > transform.position.x ? 1 : -1;
			
		//Call the AttemptMove and pass Player
		AttemptMove <Player> (xDir, yDir);
	}
		
	/// <summary>
	/// If the enemy can't move because of a collision this method causes the enemy to take 
	/// appropriate action based on what it has collided with. Overrides the base class' OnCantMove.
	/// </summary>
	/// <param name="component">
	/// The type of component the enemy has collided with.
	/// </param>
	protected override void OnCantMove <T> (T component)
	{
		//Set hitPlayer equal to the encountered component.
		Player hitPlayer = component as Player;
			
		//Damager the player
		hitPlayer.LoseHealth (playerDamage);
			
		//Play eneemy attack animation.
		animator.SetTrigger ("enemyAttack");
			
		//Pick one of the enemy attack sounds.
		SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
	}

	/// <summary>
	/// Subtracts enemy health whenever the enemy is attacked by the player. 
	/// </summary>
	/// <param name="loss">
	/// The amount of HP to subtract from the enemy's health.
	/// </param>
	public int ApplyDamage(int loss) {
		hp -= loss;
		return hp;
	}

	/// <summary>
	/// When the player attacks the enemy this method runs the logic to apply any damage done by the player.
	/// </summary>
	/// <param name="loss">
	/// The amount of damage to be applied. 
	/// </param>
	public void DamageEnemy (int loss)
	{
        Player player = GameObject.Find("Player").GetComponent<Player>();
        //Play one of the player chop sounds.
        SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);

		//Apply damamge to this enemy.
		int newhp = ApplyDamage(loss);
        //Once the enemy is dead disable its gameObject:
        if (newhp <= 0)
            gameObject.SetActive(false);
        
        
    }

}
