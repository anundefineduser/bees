//using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarningScreenScript : MonoBehaviour
{
    private void Start()
    {
        //this.player = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    //public Player player;
}