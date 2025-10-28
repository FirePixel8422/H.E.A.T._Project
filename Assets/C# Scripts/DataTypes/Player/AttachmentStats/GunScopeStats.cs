using UnityEngine;
using Unity.Mathematics;


[System.Serializable]
public class GunScopeStats : IGunAtachment
{
    public int AttachmentId { get; set; }
    public AttachmentType Type { get; set; }


    [SerializeField] private SmartAttributeFloat damage = new(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat headShotMultiplier = new(1.5f, ApplyMode.Skip);

    [SerializeField] private FilterableContainer<NativeSampledAnimationCurve> damageFallOffCurve = new(NativeSampledAnimationCurve.Default, true);
    [SerializeField] private SmartAttributeFloat maxEffectiveRange = new(1, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat zoomMultiplier = new SmartAttributeFloat(1, ApplyMode.Override);
    [SerializeField] private FilterableContainer<Vector3> gunOffset = new FilterableContainer<Vector3>(true);

    [SerializeField] private SmartAttributeFloat2 adsRecoilForce = new SmartAttributeFloat2(new float2(1, 1), ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat adsRecoilRecovery = new SmartAttributeFloat(1, ApplyMode.Skip);

    [SerializeField] private FilterableContainer<NativeSampledAnimationCurve> spreadCurve = new (NativeSampledAnimationCurve.Default, true);




    public void ApplyToBaseStats(ref CompleteGunStatsSet gunStatsSet)
    {
        damage.ApplyToStat(ref gunStatsSet.coreStats.damage);
        headShotMultiplier.ApplyToStat(ref gunStatsSet.coreStats.headShotMultiplier);

        damageFallOffCurve.ApplyToStat(ref gunStatsSet.coreStats.damageFallOffCurve);
        maxEffectiveRange.ApplyToStat(ref gunStatsSet.coreStats.maxEffectiveRange);

        zoomMultiplier.ApplyToStat(ref gunStatsSet.gunADSStats.zoomMultiplier);
        gunOffset.ApplyToStat(ref gunStatsSet.swayStats.gunOffset);
    }

    public void ApplyToGunObject(GunRefHolder gunRef)
    {
        return;
    }
}