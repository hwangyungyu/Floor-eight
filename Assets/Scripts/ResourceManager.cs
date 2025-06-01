using System;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    private int food;
    private int utilityItem;
    private int medicine;
    private int defense;
    private int mental;
    private int madness;
    private int population;

    public event Action OnResourceChanged;

    private void Awake() //싱글톤 선언
    {
        Instance = this;
    }

    // 자원들이 우선 0아래의 값을 가지지 않도록만 해놓은 프로퍼티입니다.
    public int Food
    {
        get { return food; }
        set
        {
            int temp = food;
            food = Mathf.Max(0, value);
            if(temp != food)
            {
                OnResourceChanged?.Invoke();
            }
        }
    }

    public int UtilityItem
    {
        get { return utilityItem; }
        set
        {
            int temp = utilityItem;
            utilityItem = Mathf.Max(0, value);
            if (temp != utilityItem)
            {
                OnResourceChanged?.Invoke();
            }
        }
    }

    public int Medicine
    {
        get { return medicine; }
        set
        {
            int temp = medicine;
            medicine = Mathf.Max(0, value);
            if (temp != medicine)
            {
                OnResourceChanged?.Invoke();
            }
        }
    }

    public int Defense
    {
        get { return defense; }
        set
        {
            int temp = defense;
            defense = Mathf.Max(0, value);
            if (temp != defense)
            {
                OnResourceChanged?.Invoke();
            }
        }
    }

    public int Mental
    {
        get { return mental; }
        set
        {
            int temp = mental;
            mental = Mathf.Max(0, value);
            if (temp != mental)
            {
                OnResourceChanged?.Invoke();
            }
        }
    }

    public int Madness
    {
        get { return madness; }
        set
        {
            int temp = madness;
            madness = Mathf.Max(0, value);
            if (temp != madness)
            {
                OnResourceChanged?.Invoke();
            }
        }
    }

    public int Population
    {
        get { return population; }
        set
        {
            int temp = population;
            population = Mathf.Max(0, value);
            if (temp != population)
            {
                OnResourceChanged?.Invoke();
            }
        }
    }

    // 실험용 
    public Button myButton;

    public void TestResource() // 디버그1 버튼 작동시 동작하는 테스트용 자원 증가 코드입니다.
    {
        Population += 1;
        Food += 1;
        UtilityItem += 1;
        Medicine += 1;
        Mental += 1;
        Defense += 1;

        // 귀찮아서 바로 반영하도록 test해놓음
        //myButton.onClick.Invoke();
        DropZoneManager.Instance.TestReset();
    }
    public int GetResourceIndex(string resourceName) //Area 관련 인덱스 용
    {
        if (string.IsNullOrEmpty(resourceName))
        {
            Debug.LogWarning("자원 증감 요소가 비어있습니다.");
            return -1;
        }


        switch (resourceName.ToLower())
        {
            case "food": return 0;
            case "utilityitem": return 1;
            case "medicine": return 2;
            case "defense": return 3;
            case "mental": return 4;
            case "madness": return 5;
            case "population": return 6;
            default:
                Debug.LogWarning($"알 수 없는 자원 이름: {resourceName}");
                return -1;
        }
    }
}