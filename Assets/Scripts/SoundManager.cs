using UnityEngine;
using System.Collections;

/// <summary>
/// This class acts as the sound controller for most sound related actions in the game. 
/// Inherits from MonoBehavior.
/// </summary>
public class SoundManager : MonoBehaviour 
{
     /// <summary>
     /// Variable to store the audio source for sound effects.
     /// </summary>
	public AudioSource efxSource;
     /// <summary>
     /// Variable to store the audio source for background music.
     /// </summary>				
     public AudioSource musicSource;					
     /// <summary>
     /// Static instance of the sound manager.
     /// </summary>
	public static SoundManager instance = null;
     /// <summary>
     /// Variable for lower end of pitch modulation.
     /// </summary>				
     public float lowPitchRange = .95f;
     /// <summary>
     /// Variable for higher end of pitch modulation.
     /// </summary>
     public float highPitchRange = 1.05f;			
		
     
	/// <summary>
     /// This method is a general UnityEngine method used to initialize data before the game actually starts.
     /// </summary>	
	void Awake ()
	{
		//Check if there is already an instance of SoundManager
		if (instance == null)
			//if not, set it to this.
			instance = this;
		//If instance already exists:
		else if (instance != this)
			//Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
			Destroy (gameObject);
			
		//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		DontDestroyOnLoad (gameObject);
	}

     /// <summary>
     /// This method initiates the playing of single sound audio clips.
     /// </summary>	
     /// <param name="clip">
     /// AudioClip to be played.
     /// </param> 
     public void PlaySingle(AudioClip clip)
	{
		//Set the clip of our efxSource audio source to the clip passed in as a parameter.
		efxSource.clip = clip;
			
		//Play the clip.
		efxSource.Play ();
	}

     /// <summary>
     /// Chooses randomly between various audio clips and slightly changes their pitch
     /// </summary>	
     /// <param name="clips">
     /// Array containing audio clips to choose from. 
     /// </param>
     public void RandomizeSfx (params AudioClip[] clips)
	{
		//Generate a random number between 0 and the length of our array of clips passed in.
		int randomIndex = Random.Range(0, clips.Length);
			
		//Choose a random pitch to play back our clip at between our high and low pitch ranges.
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);
			
		//Set the pitch of the audio source to the randomly chosen pitch.
		efxSource.pitch = randomPitch;
			
		//Set the clip to the clip at our randomly chosen index.
		efxSource.clip = clips[randomIndex];
			
		//Play the clip.
		efxSource.Play();
	}
}

