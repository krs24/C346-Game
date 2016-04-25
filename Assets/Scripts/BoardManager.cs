using UnityEngine;
using System;
using System.Collections.Generic; 		
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.
using UnityEngine.UI;

/// <summary>
/// This class takes care of setting up the game world. Inherits from MonoBehaviour.
/// </summary>
public class BoardManager : MonoBehaviour
{
	/// <summary>
	/// Internal class used for storing a count range.
	/// </summary>
	[Serializable]
	public class Count
	{
		/// <summary>
		/// Stores the minimum number for the range.
		/// </summary>
		public int minimum; 	\

		/// <summary>
		/// Stores the maximum number for the range. 
		/// </summary>		
		public int maximum; 			
			
			
		/// <summary>
		/// Basic constructor. 
		/// </summary>
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}
		

	/// <summary>
	/// Number of columns in game board.
	/// </summary>
	public int columns = 8; 
	
	/// <summary>
	/// Number of rows in game board.
	/// </summary>	
	public int rows = 8;	
	
	/// <summary>
	/// Lower and upper limit for shrubs and walls.
	/// </summary>
	public Count wallCount = new Count (5, 9);		
	
	/// <summary>
	/// Lower and upper limit for food items.
	/// </summary>
	public Count foodCount = new Count (1, 5);						
   
	/// <summary>
	/// Lower and upper limit for water items. 
	/// </summary>
    public Count waterCount = new Count(1, 5);
	
	/// <summary>
	/// The exit object. 
	/// </summary>
	public GameObject exit;											
	
	/// <summary>
	/// Array storing floor tile prefabs. 
	/// </summary>
	public GameObject[] floorTiles;									
	
	/// <summary>
	/// Array storing wall tile prefabs. 
	/// </summary>
	public GameObject[] wallTiles;									
	
	/// <summary>
	/// Array storing food tile prefabs. 
	/// </summary>
	public GameObject[] foodTiles;								
    
	/// <summary>
	/// Array storing water tile prefabs. 
	/// </summary>
	public GameObject[] waterTiles;
	
	/// <summary>
	/// Array storing enemy tile prefabs. 
	/// </summary>
	public GameObject[] enemyTiles;								
	
	/// <summary>
	/// NArray storing outer wall tile prefabs. 
	/// </summary>
	public GameObject[] outerWallTiles;								
		
	/// <summary>
	/// A variable to store a reference to the transform of our Board object.
	/// </summary>
	private Transform boardHolder;		
	
	/// <summary>
	/// A list of possible locations to place tiles.
	/// </summary>
	private List <Vector3> gridPositions = new List <Vector3> ();	
		
		
	/// <summary>
	/// Initializes the board array.
	/// </summary>
	void InitialiseList ()
	{
		//Clear our list of gridPositions.
		gridPositions.Clear ();
			
		//Loop through x axis (columns).
		for(int x = 1; x < columns-1; x++)
		{
			//Within each column, loop through y axis (rows).
			for(int y = 1; y < rows-1; y++)
			{
				//At each index add a new Vector3 to our list with the x and y coordinates of that position.
				gridPositions.Add (new Vector3(x, y, 0f));
			}
		}
	}
		
	/// <summary>
	/// Sets up the outer walls and floor (background) of the game board.
	/// </summary>
	void BoardSetup ()
	{
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;
			
		//Loop along x axis, starting from -1 with floor or outerwall edge tiles.
		for(int x = -1; x < columns + 1; x++)
		{
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for(int y = -1; y < rows + 1; y++)
			{
				//Choose a floor tile.
				GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
					
				//Check if current position is at board edge, if so choose an outer wall tile.
				if(x == -1 || x == columns || y == -1 || y == rows)
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					
				//Instantiate the GameObject using the chosen prefab at the current grid position.
				GameObject instance =
					Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					
				//Set the parent of our newly instantiated object instance to boardHolder.
				instance.transform.SetParent (boardHolder);
			}
		}
	}
		
	/// <summary>
	/// Calculates a random position on the grid.
	/// </summary>	
	/// <returns>
	/// A Vector3 representing the random grid position. 
	/// </returns>
	Vector3 RandomPosition ()
	{
		//Random number between 0 and the count of items in gridPositions.
		int randomIndex = Random.Range (0, gridPositions.Count);
			
		//randomIndex from gridPositions.
		Vector3 randomPosition = gridPositions[randomIndex];
			
		//Remove gridPositions[randomIndex] since it already has a tile.
		gridPositions.RemoveAt (randomIndex);
			
		//Return the randomly selected Vector3 position.
		return randomPosition;
	}
		
	/// <summary>
	/// Calculates a random position on the grid.
	/// </summary>	
	/// <param name="tileArray">
	/// The tile array to fill.
	/// </param>
	/// <param name="minimum">
	/// The minimum number of corresponding items to insert. 
	/// </param>
	/// <param name="maximum">
	/// The maximum number of corresponding items to insert.
	/// </param>
	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		//Choose a random number of objects to instantiate within the minimum and maximum limits
		int objectCount = Random.Range (minimum, maximum+1);
			
		//Instantiate objects until the randomly chosen limit objectCount is reached
		for(int i = 0; i < objectCount; i++)
		{
			//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
			Vector3 randomPosition = RandomPosition();
				
			//Choose a random tile from tileArray and assign it to tileChoice
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				
			//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}
		
	/// <summary>
	/// SetupScene initializes our level and calls the previous functions to lay out the game board
	/// </summary>	
	/// <param name="level">
	/// The current level number. 
	/// </param>		
	public void SetupScene(int level)
	{
		//Creates the outer walls and floor.
		BoardSetup ();
			
		//Reset our list of gridpositions.
		InitialiseList ();
			
		//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
			
		//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);

        //Instantiate a random number of water tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(waterTiles, waterCount.minimum, waterCount.maximum);

        //Determine number of enemies based on current level number, based on a logarithmic progression
        int enemyCount = (int)Mathf.Log(level, 2f);
			
		//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
			
		//Instantiate the exit tile in the upper right hand corner of our game board
		Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
	}
}