using UnityEngine;
using UnityEngine.AI;

public class BaldiScript : MonoBehaviour
{
    private void Start()
    {
        this.baldiAudio = base.GetComponent<AudioSource>(); //Get The Baldi Audio Source(Used mostly for the slap sound)
        this.agent = base.GetComponent<NavMeshAgent>(); //Get the Nav Mesh Agent
        this.timeToMove = this.baseTime; //Sets timeToMove to baseTime
        this.Wander(); //Start wandering
        if (PlayerPrefs.GetInt("Rumble") == 1)
        {
            this.rumble = true;
        }
    }

    private void Update()
    {
        if (this.timeToMove > 0f) //If timeToMove is greater then 0, decrease it
        {
            this.timeToMove -= 1f * Time.deltaTime;
        }
        else
        {
            this.Move(); //Start moving
        }
        if (this.coolDown > 0f) //If coolDown is greater then 0, decrease it
        {
            this.coolDown -= 1f * Time.deltaTime;
        }
        if (this.baldiTempAnger > 0f) //Slowly decrease Baldi's temporary anger over time.
        {
            this.baldiTempAnger -= 0.02f * Time.deltaTime;
        }
        else
        {
            this.baldiTempAnger = 0f; //Cap its lowest value at 0
        }
        if (this.antiHearingTime > 0f) //Decrease antiHearingTime, then when it runs out stop the effects of the antiHearing tape
        {
            this.antiHearingTime -= Time.deltaTime;
        }
        else
        {
            this.antiHearing = false;
        }
        if (this.endless) //Only activate if the player is playing on endless mode
        {
            if (this.timeToAnger > 0f) //Decrease the timeToAnger
            {
                this.timeToAnger -= 1f * Time.deltaTime;
            }
            else
            {
                this.timeToAnger = this.angerFrequency; //Set timeToAnger to angerFrequency
                this.GetAngry(this.angerRate); //Get angry based on angerRate
                this.angerRate += this.angerRateRate; //Increase angerRate for next time
            }
        }
    }

    private void FixedUpdate()
    {
        if (this.moveFrames > 0f) //Move for a certain amount of frames, and then stop moving.(Ruler slapping)
        {
            this.moveFrames -= 1f;
            this.agent.speed = this.speed;
        }
        else
        {
            this.agent.speed = 0f;
        }
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        if (Physics.Raycast(base.transform.position + Vector3.up * 2f, direction, out raycastHit, float.PositiveInfinity, 769, QueryTriggerInteraction.Ignore) & raycastHit.transform.tag == "Player") //Create a raycast, if the raycast hits the player, Baldi can see the player
        {
            this.db = true;
            this.TargetPlayer(); //Start attacking the player
        }
        else
        {
            this.db = false;
        }
    }

    private void Wander()
    {
        this.wanderer.GetNewTarget(); //Get a new location
        this.agent.SetDestination(this.wanderTarget.position); //Head towards the position of the wanderTarget object
        this.coolDown = 1f; //Set the cooldown
        this.currentPriority = 0f;
    }

    public void TargetPlayer()
    {
        this.agent.SetDestination(this.player.position); //Target the player
        this.coolDown = 1f;
        this.currentPriority = 0f;
    }

    private void Move()
    {
        if (base.transform.position == this.previous & this.coolDown < 0f) // If Baldi reached his destination, start wandering
        {
            this.Wander();
        }
        this.moveFrames = 10f;
        this.timeToMove = this.baldiWait - this.baldiTempAnger;
        this.previous = base.transform.position; // Set previous to Baldi's current location
        this.baldiAudio.PlayOneShot(this.slap); //Play the slap sound
        this.baldiAnimator.SetTrigger("slap"); // Play the slap animation
        if (this.rumble)
        {
            float num = Vector3.Distance(base.transform.position, this.player.position);
            if (num < this.vibrationDistance)
            {
                float motorLevel = 1f - num / this.vibrationDistance;
            }
        }
    }

    public void GetAngry(float value)
    {
        this.baldiAnger += value; // Increase Baldi's anger by the value provided
        if (this.baldiAnger < 0.5f) //Cap Baldi anger at a minimum of 0.5
        {
            this.baldiAnger = 0.5f;
        }
        this.baldiWait = -3f * this.baldiAnger / (this.baldiAnger + 2f / this.baldiSpeedScale) + 3f; //Some formula I don't understand.
    }

    public void GetTempAngry(float value)
    {
        this.baldiTempAnger += value; //Increase Baldi's Temporary Anger
    }

    public void Hear(Vector3 soundLocation, float priority)
    {
        if (!this.antiHearing && priority >= this.currentPriority) //If anti-hearing is not active and the priority is greater then the priority of the current sound
        {
            this.agent.SetDestination(soundLocation); //Go to that sound
            this.currentPriority = priority; //Set the current priority to the priority
        }
    }

    public void ActivateAntiHearing(float t)
    {
        this.Wander(); //Start wandering
        this.antiHearing = true; //Set the antihearing variable to true for other scripts
        this.antiHearingTime = t; //Set the time the tape's effect on baldi will last
    }

    public bool db;

    public float baseTime;

    public float speed;

    public float timeToMove;

    public float baldiAnger;

    public float baldiTempAnger;

    public float baldiWait;

    public float baldiSpeedScale;

    private float moveFrames;

    private float currentPriority;

    public bool antiHearing;

    public float antiHearingTime;

    public float vibrationDistance;

    public float angerRate;

    public float angerRateRate;

    public float angerFrequency;

    public float timeToAnger;

    public bool endless;

    public Transform player;

    public Transform wanderTarget;

    public AILocationSelectorScript wanderer;

    private AudioSource baldiAudio;

    public AudioClip slap;

    public AudioClip[] speech = new AudioClip[3];

    public Animator baldiAnimator;

    public float coolDown;

    private Vector3 previous;

    private bool rumble;

    private NavMeshAgent agent;
}