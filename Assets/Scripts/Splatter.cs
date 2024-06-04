using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splatter : MonoBehaviour
{
    public float splatterLifespan = 2f;

    public void InitializeSplatter(Color juiceColor, Vector3 position)
    {
        transform.position = position;
        SpriteRenderer splatterSpriteRenderer = GetComponent<SpriteRenderer>();
        splatterSpriteRenderer.color = juiceColor;
        StartCoroutine(FadeOut(splatterLifespan, splatterSpriteRenderer));
    }    

    private IEnumerator FadeOut(float duration, SpriteRenderer splatterSpriteRenderer)
    {
        float alpha = splatterSpriteRenderer.color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {   
            Color newColor = new Color(splatterSpriteRenderer.color.r, splatterSpriteRenderer.color.g, splatterSpriteRenderer.color.b, Mathf.Lerp(alpha, 0 , t));
            splatterSpriteRenderer.color = newColor;
            yield return null;
        }
        Destroy(gameObject);
    }    

}
