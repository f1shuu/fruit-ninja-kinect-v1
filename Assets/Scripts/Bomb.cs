using UnityEngine;

public class Bomb : MonoBehaviour
{

    public GameObject bomb;
    private MeshFilter mesh;
    private MeshFilter outlineMesh;
    private ParticleSystem explosion;
    private ParticleSystem smoke;
    private Collider bombCollider;

    public int pointsValue = - 100;

    private float randomX;
    private float randomY;
    private Vector2 randomDirection;
    private float randomSpeed;

    private AudioSource audioSource;

    public AudioClip bombFuseClip;
    public AudioClip bombExplodeClip;

    private void Awake()
    {
        explosion = transform.GetChild(0).GetComponentInChildren<ParticleSystem>();
        smoke = transform.GetChild(1).GetComponentInChildren<ParticleSystem>();
        mesh = bomb.GetComponent<MeshFilter>();
        outlineMesh = transform.GetChild(2).GetComponentInChildren<MeshFilter>();
        bombCollider = GetComponent<Collider>();
        randomX = Random.Range(-1f, 1f);
        randomY = Random.Range(-1f, 1f);
        randomDirection = new Vector2(randomX, randomY);
        randomDirection.Normalize();
        randomSpeed = Random.Range(40f, 100f);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bombFuseClip;
        audioSource.loop = true;
        audioSource.volume = 0.2f;
        audioSource.Play();
    }

    private void Update()
    {
        transform.Rotate(randomDirection * Time.deltaTime * randomSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("KinectPlayer")) 
        {
            explosion.Play();
            smoke.Stop();
            audioSource.clip = bombExplodeClip;
            audioSource.volume = 1f;
            audioSource.loop = false;
            audioSource.Play();
            var foundGameManager = FindObjectOfType<GameManager>();
            foundGameManager.AddScore(pointsValue);
            foundGameManager.ResetCombo();
            Destroy(mesh);
            Destroy(outlineMesh);
            FindObjectOfType<GameManager>().RemoveLife();
            bombCollider.enabled = false;
        }
    }


}
