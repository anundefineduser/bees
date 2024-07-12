using TMPro;
using UnityEngine;

public class JumpRopeScript : MonoBehaviour
{
    private void OnEnable()
    {
        this.jumpDelay = 1f;
        this.ropeHit = true;
        this.jumpStarted = false;
        this.jumps = 0;
        this.jumpCount.text = 0 + "/5";
        this.cs.jumpHeight = 0f;
        this.playtime.audioDevice.PlayOneShot(this.playtime.aud_ReadyGo);
    }

    private void Update()
    {
        if (this.jumpDelay > 0f) //Decrease jumpDelay countdown
        {
            this.jumpDelay -= Time.deltaTime;
        }
        else if (!this.jumpStarted) //If the jump hasn't started
        {
            this.jumpStarted = true; //Start the jump
            this.ropePosition = 1f; //Set the rope position to 1f
            this.rope.SetTrigger("ActivateJumpRope"); //Activate the jumprope
            this.ropeHit = false;
        }
        if (this.ropePosition > 0f)
        {
            this.ropePosition -= Time.deltaTime;
        }
        else if (!this.ropeHit) //If the player has not tried to hit the rope
        {
            this.RopeHit();
        }
    }

    private void RopeHit()
    {
        this.ropeHit = true; //Set ropehit to true
        if (this.cs.jumpHeight <= 0.2f)
        {
            this.Fail(); //Fail
        }
        else
        {
            this.Success(); //Succeed
        }
        this.jumpStarted = false;
    }

    private void Success()
    {
        this.playtime.audioDevice.Stop(); //Stop all of the lines playtime is currently speaking
        this.playtime.audioDevice.PlayOneShot(this.playtime.aud_Numbers[this.jumps]);
        this.jumps++;
        this.jumpCount.text = this.jumps + "/5";
        this.jumpDelay = 0.5f;
        if (this.jumps >= 5) //If players complete the minigame
        {
            this.playtime.audioDevice.Stop(); //Stop playtime from talking
            this.playtime.audioDevice.PlayOneShot(this.playtime.aud_Congrats);
            this.ps.DeactivateJumpRope(); //Deactivate the jumprope
        }
    }

    private void Fail()
    {
        this.jumps = 0; //Reset jumps
        this.jumpCount.text = this.jumps + "/5";
        this.jumpDelay = 2f; //Set the jump delay to 2 seconds to allow playtime to finish her line before the rope starts again
        this.playtime.audioDevice.PlayOneShot(this.playtime.aud_Oops);
    }

    public TMP_Text jumpCount;

    public Animator rope;

    public CameraScript cs;

    public PlayerScript ps;

    public PlaytimeScript playtime;

    public GameObject mobileIns;

    public int jumps;

    public float jumpDelay;

    public float ropePosition;

    public bool ropeHit;

    public bool jumpStarted;
}