using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardController : MonoBehaviour
{
    [Header("Card Faces")]
    [SerializeField] private GameObject frontFace;
    [SerializeField] private GameObject backFace;

    [Header("Card Image")]
    [SerializeField] private Image frontImage; // shows card value/icon

    [Header("Flip Settings")]
    [SerializeField] private float flipDuration = 0.3f;

    public enum CardState
    {
        FaceDown,
        FaceUp,
        Matched
    }

    public CardState State { get; private set; } = CardState.FaceDown;
    public int CardID { get; private set; }

    private bool isFlipping = false;
    private RectTransform rectTransform;
    private Button cardButton;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cardButton = GetComponent<Button>();

        // button click listener add
        if (cardButton != null)
            cardButton.onClick.AddListener(OnCardClicked);
    }
    
    /// <summary>
    /// Instantly set matched on load without no animation
    /// </summary>
    public void SetMatchedInstant()
    {
        State = CardState.Matched;
        frontFace.SetActive(true);
        backFace.SetActive(false);
    }
    
    /// <summary>
    /// setup this card
    /// </summary>
    public void Setup(int id, Sprite frontSprite)
    {
        CardID = id;
        if (frontImage != null)
            frontImage.sprite = frontSprite;

        // Always start face down
        frontFace.SetActive(false);
        backFace.SetActive(true);
        State = CardState.FaceDown;
    }
    
    public void OnCardClicked()
    {
        // Block if matched
        if (State == CardState.Matched) return;

        // Block if already face up
        if (State == CardState.FaceUp) return;

        // Block if currently flipping this card
        if (isFlipping) return;

        // Flip up and notify GameManager
        FlipUp();
        
        // Play flip sound
        AudioManager.Instance.PlayFlip();
        
        GameManager.Instance.OnCardFlipped(this);
    }

    public void FlipUp()
    {
        if (State == CardState.Matched) return;
        State = CardState.FaceUp;
        StartCoroutine(FlipAnimation(true));
    }

    public void FlipDown()
    {
        if (State == CardState.Matched) return;
        State = CardState.FaceDown;
        StartCoroutine(FlipAnimation(false));
    }

    public void SetMatched()
    {
        State = CardState.Matched;
    }

    private IEnumerator FlipAnimation(bool isFacingUp)
    {
        isFlipping = true;

        float elapsed = 0f;
        float half = flipDuration / 2f;

        Vector3 originalScale = rectTransform.localScale;
        Vector3 midScale = new Vector3(0f, originalScale.y, originalScale.z);

        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / half;
            rectTransform.localScale = Vector3.Lerp(originalScale, midScale, t);
            yield return null;
        }

        rectTransform.localScale = midScale;

        // Swap faces at midpoint
        frontFace.SetActive(isFacingUp);
        backFace.SetActive(!isFacingUp);

        elapsed = 0f;
        Vector3 fullScale = new Vector3(1f, originalScale.y, originalScale.z);

        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / half;
            rectTransform.localScale = Vector3.Lerp(midScale, fullScale, t);
            yield return null;
        }

        rectTransform.localScale = fullScale;
        isFlipping = false;
    }
}