using UnityEngine;
using System.Collections;

public class QuitButton : Fruit
{


    public override void Update()
    {
        transform.Rotate(Vector2.up * Time.deltaTime * 20);
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Invoke("Quit", 1f);
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
