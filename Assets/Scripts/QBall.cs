using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QBall : MonoBehaviour {
    Rigidbody rb; // Rigidbody for the ball

    [SerializeField]
    Transform goal; // Goal state

    float distance;

    // Starting position of the ball
    public static float xStart = 0f;
    float yStart = 6.58f;
    float zStart = 0f;

    public static bool toLearn = true;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}

    void OnCollisionEnter(Collision col) {
        //Checks if the ball touches the gorund
        if(col.gameObject.name == "Ground") {
            rb.position = new Vector3(xStart, yStart, zStart);
            rb.angularVelocity = Vector3.zero;
            toLearn = true;
        }
    }

    void OnTriggerEnter(Collider col) {
        // Checks if the ball touches the board collider
        if(col.gameObject.name == "Board Collider") {
            distance = Vector3.Distance(rb.position, new Vector3(goal.position.x, goal.position.y, goal.position.z));
        }
    }
}
