﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

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
        HandleThrust();
        HandleRotation();
        Reset();
    }

    private void HandleThrust() {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
            Throttle();
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            ThrottleBackwards();
        } else {
            ReleaseThrottle();
        }
    }

    private void Throttle() {
        rigidBody.AddRelativeForce(Vector3.up);
        PlayAudio();
    }    

    private void ThrottleBackwards() {
        rigidBody.AddRelativeForce(Vector3.down);
        PlayAudio();
    }

    private void ReleaseThrottle() {
        StopAudio();
    }

    private void HandleRotation() {
        rigidBody.freezeRotation = true;
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))) {
            if (rotation == Direction.Left) {
                RotateLeft();
            } else {
                RotateRight();
            }
        } else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            RotateLeft();
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            RotateRight();
        }
        rigidBody.freezeRotation = false;
    }

    private void RotateLeft() {
        rotation = Direction.Left;
        transform.Rotate(Vector3.forward);
    }

    private void RotateRight() {
        rotation = Direction.Right;
        transform.Rotate(-Vector3.forward);
    }

    private void Reset() {
        if (Input.GetKey(KeyCode.R)) {
            rigidBody.velocity = Vector3.zero;
            rigidBody.transform.rotation = Quaternion.identity;
            rigidBody.MovePosition(new Vector3(0, 3.14f, 0));
        }
    }

    private void PlayAudio() {
        if (!audioSource.isPlaying) {
            audioSource.Play();
        }
    }

    private void StopAudio() {
        audioSource.Stop();
    }
}