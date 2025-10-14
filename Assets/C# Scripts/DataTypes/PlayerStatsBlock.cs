using UnityEngine;


[System.Serializable]
public class PlayerStatsBlock
{
    [SerializeField] private float maxHealth = 250;
    public float maxHealthMultiplier = 1;
    public float MaxHealth => maxHealth * maxHealthMultiplier;

    public float damageMultiplier = 1;

    public float lifeStealMultiplier = 0;
    public float lifeStealOverflowMultiplier = 0;

    public float agilityMultiplier = 1;
    public float jumpStrengthMultiplier = 1;

    public float heatGenerationMultiplier = 1;
    public float heatDecayMultiplier = 1;

    public float recoilMultiplier = 1;
    public float spreadMultiplier = 1;
}