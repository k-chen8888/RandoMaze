using UnityEngine;
using System.Collections;

public class WallController : MonoBehaviour
{
    // Information about how this object moves
    private Rigidbody rb;
	public float speed = 5.0f,
				 moveDistance = 5.0f,
				 waitToStart = 1.0f,
				 waitTime = 1.0f;
	public bool sentry = false;
	public bool random = true;
	public Vector3[] waypoints;

	// Forbidden zones
	// Check to see that forbiddenBackward < z < forbiddenForward and forbiddenLeft < x < forbiddenRight before moving
	public float forbiddenForward = 47.0f,
				 forbiddenBackward = -47.0f,
				 forbiddenLeft = -22.0f,
				 forbiddenRight = 22.0f;

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
		if (random)
		{
			StartCoroutine (RandomWallMovement ());
		}
		else if (sentry && waypoints.Length > 0)
		{
			StartCoroutine (SentryWallMovement ());
		}
		else
		{

		}
    }

    // Update is called once per frame
    void Update()
    {

    }


    /* Co-Routines */

	// Moves the walls between a set of fixed waypoints
	IEnumerator SentryWallMovement()
	{
		float percentTravelled = 1.0f;
		int location = 0; // waypoint index

		// Wait 1 second before starting
		yield return new WaitForSeconds(waitToStart);

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
				yield return new WaitForSeconds(waitTime);

				// Save starting point
				startPosition = transform.position;

				// Target of movement is next waypoint
				location = (location + 1) % waypoints.Length;
				targetLocation = waypoints[location];

				// Set things in motion
				percentTravelled = 0.0f;
			}
		}
	}

    // Moves the walls randomly
	IEnumerator RandomWallMovement()
    {
        float percentTravelled = 1.0f;

		// Wait 1 second before starting
		yield return new WaitForSeconds(waitToStart);

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
				yield return new WaitForSeconds(waitTime);

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
					if (this.transform.position.z < forbiddenForward)
                    {
                        targetLocation = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + moveDistance);
                    }

                    break;

                case 1: // Backwards
					if (this.transform.position.z > forbiddenBackward)
                    {
                        targetLocation = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - moveDistance);
                    }

                    break;

                case 2: // Left
					if (this.transform.position.x > forbiddenLeft)
                    {
                        targetLocation = new Vector3(this.transform.position.x - moveDistance, this.transform.position.y, this.transform.position.z);
                    }

                    break;

                case 3: // Right
					if (this.transform.position.x < forbiddenRight)
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