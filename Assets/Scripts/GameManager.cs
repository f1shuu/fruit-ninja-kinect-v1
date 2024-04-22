using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text scoreText;
    public Image fadeOutImage;

    private Blade blade;
    private Spawner spawner;

    private int score;

    private void Awake()
    {
        blade = FindObjectOfType<Blade>();
        spawner = FindObjectOfType<Spawner>();
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        Time.timeScale = 1f;

        blade.enabled = true;
        spawner.enabled = true;  

        score = 0;
        scoreText.text = "Score: " + score.ToString();

        ClearScene();

    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();

        foreach (Fruit fruit in fruits)
        {
            Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();

        foreach (Bomb bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }
    }

    public void AddScore(int pointsAmount)
    {
        score += pointsAmount;
        scoreText.text = "Score: " + score.ToString();
    }

    public void Explode()
    {
        blade.enabled = false;
        spawner.enabled = false;

        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            float fadeOutProgress = Mathf.Clamp01(elapsed / duration);
            fadeOutImage.color = Color.Lerp(Color.clear, Color.white, fadeOutProgress);

            Time.timeScale = 1f - fadeOutProgress;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

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
}
