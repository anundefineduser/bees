using KOTLIN.Subtitles;
using UnityEngine;

public class DoorScript : KOTLIN.Interactions.Interactable
{
    private void Start()
    {
        this.myAudio = base.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (this.lockTime > 0f) // If the lock time is greater then 0, decrease lockTime
        {
            this.lockTime -= 1f * Time.deltaTime;
        }
        else if (this.bDoorLocked) //If the door is locked, unlock it
        {
            this.UnlockDoor();
        }
        if (this.openTime > 0f) // If the open time is greater then 0, decrease lockTime Decrease open time
        {
            this.openTime -= 1f * Time.deltaTime;
        }
        if (this.openTime <= 0f & this.bDoorOpen)
        {
            this.barrier.enabled = true; // Turn on collision
            this.invisibleBarrier.enabled = true; //Enable the invisible barrier
            this.bDoorOpen = false; //Set the door open status to false
            this.inside.material = this.closed; // Change one side of the door to the closed material
            this.outside.material = this.closed; // Change the other side of the door to the closed material
            if (this.silentOpens <= 0) //If the door isn't silent
            {
                this.myAudio.PlayOneShot(this.doorClose, 1f); //Play the door close sound
            }
        }
    }

    public override void Interact()
    {
        if (this.baldi.isActiveAndEnabled & this.silentOpens <= 0)
        {
            this.baldi.Hear(base.transform.position, 1f); //If the door isn't silent, Baldi hears the door with a priority of 1.
        }
        this.OpenDoor();
        SubtitleManager.Instance.CreateSubtitleTranslated(SubtitleType.ThreeD, "World_DoorOpen", 3, false, Color.blue, myAudio, transform);
        if (this.silentOpens > 0) //If the door is silent
        {
            this.silentOpens--; //Decrease the amount of opens the door will stay quite for.
        }
    }

    public void OpenDoor()
    {
        if (this.silentOpens <= 0 && !this.bDoorOpen) //Play the door sound if the door isn't silent
        {
            this.myAudio.PlayOneShot(this.doorOpen, 1f);
        }
        this.barrier.enabled = false; //Disable the Barrier
        this.invisibleBarrier.enabled = false;//Disable the invisible Barrier
        this.bDoorOpen = true; //Set the door open status to false
        this.inside.material = this.open; //Change one side of the door to the open material
        this.outside.material = this.open; //Change the other side of the door to the open material
        this.openTime = 3f; //Set the open time to 3 seconds
    }

    private void OnTriggerStay(Collider other)
    {
        if (!this.bDoorLocked & other.CompareTag("NPC")) //Open the door if it isn't locked and it is an NPC
        {
            this.OpenDoor();
        }
    }

    public void LockDoor(float time) //Lock the door for a specified amount of time
    {
        this.bDoorLocked = true;
        this.lockTime = time;
    }

    public void UnlockDoor() //Unlock the door
    {
        this.bDoorLocked = false;
    }

    public bool DoorLocked
    {
        get
        {
            return this.bDoorLocked;
        }
    }

    public void SilenceDoor() //Set the amount of times the door can be open silently
    {
        this.silentOpens = 4;
    }

    public float openingDistance;

    public Transform player;

    public BaldiScript baldi;

    public MeshCollider barrier;

    public MeshCollider trigger;

    public MeshCollider invisibleBarrier;

    public MeshRenderer inside;

    public MeshRenderer outside;

    public AudioClip doorOpen;

    public AudioClip doorClose;

    public Material closed;

    public Material open;

    private bool bDoorOpen;

    private bool bDoorLocked;

    public int silentOpens;

    private float openTime;

    public float lockTime;

    private AudioSource myAudio;
}