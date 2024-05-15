using UnityEngine;
using System.Collections;

public class Freeze_Banana : Fruit
{
    private float freezeDuration = 4f;
    private float slowFactor = 0.75f;

    public override void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        base.Slice(direction, position, force);
        StartCoroutine(StopTimer());
        
    }

    private IEnumerator StopTimer()
    {
        foundGameManager.StopTimer(freezeDuration, slowFactor);
        yield return new WaitForSeconds(freezeDuration);
    }
}
