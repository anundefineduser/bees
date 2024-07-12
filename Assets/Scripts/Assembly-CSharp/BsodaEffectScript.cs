using UnityEngine;
using UnityEngine.AI;

public class BsodaEffectScript : MonoBehaviour
{
    private void Start()
    {
        this.agent = base.GetComponent<NavMeshAgent>(); //Get the object/character's AI Agent
    }

    private void Update()
    {
        if (this.inBsoda)
        {
            this.agent.velocity = this.otherVelocity; //Set the agent's velocity to the velocity of the other object
        }
        if (this.failSave > 0f)
        {
            this.failSave -= Time.deltaTime;
        }
        else
        {
            this.inBsoda = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "BSODA") //If its a BSODA
        {
            this.inBsoda = true;
            this.otherVelocity = other.GetComponent<Rigidbody>().velocity; // Set the velocity to the velocity of the BSODA
            this.failSave = 1f;
        }
        else if (other.transform.name == "Gotta Sweep") //If its Gotta Sweep
        {
            this.inBsoda = true;
            this.otherVelocity = base.transform.forward * this.agent.speed * 0.1f + other.GetComponent<NavMeshAgent>().velocity;
            this.failSave = 1f;
        }
    }

    private void OnTriggerExit()
    {
        this.inBsoda = false; // When they are out of the BSODA, set inBsoda to false
    }

    private NavMeshAgent agent;

    private Vector3 otherVelocity;

    private bool inBsoda;

    private float failSave;
}