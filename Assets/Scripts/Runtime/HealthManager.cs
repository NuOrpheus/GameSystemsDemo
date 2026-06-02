using UnityEngine;
using TMPro;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    public int health = 20;
    public TextMeshProUGUI healthText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        UpdateUI();

        if (health <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }


    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + health;
    }
}