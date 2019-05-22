using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BallScript : MonoBehaviour {   
    Rigidbody rb;

    [SerializeField]
    Transform transform;
    float totalCol = 0;

    float sqrLen;

    float xStart = 0f;
    float yStart = 6.58f;
    float zStart = 0f;

    float Goalx = -0.187f;
    float Goaly = 9.884f;
    float Goalz = 2.986f;

    float targetX = -2.403f;
    float targetY = 11.958f;
    float targetZ = 3.685f;

    bool toThrow;

    float distance;

    float startTime = 0;

    bool stop = false;

    bool touchedBoard = false;
    bool touchedGoal = false;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        //EditorApplication.isPlaying = false;
        //Time.timeScale = 10;
        startTime += Time.deltaTime;

        if (stop == false) {
            // Sets location of the ball back to its base
            if (startTime >= 4) {
                startTime = 0;
                rb.velocity = Vector3.zero;

                // Changes the postion of the target
                targetX += 0.1f;
                if (targetX >= 2.082) {
                    targetX = -2.403f;
                    targetY -= 0.1f;
                }

                transform.position = new Vector3(targetX, targetY, targetZ);

                // Sets the position of the ball to its base
                rb.transform.position = new Vector3(xStart, yStart, zStart);
            }

            // Stops the ball once it reaches the end
            if(targetX >= 2.082 || targetY <= 7.9) {
                stop = true;
            }
        }
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.name == "Ground") {
            toThrow = true;
            Debug.Log("Touched The Ground");
            //rb.velocity = Vector3.zero;
            //rb.transform.position = new Vector3(xStart, yStart, zStart);
            //rb.useGravity = false;
        }
    }

    private void OnTriggerEnter(Collider col) {
        if (col.gameObject.name == "Board Collider") {
            //Debug.Log("Touched The Board");
            Debug.Log(rb.transform.position);
            
            Debug.Log("Touched The Board");
            distance = Vector3.Distance(rb.position, new Vector3(Goalx, Goaly, Goalz));

            touchedBoard = true;

            //Debug.Log("Distance is : " + distance);
            //WriteString(distance);
        }

        if (col.gameObject.name == "Goal") {
            touchedGoal = true;
            //Debug.Log("Reached The Goal");
            //Debug.Log(rb.transform.position);
            //distance = Vector3.Distance(rb.position, new Vector3(Goalx, Goaly, Goalz)) * 100;
            //WriteString(distance);
        }

        if(col.gameObject.name == "Print") {
            /*Debug.Log("Touched Print");
            if(touchedBoard == false && touchedGoal == false) {
                EditorApplication.isPlaying = false;
            }*/
            totalCol += 1;
            //Debug.Log("Touched Print");
            if (touchedBoard == true && touchedGoal == true) {
                distance = 100;
                WriteString(distance);

                touchedGoal = false;
                touchedBoard = false;
            } else if (touchedBoard == false && touchedGoal == true) {
                distance = 100;
                WriteString(distance);

                touchedGoal = false;
                touchedBoard = false;
            } else if (touchedBoard == false && touchedGoal == false && targetY <= 7.9) {
                WriteString(00);
                //EditorApplication.Exit(0);
                //Application.Quit();
                EditorApplication.isPlaying = false;
            } else {
                WriteString(distance);

                touchedGoal = false;
                touchedBoard = false;
            }
        }

        

        if (transform.position.x >= 2.082 || transform.position.y < 9.666) {
            Debug.Log("Total Row is : " + totalCol);
        }
    }

    static void WriteString(float value) {

        string path = "Assets/Resources/output.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(value);
        writer.Close();
    }
}
