using UnityEngine;
using UnityEngine.UI;


public class ResourceUI : MonoBehaviour
{
    [SerializeField] private Text amountFood;
    [SerializeField] private Text amountUtil;
    [SerializeField] private Text amountMedi;
    [SerializeField] private Text amountDef;
    [SerializeField] private Text amountMen;
    [SerializeField] private Text amountMad;
    [SerializeField] private Text amountPopulation;  //각 자원의 수량을 보여주는 텍스트들 입니다. 일부는 지정되지 않았습니다.

    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        if (resourceManager != null)
        {
            resourceManager.OnResourceChanged += RefreshAmount; //자원 숫자가 변경될때 실행되는 이벤트를 구독
        }
        else
        {
            Debug.LogWarning("ResourceManager.Instance is null!");
        }
    }

    private void OnDisable()
    {
        // ResourceManager의 자원 변경 이벤트 구독 해제
        resourceManager.OnResourceChanged -= RefreshAmount;
    }

    public void RefreshAmount() //자원 증감시 자동으로 호출되서 UI의 현재 자원 숫자를 업데이트
    {
        if (amountFood != null)
            amountFood.text = $"{resourceManager.Food}";

        if (amountUtil != null)
            amountUtil.text = $"{resourceManager.UtilityItem}";

        if (amountMedi != null)
            amountMedi.text = $"{resourceManager.Medicine}";

        if (amountDef != null)
            amountDef.text = $"{resourceManager.Defense}";

        if (amountMen != null)
            amountMen.text = $"{resourceManager.Mental}";

        if (amountMad != null)
            amountMad.text = $"{resourceManager.Madness}";

        if (amountPopulation != null)
            amountPopulation.text = $"{resourceManager.Population}";
    }
}
