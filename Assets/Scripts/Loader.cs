using UnityEngine;
using System.Collections;
/// <summary>
/// This class acts as the inital loader to begin running the GameManager and SoundManager. Its sole purpose is
/// to instantiate the instances of these two classes used in the game. Inherits from MonoBehavior.
/// </summary>
public class Loader : MonoBehaviour 
{
     /// <summary>
     /// Stores an instance of the GameManager prefab.
     /// </summary>
	public GameObject gameManager;	
     
     /// <summary>
     /// Stores an instance of the SoundManager prefab.
     /// </summary>		
	public GameObject soundManager;              

     /// <summary>
     /// This method is a general UnityEngine method used to initialize data before the game actually starts.
     /// </summary>		
     void Awake ()
	{
		//Check if GameManager.instance has been assigned a GameObject.
		if (GameManager.instance == null)
			Instantiate(gameManager);		//Instantiate gameManager prefab
			
		//Check if SoundManager.instance has been assigned a GameObject.
		if (SoundManager.instance == null)
			Instantiate(soundManager);		//Instantiate SoundManager prefab
	}
}