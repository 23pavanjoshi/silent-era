using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;

    private List<CardController> _flippedCards  = new List<CardController>();
    private List<CardController> _matchedCards  = new List<CardController>();
    private bool _isCheckingMatch = false;
    private int _totalPairs = 0;
    private int _matchedPairs = 0;

    [Header("Settings")]
    [SerializeField] private float flipBackDelay = 0.8f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitGame();
    }
    
    public void InitGame()
    {
        // Clear all records if restarted game
        _flippedCards.Clear();
        _matchedCards.Clear();
        _matchedPairs  = 0;
        CurrentState  = GameState.Playing;

        // Get total pairs from GridManager
        int totalCards = GridManager.Instance.GetAllCards().Count;
        _totalPairs = totalCards / 2;

        Debug.Log($"Game Started! Total Pairs: {_totalPairs}");
    }
    
    /// <summary>
    /// Called by CardController when card is flipped
    /// </summary>
    public void OnCardFlipped(CardController card)
    {
        // Block if game is over
        if (CurrentState != GameState.Playing) return;

        // Block duplicate card
        if (_flippedCards.Contains(card)) return;

        // Add to flipped list
        _flippedCards.Add(card);

        Debug.Log($"Card Flipped: ID {card.CardID} | Total Flipped: {_flippedCards.Count}");

        // Check match when 2 cards are flipped
        if (_flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }
    
    /// <summary>
    /// Compare two flipped cards
    /// </summary>
    private IEnumerator CheckMatch()
    {
        _isCheckingMatch = true;

        // Grab the two cards
        CardController cardA = _flippedCards[0];
        CardController cardB = _flippedCards[1];

        // Clear list immediately
        // This allows player to keep flipping other cards
        // without waiting for match animation to finish
        _flippedCards.Clear();

        // Wait before checking
        yield return new WaitForSeconds(flipBackDelay);

        if (cardA.CardID == cardB.CardID)
        {
            cardA.SetMatched();
            cardB.SetMatched();

            _matchedCards.Add(cardA);
            _matchedCards.Add(cardB);
            _matchedPairs++;

            Debug.Log($"Match Found! ID: {cardA.CardID} | Matched Pairs: {_matchedPairs}/{_totalPairs}");

            // Check if game is over
            CheckGameOver();
        }
        else
        {
            Debug.Log($"Mismatch! Card {cardA.CardID} vs Card {cardB.CardID}");

            // Flip both cards back down
            cardA.FlipDown();
            cardB.FlipDown();
        }

        _isCheckingMatch = false;
    }

    private void CheckGameOver()
    {
        if (_matchedPairs >= _totalPairs)
        {
            CurrentState = GameState.GameOver;

            Debug.Log("Game Over! All Pairs Matched!");
        }
    }

    /// <summary>
    /// Restart game — called by UI restart button
    /// </summary>
    public void RestartGame()
    {
        StopAllCoroutines();

        // Regenerate grid
        GridManager.Instance.GenerateGrid(
            GridManager.Instance.TotalColumns,
            GridManager.Instance.TotalRows
        );

        InitGame();

        Debug.Log("Game Restarted!");
    }

    public int GetMatchedPairs() => _matchedPairs;
    public int GetTotalPairs() => _totalPairs;
    public bool IsPlaying() => CurrentState == GameState.Playing;
}