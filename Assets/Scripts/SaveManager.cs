using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string SavePath => Path.Combine(
        Application.persistentDataPath,
        "gamesave.json"
    );

    public bool HasSaveData { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        HasSaveData = File.Exists(SavePath);

        Debug.Log($"Save Path: {SavePath}");
        Debug.Log($"Has Save: {HasSaveData}");
    }
    
    public void SaveGame()
    {
        try
        {
            SaveData data = new SaveData();
            
            data.totalColumns = GridManager.Instance.TotalColumns;
            data.totalRows    = GridManager.Instance.TotalRows;

            data.currentScore = ScoreManager.Instance.CurrentScore;
            data.turnScore = ScoreManager.Instance.TurnScore;
            data.highScore = ScoreManager.Instance.HighScore;
            
            data.cardStates = new List<CardSaveData>();
            List<CardController> allCards = GridManager.Instance.GetAllCards();

            for (int i = 0; i < allCards.Count; i++)
            {
                data.cardStates.Add(new CardSaveData
                {
                    cardID    = allCards[i].CardID,
                    cardIndex = i,
                    cardState = allCards[i].State.ToString()
                });
            }

            // Write data into JSON file
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
            HasSaveData = true;

            Debug.Log($"Game Saved!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Save Failed: {e.Message}");
        }
    }
    
    public SaveData LoadGame()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("No save file found!");
                return null;
            }

            string   json = File.ReadAllText(SavePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("Game Loaded!");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Load Failed: {e.Message}");
            return null;
        }
    }
    
    public void DeleteSave()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                HasSaveData = false;
                Debug.Log("Save Deleted!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Delete Failed: {e.Message}");
        }
    }
}