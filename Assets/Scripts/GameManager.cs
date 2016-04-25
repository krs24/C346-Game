using UnityEngine;
using System.Collections;
using System.Collections.Generic;		
using UnityEngine.UI;					//Allows us to use UI.


/// <summary>
/// This class acts as the controller for the overall game instance. Inherits from MonoBehavior.
/// </summary>	
public class GameManager : MonoBehaviour
{
     /// <summary>
     /// The delay to be used in between levels. Unit is seconds.
     /// </summary>
	public float levelStartDelay = 2f;		
     
     /// <summary>
     /// Delay between each player turn. Unit is seconds. 
     /// </summary>				
	public float turnDelay = 0.1f;

     /// <summary>
     /// Player character's starting health. 
     /// </summary>		
     public int playerHealth = 100;

     /// <summary>
     /// Player character's starting water. 
     /// </summary>		
     public int playerWater = 200;
    
     /// <summary>
     /// Player character's starting food. 
     /// </summary>		
     public int playerFood = 200;

     /// <summary>
     /// Stores a static reference to the current GameManager.
     /// </summary>		
     public static GameManager instance = null;

     /// <summary>
     /// Boolean storing whether it is the player's turn. 
     /// </summary>		
     [HideInInspector] public bool playersTurn = true;

     /// <summary>
     /// Text displaying the current level whenever a new level is loaded. 
     /// </summary>		
     private Text levelText;

     /// <summary>
     /// Stores a reference to the level load screen. 
     /// </summary>		
     private GameObject levelImage;

     /// <summary>
     /// Stores a reference to the instance of the BoardManager being used. 
     /// </summary>		
     private BoardManager boardScript;

     /// <summary>
     /// The current level. 
     /// </summary>		
     private int level = 1;

     /// <summary>
     /// Stores the enemies to be spawned on the level. 
     /// </summary>		
     private List<Enemy> enemies;

     /// <summary>
     /// Boolean to detect if enemies are moving. Used to determine if player or enemy move. 
     /// </summary>		
     private bool enemiesMoving;

     /// <summary>
     /// Boolean to check if we're setting up board. Used to prevent Player movement during setup.
     /// </summary>		
     private bool doingSetup = true;

     /// <summary>
     /// This method is a general UnityEngine method used to initialize data before the game actually starts.
     /// </summary>	
     void Awake()
	{
		//Check if instance already exists
		if (instance == null)
				
			//if not, set instance to this
			instance = this;
			
		//If instance already exists and it's not this:
		else if (instance != this)
				
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);	
			
		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);
			
		//Create new list of Enemy objects.
		enemies = new List<Enemy>();
			
		//Get a component reference BoardManager script
		boardScript = GetComponent<BoardManager>();
			
		//Call the InitGame function to initialize the first level 
		InitGame();
	}
		
	/// <summary>
	/// Takes care minutiae to be done at beginning of level load. 
	/// </summary>
	void OnLevelWasLoaded(int index)
	{
		//Add one to our level number.
		level++;
		//Call InitGame to initialize our level.
		InitGame();
	}
		
	/// <summary>
	/// Initializes the game instance and begins prep for the game to be played. 
	/// </summary>
	void InitGame()
	{
		//doingSetup is true to prevent player movement.
		doingSetup = true;
			
		//Get a reference to LevelImage by finding it by name.
		levelImage = GameObject.Find("LevelImage");
			
		//Get a reference to LevelText's text component by finding it by name and calling GetComponent.
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
			
		//Set the text of levelText to the string "Day" + level.
		levelText.text = "Day " + level;
			
		//Set levelImage to active blocking player's view of the game board during setup.
		levelImage.SetActive(true);
			
		//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
		Invoke("HideLevelImage", levelStartDelay);
			
		//Clear Enemy list to prepare for next level.
		enemies.Clear();
			
		//Call the SetupScene function of the BoardManager script, pass it current level number.
		boardScript.SetupScene(level);
			
	}
		
		
	/// <summary>
	/// Hides the level image when the level is loaded and the player begins to play.
	/// </summary>
	void HideLevelImage()
	{
		//Disable the levelImage gameObject.
		levelImage.SetActive(false);
			
		//Set doingSetup to false allowing player to move again.
		doingSetup = false;
	}
		
	/// <summary>
	/// Updates the scene every frame to reflect all changes made.  
	/// </summary>
	void Update()
	{
		//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
		if(playersTurn || enemiesMoving || doingSetup)
				
			//If any of these are true, return and do not start MoveEnemies.
			return;
			
		//Start moving enemies.
		StartCoroutine (MoveEnemies ());
	}
		
	/// <summary>
	/// Adds enemy to list of enemies so it can be displayed in the game world. 
	/// </summary>
	/// <param name="script">
	/// An instance of the Enemy class to add to the enemies list. 
	/// </param>
	public void AddEnemyToList(Enemy script)
	{
		//Add Enemy to List enemies.
		enemies.Add(script);
	}
		
		
	/// <summary>
	/// GameOver is run whenever the player health >= 0. Starts the death screen. 
	/// </summary>
	public void GameOver()
	{
		//Set levelText to display number of levels passed and game over message
		levelText.text = "After " + level + " days, you starved.";
			
		//Enable black background image gameObject.
		levelImage.SetActive(true);
			
		//Disable this GameManager.
		enabled = false;
	}
		
	/// <summary>
	/// Coroutine used to aid in enemy movement. 
	/// </summary>
	/// <returns>
	/// Returns an IEnumerator causing the Enemy to wait for a specified time before moving again.
	/// </returns>
	IEnumerator MoveEnemies()
	{
		//While enemiesMoving is true player is unable to move.
		enemiesMoving = true;
			
		//Wait for turnDelay seconds, defaults to .1 (100 ms).
		yield return new WaitForSeconds(turnDelay);
			
		//If there are no enemies spawned (IE in first level):
		if (enemies.Count == 0) 
		{
			//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
			yield return new WaitForSeconds(turnDelay);
		}
			
		//Loop through List of Enemy objects.
		for (int i = 0; i < enemies.Count; i++)
		{
			//Call the MoveEnemy function of Enemy at index i in the enemies List.
			enemies[i].MoveEnemy ();
				
			//Wait for Enemy's moveTime before moving next Enemy, 
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		//Once Enemies are done moving, set playersTurn to true so player can move.
		playersTurn = true;
			
		//Enemies are done moving, set enemiesMoving to false.
		enemiesMoving = false;
	}
}


