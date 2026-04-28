using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Home Panel")]
    [SerializeField] private GameObject homePanel;
    [SerializeField] private TMP_Dropdown gridDropdown;
    
    [Header("Game Panel")]
    [SerializeField] private GameObject gamePanel;
    
    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI totalTurnsText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Hide game over panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // restart button event handle
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
    }
    
    /// <summary>
    /// Called by GameManager for Resume last game
    /// </summary>
    public void LoadSavedGame(SaveData data)
    {
        homePanel.SetActive(false);
        gamePanel.SetActive(true);
        GameManager.Instance.ResumeGameStart(data);
        ScoreManager.Instance.LoadHighScore();
    }
    
    /// <summary>
    /// Called by Play button from Home Screen
    /// </summary>
    public void StartGame()
    {
        // Get selected text from dropdown (4x4)
        string selectedValue = gridDropdown.options[gridDropdown.value].text;

        // Split "4x4"
        string[] parts = selectedValue.Split('x');

        // Parse columns and rows
        int columns = int.Parse(parts[0]);
        int rows = int.Parse(parts[1]);
        
        homePanel.SetActive(false);
        gamePanel.SetActive(true);
        GridManager.Instance.UpdateGrid(columns, rows);
        GameManager.Instance.RestartGame();
        ScoreManager.Instance.LoadHighScore();
    }
    
    /// <summary>
    /// Called by GameManager when game is over
    /// </summary>
    public void ShowGameOver()
    {
        if (gameOverPanel == null) return;

        // Set final values
        if (finalScoreText != null)
            finalScoreText.text = "" + ScoreManager.Instance.CurrentScore;

        if (totalTurnsText != null)
            totalTurnsText.text = "" + ScoreManager.Instance.TurnScore;
        
        if (highScoreText != null)
            highScoreText.text = "" + ScoreManager.Instance.HighScore;
        
        // Show with animation
        StartCoroutine(ShowGameOverAnimation());
    }

    private IEnumerator ShowGameOverAnimation()
    {
        gameOverPanel.SetActive(true);

        // Scale from 0 to 1
        Transform panelTransform = gameOverPanel.transform;
        panelTransform.localScale = Vector3.zero;

        float elapsed  = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Ease out back effect
            float scale = EaseOutBack(t);
            panelTransform.localScale = Vector3.one * scale;

            yield return null;
        }

        panelTransform.localScale = Vector3.one;
    }

    private void OnRestartClicked()
    {
        // Hide panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Restart game
        GameManager.Instance.RestartGame();
    }

    public void OnHomeClicked()
    {
        GameManager.Instance.ResetGame();
    }

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}