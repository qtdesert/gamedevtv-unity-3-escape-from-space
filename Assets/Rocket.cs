using UnityEngine;
using UnityEngine.SceneManagement;

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

    void OnCollisionEnter(Collision collision) {
        switch (collision.gameObject.tag) {
            case "Friendly":
                break;
            case "Finish":
                print("Finish");
                SceneManager.LoadScene(1);
                break;
            default:
                print("Dead");
                SceneManager.LoadScene(0);
                break;
        }
    }

    private void HandleThrust() {
        float forceThisFrame = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
            Thrust(forceThisFrame);
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            ThrustBackwards(forceThisFrame);
        } else {
            ReleaseThrustLever();
        }
    }

    private void Thrust(float forceThisFrame) {
        rigidBody.AddRelativeForce(Vector3.up * forceThisFrame);
        PlayAudio();
    }    

    private void ThrustBackwards(float forceThisFrame) {
        rigidBody.AddRelativeForce(Vector3.down * forceThisFrame);
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
            Reset();
        }
    }

    private void Reset() {
        rigidBody.velocity = Vector3.zero;
        rigidBody.transform.rotation = Quaternion.identity;
        rigidBody.MovePosition(new Vector3(-27f, 7f, 0));
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
