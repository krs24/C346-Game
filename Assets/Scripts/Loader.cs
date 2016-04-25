using UnityEngine;
using System.Collections;
/// <summary>
/// This class acts as the inital loader to begin running the GameManager and SoundManager. Its sole purpose is
/// to instantiate the instances of these two classes used in the game. 
/// </summary>
public class Loader : MonoBehaviour 
{
	public GameObject gameManager;			//GameManager prefab to instantiate.
	public GameObject soundManager;              //SoundManager prefab to instantiate.

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