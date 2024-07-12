using UnityEngine;

public class BullyScript : MonoBehaviour
{
    private void Start()
    {
        this.audioDevice = base.GetComponent<AudioSource>(); //Get the Audio Source
        this.waitTime = UnityEngine.Random.Range(60f, 120f); //Set the amount of time before the bully appears again
    }

    private void Update()
    {
        if (this.waitTime > 0f) //Decrease the waittime
        {
            this.waitTime -= Time.deltaTime;
        }
        else if (!this.active)
        {
            this.Activate(); //Activate the Bully
        }
        if (this.active) //If the Bully is on the map
        {
            this.activeTime += Time.deltaTime; //Increase active time
            if (this.activeTime >= 180f & (base.transform.position - this.player.position).magnitude >= 120f) //If the bully has been in the map for a long time and the player is far away
            {
                this.Reset(); //Reset the bully
            }
        }
        if (this.guilt > 0f)
        {
            this.guilt -= Time.deltaTime; //Decrease Bully's guilt
        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        if (Physics.Raycast(base.transform.position + new Vector3(0f, 4f, 0f), direction, out raycastHit, float.PositiveInfinity, 769, QueryTriggerInteraction.Ignore) & raycastHit.transform.tag == "Player" & (base.transform.position - this.player.position).magnitude <= 30f & this.active)
        {
            if (!this.spoken) // If the bully hasn't already spoken
            {
                int num = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f)); //Get a random number between 0 and 1
                this.audioDevice.PlayOneShot(this.aud_Taunts[num]); //Say a line in an index using num
                this.spoken = true; //Sets spoken to true, preventing the bully from talking again
            }
            this.guilt = 10f; //Makes the bully guilty for "Bullying in the halls"
        }
    }

    private void Activate()
    {
        this.wanderer.GetNewTargetHallway(); //Get a hallway position
        base.transform.position = this.wanderTarget.position + new Vector3(0f, 5f, 0f); // Go to the wanderTarget + 5 on the Y axis
        while ((base.transform.position - this.player.position).magnitude < 20f) // While the Bully is close to the player
        {
            this.wanderer.GetNewTargetHallway(); //Get a new target
            base.transform.position = this.wanderTarget.position + new Vector3(0f, 5f, 0f);// Go to the wanderTarget + 5 on the Y axis
        } //This is here to prevent the bully from spawning ontop iof the player
        this.active = true; //Set the bully to active
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player") // If touching the player
        {
            if (this.gc.item[0] == 0 & this.gc.item[1] == 0 & this.gc.item[2] == 0) // If the player has no items
            {
                this.audioDevice.PlayOneShot(this.aud_Denied); // "What, no items? No Items? No passsssss"
            }
            else
            {
                int num = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 2f)); //Get a random item slot
                while (this.gc.item[num] == 0) //If the selected slot is empty
                {
                    num = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 2f)); // Choose another slot
                }
                this.gc.LoseItem(num); // Remove the item selected
                int num2 = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f));
                this.audioDevice.PlayOneShot(this.aud_Thanks[num2]);
                this.Reset();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.name == "Principal of the Thing" & this.guilt > 0f) //If touching the principal and the bully is guilty
        {
            this.Reset(); //Reset the bully
        }
    }

    private void Reset()
    {
        base.transform.position = base.transform.position - new Vector3(0f, 20f, 0f); // Go to X: 0, Y: 20, Z: 20
        this.waitTime = UnityEngine.Random.Range(60f, 120f); //Set the amount of time before the bully appears again
        this.active = false; //Set active to false
        this.activeTime = 0f; //Reset active time
        this.spoken = false; //Reset spoken
    }

    public Transform player;

    public GameControllerScript gc;

    public Renderer bullyRenderer;

    public Transform wanderTarget;

    public AILocationSelectorScript wanderer;

    public float waitTime;

    public float activeTime;

    public float guilt;

    public bool active;

    public bool spoken;

    private AudioSource audioDevice;

    public AudioClip[] aud_Taunts = new AudioClip[2];

    public AudioClip[] aud_Thanks = new AudioClip[2];

    public AudioClip aud_Denied;
}