using UnityEngine;
using System.Collections;

public class PlayButton : Fruit
{

    public float lifeTime = 1f;

    public override void Update()
    {
        transform.GetChild(0).Rotate(transform.forward * Time.deltaTime * 20);
        transform.GetChild(3).Rotate(Vector2.up * Time.deltaTime * 20);
    }

    public override void OnTriggerEnter(Collider other)
    {
        int randomBackgroundNumber = Random.Range(1, 6);
        base.OnTriggerEnter(other);
        foundGameManager = FindObjectOfType<GameManager>();
        StartCoroutine(FadeOutMeshRenderer(GameObject.Find("MainMenuBackground"), 0.5f));
        Destroy(GameObject.FindObjectOfType<QuitButton>().gameObject);
        foundGameManager.NewGame();
    }

    public override void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        base.Slice(direction, position, force);
        transform.GetChild(3).gameObject.SetActive(false);
        Destroy(transform.gameObject, lifeTime);
        
    }

    private IEnumerator FadeOutMeshRenderer(GameObject gameObject, float duration)
    {
        if (gameObject == null)
        {
            Debug.LogError("GameObject is null");
            yield break;
        }

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("GameObject does not have a MeshRenderer component");
            yield break;
        }

        Material material = meshRenderer.material;
        if (material == null || !material.HasProperty("_Color"))
        {
            Debug.LogError("Material does not have a _Color property");
            yield break;
        }

        Color initialColor = material.color;
        float alpha = initialColor.a;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(alpha, 0, normalizedTime));
            material.color = newColor;
            yield return null;
        }

        material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
    }
}
