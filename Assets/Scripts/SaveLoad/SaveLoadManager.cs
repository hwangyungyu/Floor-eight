
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private string saveFilePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
    }

    public void SaveGame()
    {
        GameData data = new GameData();
        var eventCardManager = GameManager.Instance.eventCardManager;

        // --- Populate data from Managers ---
        // ResourceManager
        data.food = ResourceManager.Instance.Food;
        data.utilityItem = ResourceManager.Instance.UtilityItem;
        data.medicine = ResourceManager.Instance.Medicine;
        data.defense = ResourceManager.Instance.Defense;
        data.mental = ResourceManager.Instance.Mental;
        data.madness = ResourceManager.Instance.Madness;
        data.population = ResourceManager.Instance.Population;

        // FlagManager
        data.flags = FlagManager.Instance.GetAllFlags();

        // EventCardManager
        data.currentCardDay = eventCardManager.currentCardDay;
        data.currentCardIndex = eventCardManager.currentCardIndex;
        data.serializedCardDecks = SerializeDecks(eventCardManager.GetDecksForSave());

        // --- Save to JSON ---
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game data saved to: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            // --- Load from JSON ---
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            var eventCardManager = GameManager.Instance.eventCardManager;

            // --- Apply data to Managers ---
            // ResourceManager
            ResourceManager.Instance.Food = data.food;
            ResourceManager.Instance.UtilityItem = data.utilityItem;
            ResourceManager.Instance.Medicine = data.medicine;
            ResourceManager.Instance.Defense = data.defense;
            ResourceManager.Instance.Mental = data.mental;
            ResourceManager.Instance.Madness = data.madness;
            ResourceManager.Instance.Population = data.population;

            // FlagManager
            FlagManager.Instance.ClearAllFlags();
            if (data.flags != null)
            {
                foreach (var flag in data.flags)
                {
                    FlagManager.Instance.SetFlag(flag.Key, flag.Value);
                }
            }

            // EventCardManager
            eventCardManager.currentCardDay = data.currentCardDay;
            eventCardManager.currentCardIndex = data.currentCardIndex;
            eventCardManager.RestoreDecks(DeserializeDecks(data.serializedCardDecks, eventCardManager));

            Debug.Log("Game data loaded.");
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
    }

    private List<string> SerializeDecks(List<EventCardDeck> decks)
    {
        List<string> serializedDecks = new List<string>();
        foreach (var deck in decks)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var cardInfo in deck.GetCardInfoList())
            {
                sb.Append(cardInfo.Card.EventID);
                sb.Append(';');
                sb.Append(cardInfo.Area ?? "");
                sb.Append('|');
            }
            if (sb.Length > 0) sb.Length--; // Remove last '|'
            serializedDecks.Add(sb.ToString());
        }
        return serializedDecks;
    }

    private List<EventCardDeck> DeserializeDecks(List<string> serializedDecks, EventCardManager manager)
    {
        List<EventCardDeck> decks = new List<EventCardDeck>();
        foreach (var s_deck in serializedDecks)
        {
            EventCardDeck deck = new EventCardDeck();
            if (string.IsNullOrEmpty(s_deck)) continue;

            string[] cardInfos = s_deck.Split('|');
            foreach (var s_info in cardInfos)
            {
                string[] parts = s_info.Split(';');
                EventCard card = manager.GetEventCardById(parts[0]);
                if (card != null)
                {
                    string area = parts.Length > 1 ? parts[1] : null;
                    deck.AddEventCard(card, area);
                }
            }
            decks.Add(deck);
        }
        return decks;
    }
}
