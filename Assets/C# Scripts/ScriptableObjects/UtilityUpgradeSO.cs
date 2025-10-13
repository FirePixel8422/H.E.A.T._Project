using UnityEngine;



[CreateAssetMenu(fileName = "UtilityUpgrade", menuName = "ScriptableObjects/Upgrades/Utility")]
public class UtilityUpgradeSO : UpgradeSO
{
    [SerializeField] private UtilitySO utility;
    [SerializeField] private int utilityId;

    public override void ApplyUpgrade(GunManager gunManager, UtilityHandler utilityHandler, PlayerStatsHandler statsHandler)
    {
        utilityHandler.utility[utilityId] = utility.Stats;
    }
}