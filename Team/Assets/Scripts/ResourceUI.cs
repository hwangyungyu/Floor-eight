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
    [SerializeField] private Text amountPopulation;

    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        if (resourceManager != null)
        {
            resourceManager.OnResourceChanged += RefreshAmount;
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

    public void RefreshAmount()
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
