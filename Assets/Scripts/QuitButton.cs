using UnityEngine;
using System.Collections;

public class QuitButton : Fruit
{


    public override void Update()
    {
        transform.GetChild(0).Rotate(transform.forward * Time.deltaTime * 20);
        transform.GetChild(3).Rotate(Vector2.up * Time.deltaTime * 20);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KinectPlayer") || other.CompareTag("Player"))
        {
        base.OnTriggerEnter(other);
        Invoke("Quit", 1f);
        }
    }

    public override void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        base.Slice(direction, position, force);
        transform.GetChild(3).gameObject.SetActive(false);
        
    }

    private void Quit() {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
}    

}
