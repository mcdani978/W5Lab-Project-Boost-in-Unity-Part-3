using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float LevelLoadDelay = 2f;
    [SerializeField] private float transportDistance = 10f; // Distance to transport the player
    [SerializeField] private AudioSource rocketAudioSource; // AudioSource for the rocket sound
    [SerializeField] private AudioSource effectsAudioSource; // Separate AudioSource for sound effects
    [SerializeField] private AudioClip transporterClip; // Sound to play when hitting the transporter
    [SerializeField] private AudioClip obstacleCollisionClip; // Sound for collisions with obstacles or the ground
    [SerializeField] private Image fadeImage; // UI Image for fading transitions
    [SerializeField] private float fadeDuration = 1f; // Duration of fade

    void Start()
    {
        // Ensure the fade image is disabled at the start
        if (fadeImage != null)
        {
            Color fadeColor = fadeImage.color;
            fadeColor.a = 0f; // Set alpha to 0 (fully transparent)
            fadeImage.color = fadeColor;
            fadeImage.enabled = false; // Disable the image at the start
        }
    }

    void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("This thing is friendly");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            case "Fuel":
                Debug.Log("You picked up fuel");
                break;
            case "Transporter":
                TransportPlayer();
                PlayTransporterSound();
                break;
            default:
                Debug.Log("Sorry, you blew up!");
                PlayObstacleCollisionSound();
                StartCrashSequence();
                break;
        }
    }

    void StartSuccessSequence()
    {
        GetComponent<Movement>().enabled = false;
        Invoke("LoadNextLevel", LevelLoadDelay);
    }

    void StartCrashSequence()
    {
        GetComponent<Movement>().enabled = false;
        Invoke("ReloadLevel", LevelLoadDelay);
    }

    void TransportPlayer()
    {
        Vector3 newPosition = transform.position + new Vector3(transportDistance, 0, 0);
        transform.position = newPosition;
        Debug.Log("Player transported to: " + newPosition);
    }

    void PlayTransporterSound()
    {
        if (effectsAudioSource != null && transporterClip != null)
        {
            effectsAudioSource.PlayOneShot(transporterClip);
            Debug.Log("Transporter sound played");
        }
    }

    void PlayObstacleCollisionSound()
    {
        if (effectsAudioSource != null && obstacleCollisionClip != null)
        {
            effectsAudioSource.PlayOneShot(obstacleCollisionClip);
            Debug.Log("You hit an obstacle or the ground");
        }
    }

    // Coroutine to handle scene transition with fade effect
    IEnumerator HandleSceneTransition()
    {
        if (fadeImage != null)
        {
            fadeImage.enabled = true; // Enable the fade image only during transition
        }

        yield return StartCoroutine(FadeOut()); // Fade out before loading the next scene
        LoadNextLevel(); // Load the next scene
        yield return StartCoroutine(FadeIn()); // Fade in after the new scene is loaded

        if (fadeImage != null)
        {
            fadeImage.enabled = false; // Disable the fade image after the transition
        }
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color fadeColor = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = fadeColor;
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color fadeColor = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeColor.a = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            fadeImage.color = fadeColor;
            yield return null;
        }
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; // Loop back to the first scene
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}

