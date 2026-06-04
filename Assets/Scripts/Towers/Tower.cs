using UnityEngine;
using UnityEngine.InputSystem;

public class Tower : MonoBehaviour
{
    public TowerData data;
    public BuildNode buildNode;

    private float fireCooldown;
    private float resourceTimer;
    private int upgradeIndex = -1;

    private WaveSpawner waveSpawner;
    private TowerUpgradeVisuals upgradeVisuals;

    public int CurrentUpgradeLevel => upgradeIndex + 1;

    void Awake()
    {
        waveSpawner = FindAnyObjectByType<WaveSpawner>();
        upgradeVisuals = GetComponent<TowerUpgradeVisuals>();
    }

    void Update()
    {
        if (data == null)
            return;

        if (data.effectType == TowerEffectType.Resource)
            UpdateResourceTower();
        else
            UpdateCombatTower();
    }

    void UpdateCombatTower()
    {
        fireCooldown -= Time.deltaTime;

        Enemy target = FindTarget();

        if (target == null || fireCooldown > 0f)
            return;

        Shoot(target);
        fireCooldown = 1f / GetFireRate();
    }

    void UpdateResourceTower()
    {
        if (waveSpawner != null && !waveSpawner.IsWaveRunning)
            return;

        resourceTimer += Time.deltaTime;

        if (resourceTimer < GetResourceInterval())
            return;

        resourceTimer = 0f;

        int gain = GetResourceGain();

        ResourceManager.Instance?.Add(gain);
        WaveAnalyticsManager.Instance?.RecordTowerGeneratedResources(gain);

        FloatingTextManager.Instance?.Spawn(
            "+" + gain,
            transform.position + Vector3.up,
            FloatingText.FloatingTextType.Resource
        );
    }

    public bool CanUpgrade()
    {
        return data != null &&
               data.upgradeLevels != null &&
               upgradeIndex + 1 < data.upgradeLevels.Length;
    }

    public int GetUpgradeCost()
    {
        return CanUpgrade()
            ? data.upgradeLevels[upgradeIndex + 1].upgradeCost
            : 0;
    }

    public void TryUpgrade()
    {
        if (!CanUpgrade())
        {
            Debug.Log("Tower is already max level.");
            return;
        }

        int cost = GetUpgradeCost();

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("No ResourceManager in scene.");
            return;
        }

        if (!ResourceManager.Instance.CanAfford(cost))
        {
            Debug.Log("Not enough resources to upgrade.");
            return;
        }

        ResourceManager.Instance.Spend(cost);
        WaveAnalyticsManager.Instance?.RecordResourcesSpent(cost);

        upgradeIndex++;

        upgradeVisuals?.PlayUpgradeVisuals(
            CurrentUpgradeLevel,
            GetRange()
        );

        FloatingTextManager.Instance?.Spawn(
            "UPGRADE",
            transform.position + Vector3.up,
            FloatingText.FloatingTextType.Resource
        );

        Debug.Log(data.displayName + " upgraded to level " + CurrentUpgradeLevel);
    }

    public void Sell()
    {
        if (data == null)
            return;

        ResourceManager.Instance?.Add(GetSellValue());
        buildNode?.SetOccupied(false);
        AudioManager.Instance?.PlaySellTower();

        Destroy(gameObject);
    }

    Enemy FindTarget()
    {
        var enemies = Enemy.ActiveEnemies;
        Enemy bestTarget = null;

        float towerRange = GetRange();

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance > towerRange)
                continue;

            if (bestTarget == null || IsBetterTarget(enemy, bestTarget, distance))
                bestTarget = enemy;
        }

        return bestTarget;
    }

    bool IsBetterTarget(Enemy enemy, Enemy currentBest, float enemyDistance)
    {
        switch (data.targetMode)
        {
            case TowerTargetMode.Closest:
                float bestDistance = Vector3.Distance(
                    transform.position,
                    currentBest.transform.position
                );
                return enemyDistance < bestDistance;

            case TowerTargetMode.Strongest:
                return enemy.CurrentHealth > currentBest.CurrentHealth;

            case TowerTargetMode.First:
                return enemy.Progress > currentBest.Progress;

            case TowerTargetMode.Last:
                return enemy.Progress < currentBest.Progress;

            default:
                return false;
        }
    }

    void Shoot(Enemy target)
    {
        if (data.projectilePrefab == null)
            return;

        GameObject projectileObj = Instantiate(
            data.projectilePrefab,
            transform.position + Vector3.up,
            Quaternion.identity
        );

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile?.Init(target, this);

        AudioManager.Instance?.PlayShoot();
    }

    TowerUpgradeLevel Upgrade(int index)
    {
        return data.upgradeLevels[index];
    }

    int UpgradeCount()
    {
        return Mathf.Clamp(upgradeIndex + 1, 0, data.upgradeLevels.Length);
    }

    public int GetDamage()
    {
        int value = data.damage;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).damageBonus;

        return value;
    }

    public float GetRange()
    {
        float value = data.range;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).rangeBonus;

        return value;
    }

    public float GetFireRate()
    {
        float value = data.fireRate;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).fireRateBonus;

        return Mathf.Max(0.1f, value);
    }

    public int GetSplashDamage()
    {
        int value = data.splashDamage;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).splashDamageBonus;

        return value;
    }

    public float GetSplashRadius()
    {
        float value = data.splashRadius;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).splashRadiusBonus;

        return value;
    }

    public int GetResourceGain()
    {
        int value = data.resourceGain;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).resourceGainBonus;

        return value;
    }

    public float GetResourceInterval()
    {
        float value = data.resourceInterval;

        for (int i = 0; i < UpgradeCount(); i++)
            value *= Upgrade(i).resourceIntervalMultiplier;

        return Mathf.Max(0.1f, value);
    }

    public float GetSlowMultiplier()
    {
        float value = data.slowMultiplier;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).slowMultiplierBonus;

        return Mathf.Clamp(value, 0.05f, 1f);
    }

    public float GetSlowDuration()
    {
        float value = data.slowDuration;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).slowDurationBonus;

        return value;
    }

    public int GetPoisonDamagePerTick()
    {
        int value = data.poisonDamagePerTick;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).poisonDamagePerTickBonus;

        return value;
    }

    public float GetPoisonDuration()
    {
        float value = data.poisonDuration;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).poisonDurationBonus;

        return value;
    }

    public float GetPoisonTickRate()
    {
        float value = data.poisonTickRate;

        for (int i = 0; i < UpgradeCount(); i++)
            value *= Upgrade(i).poisonTickRateMultiplier;

        return Mathf.Max(0.05f, value);
    }

    public float GetStunDuration()
    {
        float value = data.stunDuration;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).stunDurationBonus;

        return value;
    }

    public int GetBurnDamagePerTick()
    {
        int value = data.burnDamagePerTick;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).burnDamagePerTickBonus;

        return value;
    }

    public float GetBurnDuration()
    {
        float value = data.burnDuration;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Upgrade(i).burnDurationBonus;

        return value;
    }

    public float GetBurnTickRate()
    {
        float value = data.burnTickRate;

        for (int i = 0; i < UpgradeCount(); i++)
            value *= Upgrade(i).burnTickRateMultiplier;

        return Mathf.Max(0.05f, value);
    }

    public int GetSellValue()
    {
        int value = data.sellValue;

        for (int i = 0; i < UpgradeCount(); i++)
            value += Mathf.RoundToInt(Upgrade(i).upgradeCost * 0.5f);

        return value;
    }

    void OnMouseOver()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
            Sell();

        if (Mouse.current.middleButton.wasPressedThisFrame)
            TryUpgrade();
    }
}