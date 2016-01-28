using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Information about the RigidBody
    private Rigidbody rb;
    public float speed = 3.0f;
    public float maxSpeed = 20.0f;
    public float fallToDeath = -1.0f;

    // Information about the Player object
    private GameObject self;

    // Use this for initialization
    void Start()
    {
        self = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Well... are you?
        AmIDead();

        // Move the object
        // Relative forces play better in the low-friction environment
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveHorizontal, 0, moveVertical);
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddRelativeForce(move * speed);
        }
        else
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }

    // For that moment when you're very, very dead
    void AmIDead()
    {
        if (self.transform.position.y < fallToDeath)
        {
            DeathPause.S.PauseDead();
        }
    }
}