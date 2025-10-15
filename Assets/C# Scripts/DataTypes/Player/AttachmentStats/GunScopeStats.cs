using UnityEngine;


[System.Serializable]
public class GunScopeStats : IGunAtachment
{
    public int AttachmentId { get; set; }
    public AttachmentType Type { get; set; }
    

    [SerializeField] private SmartAttributeFloat zoomMultiplier = new SmartAttributeFloat(1, ApplyMode.Override);
    [SerializeField] private SmartAttributeFloat scopeCamZoomMultiplier = new SmartAttributeFloat(1, ApplyMode.Override);
    [SerializeField] private FilterableContainer<Vector3> gunOffset = new FilterableContainer<Vector3>(true);


    public void ApplyToBaseStats(ref CompleteGunStatsSet gunStatsSet)
    {
        zoomMultiplier.ApplyToStat(ref gunStatsSet.gunADSStats.zoomMultiplier);
        scopeCamZoomMultiplier.ApplyToStat(ref gunStatsSet.gunADSStats.scopeCamZoomMultiplier);
        gunOffset.ApplyToStat(ref gunStatsSet.swayStats.gunOffset);
    }

    public void ApplyToGunObject(GunRefHolder gunRef)
    {
        return;
    }
}