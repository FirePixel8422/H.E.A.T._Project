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



    public override void ApplyUpgrade(GunManager gunManager, UtilityHandler utilityHandler, PlayerStatsHandler statsHandler)
    {
        damageMultiplier.ApplyToStat(ref statsHandler.Stats.damageMultiplier);
        maxHealth.ApplyToStat(ref statsHandler.Stats.maxHealth);

        lifeStealMultiplier.ApplyToStat(ref statsHandler.Stats.lifeStealMultiplier);
        lifeStealOverflowMultiplier.ApplyToStat(ref statsHandler.Stats.lifeStealOverflowMultiplier);

        agilityMultiplier.ApplyToStat(ref statsHandler.Stats.agilityMultiplier);
        jumpStrengthMultiplier.ApplyToStat(ref statsHandler.Stats.jumpStrengthMultiplier);

        heatGenerationMultiplier.ApplyToStat(ref statsHandler.Stats.heatGenerationMultiplier);
        heatDecayMultiplier.ApplyToStat(ref statsHandler.Stats.heatDecayMultiplier);

        recoilMultiplier.ApplyToStat(ref statsHandler.Stats.recoilMultiplier);
        spreadMultiplier.ApplyToStat(ref statsHandler.Stats.spreadMultiplier);
    }
}