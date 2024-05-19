using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameBackground;
    public Text scoreText;
    public Text highScoreText;
    public Text doubleScoreText;
    public Text freezeText;
    public Text frenzyText;
    public Text livesText;
    public Text playText;
    public Text quitText;
    public Text timerText;
    public Image fadeOutImage;
    public Image tintImage;
    public Image iceTextbox;
    public Image iceFrame;

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
    private float comboTimeWindow = 0.8f;
    private float lastSliceTime;
    private bool isFrenzy = false;
    private float tintAlpha = 0.1f;

    public float scoreMultiplier = 1f;

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
        LoadGameBackgroundMats();
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
        playText.gameObject.SetActive(false);
        quitText.gameObject.SetActive(false);
        doubleScoreText.gameObject.SetActive(false);
        freezeText.gameObject.SetActive(false);
        frenzyText.gameObject.SetActive(false);
        iceTextbox.gameObject.SetActive(false);
        iceFrame.gameObject.SetActive(false);
        isTimerPaused = false;
        isFrenzy = false;
        lastSliceTime = Time.time;
        scoreText.color = scoreColor;
        timerText.color = timerColor;
        tintImage.color = new Color(tintImage.color.r, tintImage.color.g, tintImage.color.b, 0.0f);

        UpdateHighScore();
        StartCoroutine(FadeIn(scoreText, 1f));
        StartCoroutine(FadeIn(highScoreText, 1f));
        StartCoroutine(FadeIn(livesText, 1f));
        StartCoroutine(FadeIn(timerText, 1f));
        StartCoroutine(Timer());

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


    private IEnumerator GameOverSequence()
    {
        float elapsed = 0f;
        float duration = 0.6f;

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
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if (time <= 0.0f || lives == 0)
            {
                GameOver();
            }
            if(!isTimerPaused)
            {
                time -= Time.deltaTime;
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

    public IEnumerator FadeIn(Graphic graphic, float duration)
    {
        graphic.gameObject.SetActive(true);
        Color originalColor = graphic.color;
        originalColor.a = 0;
        graphic.color = originalColor;

        for (float t = 0.01f; t < duration; t += Time.deltaTime)
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

        for (float t = 0.01f; t < duration; t += Time.deltaTime)
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

    
      
}


