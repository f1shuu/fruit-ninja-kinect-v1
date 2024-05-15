using UnityEngine;
using System.Collections;

public class SpecialSpawner : MonoBehaviour
{
    private Collider[] spawnAreas;

    private directionEnum[] spawnAreaDirections;

    public GameObject[] fruitPrefabs;


    private enum directionEnum
    {
        Right = 1,
        Left = -1
    }
    public float minAngle = -10f;
    public float maxAngle = 10f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float maxLifeTime = 5f;

    public int requiredSlicedFruits = 20;

    private int requiredSlicedFruitsStep;

    private GameManager foundGameManager;

    private void Awake()
    {
        spawnAreas = GetComponents<Collider>();
        foundGameManager = FindObjectOfType<GameManager>();
        spawnAreaDirections = new directionEnum[spawnAreas.Length];
        spawnAreaDirections[0] = directionEnum.Right;
        spawnAreaDirections[1] = directionEnum.Left;
        requiredSlicedFruitsStep = requiredSlicedFruits;
    }

    private void OnEnable()
    {
        requiredSlicedFruits = requiredSlicedFruitsStep;
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
            while (foundGameManager.getSlicedFruitCount() != requiredSlicedFruits)
            {
                yield return null;
            }         

            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            int randomSpawnIndex = Random.Range(0, spawnAreas.Length);
            Collider spawnArea = spawnAreas[randomSpawnIndex];
            directionEnum spawnDirection = spawnAreaDirections[randomSpawnIndex];

            Vector3 position = GetRandomPositionInBounds(spawnArea.bounds);
            
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));
            GameObject fruit = Instantiate(prefab, position, rotation);
            Destroy(fruit, maxLifeTime);

            float force = Random.Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(((int)spawnDirection == 1 ? 1 : -1) * fruit.transform.right * force, ForceMode.Impulse);
            requiredSlicedFruits += requiredSlicedFruitsStep;

            yield return new WaitForSeconds(0.25f);
        }
    }

    private Vector3 GetRandomPositionInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
