using UnityEngine;



[CreateAssetMenu(fileName = "AttachmentUpgrade", menuName = "ScriptableObjects/Upgrades/Attachment")]
public class UpgradeAttachmentSO : UpgradeSO
{
    [SerializeField] private GunAttachmentSO attachmentUpgrade;
    [SerializeField] private int gunId;

    public override void ApplyUpgrade(GunManager gunManager, UtilityHandler utilityHandler, PlayerStatsHandler statsHandler)
    {
        gunManager.AttachmentIdsList[gunId][(int)attachmentUpgrade.Stats.Type] = attachmentUpgrade.Stats.AttachmentId;
    }
}