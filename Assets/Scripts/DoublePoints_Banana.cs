using UnityEngine;
using System.Collections;

public class DoublePoints_Banana : Fruit
{
    private float multiplier = 2f;
    private float multiplierDuration = 8f;

    public override void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        base.Slice(direction, position, force);
        StartCoroutine(ActivateMultiplier());
    }

    private IEnumerator ActivateMultiplier()
    {
        foundGameManager.ActivateMultiplier(multiplier, multiplierDuration);
        yield return new WaitForSeconds(multiplierDuration);
    }
}