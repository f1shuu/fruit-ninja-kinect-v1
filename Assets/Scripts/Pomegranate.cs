﻿using UnityEngine;
using System.Collections;
using TMPro;

public class Pomegranate : Fruit
{
    private int sliceCount = 0;
    public float slowDownFactor = 0.5f;
    public float sliceDuration = 3f;
    public float explosionForceMultiplier = 4f;
    public float explosionRadius = 4f;
    private Rigidbody pomegranateRigidbody;

    private GameObject sliceCountPopup;

    public AudioClip pomeBurst;

    public override void Awake()
    {
        base.Awake();
        pomegranateRigidbody = GetComponent<Rigidbody>();
    }


    public override void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        foundGameManager.audioSource.PlayOneShot(foundGameManager.fruitSliceClips[Random.Range(0, foundGameManager.fruitSliceClips.Length)]);
        sliceCount++;
        ShowSliceCount();
        pomegranateRigidbody.velocity = Vector3.zero;
        pomegranateRigidbody.angularVelocity = Vector3.zero;

        if (!foundGameManager.isPomegranateSliced)
        {
            foundGameManager.audioSource.PlayOneShot(fruitImpactClip);
            StartCoroutine(SlicePomegranate());
        }
    }

    private IEnumerator SlicePomegranate()
    {
        foundGameManager.isPomegranateSliced = true;

        yield return StartCoroutine(ChangeTimeScale(Time.timeScale, slowDownFactor, 1f));
        
        yield return new WaitForSecondsRealtime(sliceDuration);

        yield return StartCoroutine(ChangeTimeScale(slowDownFactor, 1f, 1f));

        Explode();
        AddSliceReward();
    }

    private IEnumerator ChangeTimeScale(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            Time.timeScale = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = to;
    }

    private void Explode()
    {
        foundGameManager.audioSource.PlayOneShot(pomeBurst);
        isSliced = true;
        whole.SetActive(false);
        sliced.SetActive(true);

        fruitCollider.enabled = false;
        juice.Play();

        GameObject splatterPrefab = juiceSplatters[Random.Range(0, juiceSplatters.Count)];
        Color juiceColor = juice.GetComponent<Renderer>().material.color;
        Vector3 splatterPosition = transform.position;
        splatterPosition.z += 4f;
        GameObject splatter = Instantiate(splatterPrefab, splatterPosition, Quaternion.identity);
        Splatter splatterScript = splatter.GetComponent<Splatter>();
        splatterScript.InitializeSplatter(juiceColor, splatterPosition);

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        float explosionForce = 10 * explosionForceMultiplier;

        foreach (Rigidbody slice in slices)
        {
            slice.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0f, ForceMode.Impulse);
            slice.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void ShowSliceCount()
    {
        if (sliceCountPopup != null)
        {
            Destroy(sliceCountPopup);
        }
        if (comboPopup)
        {
            sliceCountPopup = Instantiate(comboPopup, transform.position + Vector3.up * 3f, Quaternion.identity);
            var textMesh = sliceCountPopup.GetComponent<TextMeshPro>();
            if (textMesh)
            {
                textMesh.text = sliceCount + " Hits";
            }
        }
    }

    private void AddSliceReward()
    {
            var pop = Instantiate(comboPopup, transform.position + Vector3.up * 1f, Quaternion.identity);
            int points = sliceCount * 5 + sliceCount / 10 * 50;
            var textMesh = pop.GetComponent<TextMeshPro>();
            textMesh.text = "+ "+ points;
            foundGameManager.AddScore(points);
            foundGameManager.audioSource.PlayOneShot(foundGameManager.comboRewardClip, 0.6f);
            foundGameManager.increaseSlicedFruitCount();
    }
}