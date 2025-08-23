using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public int food;
    public int utilityItem;
    public int medicine;
    public int defense;
    public int mental;
    public int madness;
    public int population;
    public List<SpecialResource> specialResources;

    public string flagsRaw; //직렬화 문제로 문자열로 저장됩니다.

    public int currentCardDay;
    public int currentCardIndex; //우선 자동 저장이라 날이 넘어갈때 저장되고 있어서 -1 값을 가지게 됩니다.
    public List<string> serializedCardDecks; //직렬화 문제로 문자열로 저장됩니다.

    public GameData()
    {
        food = 0;
        utilityItem = 0;
        medicine = 0;
        defense = 0;
        mental = 0;
        madness = 0;
        population = 0;
        specialResources = new List<SpecialResource>();
        flagsRaw = "";

        currentCardDay = 1;
        currentCardIndex = -1;
        serializedCardDecks = new List<string>();
    }
}
