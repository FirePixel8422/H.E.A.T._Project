using UnityEngine;



[CreateAssetMenu(fileName = "PerkUpgrade", menuName = "ScriptableObjects/Upgrades/Perk")]
public class PerkUpgradeSO : UpgradeSO
{
    [SerializeField] private SmartAttributeFloat damageMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat maxHealth = new SmartAttributeFloat(0, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat lifeStealMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat lifeStealOverflowMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat agilityMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat jumpStrengthMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat heatGenerationMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat heatDecayMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat recoilMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat spreadMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);



    public override void ApplyUpgrade(GunManager gunManager, PlayerDataLibrary playerDataLibrary)
    {
        damageMultiplier.ApplyToStat(ref playerDataLibrary.Stats.damageMultiplier);
        maxHealth.ApplyToStat(ref playerDataLibrary.Stats.maxHealth);

        lifeStealMultiplier.ApplyToStat(ref playerDataLibrary.Stats.lifeStealMultiplier);
        lifeStealOverflowMultiplier.ApplyToStat(ref playerDataLibrary.Stats.lifeStealOverflowMultiplier);

        agilityMultiplier.ApplyToStat(ref playerDataLibrary.Stats.agilityMultiplier);
        jumpStrengthMultiplier.ApplyToStat(ref playerDataLibrary.Stats.jumpStrengthMultiplier);

        heatGenerationMultiplier.ApplyToStat(ref playerDataLibrary.Stats.heatGenerationMultiplier);
        heatDecayMultiplier.ApplyToStat(ref playerDataLibrary.Stats.heatDecayMultiplier);

        recoilMultiplier.ApplyToStat(ref playerDataLibrary.Stats.recoilMultiplier);
        spreadMultiplier.ApplyToStat(ref playerDataLibrary.Stats.spreadMultiplier);
    }
}