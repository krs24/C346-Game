using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject
{
	public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
    public float lootDelay = 0.01f; 
	public int pointsPerFood = 10;				//Number of points to add to player food points when picking up a food object.
	public int pointsPerSoda = 15;				//Number of points to add to player food points when picking up a soda object.
     public int pointsPerSirloin = 40;
     public int pointsPerWater = 40;
	public int damage = 20;						//How much damage a player does to a wall when chopping it.
	public Text healthText;						//UI Text to display current player food total.
    public Text foodText;
    public Text waterText;
	public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
	public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
	public AudioClip eatSound1;					//1 of 2 Audio clips to play when player collects a food object.
	public AudioClip eatSound2;					//2 of 2 Audio clips to play when player collects a food object.
	public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
	public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
	public AudioClip gameOverSound;				//Audio clip to play when player dies.
    public Slider HealthSlider;
    public Slider WaterSlider;
    public Slider FoodSlider;
    public GameObject[] enemyLoot;

    private Animator animator;                  //Used to store a reference to the Player's animator component.
    private int health;                      //Used to store player food points total during level.		
    private int food;
    private int water;
    private Vector3 currentPos;
    private Vector3 lastPos;


    //Override MovingObject Start
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
		
	//Called when Player object is disabled.
	private void OnDisable ()
	{
		//Store player health when player object disabled.
		GameManager.instance.playerHealth = health;
        GameManager.instance.playerWater = water;
        GameManager.instance.playerFood = food;
	}
	
	//Update every frame
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
		
	//Attempt to move the Player and check if collision with Enemy
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
		
		
	//OnCantMove overrides OnCantMove in MovingObject and performs appropriate action on object collided with.
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
		
		
	//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
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
		
		
	//Restart reloads the scene when called.
	private void Restart ()
	{
          //Load the last scene loaded, in this case Main, the only scene in the game.
          Application.LoadLevel(Application.loadedLevel);
     }
		
		
	//LoseHealth is called when an enemy attacks the player.
	//It takes a parameter loss which specifies how many points to lose.
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
		
		
	//Check if player is dead and game is over.
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

    public void UpdateHUDSliders()
    {
        HealthSlider.value = health;
        WaterSlider.value = water;
        FoodSlider.value = food;

        healthText.text = "" + health;
        waterText.text = "" + water;
        foodText.text = "" + food;
    }

    public void DropLoot()
    {
        Instantiate(enemyLoot[Random.Range(0, enemyLoot.Length)], lastPos, Quaternion.identity);
    }
}

