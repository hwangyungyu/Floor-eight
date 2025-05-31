using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaData", menuName = "Scriptable Objects/AreaData")]
public class AreaData : ScriptableObject
{
    public string areaID;
    public bool enabled;
    public List<string> events;
    public List<int> bonus;
    public List<int> penalty;
    public int maxCapacity;
}