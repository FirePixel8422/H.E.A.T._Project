using Unity.Mathematics;
using UnityEngine;


[System.Serializable]
public class GunMuzzleStats : IGunAtachment
{
    public int AttachmentId { get; set; }
    public AttachmentType Type { get; set; }


    [SerializeField] private SmartAttributeFloat damage = new(1, ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat headShotMultiplier = new(1.5f, ApplyMode.Skip);

    [SerializeField] private FilterableContainer<NativeSampledAnimationCurve> damageFallOffCurve = new(NativeSampledAnimationCurve.Default, true);
    [SerializeField] private SmartAttributeFloat maxEffectiveRange = new(1, ApplyMode.Skip);

    [SerializeField] private FilterableContainer<AudioClip> shootAudioClip = new(true);
    [SerializeField] private SmartAttributeMinMaxFloat minMaxPitch = new(new MinMaxFloat(1, 1), ApplyMode.Override);
    [SerializeField] private SmartAttributeMinMaxFloat minMaxPitchAtMaxHeat = new(new MinMaxFloat(1, 1), ApplyMode.Override);

    [SerializeField] private FilterableContainer<TransformOffset> muzzleFlashTransformOffset = new(TransformOffset.Default, true);

    [SerializeField] private SmartAttributeFloat2 adsRecoilForce = new (new float2(1, 1), ApplyMode.Skip);
    [SerializeField] private SmartAttributeFloat adsRecoilRecovery = new (1, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat spread = new (0.05f, ApplyMode.Skip);

    [SerializeField] private SmartAttributeFloat volumeMultiplier = new (1, ApplyMode.Skip);



    public void ApplyToBaseStats(ref CompleteGunStatsSet gunStatsSet)
    {
        damage.ApplyToStat(ref gunStatsSet.coreStats.damage);
        headShotMultiplier.ApplyToStat(ref gunStatsSet.coreStats.headShotMultiplier);

        damageFallOffCurve.ApplyToStat(ref gunStatsSet.coreStats.damageFallOffCurve);
        maxEffectiveRange.ApplyToStat(ref gunStatsSet.coreStats.maxEffectiveRange);

        shootAudioClip.ApplyToStat(ref gunStatsSet.audioStats.shootAudioClip);
        minMaxPitch.ApplyToStat(ref gunStatsSet.audioStats.minMaxPitch);
        minMaxPitchAtMaxHeat.ApplyToStat(ref gunStatsSet.audioStats.minMaxPitchAtMaxHeat);

        adsRecoilForce.ApplyToStat(ref gunStatsSet.coreStats.adsRecoilForce);
        adsRecoilRecovery.ApplyToStat(ref gunStatsSet.coreStats.adsRecoilRecovery);

        spread.ApplyToStat(ref gunStatsSet.coreStats.spreadCurve.ValueMultiplier);

        volumeMultiplier.ApplyToStat(ref gunStatsSet.audioStats.volumeMultiplier);
    }

    public void ApplyToGunObject(GunRefHolder gunRef)
    {
        muzzleFlashTransformOffset.ApplyToStat(ref gunRef.MuzzleTransformOffset);
    }
}