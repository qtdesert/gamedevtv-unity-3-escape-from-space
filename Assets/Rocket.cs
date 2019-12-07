using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float rcsThrust = 100f;

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
        HandleReset();
    }

    private void HandleThrust() {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
            Thrust();
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            ThrustBackwards();
        } else {
            ReleaseThrustLever();
        }
    }

    private void Thrust() {
        float forceThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * forceThisFrame);
        PlayAudio();
    }    

    private void ThrustBackwards() {
        rigidBody.AddRelativeForce(Vector3.down);
        PlayAudio();
    }

    private void ReleaseThrustLever() {
        StopAudio();
    }

    private void HandleRotation() {
        rigidBody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))) {
            if (rotation == Direction.Left) {
                RotateLeft(rotationThisFrame);
            } else {
                RotateRight(rotationThisFrame);
            }
        } else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            RotateLeft(rotationThisFrame);
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            RotateRight(rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }

    private void RotateLeft(float rotationThisFrame) {
        rotation = Direction.Left;
        transform.Rotate(Vector3.forward * rotationThisFrame);
    }

    private void RotateRight(float rotationThisFrame) {
        rotation = Direction.Right;
        transform.Rotate(-Vector3.forward * rotationThisFrame);
    }

    private void HandleReset() {
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
