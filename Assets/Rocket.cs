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

    bool isRotatingLeft = true;

    bool isTransitioning = false;

    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        PlayAudio(success);
    }

    // Update is called once per frame
    void Update() {
        if (!isTransitioning) {
            RespondToThrustInput();
            RespondToRotationInput();
            if (Debug.isDebugBuild) {
                RespondToDebugKeys();
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (isTransitioning || collisionsDisabled) { return; }
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
        isTransitioning = true;
        StopAudio();
        PlayAudio(success);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence() {
        isTransitioning = true;
        StopAudio();
        PlayAudio(death);
        deathParticles.Play();
        Invoke("ReloadLevel", levelLoadDelay);
    }

    private void LoadNextLevel() {
        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextLevel % SceneManager.sceneCountInBuildSettings);
    }

    private void ReloadLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
            Thrust(mainThrust);
        }
        else {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust() {
        StopAudio();
        mainEngineParticles.Stop();
    }

    private void Thrust(float mainThrust) {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        PlayAudio(mainEngine);
        mainEngineParticles.Play();
    }

    private void RespondToRotationInput() {
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if ((Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))) {
            if (isRotatingLeft) {
                RotateLeft(rotationThisFrame);
                return;
            }
            RotateRight(rotationThisFrame);
        } else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            RotateLeft(rotationThisFrame);
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            RotateRight(rotationThisFrame);
        }
    }

    private void RotateLeft(float rotationThisFrame) {
        isRotatingLeft = true;
        Rotate(rotationThisFrame);
    }

    private void RotateRight(float rotationThisFrame) {
        isRotatingLeft = false;
        Rotate(-rotationThisFrame);
    }

    private void Rotate(float rotationThisFrame) {
        rigidBody.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false;
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
