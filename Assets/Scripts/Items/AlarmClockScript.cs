using UnityEngine;

public class AlarmClockScript : MonoBehaviour
{
    private void Start()
    {
        this.timeLeft = 30f;
        this.lifeSpan = 35f;
    }

    private void Update()
    {
        if (this.timeLeft >= 0f) //If the time is greater then 0
        {
            this.timeLeft -= Time.deltaTime; //Decrease the time variable
        }
        else if (!this.rang) // If it has not been rang
        {
            this.Alarm(); // Start the alarm function
        }
        if (this.lifeSpan >= 0f) //If the time left in the lifespan is greater then 0
        {
            this.lifeSpan -= Time.deltaTime; //Decrease the time variable
        }
        else
        {
            UnityEngine.Object.Destroy(base.gameObject, 0f); //Otherwise, if time is less then 0, destroy the alarm clock
        }
    }

    private void Alarm()
    {
        this.rang = true;
        if (this.baldi.isActiveAndEnabled) this.baldi.Hear(base.transform.position, 8f); //Baldi is told to go to this location, with a priority of 10(above most sounds)
        this.audioDevice.clip = this.ring;
        this.audioDevice.loop = false; // Tells the audio not to loop
        this.audioDevice.Play(); //Play the audio
    }

    public float timeLeft;

    private float lifeSpan;

    private bool rang;

    public BaldiScript baldi;

    public AudioClip ring;

    public AudioSource audioDevice;
}