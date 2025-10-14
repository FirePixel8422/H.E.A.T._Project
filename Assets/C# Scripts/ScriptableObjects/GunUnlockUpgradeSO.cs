using UnityEngine;



[CreateAssetMenu(fileName = "GunUnlock", menuName = "ScriptableObjects/Upgrades/GunUnlock")]
public class GunUnlockUpgradeSO : UpgradeSO
{
    [SerializeField] private int gunId;

    public override void ApplyUpgrade(GunManager gunManager, PlayerDataLibrary playerDataLibrary)
    {
        gunManager.UnlockGun(gunId);
    }
}