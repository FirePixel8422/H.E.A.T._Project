using UnityEngine;


public class UpgradeSO : ScriptableObject
{
    [Header("How rare is upgrade")]
    public UpgradeRarity rarity = UpgradeRarity.Common;

    [Header("Can you purchase this upgrade multiple times")]
    public bool stackable;

    public Sprite upgradeSprite;
    public Color rarityColor;
    public string upgradeName;


    [HideInInspector] public int upgradeId;


    public virtual void ApplyUpgrade(GunManager gunManager, PlayerDataLibrary playerDataLibrary)
    {

    }
}