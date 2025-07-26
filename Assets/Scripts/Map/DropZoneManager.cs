using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DropZoneManager : MonoBehaviour
{
    public static DropZoneManager Instance;

    public GameObject citizenPrefab;
    public Transform canvasTransform;
    private List<DropZone> dropZones = new List<DropZone>(); //드롭존 관리용 리스트
    

    private void Awake() //각 지역에서 찾기 쉽게 우선 싱글톤으로 해놓았습니다.
    {
        Instance = this;
    }

    
    public void AdjustBundleZonesToMatchPopulation()
    {//이 함수를 호출하면 현재 등록된 드롭존의 시민들과 시민 자원 수를 비교해서 수를 서로 맞춥니다.
        int desiredPopulation = ResourceManager.Instance.Population; //맞춰야할 시민 수, 시민 자원 수
        int currentPopulation = 0;

        DropZone bundleZone = null;
        List<DropZone> regularZones = new List<DropZone>();

        foreach (var pair in dropZones) //모든 드롭존에서 현재 시민개체 수를 가져옵니다
        {
            var zone = pair;
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
        List<DropZone> zonesWithCitizens = dropZones
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

    public void RegisterDropZone(DropZone zone) // 각 드롭존(지역)을 이 클래스에서 한번에 관리하기 위해 딕셔너리에 추가합니다.
    {
        dropZones.Add(zone);
    }
}