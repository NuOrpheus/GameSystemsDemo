using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    [SerializeField] TowerData towerData;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI costText;

    private Button button;
    private TowerPlacer towerPlacer;

    void Awake()
    {
        button = GetComponent<Button>();
        towerPlacer = FindAnyObjectByType<TowerPlacer>();
    }

    void Start()
    {
        RefreshUI();

        if (button != null)
            button.onClick.AddListener(SelectTower);
    }

    void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(SelectTower);
    }

    void RefreshUI()
    {
        if (towerData == null)
            return;

        if (nameText != null)
            nameText.text = towerData.displayName;

        if (costText != null)
            costText.text = $"${towerData.cost}";
    }

    void SelectTower()
    {
        if (towerData == null || towerPlacer == null)
            return;

        towerPlacer.SelectTower(towerData);

        AudioManager.Instance?.PlayButtonClick();
    }
}