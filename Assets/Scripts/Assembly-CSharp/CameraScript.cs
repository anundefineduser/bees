using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private void Start()
    {
        this.offset = base.transform.position - this.player.transform.position; //Defines the offset
    }

    private void Update()
    {
        if (this.ps.jumpRope) //If the player is jump roping
        {
            this.velocity -= this.gravity * Time.deltaTime; //Decrease the velocity using gravity
            this.jumpHeight += this.velocity * Time.deltaTime; //Increase the jump height based on the velocity
            if (this.jumpHeight <= 0f) //When the player is on the floor, prevent the player from falling through.
            {
                this.jumpHeight = 0f;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    this.velocity = this.initVelocity; //Start the jump
                }
            }
            this.jumpHeightV3 = new Vector3(0f, this.jumpHeight, 0f); //Turn the float into a vector
        }
        else if (Input.GetButton("Look Behind"))
        {
            this.lookBehind = 180; //Look behind you
        }
        else
        {
            this.lookBehind = 0; //Don't look behind you
        }
    }

    private void LateUpdate()
    {
        base.transform.position = this.player.transform.position + this.offset; //Teleport to the player, then move based on the offset vector(if all other statements fail)
        if (!this.ps.gameOver & !this.ps.jumpRope)
        {
            base.transform.position = this.player.transform.position + this.offset; //Teleport to the player, then move based on the offset vector
            base.transform.rotation = this.player.transform.rotation * Quaternion.Euler(0f, (float)this.lookBehind, 0f); //Rotate based on player direction + lookbehind
        }
        else if (this.ps.gameOver)
        {
            base.transform.position = this.baldi.transform.position + this.baldi.transform.forward * 2f + new Vector3(0f, 5f, 0f); //Puts the camera in front of Baldi
            base.transform.LookAt(new Vector3(this.baldi.position.x, this.baldi.position.y + 5f, this.baldi.position.z)); //Makes the player look at baldi with an offset so the camera doesn't look at the feet
        }
        else if (this.ps.jumpRope)
        {
            base.transform.position = this.player.transform.position + this.offset + this.jumpHeightV3; //Apply the jump rope vector onto the normal offset
            base.transform.rotation = this.player.transform.rotation; //Rotate based on player direction
        }
    }

    public GameObject player;

    public PlayerScript ps;

    public Transform baldi;

    public float initVelocity;

    public float velocity;

    public float gravity;

    private int lookBehind;

    public Vector3 offset;

    public float jumpHeight;

    public Vector3 jumpHeightV3;
}