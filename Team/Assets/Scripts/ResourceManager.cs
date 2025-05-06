using System;
using UnityEngine;

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

    private void Awake()
    {
        Instance = this;
    }

    public int Food
    {
        get { return food; }
        set
        {
            food = Mathf.Max(0, value);
            OnResourceChanged?.Invoke();
        }
    }

    public int UtilityItem
    {
        get { return utilityItem; }
        set
        {
            utilityItem = Mathf.Max(0, value);
            OnResourceChanged?.Invoke();
        }
    }

    public int Medicine
    {
        get { return medicine; }
        set
        {
            medicine = Mathf.Max(0, value);
            OnResourceChanged?.Invoke();
        }
    }

    public int Defense
    {
        get { return defense; }
        set
        {
            defense = Mathf.Max(0, value);
            OnResourceChanged?.Invoke();
        }
    }

    public int Mental
    {
        get { return mental; }
        set
        {
            mental = Mathf.Max(0, value);
            OnResourceChanged?.Invoke();
        }
    }

    public int Madness
    {
        get { return madness; }
        set
        {
            madness = Mathf.Max(0, value);
            OnResourceChanged?.Invoke();
        }
    }

    public int Population
    {
        get { return population; }
        set
        {
            population = Mathf.Max(0, value);
            OnResourceChanged?.Invoke();
        }
    }

    public void TestPopu()
    {
        Population += 1;
    }

    public void Initialize(int food, int utilityItem, int medicine, int defense, int mental, int madness, int population)
    {
        Food = food;
        UtilityItem = utilityItem;
        Medicine = medicine;
        Defense = defense;
        Mental = mental;
        Madness = madness;
        Population = population;
    }
}