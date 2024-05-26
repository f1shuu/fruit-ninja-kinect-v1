using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverButton : Fruit
{
    private AudioSource audioSource;
    public float lifeTime = 3f;


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

            audioSource.PlayOneShot(fruitImpactClip);

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
            transform.GetChild(3).gameObject.SetActive(false);
            foundGameManager.callGameOverSequence();
            Destroy(transform.gameObject, lifeTime);
    }

}