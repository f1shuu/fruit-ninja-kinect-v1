using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : Fruit
{

    public AudioClip pauseClip;

    private AudioSource audioSource;

    public float lifeTime = 1f;

    private bool isCooldown = true;

    public override void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        foundGameManager = FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(AddCooldown(1f));
    }

    public override void Update()
    {

    }

    public override void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        audioSource.PlayOneShot(pauseClip);
        isSliced = true;

        whole.SetActive(false);
        sliced.SetActive(true);

        fruitCollider.enabled = false;

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody slice in slices)
        {
            slice.velocity = fruitRigidbody.velocity;
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }
        foundGameManager.PauseGame();
        Destroy(transform.gameObject, lifeTime);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if(!isCooldown)
        {
            base.OnTriggerEnter(other);
        }
    }

    private IEnumerator AddCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        isCooldown = false;
    }

}
