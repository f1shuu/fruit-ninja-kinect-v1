using UnityEngine;
using System.Collections;

public class Fruit : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;
    public GameObject comboPopup;

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;
    private ParticleSystem juice;

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
    }

    private void Update()
    {
        if(!isSliced)
        {
            if(!CompareTag("Play Button") && !CompareTag("Quit Button"))
            {
                transform.Rotate(randomDirection * Time.deltaTime * randomSpeed);
            }
            else
            {
                transform.Rotate(Vector2.up * Time.deltaTime * 20);
            }
        }
    }

    public virtual void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        isSliced = true;
        if (!CompareTag("Play Button") && !CompareTag("Quit Button"))
        {
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
        }


        whole.SetActive(false);
        sliced.SetActive(true);
        if (outlineMesh)
        {
            Destroy(outlineMesh);
        }

        fruitCollider.enabled = false;
        juice.Play();

        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // StartCoroutine(RotateSliced(angle));

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody slice in slices)
        {
            slice.velocity = fruitRigidbody.velocity;
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(CompareTag("Play Button"))
        {
            Destroy(GameObject.FindWithTag("Quit Button"));
            Destroy(GameObject.Find("MainMenuBackground"));
            FindObjectOfType<GameManager>().NewGame();
          
        }
        if(CompareTag("Quit Button"))
        {
            Invoke("Quit", 1);
        }        
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

        

    private void Quit() {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
}

}
