using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject gameBackground;
    public GameObject startPopup;
    public GameObject pauseButton;
    public GameObject continueButton;
    public Text scoreText;
    public Text highScoreText;
    public Text doubleScoreText;
    public Text freezeText;
    public Text frenzyText;
    public Text livesText;
    public Text timerText;
    public Image fadeOutImage;
    public Image tintImage;
    public Image iceTextbox;
    public Image iceFrame;
    public Image pauseMenu;
    public Image pauseIcon;

    public AudioClip mainMenuMusic;
    public AudioClip gameStartClip;
    public AudioClip gameOverClip;
    public AudioClip timesUpClip;
    public AudioClip comboRewardClip;
    public AudioClip[] gameBackgroundMusic;
    public AudioClip[] fruitSliceClips;
    public AudioClip[] sliceComboClips;
    public AudioClip[] splatterClips;

    [HideInInspector]
    public AudioSource audioSource;

    private Color timerColor;
    private Color freezeColor;
    private Color scoreColor;
    private Color doubleScoreColor;

    private Blade blade;
    private Spawner spawner;
    private SpecialSpawner specialSpawner; 

    private List<Material> gameBackgroundMats = new List<Material>();

    private int score;
    private int lives;
    private float time;
    private bool isTimerPaused = false;
    private int comboCount;
    private int slicedFruitCount;
    [HideInInspector]
    public float comboTimeWindow = 0.8f;
    private float lastSliceTime;
    private bool isFrenzy = false;
    private float tintAlpha = 0.1f;
    private float scoreMultiplier = 1f;
    public bool isPomegranateSliced {get; set;}
    private Vector3 pauseButtonPosition;
    private Vector3 continueButtonPosition;
    private float currentTimeScale;


    private void Awake()
    {
        blade = FindObjectOfType<Blade>();
        spawner = FindObjectOfType<Spawner>();
        specialSpawner = FindObjectOfType<SpecialSpawner>();
        spawner.enabled = false;
        specialSpawner.enabled = false;
        timerColor = timerText.color;
        freezeColor = freezeText.color;
        scoreColor = scoreText.color;
        doubleScoreColor = doubleScoreText.color;
        pauseButtonPosition =  Camera.main.ScreenToWorldPoint(new Vector3(100, 100, Camera.main.nearClipPlane));
        pauseButtonPosition.z = 0;
        continueButtonPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f + 100, Screen.height / 2f, 0f));
        continueButtonPosition.z = -5;
        LoadGameBackgroundMats();

        audioSource = GetComponent<AudioSource>();

        if (mainMenuMusic != null && audioSource != null)
        {
            audioSource.clip = mainMenuMusic;
            audioSource.Play();
        }     
    }

    public void NewGame()
    {
        Time.timeScale = 1f;

        blade.enabled = true;
        spawner.enabled = true;  
        specialSpawner.enabled = true;

        gameBackground.gameObject.GetComponent<Renderer>().material = gameBackgroundMats[Random.Range(0, gameBackgroundMats.Count)];

        comboCount = 1;
        slicedFruitCount = 0;
        score = 0;
        lives = 3;
        time = 60.0f;
        scoreMultiplier = 1f;
        scoreText.text = "Score: " + score.ToString();
        livesText.text = "Lives: " + lives.ToString();
        timerText.text = "Time: " + time.ToString();
        doubleScoreText.gameObject.SetActive(false);
        freezeText.gameObject.SetActive(false);
        frenzyText.gameObject.SetActive(false);
        iceTextbox.gameObject.SetActive(false);
        iceFrame.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        pauseIcon.gameObject.SetActive(false);
        isTimerPaused = false;
        isFrenzy = false;
        isPomegranateSliced = false;
        lastSliceTime = Time.time;
        scoreText.color = scoreColor;
        timerText.color = timerColor;
        tintImage.color = new Color(tintImage.color.r, tintImage.color.g, tintImage.color.b, 0.0f);

        Instantiate(pauseButton, pauseButtonPosition, pauseButton.transform.rotation);

        UpdateHighScore();
        StartCoroutine(FadeIn(scoreText, 1f));
        StartCoroutine(FadeIn(highScoreText, 1f));
        StartCoroutine(FadeIn(livesText, 1f));
        StartCoroutine(FadeIn(timerText, 1f));
        StartCoroutine(Timer());
        StartCoroutine(showStartPopup());

        if (gameBackgroundMusic.Length > 0 && audioSource != null)
        {
            audioSource.clip = gameBackgroundMusic[Random.Range(0, gameBackgroundMusic.Length)];
            audioSource.Play();
        }     

    }

    private void ClearScene()
    {
    DestroyAllObjectsOfType<Fruit>();
    DestroyAllObjectsOfType<Bomb>();
    DestroyAllObjectsOfType<ComboPopup>();
    DestroyAllObjectsOfType<Splatter>();
    }

    private void DestroyAllObjectsOfType<T>() where T : MonoBehaviour
    {
        T[] objects = FindObjectsOfType<T>();

        foreach (T obj in objects)
        {
            Destroy((obj as MonoBehaviour).gameObject);
        }
    }

    public void AddScore(int pointsAmount)
    {
        if(pointsAmount > 0)
        {
            pointsAmount = Mathf.RoundToInt(pointsAmount * scoreMultiplier);
        }
        if(score + pointsAmount >= 0)
        {
            score += pointsAmount;
        }
        else
        {
            score = 0;
        }
        scoreText.text = "Score: " + score.ToString();
        CheckHighscore();
        UpdateHighScore();
    }

    public void PauseGame()
    {
        currentTimeScale = Time.timeScale;
        StartCoroutine(FadeIn(pauseMenu, 1f));
        StartCoroutine(FadeIn(pauseIcon, 1f));
        SetKinematic(true);
        Instantiate(continueButton, continueButtonPosition, continueButton.transform.rotation);
        Time.timeScale = 0.0f;
    }

    public void UnpauseGame()
    {
        StartCoroutine(FadeOut(pauseMenu, 1f));
        StartCoroutine(FadeOut(pauseIcon, 1f));
        SetKinematic(false);
        Instantiate(pauseButton, pauseButtonPosition, pauseButton.transform.rotation);
        Time.timeScale = currentTimeScale;

    }

    public void SetKinematic(bool isKinematic)
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();
        foreach (Fruit fruit in fruits)
        {
            if (fruit.CompareTag("PauseButton"))
            {
                continue;
            }
            Rigidbody fruitRigidbody = fruit.GetComponent<Rigidbody>();
            Rigidbody[] slices = fruit.GetComponentsInChildren<Rigidbody>();
            Collider fruitCollider = fruit.GetComponent<Collider>();

            if (isKinematic)
            {
                fruit.StoreVelocity();
            }
            else
            {
                fruit.ApplyStoredVelocity();
            }

            fruitRigidbody.isKinematic = isKinematic;
            fruitCollider.enabled = !isKinematic;
            foreach (Rigidbody slice in slices)
            {
                slice.isKinematic = isKinematic;
            }
        }
        Bomb[] bombs = FindObjectsOfType<Bomb>();
        foreach (Bomb bomb in bombs)
        {
            Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
            Collider bombCollider = bomb.GetComponent<Collider>();

            if (isKinematic)
            {
                bomb.StoreVelocity();
            }
            else
            {
                bomb.ApplyStoredVelocity();
            }

            bombRigidbody.isKinematic = isKinematic;
            bombCollider.enabled = !isKinematic;
        }
    }

    public void ActivateMultiplier(float multiplier, float duration)
    {
        StartCoroutine(ResetMultiplier(duration));
        scoreMultiplier = multiplier;
        StartCoroutine(FadeIn(doubleScoreText, 1f));
        scoreText.color = doubleScoreColor;
    }

    public void StopTimer(float duration, float slowFactor)
    {
        StartCoroutine(ResumeTimer(duration));
        isTimerPaused = true;
        StartCoroutine(FadeIn(freezeText, 1f));
        timerText.color = freezeColor;
        Time.timeScale = slowFactor;
        tintImage.color = new Color(tintImage.color.r, tintImage.color.g, tintImage.color.b, tintAlpha);
        StartCoroutine(FadeIn(iceTextbox, 1f));
        StartCoroutine(FadeIn(iceFrame, 1f));
    }

    public void ActivateFrenzy(float duration) 
    {
        isFrenzy = true;
        StartCoroutine(FadeIn(frenzyText, 1f));
        StartCoroutine(FinishFrenzy(duration));
    }

    public bool getIsFrenzy()
    {
        return isFrenzy;
    }


    public void increaseSlicedFruitCount()
    {
        slicedFruitCount += 1;
    }

    public int getSlicedFruitCount()
    {
        return slicedFruitCount;
    }

    public bool CheckIfCombo()
    {
        if (Time.time - lastSliceTime <= comboTimeWindow) 
        {
            AddComboCount();
            lastSliceTime = Time.time;
            if (comboCount > 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else 
        {
            lastSliceTime = Time.time;
            ResetCombo();
            return false; 
        }       
    }

    public void AddComboCount()
    {
        comboCount++;
    }

    public void ResetCombo()
    {
        comboCount = 1;
    }

    public int getComboCount()
    {
        return comboCount;
    }

    private void CheckHighscore()
    {
        if(score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }

    public void UpdateHighScore()
    {
        highScoreText.text = "Best: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    public void RemoveLife()
    {
        lives -= 1;
        livesText.text = "Lives: " + lives.ToString();
        StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake());
    }

    public void GameOver()
    {
        blade.enabled = false;
        spawner.enabled = false;
        specialSpawner.enabled = false;
        StopAllCoroutines();
        StartCoroutine(GameOverSequence());
        audioSource.PlayOneShot(gameOverClip);
    }

    public void TimesUp()
    {
        audioSource.PlayOneShot(timesUpClip);
        StartCoroutine(PomegranateSequence());
    }

    private void LoadGameBackgroundMats()
    {
        gameBackgroundMats.Clear();

        string folderPath = "Assets/Materials/Backgrounds";

        string[] matGUIDs = UnityEditor.AssetDatabase.FindAssets("t:Material", new[] { folderPath });
        foreach (string matGUID in matGUIDs)
        {
            string matPath = UnityEditor.AssetDatabase.GUIDToAssetPath(matGUID);
            Material mat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat != null)
            {
                gameBackgroundMats.Add(mat);
            }
        }
    }  

    private IEnumerator PomegranateSequence()
    {
        spawner.enabled = false;
        yield return new WaitForSeconds(3f);
        specialSpawner.SpawnPomegranate();
        yield return new WaitForSeconds(2f);
        if(isPomegranateSliced)
        {
            yield return new WaitForSeconds(3f);
        }
        GameOver();
    }


    private IEnumerator GameOverSequence()
    {
        float elapsed = 0f;
        float duration = 2f;

        while (elapsed < duration)
        {
            float fadeOutProgress = Mathf.Clamp01(elapsed / duration);
            fadeOutImage.color = Color.Lerp(Color.clear, Color.white, fadeOutProgress);

            Time.timeScale = 1f - fadeOutProgress;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
        yield return new WaitForSecondsRealtime(1f);

        ClearScene();
        NewGame();
        elapsed = 0f;

        while (elapsed < duration)
        {
            float fadeOutProgress = Mathf.Clamp01(elapsed / duration);
            fadeOutImage.color = Color.Lerp(Color.white, Color.clear, fadeOutProgress);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }        
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(5f);
        while (true)
        {
            if (time <= 0.0f)
            {
                TimesUp();
                yield break;
            }
            if (lives <= 0)
            {
                GameOver();
                yield break;
            }
            if(!isTimerPaused)
            {
                time -= Time.deltaTime;
            }
            if (time <= 5f && spawner.shouldSpawnBombs)
            {
                spawner.shouldSpawnBombs = false;
                specialSpawner.shouldSpawnSpecialFruits = false;
            }
            timerText.text = "Time: " + time.ToString("F0");
            yield return null;
        }

    }

    private IEnumerator ResetMultiplier(float duration)
    {
        yield return new WaitForSeconds(duration);
        scoreMultiplier = 1f;
        StartCoroutine(FadeOut(doubleScoreText, 1f));
        scoreText.color = scoreColor;
    }

    private IEnumerator ResumeTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        isTimerPaused = false;
        StartCoroutine(FadeOut(freezeText, 1f));
        timerText.color = timerColor;
        Time.timeScale = 1f;
        tintImage.color = new Color(tintImage.color.r, tintImage.color.g, tintImage.color.b, 0.0f);
        StartCoroutine(FadeOut(iceTextbox, 1f));
        StartCoroutine(FadeOut(iceFrame, 1f));
    }

    private IEnumerator FinishFrenzy(float duration)
    {
        yield return new WaitForSeconds(duration);
        StartCoroutine(FadeOut(frenzyText, 1f));
        isFrenzy = false;
    }  

    private IEnumerator showStartPopup ()
    {
        Vector2 popupPosition = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));

        yield return new WaitForSeconds(1f);

        var pop1 = Instantiate(startPopup, popupPosition, Quaternion.identity);
        var textMesh1 = pop1.GetComponent<TextMeshPro>();
        textMesh1.text = "60 seconds";

        yield return new WaitForSeconds(2f);

        var pop2 = Instantiate(startPopup, popupPosition, Quaternion.identity);
        var textMesh2 = pop2.GetComponent<TextMeshPro>();
        textMesh2.text = "Go!";
        audioSource.PlayOneShot(gameStartClip);
    }

    public IEnumerator FadeIn(Graphic graphic, float duration)
    {
        graphic.gameObject.SetActive(true);
        Color originalColor = graphic.color;
        originalColor.a = 0;
        graphic.color = originalColor;

        for (float t = 0.01f; t < duration; t += Time.fixedDeltaTime)
        {
            Color newColor = graphic.color;
            newColor.a = Mathf.Lerp(0, 1, t / duration);
            graphic.color = newColor;
            yield return null;
        }

        originalColor.a = 1;
        graphic.color = originalColor;
    }

    public IEnumerator FadeOut(Graphic graphic, float duration)
    {
        Color originalColor = graphic.color;

        for (float t = 0.01f; t < duration; t += Time.fixedDeltaTime)
        {
            Color newColor = graphic.color;
            newColor.a = Mathf.Lerp(1, 0, t / duration);
            graphic.color = newColor;
            yield return null;
        }

        originalColor.a = 0;
        graphic.color = originalColor;
        graphic.gameObject.SetActive(false);
    }

    public IEnumerator FadeOutMeshRenderer(GameObject gameObject, float duration)
    {
        if (gameObject == null)
        {
            Debug.LogError("GameObject is null");
            yield break;
        }

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("GameObject does not have a MeshRenderer component");
            yield break;
        }

        Material material = meshRenderer.material;
        if (material == null || !material.HasProperty("_Color"))
        {
            Debug.LogError("Material does not have a _Color property");
            yield break;
        }

        Color initialColor = material.color;
        float alpha = initialColor.a;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(alpha, 0, normalizedTime));
            material.color = newColor;
            yield return null;
        }

        material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
    }

    public IEnumerator FadeInMeshRenderer(GameObject gameObject, float duration)
    {
        if (gameObject == null)
        {
            Debug.LogError("GameObject is null");
            yield break;
        }

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("GameObject does not have a MeshRenderer component");
            yield break;
        }

        Material material = meshRenderer.material;
        if (material == null || !material.HasProperty("_Color"))
        {
            Debug.LogError("Material does not have a _Color property");
            yield break;
        }

        Color initialColor = material.color;
        float alpha = initialColor.a;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(0, alpha, normalizedTime));
            material.color = newColor;
            yield return null;
        }

        material.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
    }

      
}
