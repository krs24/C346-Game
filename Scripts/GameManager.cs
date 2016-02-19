using UnityEngine;
using System.Collections;
using System.Collections.Generic;		
using UnityEngine.UI;		
	
public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f;			//Time to wait before starting level in seconds.
	public float turnDelay = 0.1f;				//Delay between each Player turn.
	public int playerHealth = 100;				//Starting value for Player health.
	public static GameManager instance = null;		//Static instance of GameManager which allows it to be accessed by any other script.
	[HideInInspector] public bool playersTurn = true;	//Boolean to check if it's player's turn.
		
		
	private Text levelText;					//Text to display current level number.
	private GameObject levelImage;				//Image to block out level as levels are being set up, background for levelText.
	private BoardManager boardScript;			//Store a reference to our BoardManager which will set up the level.
	private int level = 1;					//Current level number, expressed in game as "Day 1".
	private List<Enemy> enemies;				//List of all Enemy units, used to issue them move commands.
	private bool enemiesMoving;				//Boolean to check if enemies are moving.
	private bool doingSetup = true;				//Boolean to check if we're setting up board, prevent Player movement during setup.
		
		
		
	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists. If not set it to this instance.
		if (instance == null)
			instance = this;
			
		//If instance is not this then destroy this
		else if (instance != this)
			Destroy(gameObject);	
			
		//Don't destroy this when reloading scene
		DontDestroyOnLoad(gameObject);
			
		enemies = new List<Enemy>();
			
		//Get a reference to BoardManager script
		boardScript = GetComponent<BoardManager>();
			
		//Initialize first level 
		InitGame();
	}
		
	//This is called each time a scene is loaded.
	void OnLevelWasLoaded(int index)
	{
		level++;	//Add one to our level number.
		InitGame();    	//Initialize our level.
	}
		
	//Initializes the game for each level.
	void InitGame()
	{
		//doingSetup is true to prevent player movement.
		doingSetup = true;
			
		//Get a reference to LevelImage by finding it by name.
		levelImage = GameObject.Find("LevelImage");
			
		//Get a reference to LevelText's text component 
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
		
		
	//Hides black image used between levels
	void HideLevelImage()
	{
		levelImage.SetActive(false);  	//Make game board viewable again.
		doingSetup = false;		//Allow player to move again.
	}
		
	//Update is called every frame.
	void Update()
	{
		//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
		if(playersTurn || enemiesMoving || doingSetup)
				
			//If any of these are true, return and do not start MoveEnemies.
			return;
			
		//Start moving enemies.
		StartCoroutine (MoveEnemies ());
	}
		
	//Add Enemy to List of Enemy objects
	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}
		
		
	//GameOver is called when the player reaches 0 health
	public void GameOver()
	{
		//Set levelText to display number of levels passed and game over message
		levelText.text = "After " + level + " days, you starved.";
			
		//Enable black background image gameObject.
		levelImage.SetActive(true);
			
		//Disable this GameManager.
		enabled = false;
	}
		
	//Coroutine to move enemies in sequence.
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
			
		//Loop through List of Enemy objects and move them.
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].MoveEnemy ();
				
			//Wait for Enemy's moveTime before moving next Enemy, 
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		//Enemies are done moving, sow allow player to move again.
		playersTurn = true;
			
		//Enemies are done moving, set enemiesMoving to false.
		enemiesMoving = false;
	}
}
