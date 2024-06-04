using UnityEngine;

public class KinectBlade : MonoBehaviour
{
    private Camera mainCamera;
    private Collider bladeCollider;
    private ParticleSystem bladeTrail;
    private bool slicing;
    private Vector3 lastBladePosition;
    
    public GameObject followTarget;
    public Vector3 direction { get; private set; }
    public float sliceForce = 5f;
    public float minSliceVelocity = 0.01f;


    private void Awake()
    {
        mainCamera = Camera.main;
        bladeCollider = GetComponent<Collider>();
        bladeTrail = GetComponentInChildren<ParticleSystem>();
        lastBladePosition = followTarget.transform.position;
    }

    private void OnDisable()
    {
        StopSlicing();
    }

    private void OnEnable()
    {
        StopSlicing();
    }

    private void Update()
    {
        Vector3 currentBladePosition = followTarget.transform.position;
        float deltaX = currentBladePosition.x - lastBladePosition.x;
        float deltaY = currentBladePosition.y - lastBladePosition.y;
        if (slicing) 
        {
            ContinueSlicing();
        }
        else if (Mathf.Abs(deltaX) >= 8 || Mathf.Abs(deltaY) >= 8)
        {
            StartSlicing();
        }
        else if (Mathf.Abs(deltaX) < 8 || Mathf.Abs(deltaY) < 8)
        {
            StopSlicing();
        }

    }

    private void StartSlicing()
    {
        Vector3 newPosition = followTarget.transform.position;
        newPosition.z = 0f;

        transform.position = newPosition;

        slicing = true;
        bladeCollider.enabled = true;
        bladeTrail.Play();
        bladeTrail.Clear();
    }

    private void StopSlicing()
    {
        slicing = false;
        bladeCollider.enabled = false;
        bladeTrail.Stop();
    }

    private void ContinueSlicing()
    {
        Vector3 newPosition = followTarget.transform.position;
        newPosition.z = 0f;

        direction = newPosition - transform.position;

        float velocity = direction.magnitude / Time.deltaTime;
        bladeCollider.enabled = velocity > minSliceVelocity;

        transform.position = newPosition;
    }
}
