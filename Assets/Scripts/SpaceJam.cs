using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SpaceJam : MonoBehaviour {

    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Transform transform;
    float h = 6;
    float gravity = -18;

    const int finalRow = 41;
    const int finalColumn = 45;
    int maxIndex = 0;

    float[,] rMatrix;
    float[,] qTable;

    float gamma = 0.8f;
    float epsilon = 0.5f;

    int aIndex;
    int sIndex;
    int goalCount;

    float action;
    float state;

    float valueZ = BallScript.targetZ;
    const float valueY = 11.958f;
    const float valueX = -2.403f;

    bool goal = false;

    int episode = 0;
    bool episodeBool = false;
    bool toPrint = false;
    bool stop = false;

    // Use this for initialization
    void Start() {
        FillRMatrix();
        FillQMatrix();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (stop == false) {
            if (QBall.toLearn == true) {
                QBall.toLearn = false;
                if (!episodeBool) {
                    //Debug.Log("Episode : " + episode);
                    goalCount = 0;
                    sIndex = Random.Range(0, 44); // sin
                    aIndex = Random.Range(0, 40); // ain
                }

                if (!goal) {
                    Learnt();
                }

                if (goal && goalCount >= 50) {
                    episode++;
                    episodeBool = false;
                    //QBall.xStart += 1.5f;
                    goalCount = 0;
                    Debug.Log("Episode : " + episode);
                }
                goal = false;

                if (episode >= 50) {
                    episodeBool = true;
                    toPrint = true;
                    stop = true;
                }
            }
        }
    }

    void FillRMatrix() {

        int valueCount = 0;

        string path = "Assets/Resources/output.txt";

        string fileContent = File.ReadAllText(path);

        string[] integerStrings = fileContent.Split(new char[] { ' ' });
        rMatrix = new float[finalRow, finalColumn];

        for (int i = 0; i < finalRow; i++) {
            for (int j = 0; j < finalColumn; j++) {
                rMatrix[i, j] = float.Parse(integerStrings[valueCount]);
                //Debug.Log(integerStrings[valueCount]);
                valueCount++;
            }

            //Debug.Log("Last value of " + (i + 1) + " : " + rMatrix[i,finalColumn-1]);
        }

        qTable = new float[finalRow, finalColumn];
    }

    void FillQMatrix() {
        int valueCount = 0;
        string path = "Assets/Resources/qTableOutput.txt";

        string fileContent = File.ReadAllText(path);

        string[] integerStrings = fileContent.Split(new char[] { ' ' });

        for (int i = 0; i < finalRow; i++) {
            for (int j = 0; j < finalColumn; j++) {
                qTable[i, j] = float.Parse(integerStrings[valueCount]);
                valueCount++;
            }
        }
    }

    void Learnt() {
        episodeBool = true;

        state = (0.1000f * sIndex) + valueX;
        action = valueY - (0.1000f * aIndex);

        transform.position = new Vector3(state, action, valueZ); // collider moved to this new point
        Throw();


        //        Debug.Log("Random X: " + (aIndex) + " Random Y: " + (sIndex));
        qTable[aIndex, sIndex] = rMatrix[aIndex, sIndex] + (gamma * MaxAction(sIndex));

        if (rMatrix[aIndex, sIndex] >= 100) {

            goalCount++;
            goal = true;
            sIndex = Random.Range(0, 44); // sin
            aIndex = Random.Range(0, 40); // ain
        }

        Debug.Log("Q value: " + qTable[aIndex, sIndex]);
        Debug.Log("Goal Count: " + goalCount);

        aIndex = sIndex % 41;
        sIndex = SelectAction(aIndex);
    }

    float MaxAction(int index) {
        //int index[41]
        index = index % 41;
        float max = qTable[index, 0];
        //index[index] = max
        for (int i = 1; i < finalColumn; i++) {
            if (qTable[index, i] > max) {
                max = qTable[index, i];
                maxIndex = i;
            }

        }
        return max;
    }

    int SelectAction(int index) { // implement a policy so that we dont always take the optimal path and we explore all the other paths as well

        //float value = Random.Range(0.0f, 1.0f);

        //Debug.Log("Value: " + value);
        //if (value < epsilon && episode <= 1900) { //Random selection
        //    int coloumn = Random.Range(0, 44);
        //    Debug.Log("Random Selection occuring");
        //    return coloumn;
        //}
        //else {
        MaxAction(index);
        return maxIndex;
        //}
    }

    void Throw() {
        Physics.gravity = Vector3.up * gravity;
        rb.useGravity = true;
        //rb.velocity = (transform.up * 10) + (transform.forward * 2) + (transform.right * 0);
        rb.velocity = CalculateLaunchVeclocity();
    }

    Vector3 CalculateLaunchVeclocity() {
        float displacementY = transform.position.y - rb.position.y;
        Vector3 displacementXZ = new Vector3(transform.position.x - rb.position.x, 0, transform.position.z - rb.position.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity));

        return velocityXZ + velocityY;
    }
}
