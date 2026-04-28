using UnityEngine;
using System.Collections.Generic;

public class CardPoolManager : MonoBehaviour
{
    public static CardPoolManager Instance { get; private set; }

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int        initialPoolSize = 30;

    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitPool();
    }

    private void InitPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewCard();
        }
    }

    private GameObject CreateNewCard()
    {
        GameObject card = Instantiate(cardPrefab, transform);
        card.SetActive(false);
        pool.Add(card);
        return card;
    }

    /// <summary>
    /// Get card from pool
    /// If not available then create new one
    /// </summary>
    public GameObject GetCard(Transform parent)
    {
        // Find inactive card in pool
        foreach (GameObject card in pool)
        {
            if (!card.activeInHierarchy)
            {
                card.SetActive(true);
                card.transform.SetParent(parent);
                card.transform.localScale = Vector3.one;
                return card;
            }
        }

        // No inactive card found → create new
        GameObject newCard = CreateNewCard();
        newCard.SetActive(true);
        newCard.transform.SetParent(parent);
        newCard.transform.localScale = Vector3.one;
        return newCard;
    }

    /// <summary>
    /// Return all active cards to pool
    /// </summary>
    public void ReturnAll()
    {
        foreach (GameObject card in pool)
        {
            if (card.activeInHierarchy)
            {
                card.SetActive(false);
                card.transform.SetParent(transform);
            }
        }
    }
}