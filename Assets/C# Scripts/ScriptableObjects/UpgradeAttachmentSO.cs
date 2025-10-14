using FirePixel.Networking;
using UnityEngine;



[CreateAssetMenu(fileName = "AttachmentUpgrade", menuName = "ScriptableObjects/Upgrades/Attachment")]
public class UpgradeAttachmentSO : UpgradeSO
{
    [SerializeField] private GunAttachmentSO attachmentUpgrade;
    [SerializeField] private int gunId;

    public override void ApplyUpgrade(GunManager gunManager, PlayerDataLibrary playerDataLibrary)
    {
        gunManager.UnlockAttachment_ServerRPC(ClientManager.LocalClientGameId, gunId, (int)attachmentUpgrade.Stats.Type, attachmentUpgrade.Stats.AttachmentId);
    }
}