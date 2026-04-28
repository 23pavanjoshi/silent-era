using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event Action<int> OnTurnChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;
    
    private int _turnScore = 0;
    private int _currentScore = 0;
    private int _highScore = 0;

    public int CurrentScore => _currentScore;
    public int TurnScore => _turnScore;
    public int HighScore => _highScore;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        LoadHighScore();
        ResetScore();
    }

    /// <summary>
    /// Called by GameManager on 1 card open
    /// </summary>
    public void OnTurnTaken()
    {
        _turnScore += 1;
        
        // Fire events
        OnTurnChanged?.Invoke(_turnScore);
    }
    
    /// <summary>
    /// Called by GameManager on successful match
    /// </summary>
    public void OnMatch()
    {
        // Increment combo
        _currentScore += 1;
        
        // Check High score
        CheckHighScore();
        
        // Fire events
        OnScoreChanged?.Invoke(_currentScore);
    }
    
    /// <summary>
    /// Reset score for new game
    /// </summary>
    public void ResetScore()
    {
        _turnScore = 0;
        _currentScore = 0;

        OnTurnChanged?.Invoke(_turnScore);
        OnScoreChanged?.Invoke(_currentScore);
    }
    
    private void CheckHighScore()
    {
        // Check high score
        if (_currentScore > _highScore)
        {
            _highScore = _currentScore;
            SaveHighScore();
            OnHighScoreChanged?.Invoke(_highScore);
        }
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", _highScore);
        PlayerPrefs.Save();
    }

    public void LoadHighScore()
    {
        _highScore = PlayerPrefs.GetInt("HighScore", 0);
        OnHighScoreChanged?.Invoke(_highScore);
    }
    
    /// <summary>
    /// Restore score from save data
    /// </summary>
    public void RestoreScore(int score, int turn, int savedHighScore)
    {
        _currentScore = score;
        _turnScore = turn;
        _highScore = Mathf.Max(savedHighScore, _highScore);

        // Update UI
        OnScoreChanged?.Invoke(_currentScore);
        OnTurnChanged?.Invoke(_turnScore);
        OnHighScoreChanged?.Invoke(_highScore);
    }
}