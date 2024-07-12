using UnityEngine;

public class AILocationSelectorScript : MonoBehaviour
{
    public void GetNewTarget()
    {
        this.id = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 28f)); //Get a random number between 0 and 28
        base.transform.position = this.newLocation[this.id].position; //Set it's location to a position in a list of positions using the ID variable that just got set.
        this.ambience.PlayAudio(); //Play an ambience audio
    }

    public void GetNewTargetHallway()
    {
        this.id = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 15f)); //Get a random number between 0 and 15
        base.transform.position = this.newLocation[this.id].position; //Set it's location to a position in a list of positions using the ID variable that just got set.
        this.ambience.PlayAudio(); //Play an ambience audio
    }

    public void QuarterExclusive()
    {
        this.id = Mathf.RoundToInt(UnityEngine.Random.Range(1f, 15f)); //Get a random number between 0 and 15
        base.transform.position = this.newLocation[this.id].position; //Set it's location to a position in a list of positions using the ID variable that just got set.
    }

    public Transform[] newLocation = new Transform[29];

    public AmbienceScript ambience;

    private int id;
}