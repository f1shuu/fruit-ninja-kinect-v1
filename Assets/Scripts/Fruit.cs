using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fruit : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;
    public GameObject comboPopup;

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;
    protected ParticleSystem juice;

     private List<GameObject> juiceSplatters = new List<GameObject>();

    private MeshFilter outlineMesh;

    private GameObject quitButton;

    public int pointsValue = 10;

    private bool isSliced = false;

    private float randomX;
    private float randomY;
    private Vector2 randomDirection;
    private float randomSpeed;

    protected GameManager foundGameManager;


    public virtual void Awake()
    {
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        juice = GetComponentInChildren<ParticleSystem>();
        randomX = Random.Range(-1f, 1f);
        randomY = Random.Range(-1f, 1f);
        randomDirection = new Vector2(randomX, randomY);
        randomDirection.Normalize();
        randomSpeed = Random.Range(20f, 100f);
        foundGameManager = FindObjectOfType<GameManager>();
        if(transform.childCount > 3)
        {
            Transform childTransform = transform.GetChild(3);
            if(childTransform != null)
            {
                outlineMesh = childTransform.GetComponentInChildren<MeshFilter>();
            }
        }
         LoadJuiceSplatters();
    }

    public virtual void Update()
    {
        if(!isSliced)
        {
            transform.Rotate(randomDirection * Time.deltaTime * randomSpeed);
        }
    }

    public virtual void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        isSliced = true;
        foundGameManager.AddScore(pointsValue);
        if(!foundGameManager.getIsFrenzy())
        {
            foundGameManager.increaseSlicedFruitCount();
        }
            if(foundGameManager.CheckIfCombo())
            {
                int comboCount = foundGameManager.getComboCount();
                Vector3 popupPosition = transform.position;
                ShowComboCount(comboCount);
                StartCoroutine(AddComboScore(foundGameManager, comboCount, popupPosition));
            }


        whole.SetActive(false);
        sliced.SetActive(true);
        if (outlineMesh)
        {
            Destroy(outlineMesh);
        }

        fruitCollider.enabled = false;
        juice.Play();
        
        GameObject splatterPrefab = juiceSplatters[Random.Range(0, juiceSplatters.Count)];
        Color juiceColor = juice.GetComponent<Renderer>().material.color;
        Vector3 splatterPosition = transform.position;
        splatterPosition.z += 4f;
        GameObject splatter = Instantiate(splatterPrefab, splatterPosition, Quaternion.identity);
        Splatter splatterScript = splatter.GetComponent<Splatter>();
        splatterScript.InitializeSplatter(juiceColor, splatterPosition);

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody slice in slices)
        {
            slice.velocity = fruitRigidbody.velocity;
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {    
        if (other.CompareTag("KinectPlayer")) 
        {
            KinectBlade blade = other.GetComponent<KinectBlade>();
            Slice(blade.direction, blade.transform.position, blade.sliceForce);
        }
        else if (other.CompareTag("Player")) 
        {
            Blade blade = other.GetComponent<Blade>();
            Slice(blade.direction, blade.transform.position, blade.sliceForce);
        }
    }

    private void ShowComboCount(int comboCount)
    {
        if(comboPopup)
        {
            var pop = Instantiate(comboPopup, transform.position, Quaternion.identity);
            pop.GetComponent<TextMesh>().text = "Combo x " + comboCount;
        }
    }

    private void LoadJuiceSplatters()
    {
        juiceSplatters.Clear();

        string folderPath = "Assets/Prefabs/Splatters";

        string[] splatterGUIDs = UnityEditor.AssetDatabase.FindAssets("t:GameObject", new[] { folderPath });
        foreach (string splatterGUID in splatterGUIDs)
        {
            string splatterPath = UnityEditor.AssetDatabase.GUIDToAssetPath(splatterGUID);
            GameObject splatter = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(splatterPath);
            if (splatter != null)
            {
                juiceSplatters.Add(splatter);
            }
        }
    }    

    private IEnumerator AddComboScore(GameManager foundGameManager, int comboCount, Vector3 popupPosition)
    {
        yield return new WaitForSeconds(0.8f);

        if(foundGameManager.getComboCount() == comboCount)
        {
            var pop = Instantiate(comboPopup, popupPosition, Quaternion.identity);
            int points = comboCount;
            var textMesh = pop.GetComponent<TextMesh>();
            textMesh.text = "Combo reward: "+ points;
            textMesh.color = new Color32(255, 255, 255, 255);
            textMesh.fontStyle = FontStyle.Italic;
            foundGameManager.AddScore(points);
        }

    }

      

}
