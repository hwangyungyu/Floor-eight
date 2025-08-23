using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private const string EmptyDeckMarker = "EMPTY_DECK"; // 비어있는 덱을 표시할 특수 문자열
    private string saveFilePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
    }

    public void SaveGame()
    {
        GameData data = new GameData();
        var eventCardManager = GameManager.Instance.eventCardManager;

        data.food = ResourceManager.Instance.Food;
        data.utilityItem = ResourceManager.Instance.UtilityItem;
        data.medicine = ResourceManager.Instance.Medicine;
        data.defense = ResourceManager.Instance.Defense;
        data.mental = ResourceManager.Instance.Mental;
        data.madness = ResourceManager.Instance.Madness;
        data.population = ResourceManager.Instance.Population;
        data.specialResources = ResourceManager.Instance.specialResources;

        var flags = FlagManager.Instance.GetAllFlags();
        data.flagsRaw = string.Join(";", flags.Select(pair => $"{pair.Key}:{pair.Value}"));

        data.currentCardDay = eventCardManager.currentCardDay;
        data.currentCardIndex = eventCardManager.currentCardIndex;
        data.serializedCardDecks = SerializeDecks(eventCardManager.GetDecksForSave());

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game data saved to: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            var eventCardManager = GameManager.Instance.eventCardManager;

            ResourceManager.Instance.Food = data.food;
            ResourceManager.Instance.UtilityItem = data.utilityItem;
            ResourceManager.Instance.Medicine = data.medicine;
            ResourceManager.Instance.Defense = data.defense;
            ResourceManager.Instance.Mental = data.mental;
            ResourceManager.Instance.Madness = data.madness;
            ResourceManager.Instance.Population = data.population;
            ResourceManager.Instance.specialResources = data.specialResources;

            FlagManager.Instance.ClearAllFlags();
            if (!string.IsNullOrEmpty(data.flagsRaw))
            {
                var entries = data.flagsRaw.Split(';');
                foreach (var entry in entries)
                {
                    var parts = entry.Split(':');
                    if (parts.Length == 2)
                    {
                        string key = parts[0];
                        if (bool.TryParse(parts[1], out bool value))
                        {
                            FlagManager.Instance.SetFlag(key, value);
                        }
                    }
                }
            }

            eventCardManager.currentCardDay = data.currentCardDay;
            eventCardManager.currentCardIndex = data.currentCardIndex;
            eventCardManager.RestoreDecks(DeserializeDecks(data.serializedCardDecks, eventCardManager));

            Debug.Log("로딩 성공.");
            GameManager.Instance.UIUpdate();
        }
        else
        {
            Debug.LogWarning("파일을 찾을 수 없습니다!");
        }
    }

    private List<string> SerializeDecks(List<EventCardDeck> decks)
    {
        List<string> serializedDecks = new List<string>();
        foreach (var deck in decks)
        {
            if (deck == null || deck.GetCardInfoList().Count == 0)
            {
                serializedDecks.Add(EmptyDeckMarker);
                continue;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var cardInfo in deck.GetCardInfoList())
            {
                sb.Append(cardInfo.Card.EventID);
                sb.Append(';');
                sb.Append(cardInfo.Area ?? "");
                sb.Append('|');
            }
            if (sb.Length > 0) sb.Length--; // 마지막 '|' 제거
            serializedDecks.Add(sb.ToString());
        }
        return serializedDecks;
    }

    private List<EventCardDeck> DeserializeDecks(List<string> serializedDecks, EventCardManager manager)
    {
        List<EventCardDeck> decks = new List<EventCardDeck>();
        foreach (var s_deck in serializedDecks)
        {
            if (s_deck == EmptyDeckMarker)
            {
                decks.Add(new EventCardDeck()); // 비어있는 덱 객체를 추가
                continue;
            }

            EventCardDeck deck = new EventCardDeck();
            string[] cardInfos = s_deck.Split('|');
            foreach (var s_info in cardInfos)
            {
                if (string.IsNullOrEmpty(s_info)) continue;

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