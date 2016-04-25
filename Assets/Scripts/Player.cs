using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This the main controller script for Players in the game. Inherits from MovingObject 
/// </summary>
public class Player : MovingObject
{
	/// <summary>
	/// Delay time in seconds to restart level.
	/// </summary>
	public float restartLevelDelay = 1f;        

	/// <summary>
	/// Delay time to drop loot delay.
	/// </summary>	
	public float lootDelay = 0.01f; 
		
	/// <summary>
	/// Points gained for normal food.
	/// </summary>
	public int pointsPerFood = 10;				
	
	/// <summary>
	/// Points gained for normal drink.
	/// </summary>
	public int pointsPerSoda = 15;				
   
  	/// <summary>
	/// Points gained for sirloin.
	/// </summary>
	public int pointsPerSirloin = 40;
    
	/// <summary>
	/// Points gained for enemy dropped water.
	/// </summary>
	public int pointsPerWater = 40;
	
	/// <summary>
	/// The amount of damage done to enemy units when the player attacks.
	/// </summary>
	public int damage = 20;					
	
	/// <summary>
	/// Text to display the current amount of health. 
	/// </summary>
	public Text healthText;					
    
	/// <summary>
	/// Text to display the current amount of food.
	/// </summary>
	public Text foodText;
    
	/// <summary>
	/// Text to display the current amount of water.
	/// </summary>
	public Text waterText;

	/// <summary>
	/// First movement sound. 
	/// </summary>
	public AudioClip moveSound1;		
	
	/// <summary>
	///  First movement sound. 
	/// </summary>
	public AudioClip moveSound2;
	
	/// <summary>
	/// First eat sound.
	/// </summary>
	public AudioClip eatSound1;					
	
	/// <summary>
	///  Second eat sound.
	/// </summary>
	public AudioClip eatSound2;					
	
	/// <summary>
	///  First drink sound.
	/// </summary>
	public AudioClip drinkSound1;				
	
	/// <summary>
	///  Second drink sound.
	/// </summary>
	public AudioClip drinkSound2;				
	
	/// <summary>
	/// Stores a reference to the sound played when the player dies.
	/// </summary>
	public AudioClip gameOverSound;	
    
	/// <summary>
	/// Stores reference to the health slider. 
	/// </summary>
	public Slider HealthSlider;
    
	/// <summary>
	/// Stores reference to the water slider. 
	/// </summary>
	public Slider WaterSlider;
    
	/// <summary>
	/// Stores reference to the food slider. 
	/// </summary>
	public Slider FoodSlider;
    
	/// <summary>
	/// Array storing enemy loot.
	/// </summary>
	public GameObject[] enemyLoot;

	/// <summary>
	/// Stores a reference to the animator for the player. 
	/// </summary>
    private Animator animator;                  
    
	/// <summary>
	/// The player's current health. 
	/// </summary>
	private int health;                      
    
	/// <summary>
	/// The player's current food amount. 
	/// </summary>
	private int food;
    
	/// <summary>
	/// The player's current water amount
	/// </summary>
	private int water;
        
	/// <summary>
	/// The player's last known position.
	/// </summary>
	private Vector3 lastPos;


	/// <summary>
	/// Overrides the base class' Start method. 
	/// </summary>
    protected override void Start ()
	{
		//Get reference to the Player's animator
		animator = GetComponent<Animator>();
			
		//Get Player's health.
		health = GameManager.instance.playerHealth;
        water = GameManager.instance.playerWater;
        food = GameManager.instance.playerFood;

        UpdateHUDSliders();
			
		//Call the Start function of the MovingObject base class.
		base.Start ();
	}
		
	/// <summary>
	/// When the player is disabled this method stores the current states in the G
	/// </summary>
	private void OnDisable ()
	{
		//Store player health when player object disabled.
		GameManager.instance.playerHealth = health;
        GameManager.instance.playerWater = water;
        GameManager.instance.playerFood = food;
	}
	
	/// <summary>
	/// Updates the players actions every frame.
	/// </summary>
	private void Update ()
	{
		//If it's not the player's turn, exit the function.
		if(!GameManager.instance.playersTurn) return;
			
		int horizontal = 0;  	//Horizontal move direction.
		int vertical = 0;		//Vertical move direction.
			
		//Get input from the input manager, round it to an integer and 
		//store in horizontal to set x axis move direction
		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
		//Get input from the input manager, round it to an integer and 
		//store in vertical to set y axis move direction
		vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
		//Check if moving horizontally, if so set vertical to zero.
		if(horizontal != 0)
		{
			vertical = 0;
		}

		//Check if we have a non-zero value for horizontal or vertical
		if(horizontal != 0 || vertical != 0)
		{
			//Call AttemptMove and pass Enemy
			AttemptMove<Enemy> (horizontal, vertical);
		}
	}
		
