using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : Fruit
{
    public AudioClip unpauseClip;
    private AudioSource audioSource;
    public float lifeTime = 3f;
    private bool isSimulationRunning = false;
    private bool hasBeenSliced = false;

    private void Start()
    {
        StartCoroutine(ManualPhysicsSimulation());
    }

    public override void Awake()
    {
        juice = GetComponentInChildren<ParticleSystem>();
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        foundGameManager = FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void Update()
    {
        transform.GetChild(0).Rotate(transform.forward * Time.unscaledDeltaTime * 20);
        transform.GetChild(3).Rotate(Vector2.up * Time.unscaledDeltaTime * 20);
    }

    public override void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        if (!hasBeenSliced)
        {
            audioSource.PlayOneShot(unpauseClip);

            whole.SetActive(false);
            sliced.SetActive(true);

            fruitCollider.enabled = false;
            juice.Play();

            Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody slice in slices)
            {
                slice.velocity = fruitRigidbody.velocity;
                slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
            }
            foundGameManager.UnpauseGame();
            transform.GetChild(3).gameObject.SetActive(false);
            Physics.autoSimulation = true;
            isSimulationRunning = false;
            Destroy(transform.gameObject, lifeTime);

            hasBeenSliced = true;
        }
    }

    private IEnumerator ManualPhysicsSimulation()
    {
        Physics.autoSimulation = false;

        yield return null;

        isSimulationRunning = true;

        while (isSimulationRunning)
        {
            Physics.Simulate(Time.fixedDeltaTime);

            CheckForCollisions();

            yield return null;
        }
    }

    private void CheckForCollisions()
    {
        Collider[] colliders = Physics.OverlapBox(fruitCollider.bounds.center, fruitCollider.bounds.extents, fruitCollider.transform.rotation);
        foreach (Collider other in colliders)
        {
            if (other != fruitCollider)
            {
                OnTriggerEnter(other);
            }
        }
    }
}
