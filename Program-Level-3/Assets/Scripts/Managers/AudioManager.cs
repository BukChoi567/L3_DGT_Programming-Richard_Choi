using System.Runtime.CompilerServices;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] hitAudio; // using an array to hold multiple hit sound effects
    public AudioClip solidHitAudio; // sound effect for hitting solid objects
    public AudioClip badHitAudio; // sound effect for hitting bad objects
    private AudioSource audioSource; // AudioSource component to play sounds
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject); // Persist through scenes
    }

    public void PlaySolidHitAudio() // Play sound when hitting solid objects
    {
        audioSource.PlayOneShot(solidHitAudio);
    }
    public void PlayBadHitAudio() // Play sound when hitting bad objects
    {
        audioSource.PlayOneShot(badHitAudio);
    }
    public void PlayRandomHitAudio() // Play a random hit sound from the array for when hitting target blocks
    {

        int randomIndex = Random.Range(0, hitAudio.Length); // Get a random number between 0 and length of array
        audioSource.PlayOneShot(hitAudio[randomIndex]); // Play the randomly selected audio clip
    }
    
    void Update()
    {
        
    }
    
}
