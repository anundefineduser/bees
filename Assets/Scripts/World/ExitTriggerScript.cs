using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTriggerScript : MonoBehaviour
{
    private void Start()
    {
        gc = GameControllerScript.Instance; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.gc.notebooks >= 7 & other.tag == "Player")
        {
            if (this.gc.failedNotebooks >= 7) //If the player got all the problems wrong on all the 7 notebooks
            {
                SceneManager.LoadScene("Secret"); //Go to the secret ending
            }
            else
            {
                SceneManager.LoadScene("Results"); //Go to the win screen
            }
        }
    }

    private GameControllerScript gc;
}