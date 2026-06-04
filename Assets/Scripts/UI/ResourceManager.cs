using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Resources")]
    [SerializeField] int startingResources = 100;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI resourcesText;

    public int Resources { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Resources = startingResources;
        UpdateUI();
    }

    public bool CanAfford(int amount)
    {
        return Resources >= amount;
    }

    public bool Spend(int amount)
    {
        if (!CanAfford(amount))
            return false;

        Resources -= amount;
        UpdateUI();

        return true;
    }

    public void Add(int amount)
    {
        Resources += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (resourcesText != null)
            resourcesText.text = $"Resources: {Resources}";
    }
}