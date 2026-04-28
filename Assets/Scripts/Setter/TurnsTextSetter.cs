using System.Collections;
using TMPro;
using UnityEngine;

public class TurnsTextSetter : MonoBehaviour
{
    [Header("UI References")]
    private TextMeshProUGUI _turnsText;

    private void Awake()
    {
        _turnsText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        ScoreManager.Instance.OnTurnChanged += UpdateTurnUI;
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance == null) return;

        ScoreManager.Instance.OnTurnChanged -= UpdateTurnUI;
    }

    private void UpdateTurnUI(int turnsCount)
    {
        if (_turnsText == null) return;
        _turnsText.text = ""+turnsCount;

        // Animate score text
        StartCoroutine(PunchScale(_turnsText.transform));
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
