using UnityEngine;
using UnityEngine.InputSystem;

public class Tower : MonoBehaviour
{
    public TowerData data;
    public BuildNode buildNode;

    private float fireCooldown;
    private float resourceTimer;
    private int upgradeIndex = -1;

    public int CurrentUpgradeLevel => upgradeIndex + 1;

    public bool CanUpgrade()
    {
        return data != null &&
               data.upgradeLevels != null &&
               upgradeIndex + 1 < data.upgradeLevels.Length;
    }

    public int GetUpgradeCost()
    {
        if (!CanUpgrade())
            return 0;

        return data.upgradeLevels[upgradeIndex + 1].upgradeCost;
    }

    void Update()
    {
        if (data == null)
            return;

        if (data.effectType == TowerEffectType.Resource)
        {
            UpdateResourceTower();
            return;
        }

        fireCooldown -= Time.deltaTime;

        Enemy target = FindTarget();

        if (target == null)
            return;

        if (fireCooldown <= 0f)
        {
            Shoot(target);
            fireCooldown = 1f / GetFireRate();
        }
    }

    void UpdateResourceTower()
    {
        resourceTimer += Time.deltaTime;

        if (resourceTimer >= GetResourceInterval())
        {
            resourceTimer = 0f;

            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.Add(GetResourceGain());

                if (WaveAnalyticsManager.Instance != null)
                {
                    WaveAnalyticsManager.Instance?.RecordTowerGeneratedResources(GetResourceGain());
                }

                if (FloatingTextManager.Instance != null)
                {
                    FloatingTextManager.Instance.Spawn(
                        "+" + GetResourceGain(),
                        transform.position + Vector3.up,
                        FloatingText.FloatingTextType.Resource
                    );
                }
            }
        }
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

        TowerUpgradeVisuals visuals = GetComponent<TowerUpgradeVisuals>();

        if (visuals != null)
        {
            visuals.PlayUpgradeVisuals(
                CurrentUpgradeLevel,
                GetRange()
            );
        }

        if (FloatingTextManager.Instance != null)
        {
            FloatingTextManager.Instance.Spawn(
                "UPGRADE",
                transform.position + Vector3.up,
                FloatingText.FloatingTextType.Resource
            );
        }

        Debug.Log(data.displayName + " upgraded to level " + CurrentUpgradeLevel);
    }

    public void Sell()
    {
        if (data == null)
            return;

        if (ResourceManager.Instance != null)
            ResourceManager.Instance.Add(GetSellValue());

        if (buildNode != null)
            buildNode.SetOccupied(false);

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(AudioManager.Instance.sellTower);

        Destroy(gameObject);
    }

    Enemy FindTarget()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>();

        Enemy bestTarget = null;

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance > GetRange())
                continue;

            if (bestTarget == null)
            {
                bestTarget = enemy;
                continue;
            }

            switch (data.targetMode)
            {
                case TowerTargetMode.Closest:
                    if (distance < Vector3.Distance(transform.position, bestTarget.transform.position))
                        bestTarget = enemy;
                    break;

                case TowerTargetMode.Strongest:
                    if (enemy.CurrentHealth > bestTarget.CurrentHealth)
                        bestTarget = enemy;
                    break;

                case TowerTargetMode.First:
                    if (enemy.Progress > bestTarget.Progress)
                        bestTarget = enemy;
                    break;

                case TowerTargetMode.Last:
                    if (enemy.Progress < bestTarget.Progress)
                        bestTarget = enemy;
                    break;
            }
        }

        return bestTarget;
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

        if (projectile != null)
            projectile.Init(target, this);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayShoot();
    }

    public int GetDamage()
    {
        int value = data.damage;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].damageBonus;

        return value;
    }

    public float GetRange()
    {
        float value = data.range;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].rangeBonus;

        return value;
    }

    public float GetFireRate()
    {
        float value = data.fireRate;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].fireRateBonus;

        return Mathf.Max(0.1f, value);
    }

    public int GetSplashDamage()
    {
        int value = data.splashDamage;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].splashDamageBonus;

        return value;
    }

    public float GetSplashRadius()
    {
        float value = data.splashRadius;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].splashRadiusBonus;

        return value;
    }

    public int GetResourceGain()
    {
        int value = data.resourceGain;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].resourceGainBonus;

        return value;
    }

    public float GetResourceInterval()
    {
        float value = data.resourceInterval;

        for (int i = 0; i <= upgradeIndex; i++)
            value *= data.upgradeLevels[i].resourceIntervalMultiplier;

        return Mathf.Max(0.1f, value);
    }

    public float GetSlowMultiplier()
    {
        float value = data.slowMultiplier;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].slowMultiplierBonus;

        return Mathf.Clamp(value, 0.05f, 1f);
    }

    public float GetSlowDuration()
    {
        float value = data.slowDuration;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].slowDurationBonus;

        return value;
    }

    public int GetPoisonDamagePerTick()
    {
        int value = data.poisonDamagePerTick;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].poisonDamagePerTickBonus;

        return value;
    }

    public float GetPoisonDuration()
    {
        float value = data.poisonDuration;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].poisonDurationBonus;

        return value;
    }

    public float GetPoisonTickRate()
    {
        float value = data.poisonTickRate;

        for (int i = 0; i <= upgradeIndex; i++)
            value *= data.upgradeLevels[i].poisonTickRateMultiplier;

        return Mathf.Max(0.05f, value);
    }

    public float GetStunDuration()
    {
        float value = data.stunDuration;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].stunDurationBonus;

        return value;
    }

    public int GetBurnDamagePerTick()
    {
        int value = data.burnDamagePerTick;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].burnDamagePerTickBonus;

        return value;
    }

    public float GetBurnDuration()
    {
        float value = data.burnDuration;

        for (int i = 0; i <= upgradeIndex; i++)
            value += data.upgradeLevels[i].burnDurationBonus;

        return value;
    }

    public float GetBurnTickRate()
    {
        float value = data.burnTickRate;

        for (int i = 0; i <= upgradeIndex; i++)
            value *= data.upgradeLevels[i].burnTickRateMultiplier;

        return Mathf.Max(0.05f, value);
    }

    public int GetSellValue()
    {
        int value = data.sellValue;

        for (int i = 0; i <= upgradeIndex; i++)
            value += Mathf.RoundToInt(data.upgradeLevels[i].upgradeCost * 0.5f);

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