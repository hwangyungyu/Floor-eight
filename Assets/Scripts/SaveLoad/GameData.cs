
using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    // Resource Data
    public int food;
    public int utilityItem;
    public int medicine;
    public int defense;
    public int mental;
    public int madness;
    public int population;

    // Flag Data
    public Dictionary<string, bool> flags;

    // EventCardManager Data
    public int currentCardDay;
    public int currentCardIndex;
    public List<string> serializedCardDecks;

    public GameData()
    {
        // Default values
        food = 0;
        utilityItem = 0;
        medicine = 0;
        defense = 0;
        mental = 0;
        madness = 0;
        population = 0;
        flags = new Dictionary<string, bool>();

        currentCardDay = 1;
        currentCardIndex = -1;
        serializedCardDecks = new List<string>();
    }
}
