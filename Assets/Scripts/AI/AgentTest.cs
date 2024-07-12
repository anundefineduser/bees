using UnityEngine;
using UnityEngine.AI;

public class AgentTest : MonoBehaviour
{
    private void Start()
    {
        this.agent = base.GetComponent<NavMeshAgent>(); // Define the AI Agent
        this.Wander(); //Start wandering
    }

    private void Update()
    {
        if (this.coolDown > 0f)
        {
            this.coolDown -= 1f * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = this.player.position - base.transform.position;
        RaycastHit raycastHit;
        if (Physics.Raycast(base.transform.position, direction, out raycastHit, float.PositiveInfinity, 3, QueryTriggerInteraction.Ignore) & raycastHit.transform.tag == "Player") //Check if its the player
        {
            this.db = true;
            this.TargetPlayer(); //Head towards the player
        }
        else
        {
            this.db = false;
            if (this.agent.velocity.magnitude <= 1f & this.coolDown <= 0f)
            {
                this.Wander(); //Just wander
            }
        }
    }

    private void Wander()
    {
        this.wanderer.GetNewTarget(); //Get a new target on the map
        this.agent.SetDestination(this.wanderTarget.position); //Set its destination to position of the wanderTarget
        this.coolDown = 1f;
    }

    private void TargetPlayer()
    {
        this.agent.SetDestination(this.player.position); //Set it's destination to the player
        this.coolDown = 1f;
    }

    public bool db;

    public Transform player;

    public Transform wanderTarget;

    public AILocationSelectorScript wanderer;

    public float coolDown;

    private NavMeshAgent agent;
}