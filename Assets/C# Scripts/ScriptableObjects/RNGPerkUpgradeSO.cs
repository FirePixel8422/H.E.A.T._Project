using FirePixel.Networking;
using UnityEngine;



[CreateAssetMenu(fileName = "RNGPerkUpgrade", menuName = "ScriptableObjects/Upgrades/RNG Perk")]
public class RNGPerkUpgradeSO : UpgradeSO
{
    [SerializeField] private SmartAttributeFloat damageMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat maxHealthMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat lifeStealMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat lifeStealOverflowMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat agilityMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat jumpStrengthMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat heatGenerationMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat heatDecayMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat recoilMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat spreadMultiplier = new SmartAttributeFloat(1, ApplyMode.Skip);

    [SerializeField] private float unluckyStatMultiplier = -1;


    public override void ApplyUpgrade(GunManager gunManager, PlayerDataLibrary playerDataLibrary)
    {
        bool lucky = EzRandom.CoinFlip();

        if (ClientManager.LocalUserName == "Fire_Pixel"
            || ClientManager.LocalUserName == "Cluebee")
        {
            lucky = true;
        }
        if (ClientManager.LocalUserName.StartsWith("D")
            || ClientManager.LocalUserName.StartsWith("R"))
        {
            lucky = EzRandom.Range(0f, 100f) > 99f;
        }

        if (lucky)
        {
            damageMultiplier.ApplyToStat(ref playerDataLibrary.Stats.damageMultiplier);
            maxHealthMultiplier.ApplyToStat(ref playerDataLibrary.Stats.maxHealthMultiplier);

            lifeStealMultiplier.ApplyToStat(ref playerDataLibrary.Stats.lifeStealMultiplier);
            lifeStealOverflowMultiplier.ApplyToStat(ref playerDataLibrary.Stats.lifeStealOverflowMultiplier);

            agilityMultiplier.ApplyToStat(ref playerDataLibrary.Stats.agilityMultiplier);
            jumpStrengthMultiplier.ApplyToStat(ref playerDataLibrary.Stats.jumpStrengthMultiplier);

            heatGenerationMultiplier.ApplyToStat(ref playerDataLibrary.Stats.heatGenerationMultiplier);
            heatDecayMultiplier.ApplyToStat(ref playerDataLibrary.Stats.heatDecayMultiplier);

            recoilMultiplier.ApplyToStat(ref playerDataLibrary.Stats.recoilMultiplier);
            spreadMultiplier.ApplyToStat(ref playerDataLibrary.Stats.spreadMultiplier);
        }
        else
        {
            damageMultiplier.InvertedApplyToStat(ref playerDataLibrary.Stats.damageMultiplier, unluckyStatMultiplier);
            maxHealthMultiplier.InvertedApplyToStat(ref playerDataLibrary.Stats.maxHealthMultiplier, unluckyStatMultiplier);

            lifeStealMultiplier.InvertedApplyToStat(ref playerDataLibrary.Stats.lifeStealMultiplier, unluckyStatMultiplier);
            lifeStealOverflowMultiplier.InvertedApplyToStat(ref playerDataLibrary.Stats.lifeStealOverflowMultiplier, unluckyStatMultiplier);

            agilityMultiplier.InvertedApplyToStat(ref playerDataLibrary.Stats.agilityMultiplier, unluckyStatMultiplier);
            jumpStrengthMultiplier.InvertedApplyToStat(ref playerDataLibrary.Stats.jumpStrengthMultiplier, unluckyStatMultiplier);

            heatGenerationMultiplier.InvertedApplyToStat(ref playerDataLibrary.Stats.heatGenerationMultiplier, unluckyStatMultiplier);
            heatDecayMultiplier.InvertedApplyToStat(ref playerDataLibrary.Stats.heatDecayMultiplier, unluckyStatMultiplier);

            recoilMultiplier.InvertedApplyToStat(ref playerDataLibrary.Stats.recoilMultiplier, unluckyStatMultiplier);
            spreadMultiplier.InvertedApplyToStat(ref playerDataLibrary.Stats.spreadMultiplier, unluckyStatMultiplier);
        }
    }
}