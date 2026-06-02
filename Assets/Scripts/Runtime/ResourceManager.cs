using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public int resources = 100;
    public TextMeshProUGUI resourcesText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public bool CanAfford(int amount)
    {
        return resources >= amount;
    }

    public void Spend(int amount)
    {
        resources -= amount;
        UpdateUI();
    }

    public void Add(int amount)
    {
        resources += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (resourcesText != null)
        {
            resourcesText.text = "Resources: " + resources;
        }
    }
}