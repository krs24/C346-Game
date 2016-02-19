using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour 
{
	public GameObject gameManager;			//GameManager prefab to instantiate.
	public GameObject soundManager;			//SoundManager prefab to instantiate.
		
		
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