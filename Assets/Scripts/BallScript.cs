using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BallScript : MonoBehaviour {
    Rigidbody rb; // Adds the balls rigidbody to the script

    [SerializeField]
    Transform transform; // Target

    [SerializeField]
    Transform goal; // Goal State
    float h = 6; // Adds a curve to the ball when thrown
    float gravity = -18; // Gravity needed for the ball

    float totalCol = 0f;

    float sqrLen;

    // Starting position of the ball
    public static float xStart = 0f;
    public static float yStart = 6.58f;
    public static float zStart = 0f;

    // Position of the target
    float targetX = -2.403f;
    float targetY = 11.958f;
    public static float targetZ = 3.685f;

    bool toThrow = true;

    float distance;

    float startTime = 0f;

    bool stop = false;

    bool touchedBoard = false;
    bool touchedGoal = false;

	static bool newLine = false;

	public int counter = 1;
	public int collider_counter = 1;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        if (stop == false) {
            // Sets location of the ball back to its base
            if (toThrow == true) {
                toThrow = false;

                Debug.Log("Throw is called");
                Throw();
            }

            // Stops the ball once it reaches the end
            if(targetX >= 2.082f || targetY <= 7.9f) {
                Debug.Log("Program has stopped");
                stop = true;
            }
        }
    }

    // Throws the ball with velocity
    void Throw() {
        Physics.gravity = Vector3.up * gravity;
        rb.useGravity = true;
        rb.velocity = CalculateLaunchVeclocity();
    }

    // Collision for the ball
    void OnCollisionEnter(Collision col) {
        // if the ball touches the ground
        if (col.gameObject.name == "Ground") {
            // Changes the postion of the target
            targetX += 0.1f;
            counter++;
            if (targetX > 2.082f) {
                targetX = -2.403f;
                targetY -= 0.1f;
                newLine = true;
                counter = 1;
                collider_counter = 1;
            }

            // Moves the target
            Debug.Log("Target X: " + targetX);
            transform.position = new Vector3(targetX, targetY, targetZ);

            // Sets the ball back to its base location
            Debug.Log("Touched The Ground");
            rb.angularVelocity = Vector3.zero;
            rb.transform.position = new Vector3(xStart, yStart, zStart);
            rb.useGravity = false;

            toThrow = true;

            totalCol += 1;
            Debug.Log("Distance near goal: " + distance);
            if (touchedBoard == true && touchedGoal == true) {
                collider_counter++;
                distance = 100 - (distance * 10);
                distance += 100;
                WriteString(distance);

                touchedGoal = false;
                touchedBoard = false;
            } else if (touchedBoard == false && touchedGoal == true) {
                distance = 100;
                collider_counter++;
                distance += 100;
                WriteString(distance);

                touchedGoal = false;
                touchedBoard = false;
            } else if (touchedBoard == false && touchedGoal == false) {
                distance = -1;
                WriteString(distance);
            } else if (touchedBoard == false && touchedGoal == false && targetY <= 7.9) {
                EditorApplication.isPlaying = false;
            } else {
                collider_counter++;
                distance = 100 - (distance * 10);
                WriteString(distance);
                
                touchedGoal = false;
                touchedBoard = false;
            }
        }
    }

    // Triggers needed for the ball
    private void OnTriggerEnter(Collider col) {
        // Checks if the ball touched the board
        if (col.gameObject.name == "Board Collider") {
            Debug.Log(rb.transform.position);
            
            Debug.Log("Touched The Board");
            distance = Vector3.Distance(rb.position, new Vector3(goal.position.x, goal.position.y, goal.position.z));
            
            touchedBoard = true;
        }

        // Checks if the ball touched the goal
        if (col.gameObject.name == "Goal") {
            touchedGoal = true;
        }

        if (transform.position.x >= 2.082f || transform.position.y < 9.666f) {
            Debug.Log("Total Row is : " + totalCol);
        }
    }

    // Writes values to file
    static void WriteString(float value) {

        string path = "Assets/Resources/output.txt";
        StreamWriter writer = new StreamWriter(path, true);
		writer.Write(value + " ");

		if (newLine) {
			writer.WriteLine ();
			newLine = false;
		}

        writer.Close();
    }

    // Calculates velocity needed to get the ball to the target
    Vector3 CalculateLaunchVeclocity() {
        float displacementY = transform.position.y - rb.position.y;
        Vector3 displacementXZ = new Vector3(transform.position.x - rb.position.x, 0, transform.position.z - rb.position.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity));

        return velocityXZ + velocityY;
    }


}
