using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // Grid
    public int totalColumns;
    public int totalRows;

    // Score
    public int currentScore;
    public int turnScore;
    public int highScore;

    // Cards
    public List<CardSaveData> cardStates;
}

[Serializable]
public class CardSaveData
{
    public int    cardID;
    public int    cardIndex;
    public string cardState; // "FaceDown" / "Matched"
}