using UnityEngine;

public class BsodaSparyScript : MonoBehaviour
{
    private void Start()
    {
        this.rb = base.GetComponent<Rigidbody>(); //Get the RigidBody
        this.rb.velocity = base.transform.forward * this.speed; //Move forward
        this.lifeSpan = 30f; //Set the lifespan
    }

    private void Update()
    {
        this.rb.velocity = base.transform.forward * this.speed; //Move forward
        this.lifeSpan -= Time.deltaTime; // Decrease the lifespan variable
        if (this.lifeSpan < 0f) //When the lifespan timer ends, destroy the BSODA
        {
            UnityEngine.Object.Destroy(base.gameObject, 0f);
        }
    }

    public float speed;

    private float lifeSpan;

    private Rigidbody rb;
}