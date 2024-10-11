using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    Rigidbody rb;
    public float thrustForce = 10f;
    public float rotationSpeed = 100f;
    public float moveSpeed = 5f;

    public float fuel = 100f;
    public float fuelConsumptionRate = 10f;
    public TextMeshProUGUI fuelText;
    AudioSource audioSource;
    [SerializeField] AudioClip mainEngine;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem leftThrusterParticles;
    [SerializeField] ParticleSystem rightThrusterParticles;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ; // Freeze Z position
        UpdateFuelText();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        ProcessThrust();
        ProcessRotation();
        ProcessHorizontalMovement();
    }

    void ProcessThrust()
    {
        if (Input.GetKey(KeyCode.Space) && fuel > 0)
        {
            StartThrusting();

        }

        else
        {
            StopThrusting();
        }
    }



    private void StartThrusting()
    {
        rb.AddRelativeForce(Vector3.up * thrustForce);
        ConsumeFuel();

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }

        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    private void StopThrusting()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    void ProcessRotation()
    {
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        float rotationThisFrame = rotationSpeed * Time.deltaTime;
        rb.freezeRotation = true; // freezing rotation so we can manually rotate

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RotateLeft(rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            RotateRight(rotationThisFrame);
        }

        else
        {
            Stoprotating();
        }

        rb.freezeRotation = false; // Unfreezing rotation so the physics system can take over
    }

    private void Stoprotating()
    {
        rightThrusterParticles.Stop();
        leftThrusterParticles.Stop();
    }

    private void RotateRight(float rotationThisFrame)
    {
        transform.Rotate(-Vector3.forward * rotationThisFrame);
        if (!leftThrusterParticles.isPlaying)
        {
            leftThrusterParticles.Play();
        }
    }

    private void RotateLeft(float rotationThisFrame)
    {
        transform.Rotate(Vector3.forward * rotationThisFrame);
        if (!rightThrusterParticles.isPlaying)
        {
            rightThrusterParticles.Play();
        }
    }

    void ProcessHorizontalMovement()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeForce(Vector3.left * moveSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeForce(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }

    void ConsumeFuel() // Method to reduce fuel
    {
        fuel -= fuelConsumptionRate * Time.deltaTime;
        fuel = Mathf.Clamp(fuel, 0, 100);
        UpdateFuelText();
    }

    void UpdateFuelText()
    {
        fuelText.text = "Fuel: " + Mathf.RoundToInt(fuel).ToString();
    }
}
