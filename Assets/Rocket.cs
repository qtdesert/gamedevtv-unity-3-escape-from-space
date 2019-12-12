using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum Direction { Left, Right };
    Direction rotation;
    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    bool collisionsDisabled = false;

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
            RespondToDebugKeys();
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (state != State.Alive || collisionsDisabled) { return; }
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
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence() {
        state = State.Dying;
        StopAudio();
        PlayAudio(death);
        deathParticles.Play();
        Invoke("ReloadLevel", levelLoadDelay);
    }

    private void LoadFirstLevel() {
        state = State.Alive;
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel() {
        state = State.Alive;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void ReloadLevel() {
        state = State.Alive;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
            Thrust(mainThrust);
        }
        else {
            StopAudio();
            mainEngineParticles.Stop();
        }
    }

    private void Thrust(float mainThrust) {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        PlayAudio(mainEngine);
        mainEngineParticles.Play();
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

    private void RespondToDebugKeys() {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextLevel();
        }
        if (Input.GetKey(KeyCode.C)) {
            collisionsDisabled = !collisionsDisabled;
        }
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
