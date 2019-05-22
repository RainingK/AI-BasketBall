using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch : MonoBehaviour {
    [SerializeField]
    public Rigidbody rb;

    [SerializeField]
    public Transform target;

    float h = 6;
    float gravity = -18;

    public bool toThrow = true;

    float distance;

    float startTime = 0;

    // Use this for initialization
    void Start () {
		
	}
}
