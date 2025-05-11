using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropZoneManager : MonoBehaviour
{
    public static DropZoneManager Instance;

    private void Awake() //각 지역에서 찾기 쉽게 우선 싱글톤으로 해놓았습니다.
    {
        Instance = this;
    }

    private Dictionary<string, DropZone> dropZones = new Dictionary<string, DropZone>(); //드롭존 관리용 딕셔너리
    public Text totalText; //지도UI 기준 상단부에 위치한 전체 배치 수를 보여주는 텍스트

    public event Action OnTestReset;  // 테스트 이벤트 정의

    public void TestReset() // 디버그2 버튼 작동시 실행되는 테스트용 시민 배치 초기화 코드입니다. 여기서 이벤트를 시작합니다.
    {
        Debug.Log("DropZoneManager: TestReset triggered");
        OnTestReset?.Invoke(); // 이벤트 발생
        UpdateTotal();
    }



    public void RegisterDropZone(string id, DropZone zone) // 각 드롭존(지역)을 이 클래스에서 한번에 관리하기 위해 딕셔너리에 추가합니다.
    {
        if (!dropZones.ContainsKey(id))
            dropZones.Add(id, zone);
    }

    public void UpdateTotal() //전체 배치 숫자 업데이트 코드
    {
        int total = 0;
        int max = ResourceManager.Instance.Population;

        foreach (var zone in dropZones.Values) //드롭존(지역)에 배치된 수를 전부 불러와서 현재 배치된 시민 수를 파악합니다.
        {
            total += zone.GetCurrentCount();
        }

        totalText.text = $"전체 배치: {total} / {max}";
    }

    
    public int GetZoneCount(string id) // 나중에 특정 지역에 몇명이 있는지 파악하기 위해 작성한 코드입니다.
    {
        if (dropZones.ContainsKey(id))
            return dropZones[id].GetCurrentCount();

        return 0;
    }
}