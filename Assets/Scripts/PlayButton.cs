using UnityEngine;
using System.Collections;

public class PlayButton : Fruit
{


    public override void Update()
    {
        transform.Rotate(Vector2.up * Time.deltaTime * 20);
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        Destroy(GameObject.Find("MainMenuBackground"));
        Destroy(GameObject.FindObjectOfType<QuitButton>().gameObject);
        FindObjectOfType<GameManager>().NewGame();
    }

}
