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
    public Text scoreLabel;

    public Text highScoreLabel;
    public Text finalStatsLabel;
    private int highScore = 0;

    public GameObject gameOver;

    [Header("Boutons")]
    public Button restartButton;      // Bouton principal du GameOver
    public Button pauseRestartButton; // Bouton "Recommencer" du menu pause
    public Button resumeButton;       // Bouton "Reprendre"

    public MusicControl music;

    private int lives;
    private int score;

    [Header("Menu Pause")]
    public GameObject pauseMenu;       
    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        lives = maxLives;
        if (livesLabel != null) livesLabel.text = $"Lives: {lives}";

        score = 0;
        if (scoreLabel != null) scoreLabel.text = $"Score: {score}";

        if (gameOver != null) gameOver.SetActive(false);

        // ----- Assignation des boutons -----
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            restartButton.gameObject.SetActive(false);
        }

        if (pauseRestartButton != null)
            pauseRestartButton.onClick.AddListener(RestartGame);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        // ---- HIGH SCORE ----
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreLabel != null)
            highScoreLabel.text = $"High Score: {highScore}";

        if (finalStatsLabel != null)
            finalStatsLabel.gameObject.SetActive(false);

        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    void Update()
    {
        // Appuyer sur P pour mettre en pause / reprendre
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // ====================
    //   FONCTIONS PAUSE
    // ====================

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ====================
    //   AUTRES FONCTIONS
    // ====================

    public void UpdateScore(int value)
    {
        score += value;
        if (scoreLabel != null) scoreLabel.text = $"Score: {score}";
    }

    public void TriggerGameOver(bool failure = true)
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            if (highScoreLabel != null)
                highScoreLabel.text = $"High Score: {highScore}";
        }

        if (finalStatsLabel != null)
        {
            int wavesCompleted = InvaderSwarm.Instance != null ? InvaderSwarm.Instance.GetWavesCompleted() : 0;
            finalStatsLabel.text = $"Score: {score}\nVagues battues: {wavesCompleted}\nHigh Score: {highScore}";
            finalStatsLabel.gameObject.SetActive(true);
        }

        if (gameOver != null) gameOver.SetActive(failure);
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
