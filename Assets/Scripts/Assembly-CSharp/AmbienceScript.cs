using UnityEngine;

public class AmbienceScript : MonoBehaviour
{
    private void Start()
    {
    }

    public void PlayAudio()
    {
        int num = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 49f));
        if (!this.audioDevice.isPlaying & num == 0) // If it is not currently playing an audio device, and num is equal to 0 (1/50 chance)
        {
            base.transform.position = this.aiLocation.position; // Go to the location of the AILocation object
            int num2 = Mathf.RoundToInt(UnityEngine.Random.Range(0f, (float)(this.sounds.Length - 1))); // Choose a random number for playing a sound
            this.audioDevice.PlayOneShot(this.sounds[num2]); // Play the sound
        }
    }

    public Transform aiLocation;

    public AudioClip[] sounds;

    public AudioSource audioDevice;
}