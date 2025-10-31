using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public AudioSource sfx;
    public GameObject explosionPrefab;
    public float explosionTime = 1f;
    public AudioClip explosionClip;

    public int maxLives = 3;
    public Text livesLabel;

    public MusicControl music;

    public Text scoreLabel;
    public GameObject gameOver;
    public GameObject allClear;
    public Button restartButton;

    private int lives;
    private int score;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        lives = maxLives;
        if (livesLabel != null) livesLabel.text = $"Lives: {lives}";

        score = 0;
        if (scoreLabel != null) scoreLabel.text = $"Score: {score}";

        if (gameOver != null) gameOver.SetActive(false);
        if (allClear != null) allClear.SetActive(false);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                Time.timeScale = 1f;
            });
            restartButton.gameObject.SetActive(false);
        }
    }

    public void UpdateScore(int value)
    {
        score += value;
        if (scoreLabel != null) scoreLabel.text = $"Score: {score}";
    }

    public void TriggerGameOver(bool failure = true)
    {
        if (gameOver != null) gameOver.SetActive(failure);
        if (allClear != null) allClear.SetActive(!failure);
        if (restartButton != null) restartButton.gameObject.SetActive(true);

        Time.timeScale = 0f;
        if (music != null) music.StopPlaying();
    }

    public void UpdateLives()
    {
        lives = Mathf.Clamp(lives - 1, 0, maxLives);
        if (livesLabel != null) livesLabel.text = $"Lives: {lives}";

        if (lives <= 0) TriggerGameOver();
    }

    public void CreateExplosion(Vector2 position)
    {
        if (explosionClip != null) PlaySfx(explosionClip);
        if (explosionPrefab != null)
        {
            var explosion = Instantiate(explosionPrefab, position, Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f)));
            Destroy(explosion, explosionTime);
        }
    }

    public void PlaySfx(AudioClip clip)
    {
        if (sfx != null && clip != null) sfx.PlayOneShot(clip);
    }
}
