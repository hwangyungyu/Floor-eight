using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    {
        int desiredPopulation = ResourceManager.Instance.Population;
        int currentPopulation = 0;

        DropZone bundleZone = null;
        List<DropZone> regularZones = new List<DropZone>();

        foreach (var pair in dropZones)
        {
            var zone = pair.Value;
            if(zone.isBundle)
            {
                bundleZone = zone;
            }
            regularZones.Add(zone);
            currentPopulation += zone.citizens.Count;
        }

        int diff = desiredPopulation - currentPopulation;

        if (diff > 0)
        {
            AddCitizensToBundleZone(bundleZone, diff);
        }
        else if (diff < 0)
        {
            RemoveCitizensFromZonesRandomly(-diff);
        }
    }
    private void AddCitizensToBundleZone(DropZone bundleZone, int countToAdd)
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
    private void RemoveCitizensFromZonesRandomly(int countToRemove)
    {
        int remainingToRemove = countToRemove;

        // 시민이 존재하는 DropZone만 필터링
        List<DropZone> zonesWithCitizens = dropZones.Values
            .Where(zone => zone.citizens.Count > 0)
            .ToList();

        System.Random rng = new System.Random();

        while (remainingToRemove > 0 && zonesWithCitizens.Count > 0)
        {
            // 무작위 DropZone 선택
            int index = rng.Next(zonesWithCitizens.Count);
            DropZone selectedZone = zonesWithCitizens[index];

            // 시민 제거
            CitizenDrag citizenToRemove = selectedZone.GetLastCitizen();
            if (citizenToRemove != null)
            {
                selectedZone.UnregisterCitizen(citizenToRemove);
                Destroy(citizenToRemove.gameObject);
                remainingToRemove--;
            }

            // 더 이상 시민이 없으면 제거 대상에서 제외
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
    public List<DropZone> GetAllDropZones()
    {
        return new List<DropZone>(dropZones.Values); // dropZones는 DropZone 등록 딕셔너리
    }
}