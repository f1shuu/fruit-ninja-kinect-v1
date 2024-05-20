using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject[] fruitPrefabs;

    public GameObject bombPrefab;

    [Range(0f, 1f)]
    public float bombChance = 0.1f;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 18f;
    public float maxForce = 22f;

    private float maxLifeTime;
    public float maxFruitLifeTime = 10f;
    public float maxBombLifeTime = 5f;

    private AudioSource audioSource;

    public AudioClip throwFruitSound;
    public AudioClip throwBombSound;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (enabled)
        {
            AudioClip clip = throwFruitSound;
            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
            maxLifeTime = maxFruitLifeTime;

            if (Random.value < bombChance)
            {
                prefab = bombPrefab;
                clip = throwBombSound;
                maxLifeTime = maxBombLifeTime;
            }

            Vector3 position = new Vector3();
            position.x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
            position.y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);
            position.z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z);

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));
            GameObject fruit = Instantiate(prefab, position, rotation);
            Destroy(fruit, maxLifeTime);

            float force = Random.Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(fruit.transform.up * force, ForceMode.Impulse);
            audioSource.PlayOneShot(clip, 1f);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }
}
