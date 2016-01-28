using UnityEngine;
using System.Collections;

public class WallController : MonoBehaviour
{
    // Information about how this object moves
    private Rigidbody rb;
    public float speed = 5;
    public float moveDistance = 5;

    // Information about the Player object
    private GameObject player;

    // Initial state
    private Vector3 startPosition,
                    targetLocation;
    

    /* Variables for Utilities */

    // Easing function
    [Range(0, 2)]
    public float easeFactor = 1;


    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");

        startPosition = transform.position;
        targetLocation = transform.position;

        // Fire up the movement coroutine
        StartCoroutine(WallMovement());
    }

    // Update is called once per frame
    void Update()
    {

    }


    /* Co-Routines */

    // Moves the walls 
    IEnumerator WallMovement()
    {
        float percentTravelled = 1.0f;

        // The first movements are calculated after 1 second
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            // Perform movements
            if (percentTravelled < 1.0f)
            {
                // Calculate easing between current and target locations
                percentTravelled += (Time.deltaTime * speed) / moveDistance;
                percentTravelled = Mathf.Clamp01(percentTravelled);
                float easedPercent = Ease(percentTravelled);

                // Calculate new position based on easing
                Vector3 newPos = Vector3.Lerp(startPosition, targetLocation, easedPercent);
                
                // Move to the new position and immediately go to the next iteration
                rb.MovePosition(newPos);
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(1.0f);

                // Try to generate a new place to move to
                targetLocation = RandomMove();

                // Save starting point
                startPosition = transform.position;

                // Set things in motion
                percentTravelled = 0.0f;
            }
        }
    }


    /* Utilities */

    // An RNG to determine whether or not the wall moves at any given time
    Vector3 RandomMove()
    {
        Vector3 targetLocation = transform.position;

        // The closer the player is to the wall, the more likely the wall is to move
        if (Random.Range(0.0f, 1.0f) > (Vector3.Distance(this.transform.position, player.transform.position) + 2.5f) / 100.0f)
        {
            // Randomly decide which direction to go
            // Do NOT move off the edge
            switch (Random.Range(0, 4))
            {
                case 0: // Forwards
                    if (this.transform.position.z < 47.0f)
                    {
                        targetLocation = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + moveDistance);
                    }

                    break;

                case 1: // Backwards
                    if (this.transform.position.z > -47.0f)
                    {
                        targetLocation = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - moveDistance);
                    }

                    break;

                case 2: // Left
                    if (this.transform.position.x > -22.0f)
                    {
                        targetLocation = new Vector3(this.transform.position.x - moveDistance, this.transform.position.y, this.transform.position.z);
                    }

                    break;

                case 3: // Right
                    if (this.transform.position.x < 22.0f)
                    {
                        targetLocation = new Vector3(this.transform.position.x + moveDistance, this.transform.position.y, this.transform.position.z);
                    }

                    break;
            }
        }

        return targetLocation;
    }

    // Movement Easing equation: y = x^a / (x^a + (1-x)^a)
    //
    // Takes x values between 0 and 1 and maps them to y values also between 0 and 1
    //  a = 1 -> straight line
    //  This is a logistic function; as a increases, y increases faster for values of x near .5 and slower for values of x near 0 or 1
    //
    // For animation, 1 < a < 3 is pretty good
    float Ease(float x)
    {
        float a = easeFactor + 1.0f;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
}