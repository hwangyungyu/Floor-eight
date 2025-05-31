using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EventCard", menuName = "EventCard/New EventCard")]
public class EventCard : ScriptableObject
{
    [Header("이벤트 ID")]
    public string EventID;

    [Header("이벤트 텍스트")]
    [TextArea]
    public string EventText;

    [Header("자원 트리거 (자원 이름, 최소 수량)")]
    public List<ItemTrigger> ItemTrigger;

    [Header("플래그 트리거 (플래그 이름, bool 값)")]
    public List<FlagTrigger> FlagTrigger;

    [Header("1번 선택지 활성화 여부")]
    public bool ChoiceEnabled1 = true;
    [Header("2번 선택지 활성화 여부")]
    public bool ChoiceEnabled2 = true;
    [Header("3번 선택지 활성화 여부")]
    public bool ChoiceEnabled3 = false;

    [Header("1번 선택지 텍스트")]
    public string ChoiceText1;
    [Header("2번 선택지 텍스트")]
    public string ChoiceText2;
    [Header("3번 선택지 텍스트")]
    public string ChoiceText3;

    [Header("1번 선택지 효과")]
    public List<string> ChoiceEffect1;
    [Header("2번 선택지 효과")]
    public List<string> ChoiceEffect2;
    [Header("3번 선택지 효과")]
    public List<string> ChoiceEffect3;
}

[System.Serializable]
public class ItemTrigger
{
    public string ItemName;
    public int RequiredAmount;
}

[System.Serializable]
public class FlagTrigger
{
    public string FlagName;
    public bool RequiredValue;
}