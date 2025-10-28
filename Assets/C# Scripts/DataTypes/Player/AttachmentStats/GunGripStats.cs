using Unity.Mathematics;
using UnityEngine;


[System.Serializable]
public class GunGripStats : IGunAtachment
{
    public int AttachmentId { get; set; }
    public AttachmentType Type { get; set; }


    [SerializeField] private SmartAttributeFloat damage = new(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat headShotMultiplier = new(1.5f, ApplyMode.Skip);

    [SerializeField] private FilterableContainer<NativeSampledAnimationCurve> damageFallOffCurve = new(NativeSampledAnimationCurve.Default, true);
    [SerializeField] private SmartAttributeFloat maxEffectiveRange = new(1, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat2 adsRecoilForce = new SmartAttributeFloat2(new float2(1, 1), ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat adsRecoilRecovery = new SmartAttributeFloat(1, ApplyMode.Skip);

    [SerializeField] private FilterableContainer<NativeSampledAnimationCurve> spreadCurve = new(NativeSampledAnimationCurve.Default, true);


    public void ApplyToBaseStats(ref CompleteGunStatsSet gunStatsSet)
    {
        damage.ApplyToStat(ref gunStatsSet.coreStats.damage);
        headShotMultiplier.ApplyToStat(ref gunStatsSet.coreStats.headShotMultiplier);

        damageFallOffCurve.ApplyToStat(ref gunStatsSet.coreStats.damageFallOffCurve);
        maxEffectiveRange.ApplyToStat(ref gunStatsSet.coreStats.maxEffectiveRange);
    }

    public void ApplyToGunObject(GunRefHolder gunRef)
    {
        return;
    }
}