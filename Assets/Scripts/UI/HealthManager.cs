using TMPro;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    [Header("Health")]
    [SerializeField] int startingHealth = 20;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI healthText;

    public int Health { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        Health = startingHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
            return;

        Health -= amount;
        UpdateUI();

        if (Health <= 0)
            GameManager.Instance?.GameOver();
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
            return;

        Health += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = $"Health: {Health}";
    }
}