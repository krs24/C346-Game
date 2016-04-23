using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MovingObject
{
	public int playerDamage; 							//Amount of health to subtract from player.
	public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
	public AudioClip attackSound2;						//Second of two audio clips to play when attacking the player.
	public int hp;										//Enemy health
	public AudioClip chopSound1;						//1 of 2 audio clips for when player attacks enemy.
	public AudioClip chopSound2;						//2 of 2 audio clips for when player attacks enemy.	
    public GameObject[] enemyLoot;
    public Vector3 playerPos;
    private Animator animator;							//Stores a reference to enemy's Animator component.
	private Transform target;							//Transform to attempt to move toward each turn.
	private bool skipMove;								//Boolean to determine whether or not enemy should skip a turn or move this turn.		
		
	//Override MovingObject's start.
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
		
		
	//Override MovingObject's AttemptMove
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
		
		
	//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
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
		
		
	//OnCantMove is called if Enemy attempts to move into a space occupied by a Player.
	//It overrides the OnCantMove function of MovingObject.
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

	//Reduces enemy hp when attacked
	public int ApplyDamage(int loss) {
		hp -= loss;
		return hp;
	}

	//Called when player attacks an enemy.
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
