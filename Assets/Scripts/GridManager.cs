using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    
    [Header("References")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GridLayoutGroup cardGrid;
    [SerializeField] private RectTransform gameBoardRect;

    [Header("Grid Settings")]
    [SerializeField] private int totalColumns = 4;
    [SerializeField] private int totalRows = 4;
    
    public int TotalColumns => totalColumns;
    public int TotalRows => totalRows;
    
    [Header("Card Spacing")]
    [SerializeField] private float cardSpacing = 10f;

    [Header("Card Sprites")]
    [SerializeField] private List<Sprite> cardSprites;

    
    private List<CardController> allCards = new List<CardController>();

    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
    }

    public void InitGrid()
    {
        GenerateGrid(totalColumns, totalRows);
    }
    
    /// <summary>
    /// Call this to generate any grid layout
    /// </summary>
    public void GenerateGrid(int columns, int rows)
    {
        int totalCards = columns * rows;
        if (totalCards % 2 != 0)
        {
            Debug.LogWarning("Total cards must be even number for pairing!!!");
            return;
        }

        // Clear existing cards
        ClearGrid();

        // Store grid settings row and col
        totalColumns = columns;
        totalRows    = rows;

        // Calculate correct card size
        Vector2 cardSize = CalculateCardSize(columns, rows);

        // Apply to Grid Layout Group
        cardGrid.constraint      = GridLayoutGroup.Constraint.FixedColumnCount;
        cardGrid.constraintCount = columns;
        cardGrid.cellSize        = cardSize;
        cardGrid.spacing         = new Vector2(cardSpacing, cardSpacing);
        cardGrid.childAlignment  = TextAnchor.MiddleCenter;
        cardGrid.padding         = new RectOffset(10, 10, 10, 10);

        // Generate paired card IDs and shuffle
        List<int> cardIDs = GenerateCardIDs(totalCards);

        // Spawn cards
        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardGrid.transform);
            CardController card = cardObj.GetComponent<CardController>();

            // Get sprite for this card ID
            Sprite sprite = GetSpriteForID(cardIDs[i]);

            // Setup card
            card.Setup(cardIDs[i], sprite);

            allCards.Add(card);
        }

        // Force layout rebuild after spawning
        LayoutRebuilder.ForceRebuildLayoutImmediate(cardGrid.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Get all cards list
    /// </summary>
    public List<CardController> GetAllCards()
    {
        return allCards;
    }
    
    /// <summary>
    /// Calculate card size to fit inside GameBoard
    /// </summary>
    private Vector2 CalculateCardSize(int columns, int rows)
    {
        // Get available board size
        float boardWidth  = gameBoardRect.rect.width;
        float boardHeight = gameBoardRect.rect.height;

        // Subtract padding (10 on each side)
        float paddingX = 20f;
        float paddingY = 20f;

        // Subtract total spacing
        float totalSpacingX = cardSpacing * (columns - 1);
        float totalSpacingY = cardSpacing * (rows - 1);

        // Calculate available space per card
        float cardWidth  = (boardWidth  - paddingX - totalSpacingX) / columns;
        float cardHeight = (boardHeight - paddingY - totalSpacingY) / rows;

        // Use smaller value to keep cards square
        float cardSize = Mathf.Min(cardWidth, cardHeight);

        Debug.Log($"Calculated Card Size: {cardSize} x {cardSize}");

        return new Vector2(cardSize, cardSize);
    }

    /// <summary>
    /// Generate shuffled paired card IDs
    /// </summary>
    private List<int> GenerateCardIDs(int totalCards)
    {
        List<int> ids = new List<int>();
        int pairs = totalCards / 2;

        // Add pairs Example: [0,0,1,1,2,2,3,3]
        for (int i = 0; i < pairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        // Shuffle using Fisher-Yates
        for (int i = ids.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = ids[i];
            ids[i] = ids[randomIndex];
            ids[randomIndex]= temp;
        }

        return ids;
    }

    /// <summary>
    /// Get sprite by card ID
    /// </summary>
    private Sprite GetSpriteForID(int id)
    {
        if (cardSprites != null && id < cardSprites.Count)
            return cardSprites[id];

        return null;
    }

    /// <summary>
    /// Clear all existing cards from grid and list
    /// </summary>
    private void ClearGrid()
    {
        foreach (Transform child in cardGrid.transform)
            Destroy(child.gameObject);

        allCards.Clear();
    }
    
    /// <summary>
    /// Restore grid from save data
    /// </summary>
    public void LoadFromSave(SaveData data)
    {
        ClearGrid();

        totalColumns = data.totalColumns;
        totalRows    = data.totalRows;

        StartCoroutine(LoadGridCoroutine(data));
    }

    private IEnumerator LoadGridCoroutine(SaveData data)
    {
        yield return new WaitForEndOfFrame();

        Vector2 cardSize         = CalculateCardSize(totalColumns, totalRows);
        cardGrid.constraint      = GridLayoutGroup.Constraint.FixedColumnCount;
        cardGrid.constraintCount = totalColumns;
        cardGrid.cellSize        = cardSize;
        cardGrid.spacing         = new Vector2(cardSpacing, cardSpacing);
        cardGrid.childAlignment  = TextAnchor.MiddleCenter;
        cardGrid.padding         = new RectOffset(10, 10, 10, 10);

        foreach (CardSaveData cardData in data.cardStates)
        {
            GameObject     cardObj = Instantiate(cardPrefab, cardGrid.transform);
            CardController card    = cardObj.GetComponent<CardController>();
            Sprite         sprite  = GetSpriteForID(cardData.cardID);

            card.Setup(cardData.cardID, sprite);

            // Restore matched state instantly
            if (cardData.cardState == "Matched")
                card.SetMatchedInstant();

            allCards.Add(card);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            cardGrid.GetComponent<RectTransform>()
        );

        // Tell GameManager grid is ready
        GameManager.Instance.OnGridLoaded(data);
    }
}