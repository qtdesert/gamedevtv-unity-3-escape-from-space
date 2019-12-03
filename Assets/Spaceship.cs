using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour {
    
    enum Direction { Left, Right };
    Direction rotation;
    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.mass = 0.1f;
    }

    // Update is called once per frame
    void Update() {
        ProcessInput();
    }

    private void ProcessInput() {
        if (Input.GetKey(KeyCode.Space)) {
            Thrust();
        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            if (rotation == Direction.Left) {
                RotateLeft();
            } else {
                RotateRight();
            }
        }
        else if (Input.GetKey(KeyCode.A)) {
            RotateLeft();
        }
        else if (Input.GetKey(KeyCode.D)) {
            RotateRight();
        }
    }

    private void Thrust() {
        rigidBody.AddRelativeForce(Vector3.up);
    }

    private void RotateLeft() {
        rotation = Direction.Left;
        transform.Rotate(Vector3.forward);
    }

    private void RotateRight() {
        rotation = Direction.Right;
        transform.Rotate(-Vector3.forward);
    }
}
