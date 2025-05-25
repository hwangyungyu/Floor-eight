using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DropZoneManager : MonoBehaviour
{
    public static DropZoneManager Instance;

    public Button confirmButton;

    public GameObject testPanel;
    public GameObject citizenPrefab;
    public Transform canvasTransform;
    private Dictionary<string, DropZone> dropZones = new Dictionary<string, DropZone>(); //드롭존 관리용 딕셔너리
    public Text totalText; //지도UI 기준 상단부에 위치한 전체 배치 수를 보여주는 텍스트

    public event Action OnPopulationPlacementComplete; //테스트용 이벤트

    private void Awake() //각 지역에서 찾기 쉽게 우선 싱글톤으로 해놓았습니다.
    {
        Instance = this;
    }

    private void Start()
    {
        confirmButton.interactable = false; //확인버튼 상호작용금지
    }

    public event Action OnTestReset;  // 테스트 이벤트 정의

    public void TestReset() // 디버그2 버튼 작동시 실행되는 테스트용 시민 배치 초기화 코드입니다. 여기서 이벤트를 시작합니다.
    {
        Debug.Log("DropZoneManager: TestReset triggered");
        OnTestReset?.Invoke(); // 이벤트 발생
        AdjustBundleZonesToMatchPopulation();
        UpdateTotal();
        testPanel.SetActive(false);
    }

    
    private void AdjustBundleZonesToMatchPopulation()
    {//이 함수를 호출하면 현재 등록된 드롭존의 시민들과 시민 자원 수를 비교해서 수를 서로 맞춥니다.
        int desiredPopulation = ResourceManager.Instance.Population; //맞춰야할 시민 수, 시민 자원 수
        int currentPopulation = 0;

        DropZone bundleZone = null;
        List<DropZone> regularZones = new List<DropZone>();

        foreach (var pair in dropZones) //모든 드롭존에서 현재 시민개체 수를 가져옵니다
        {
            var zone = pair.Value;
            if(zone.isBundle)
            {
                bundleZone = zone;
            }
            regularZones.Add(zone);
            currentPopulation += zone.citizens.Count;
        }

        int diff = desiredPopulation - currentPopulation; //현재차이량을 구합니다

        if (diff > 0)
        {
            AddCitizensToBundleZone(bundleZone, diff); //번들존에 새로 필요한 시민 수 만큼을 추가
        }
        else if (diff < 0)
        {
            RemoveCitizensFromZonesRandomly(-diff); //모든 존에서 랜덤으로 차이만큼 제거
        }
    }
    private void AddCitizensToBundleZone(DropZone bundleZone, int countToAdd) //번들존에 입력받은 수만큼 시민을 추가합니다.
    {
        int remainingToAdd = countToAdd;
        while (remainingToAdd > 0)
        {
            GameObject newCitizen = Instantiate(citizenPrefab, canvasTransform);

            CitizenDrag draggingCitizen = newCitizen.GetComponent<CitizenDrag>();
            draggingCitizen.GetComponent<CitizenDrag>().bundle = bundleZone;

            draggingCitizen.GetComponent<CitizenDrag>().AssignButtonEvent();
            bundleZone.RegisterCitizen(draggingCitizen);
            remainingToAdd--;
        }
    }
    private void RemoveCitizensFromZonesRandomly(int countToRemove) //랜덤으로 시민을 제거
    {
        int remainingToRemove = countToRemove;

        // 시민이 존재하는 DropZone만 필터링
        List<DropZone> zonesWithCitizens = dropZones.Values
            .Where(zone => zone.citizens.Count > 0)
            .ToList();

        while (remainingToRemove > 0 && zonesWithCitizens.Count > 0)
        {
            // 무작위 DropZone 선택
            int index = UnityEngine.Random.Range(0, zonesWithCitizens.Count);
            DropZone selectedZone = zonesWithCitizens[index];

            // 마지막 시민 제거
            CitizenDrag citizenToRemove = selectedZone.GetLastCitizen();
            if (citizenToRemove != null)
            {
                selectedZone.UnregisterCitizen(citizenToRemove);
                Destroy(citizenToRemove.gameObject);
                remainingToRemove--;
            }

            // 더 이상 시민이 없으면 리스트에서 제거
            if (selectedZone.citizens.Count == 0)
            {
                zonesWithCitizens.RemoveAt(index);
            }
        }
    }

    public void RegisterDropZone(string id, DropZone zone) // 각 드롭존(지역)을 이 클래스에서 한번에 관리하기 위해 딕셔너리에 추가합니다.
    {
        if (!dropZones.ContainsKey(id))
            dropZones.Add(id, zone);
    }

    public void UpdateTotal() //전체 배치 숫자 업데이트 코드 및 최대치 배치 검사
    {
        int total = 0;
        int max = ResourceManager.Instance.Population;

        foreach (var zone in dropZones.Values) //드롭존(지역)에 배치된 수를 전부 불러와서 현재 배치된 시민 수를 파악합니다.
        {
            if (zone.isBundle) continue;
            total += zone.citizens.Count;
        }

        totalText.text = $"전체 배치: {total} / {max}";

        if(total == max)
        {
            confirmButton.interactable = true;
        }
        else
        {
            confirmButton.interactable = false;
        }
    }

    public void EndPopulationPlace() //테스트용으로 작성한 시민 배치 완료 함수, 버튼에 연결됩니다.
    {
        foreach (var dropZone in dropZones)
        {
            DropZone zone = dropZone.Value; 
            if (zone.isBundle) continue;
            int count = zone.citizens.Count;
            if (count == 0)
                continue;
            List<EventCard> selectedCards = zone.GetRandomEventCards(count);
            if (selectedCards == null || selectedCards.Count == 0)
                continue;
            foreach(var selectedCard in selectedCards)
            {
                if (selectedCard != null)
                {
                    GameManager.Instance.eventCardManager.AddEventCardWithShuffle(GameManager.Day, selectedCard);
                    Debug.Log($"지역 {dropZone.Key}에서 {selectedCard.name} 카드가 {GameManager.Day}일차에 추가됨");
                }
            }
        }
        confirmButton.interactable = false;

        OnPopulationPlacementComplete?.Invoke();

        testPanel.SetActive(true);
    }
}