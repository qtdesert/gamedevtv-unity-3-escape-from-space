using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour {
    
    enum Direction { Left, Right };
    Direction rotation;
    Rigidbody rigidBody;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        ProcessInput();
    }

    private void ProcessInput() {
        if (Input.GetKey(KeyCode.Space)) {
            Thrust();
        } else {
            ThrustRelease();
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
        if (!audioSource.isPlaying) {
            audioSource.Play();
        }
    }

    private void ThrustRelease() {
        audioSource.Stop();
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
