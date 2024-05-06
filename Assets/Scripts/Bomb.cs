using UnityEngine;

public class Bomb : MonoBehaviour
{

    public GameObject bomb;
    private MeshFilter mesh;
    private ParticleSystem explosion;
    private ParticleSystem smoke;
    private Collider bombCollider;

    public int pointsValue = - 100;

    private float randomX;
    private float randomY;
    private Vector2 randomDirection;
    private float randomSpeed;

    private void Awake()
    {
        explosion = transform.GetChild(0).GetComponentInChildren<ParticleSystem>();
        smoke = transform.GetChild(1).GetComponentInChildren<ParticleSystem>();
        mesh = bomb.GetComponent<MeshFilter>();
        bombCollider = GetComponent<Collider>();
        randomX = Random.Range(-1f, 1f);
        randomY = Random.Range(-1f, 1f);
        randomDirection = new Vector2(randomX, randomY);
        randomDirection.Normalize();
        randomSpeed = Random.Range(40f, 100f);
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
            var foundGameManager = FindObjectOfType<GameManager>();
            foundGameManager.AddScore(pointsValue);
            foundGameManager.ResetCombo();
            Destroy(mesh);
            FindObjectOfType<GameManager>().RemoveLife();
            bombCollider.enabled = false;
        }
    }


}
