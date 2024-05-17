using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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

    private Color timerColor;
    private Color freezeColor;
    private Color scoreColor;
    private Color doubleScoreColor;

    private Blade blade;
    private Spawner spawner;
    private SpecialSpawner specialSpawner; 

    private int score;
    private int lives;
    private float time;
    private bool isTimerPaused = false;
    private int comboCount;
    private int slicedFruitCount;
    private float comboTimeWindow = 0.8f;
    private float lastSliceTime;
    private bool isFrenzy = false;

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
    }

    public void NewGame()
    {
        Time.timeScale = 1f;

        blade.enabled = true;
        spawner.enabled = true;  
        specialSpawner.enabled = true;

        comboCount = 1;
        slicedFruitCount = 0;
        score = 0;
        lives = 3;
        time = 60.0f;
        scoreMultiplier = 1f;
        scoreText.text = "Score: " + score.ToString();
        livesText.text = "Lives: " + lives.ToString();
        timerText.text = "Time: " + time.ToString();
        scoreText.gameObject.SetActive(true);
        highScoreText.gameObject.SetActive(true);
        livesText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        playText.gameObject.SetActive(false);
        quitText.gameObject.SetActive(false);
        doubleScoreText.gameObject.SetActive(false);
        freezeText.gameObject.SetActive(false);
        frenzyText.gameObject.SetActive(false);
        isTimerPaused = false;
        isFrenzy = false;
        lastSliceTime = Time.time;
        scoreText.color = scoreColor;
        timerText.color = timerColor;
        UpdateHighScore();
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
        doubleScoreText.gameObject.SetActive(true);
        scoreText.color = doubleScoreColor;
    }

    public void StopTimer(float duration, float slowFactor)
    {
        StartCoroutine(ResumeTimer(duration));
        isTimerPaused = true;
        freezeText.gameObject.SetActive(true);
        timerText.color = freezeColor;
        Time.timeScale = slowFactor;
    }

    public void ActivateFrenzy(float duration) 
    {
        isFrenzy = true;
        frenzyText.gameObject.SetActive(true);
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
        doubleScoreText.gameObject.SetActive(false);
        scoreText.color = scoreColor;
    }

    private IEnumerator ResumeTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        isTimerPaused = false;
        freezeText.gameObject.SetActive(false);
        timerText.color = timerColor;
        Time.timeScale = 1f;
    }

    private IEnumerator FinishFrenzy(float duration)
    {
        yield return new WaitForSeconds(duration);
        frenzyText.gameObject.SetActive(false);
        isFrenzy = false;
    }    

}
