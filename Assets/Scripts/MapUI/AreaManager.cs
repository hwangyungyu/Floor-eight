using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaManager : MonoBehaviour
{
    public static AreaManager Instance;
    public Dictionary<string, Area> areas = new Dictionary<string, Area>();

    public Button confirmButton;

    public GameObject testPanel;
    public Text totalText; //지도UI 기준 상단부에 위치한 전체 배치 수를 보여주는 텍스트
    public event Action OnPopulationPlacementComplete; //테스트용 이벤트

    private void Awake() //각 지역에서 찾기 쉽게 우선 싱글톤으로 해놓았습니다.
    {
        Instance = this;
    }
    private void Start()
    {

    }
    public void UpdateTotal() //전체 배치 숫자 업데이트 코드 및 최대치 배치 검사
    {
        int total = 0;
        int max = ResourceManager.Instance.Population;

        foreach (var area in areas.Values) //드롭존(지역)에 배치된 수를 전부 불러와서 현재 배치된 시민 수를 파악합니다.
        {
            total += area.currentCitizenAmount;
        }

        totalText.text = $"전체 배치: {total} / {max}";

        if (total == max)
        {
            confirmButton.interactable = true;
        }
        else
        {
            confirmButton.interactable = false;
        }
    }

    public void EndPopulationPlace() //배치 완료 버튼에서 호출(카드를 뽑아서 덱에 넣습니다.)
    {
        foreach (var areaPair in areas)
        {
            Area area = areaPair.Value;
            int count = area.currentCitizenAmount;

            if (count == 0)
                continue;

            List<EventCardInfo> selectedCardInfos = area.GetRandomEventCards(count);
            if (selectedCardInfos == null || selectedCardInfos.Count == 0)
                continue;

            foreach (var selectedCardInfo in selectedCardInfos)
            {
                if (selectedCardInfo != null)
                {
                    GameManager.Instance.eventCardManager.AddEventCardWithShuffle(GameManager.Day, selectedCardInfo.Card, selectedCardInfo.Area);
                    Debug.Log($"지역 {area.areaID}에서 {selectedCardInfo.Card.name} 카드가 {GameManager.Day}일차에 추가됨");
                }
            }
        }

        confirmButton.interactable = false;

        OnPopulationPlacementComplete?.Invoke();

        testPanel.SetActive(true);
    }

    public void RegisterArea(string id, Area area) // 각 드롭존(지역)을 이 클래스에서 한번에 관리하기 위해 딕셔너리에 추가합니다.
    {
        if (!areas.ContainsKey(id))
            areas.Add(id, area);
    }
}
