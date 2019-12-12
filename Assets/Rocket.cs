using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum Direction { Left, Right };
    Direction rotation;
    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        PlayAudio(success);
    }

    // Update is called once per frame
    void Update() {
        if (state == State.Alive) {
            RespondToThrustInput();
            RespondToRotationInput();
            RespondToResetInput();
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (state != State.Alive) { return; }
        switch (collision.gameObject.tag) {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence() {
        state = State.Transcending;
        StopAudio();
        PlayAudio(success);
        Invoke("LoadNextLevel", 1f);
    }

    private void StartDeathSequence() {
        state = State.Dying;
        StopAudio();
        PlayAudio(death);
        Invoke("LoadFirstLevel", 1f);
    }

    private void LoadFirstLevel() {
        state = State.Alive;
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel() {
        state = State.Alive;
        SceneManager.LoadScene(1);
    }

    private void RespondToThrustInput() {
        float forceThisFrame = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
            Thrust(forceThisFrame);
        }
        else {
            StopAudio();
        }
    }

    private void Thrust(float forceThisFrame) {
        rigidBody.AddRelativeForce(Vector3.up * forceThisFrame);
        PlayAudio(mainEngine);
    }

    private void RespondToRotationInput() {
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

    private void RespondToResetInput() {
        if (Input.GetKey(KeyCode.R)) {
            Reset();
        }
    }

    private void Reset() {
        rigidBody.velocity = Vector3.zero;
        rigidBody.transform.rotation = Quaternion.identity;
        rigidBody.MovePosition(new Vector3(-27f, 7f, 0));
    }

    private void PlayAudio(AudioClip audioClip) {
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(audioClip);
        }
    }

    private void StopAudio() {
        audioSource.Stop();
    }
}