	/// <summary>
	/// Overrides the base class' AttemptMove to provide Player specific functionality.
	/// </summary>
	/// <param name="xDir">
	/// X direction the player is trying to move.
	/// </param>
	/// <param name="yDir">
	/// Y direction the player is trying to move.
	/// </param>
	protected override void AttemptMove <T> (int xDir, int yDir)
	{

        //Call MovingObject's AttemptMove and pass Enemy.
        base.AttemptMove <T> (xDir, yDir);
			
		//Use for result of linecast
		RaycastHit2D hit;

        lastPos = gameObject.transform.position;
		//If Player can move.
		if (Move (xDir, yDir, out hit)) 
		{
			//Play one of the move sounds.
			SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
            water -= 2;
            food -= 2;
           
            if(water <= 0)
            {
                water = 0;
                health -= 2;
            }
            else if(food <= 0)
            {
                food = 0;
                health -= 1;
            }
            else
            {
                health += 1;
            }
            if (health >= 100)
                health = 100;
            if (water >= 200)
                water = 200;
            if (food >= 200)
                food = 200;
            UpdateHUDSliders();
		}
            
        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        GameManager.instance.playersTurn = false;
    }
		
		
	/// <summary>
	/// If the player can't move because of a collision this method causes the player to take 
	/// appropriate action based on what it has collided with. Overrides the base class' OnCantMove.
	/// </summary>
	/// <param name="component">
	/// The type of component the player has collided with.
	/// </param>
	protected override void OnCantMove <T> (T component)
	{
		//Set hitWall to equal the component passed in as a parameter.
		Enemy hitEnemy = component as Enemy;
			
		//Call the DamageEnemy function of the Enemy we are hitting.
		hitEnemy.DamageEnemy (damage);
			
		//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
		animator.SetTrigger ("playerChop");
        if (hitEnemy.hp <= 0)
               Invoke("DropLoot", lootDelay);
	}
		
		
	/// <summary>
	/// If the object is on another reference level than the player has this method controls what happens.
	/// </summary>
	/// <param name="other">
	/// The 2D object the player has collided with.
	/// </param>
	private void OnTriggerEnter2D (Collider2D other)
	{
		//Check if the tag of the trigger collided with is Exit.
		if(other.tag == "Exit")
		{
			//Start the next level with a delay of restartLevelDelay (default 1 second).
			Invoke ("Restart", restartLevelDelay);

            //Disable the player object since level is over.
            enabled = false;
		}
			
		//Check if the tag of the trigger collided with is Food.
		else if(other.tag == "Food")
		{
            food += pointsPerFood;
            if (food >= 200)
                food = 200;
            UpdateHUDSliders();
				
			//Play an eating sound.
			SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
				
			//Disable the food object.
			other.gameObject.SetActive (false);
		}
          else if (other.tag == "Sirloin")
          {
               food += pointsPerSirloin;
               if (food >= 200)
                    food = 200;
               UpdateHUDSliders();

               //Play an eating sound.
               SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

               //Disable the food object.
               other.gameObject.SetActive(false);
          }
          //Check if the tag of the trigger collided with is Soda.
          else if(other.tag == "Soda")
		{
            water += pointsPerSoda;
            if (water >= 200)
                water = 200;
            UpdateHUDSliders();
				
			//Play one of the drinking sounds.
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
				
			//Disable the soda object.
			other.gameObject.SetActive (false);
		}
          else if (other.tag == "Water")
          {
               water += pointsPerWater;
               if (water >= 200)
                    water = 200;
               UpdateHUDSliders();

               //Play one of the drinking sounds.
               SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

               //Disable the soda object.
               other.gameObject.SetActive(false);
          }
     }
		
		
	/// <summary>
	/// If the player goes to the exit this starts the next level. 
	/// </summary>
	private void Restart ()
	{
          //Load the last scene loaded, in this case Main, the only scene in the game.
          Application.LoadLevel(Application.loadedLevel);
     }
		
		
	/// <summary>
	/// When an enemy attacks a player this method applies the damage. 
	/// </summary>
	/// <param name="loss">
	/// The amount of damage to apply.
	/// </param>
	public void LoseHealth (int loss)
	{
		//Play player hit animation
		animator.SetTrigger ("playerHit");
			
		//Subtract lost health
		health -= loss;
        UpdateHUDSliders();
			
		//Check to see if game has ended.
		CheckIfGameOver ();
	}
		
		
	/// <summary>
	/// If the player is dead this calls the appropriate method from the GameManager. 
	/// </summary>
	private void CheckIfGameOver ()
	{
		//Check if health is <= zero
		if (health <= 0) 
		{
			//Play game over sound.
			SoundManager.instance.PlaySingle (gameOverSound);
				
			//Stop the background music.
			SoundManager.instance.musicSource.Stop();
				
			//Call the GameOver function of GameManager.
			GameManager.instance.GameOver ();
		}
	}

	/// <summary>
	/// Updates the HUD whenever the values are changed. 
	/// </summary>
    public void UpdateHUDSliders()
    {
        HealthSlider.value = health;
        WaterSlider.value = water;
        FoodSlider.value = food;

        healthText.text = "" + health;
        waterText.text = "" + water;
        foodText.text = "" + food;
    }

	/// <summary>
	/// When an enemy is killed selects a piece of loot to drop.
	/// </summary>
    public void DropLoot()
    {
        Instantiate(enemyLoot[Random.Range(0, enemyLoot.Length)], lastPos, Quaternion.identity);
    }
}

