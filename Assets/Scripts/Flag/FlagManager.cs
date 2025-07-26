using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance { get; private set; }

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 플래그 값을 설정합니다.
    public void SetFlag(string flagName, bool value)
    {
        flags[flagName] = value;
        Debug.Log($"플래그 설정됨: {flagName} = {value}");
    }

    // 플래그 값을 반환합니다. 설정되지 않은 경우 false 반환.
    public bool GetFlag(string flagName)
    {
        if (flags.TryGetValue(flagName, out bool value))
        {
            return value;
        }
        return false;
    }
    public Dictionary<string, bool> GetAllFlags() //모든 플래그 반환
    {
        return new Dictionary<string, bool>(flags);
    }

    // 모든 플래그를 초기화합니다.
    public void ClearAllFlags()
    {
        flags.Clear();
    }
}