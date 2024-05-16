using UnityEngine;
using System.Collections;

public class Frenzy_Banana : Fruit
{
    private float frenzyDuration = 4f;

    public override void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        base.Slice(direction, position, force);
        StartCoroutine(ActivateFrenzy());
    }

    private IEnumerator ActivateFrenzy()
    {
        foundGameManager.ActivateFrenzy(frenzyDuration);
        yield return new WaitForSeconds(frenzyDuration);
    }
}
