using UnityEngine;

public class EndlessNotebookScript : MonoBehaviour
{
    private void Start()
    {
        this.gc = GameObject.Find("Game Controller").GetComponent<GameControllerScript>(); //Find the game controller object
        this.player = GameObject.Find("Player").GetComponent<Transform>(); //Find the player object
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) //If left clicked
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit) && (raycastHit.transform.tag == "Notebook" & Vector3.Distance(this.player.position, base.transform.position) < this.openingDistance)) //If it is a notebook
            {
                base.gameObject.SetActive(false); //Disable the object being clicked
                this.gc.CollectNotebook(); //Collect the notebook
                this.learningGame.SetActive(true); //Activate the learning game
            }
        }
    }

    public float openingDistance;

    public GameControllerScript gc;

    public Transform player;

    public GameObject learningGame;
}