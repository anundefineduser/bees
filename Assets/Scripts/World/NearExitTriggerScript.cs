using UnityEngine;

public class NearExitTriggerScript : MonoBehaviour
{
    private void Start()
    {
        gc = GameControllerScript.Instance; 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (this.gc.exitsReached < 3 & this.gc.finaleMode & other.tag == "Player")
        {
            this.gc.ExitReached();
            this.es.Lower();
            if (this.gc.baldiScrpt.isActiveAndEnabled) this.gc.baldiScrpt.Hear(base.transform.position, 8f);
        }

        Debug.Log(es.decompType); 
        if (es.decompType != DecompType.Classic && !closedElevator && other.tag == "Player" && doorAnimator != null)
        {
            Debug.Log("booyah"); 
            closedElevator = true; 
            doorAnimator.SetTrigger("CLOSE"); 
        }
    }

    private GameControllerScript gc;
    public EntranceScript es;

    private bool closedElevator; 
    public Animator doorAnimator; 
}