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
