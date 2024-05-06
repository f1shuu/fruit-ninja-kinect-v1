using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;
    public GameObject comboPopup;

    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;
    private ParticleSystem juice;

    private GameObject quitButton;

    public int pointsValue = 10;

    private bool isSliced = false;

    private float randomX;
    private float randomY;
    private Vector2 randomDirection;
    private float randomSpeed;

    private GameManager foundGameManager;

    private void Awake()
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

    private void Slice(Vector3 direction = default(Vector3), Vector3 position = default(Vector3), float force = 0f)
    {
        isSliced = true;

        if (!CompareTag("Play Button") && !CompareTag("Quit Button"))
        {
        foundGameManager.AddScore(pointsValue);
            if(foundGameManager.CheckIfCombo())
            {
                ShowComboCount(foundGameManager.getComboCount());
            }
        }


        whole.SetActive(false);
        sliced.SetActive(true);

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
            quitButton = GameObject.FindWithTag("Quit Button");
            Destroy(quitButton);  
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

    private void Quit() {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
}

}
