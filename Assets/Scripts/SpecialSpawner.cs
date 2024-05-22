using UnityEngine;
using System.Collections;
using System;

public class SpecialSpawner : MonoBehaviour
{
    private Collider[] spawnAreas;

    private directionEnum[] spawnAreaDirections;

    public GameObject[] specialFruitPrefabs;

    public GameObject[] fruitPrefabs;

    public GameObject pomegranatePrefab;

    public AudioClip throwFruitSound;

    private enum directionEnum
    {
        Right = 1,
        Left = -1
    }
    public float minAngle = -5f;
    public float maxAngle = 5f;

    public float minForce = 24f;
    public float maxForce = 28f;

    public float pomegranateForce = 36f;

    public float maxLifeTime = 5f;

    public int requiredSlicedFruits = 20;

    public bool shouldSpawnSpecialFruits {get; set;}

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
        shouldSpawnSpecialFruits = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public void SpawnPomegranate()
    {
        requiredSlicedFruits = -1;
        GameObject prefab = pomegranatePrefab;
        int randomSpawnIndex = UnityEngine.Random.Range(0, spawnAreas.Length);
        Collider spawnArea = spawnAreas[randomSpawnIndex];
        directionEnum spawnDirection = spawnAreaDirections[randomSpawnIndex];
        Vector3 position = spawnArea.bounds.center;
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        GameObject fruit = Instantiate(prefab, position, rotation);
        Destroy(fruit, maxLifeTime);
        fruit.GetComponent<Rigidbody>().AddForce(((int)spawnDirection == 1 ? 1 : -1) * fruit.transform.right * pomegranateForce, ForceMode.Impulse);
        foundGameManager.audioSource.PlayOneShot(throwFruitSound);
    }
    private IEnumerator Spawn()
    {   
        yield return new WaitForSeconds(2f);

        while (enabled)
        {
            while (foundGameManager.getSlicedFruitCount() != requiredSlicedFruits && !foundGameManager.getIsFrenzy())
            {
                yield return null;
            }         
            
            GameObject[] prefabs = foundGameManager.getIsFrenzy() ? fruitPrefabs : specialFruitPrefabs;
            GameObject prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];

            if(shouldSpawnSpecialFruits && Array.Exists(specialFruitPrefabs, element => element == prefab) || Array.Exists(fruitPrefabs, element => element == prefab))
            {
            int randomSpawnIndex = UnityEngine.Random.Range(0, spawnAreas.Length);
            Collider spawnArea = spawnAreas[randomSpawnIndex];
            directionEnum spawnDirection = spawnAreaDirections[randomSpawnIndex];

            Vector3 position = GetRandomPositionInBounds(spawnArea.bounds);
            
            Quaternion rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(minAngle, maxAngle));
            GameObject fruit = Instantiate(prefab, position, rotation);
            Destroy(fruit, maxLifeTime);

            float force = UnityEngine.Random.Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(((int)spawnDirection == 1 ? 1 : -1) * fruit.transform.right * force, ForceMode.Impulse);
            foundGameManager.audioSource.PlayOneShot(throwFruitSound);
            }

            if(!foundGameManager.getIsFrenzy())
            {
                requiredSlicedFruits += requiredSlicedFruitsStep;
            }

            yield return new WaitForSeconds(0.25f);
        }
    }


    private Vector3 GetRandomPositionInBounds(Bounds bounds)
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }


}
