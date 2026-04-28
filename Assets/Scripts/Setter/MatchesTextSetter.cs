using System.Collections;
using TMPro;
using UnityEngine;

public class MatchesTextSetter : MonoBehaviour
{
    [Header("UI References")]
    private TextMeshProUGUI _matchesText;

    private void Awake()
    {
        _matchesText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance == null) return;

        ScoreManager.Instance.OnScoreChanged -= UpdateScoreUI;
    }

    private void UpdateScoreUI(int matchedScore)
    {
        if (_matchesText == null) return;

        _matchesText.text = ""+matchedScore;

        // Animate combo text
        if (matchedScore > 1)
            StartCoroutine(PunchScale(_matchesText.transform));
    }

    /// <summary>
    /// Small punch scale animation on score update
    /// </summary>
    private IEnumerator PunchScale(Transform target)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 punchScale = Vector3.one * 1.3f;
        float duration = 0.15f;
        float elapsed = 0f;

        // Scale up
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(originalScale, punchScale, elapsed / duration);
            yield return null;
        }

        elapsed = 0f;

        // Scale back down
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(punchScale, originalScale, elapsed / duration);
            yield return null;
        }

        target.localScale = originalScale;
    }
}
