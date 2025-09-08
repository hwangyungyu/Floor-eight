using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SpecialResource
{
    public string name;
    public int amount;
}

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
    
    public List<SpecialResource> specialResources = new List<SpecialResource>();

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

    public void InitializeResources()
    {
        Food = 0;
        UtilityItem = 0;
        Medicine = 0;
        Defense = 0;
        Mental = 0;
        Madness = 0;
        Population = 0;
        specialResources.Clear();
        GameManager.Instance.UIUpdate();
    }

    public void AllResource(int amount) // 모든 자원 증가
    {
        Population += amount;
        Food += amount;
        UtilityItem += amount;
        Medicine += amount;
        Mental += amount;
        Defense += amount;

        GameManager.Instance.UIUpdate();
    }
    
    public SpecialResource GetSpecialResource(string resourceName)
    {
        return specialResources.FirstOrDefault(r => r.name.Equals(resourceName, StringComparison.OrdinalIgnoreCase));
    }

    public void AddSpecialResource(string resourceName, int value)
    {
        resourceName = resourceName.Replace("_", " ");
        SpecialResource resource = GetSpecialResource(resourceName);
        if (resource != null)
        {
            resource.amount = Mathf.Max(0, resource.amount + value);
        }
        else
        {
            if (value > 0)
            {
                
                specialResources.Add(new SpecialResource { name = resourceName, amount = value });
            }
        }
        OnResourceChanged?.Invoke();
    }

    public void AddTestResource(string name)
    {
        AddSpecialResource(name, 1);
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
    public int GetResourceByName(string resourceName) //자원 이름으로 자원 불러오기
    {
        return resourceName.ToLower() switch
        {
            "food" => Food,
            "utilityitem" => UtilityItem,
            "medicine" => Medicine,
            "defense" => Defense,
            "mental" => Mental,
            "madness" => Madness,
            "population" => Population,
            _ => GetSpecialResource(resourceName.Replace("_", " "))?.amount ?? 0
        };
    }
}