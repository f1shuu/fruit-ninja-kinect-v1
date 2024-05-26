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
        if (other.CompareTag("KinectPlayer") || other.CompareTag("Player"))
        {
        int randomBackgroundNumber = Random.Range(1, 6);
        base.OnTriggerEnter(other);
        foundGameManager = FindObjectOfType<GameManager>();
        StartCoroutine(foundGameManager.FadeOutMeshRenderer(GameObject.Find("MainMenuBackground"), 0.5f));
        Destroy(GameObject.FindObjectOfType<QuitButton>().gameObject);
        foundGameManager.NewGame();
        }
    }

    public override void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        base.Slice(direction, position, force);
        transform.GetChild(3).gameObject.SetActive(false);
        Destroy(transform.gameObject, lifeTime);
        
    }

}
