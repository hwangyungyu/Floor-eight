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

    public void TestPopu() // 디버그1 버튼 작동시 동작하는 테스트용 시민 숫자 증가 코드입니다.
    {
        Population += 1;
    }
}