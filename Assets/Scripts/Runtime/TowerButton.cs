using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    public TowerData towerData;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;

    private TowerPlacer towerPlacer;

    void Start()
    {
        towerPlacer = FindAnyObjectByType<TowerPlacer>();

        nameText.text = towerData.displayName;
        costText.text = "$" + towerData.cost;

        GetComponent<Button>().onClick.AddListener(SelectTower);
    }

    void SelectTower()
    {
        towerPlacer.SelectTower(towerData);

    }
}
