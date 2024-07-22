//using Rewired;
using KOTLIN.Interactions;
using UnityEngine;

public class NotebookScript : Interactable
{
    private void Start()
    {
        //this.playerInput = ReInput.players.GetPlayer(0);
        this.up = true;
    }

    private void Update()
    {
        if (this.gc.mode == "endless")
        {
            if (this.respawnTime > 0f)
            {
                if ((base.transform.position - this.player.position).magnitude > 60f)
                {
                    this.respawnTime -= Time.deltaTime;
                }
            }
            else if (!this.up)
            {
                base.transform.position = new Vector3(base.transform.position.x, 4f, base.transform.position.z);
                this.up = true;
                this.audioDevice.Play();
            }
        }
    }

    public override void Interact()
    {
        base.transform.position = new Vector3(base.transform.position.x, -20f, base.transform.position.z);
        this.up = false;
        this.respawnTime = 120f;
        this.gc.CollectNotebook();
        this.gc.DeactivateLearningGame(null);
        if (this.gc.notebooks >= 2)
        {
            if (this.gc.notebooks == 2)
                this.gc.ActivateSpoopMode();

            this.bsc.GetAngry(1f);
        }

        //GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.learningGame);
        //gameObject.GetComponent<MathGameScript>().gc = this.gc;
        //gameObject.GetComponent<MathGameScript>().baldiScript = this.bsc;
        //gameObject.GetComponent<MathGameScript>().playerPosition = this.player.position;
    }

    public GameControllerScript gc;

    public BaldiScript bsc;

    public float respawnTime;

    public bool up;

    public Transform player;

    public GameObject learningGame;

    public AudioSource audioDevice;

    //private Player playerInput;
}