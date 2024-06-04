using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = 0.5f;
    public AnimationCurve curve;
    public IEnumerator Shake()
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;
            float strength = curve.Evaluate(elapsed / duration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }
}
