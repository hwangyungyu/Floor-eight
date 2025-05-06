using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropZoneManager : MonoBehaviour
{
    public static DropZoneManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private Dictionary<string, DropZone> dropZones = new Dictionary<string, DropZone>();
    public Text totalText;


    // 테스트 이벤트 정의
    public event Action OnTestReset;

    public void TestReset()
    {
        Debug.Log("DropZoneManager: TestReset triggered");
        OnTestReset?.Invoke(); // 이벤트 발생
        UpdateTotal();
    }



    public void RegisterDropZone(string id, DropZone zone)
    {
        if (!dropZones.ContainsKey(id))
            dropZones.Add(id, zone);
    }

    public void UpdateTotal()
    {
        int total = 0;
        int max = ResourceManager.Instance.Population;

        foreach (var zone in dropZones.Values)
        {
            total += zone.GetCurrentCount();
        }

        totalText.text = $"전체 배치: {total} / {max}";
    }

    // 외부에서 특정 DropZone 수량 조회
    public int GetZoneCount(string id)
    {
        if (dropZones.ContainsKey(id))
            return dropZones[id].GetCurrentCount();

        return 0;
    }

    public Dictionary<string, int> GetAllZoneCounts()
    {
        Dictionary<string, int> counts = new Dictionary<string, int>();

        foreach (var kvp in dropZones)
            counts.Add(kvp.Key, kvp.Value.GetCurrentCount());

        return counts;
    }
}