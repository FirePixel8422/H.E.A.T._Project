using System;
using Unity.Netcode;
using UnityEngine;


namespace FirePixel.Networking
{
    public class UpgradeManager : NetworkBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        [SerializeField] private UpgradeSO[] globalUpgradesList;
        [SerializeField] private UpgradeSO[] upgrades;

        [SerializeField] private DeathBehavior deathBehavior;
        [SerializeField] private UpgradeUISlot[] uiSlots;

        [Header("Glock, AR, UMP, AWS")]
        [SerializeField] private ArrayWrapper<UpgradeSO>[] gunAttachmentUpgrades;

        [SerializeField] private UpgradeSO[] upgradesLeft;
        [SerializeField] private int totalWeightLeft;


        private void Awake()
        {
            Instance = this;


            int globalUpgradesCount = globalUpgradesList.Length;

            for (int i = 0; i < globalUpgradesCount; i++)
            {
                globalUpgradesList[i].UpgradeId = i;
            }

            // Set UpgradeSO ids and calculate totalWeight
            int upgradesLeftCount = upgrades.Length;

            upgradesLeft = new UpgradeSO[globalUpgradesCount];

            for (int i = 0; i < upgradesLeftCount; i++)
            {
                int targetUpgradeId = upgrades[i].UpgradeId;
                upgradesLeft[targetUpgradeId] = upgrades[i];
            }
        }

        public void CreateUpgradeUI()
        {
            deathBehavior.StartUpgradeMenus();

            UpgradeSO[] upgrades = GetRandomUpgrades(GlobalGameData.UpgradeCount);

            // Enable and setup up UI slots for found upgrades
            int upgradeCount = upgrades.Length;
            for (int i = 0; i < upgradeCount; i++)
            {
                uiSlots[i].SetActiveAndUpdateUI(upgrades[i].upgradeSprite, upgrades[i].upgradeName, upgrades[i].upgradeDescription, upgrades[i].rarityColor);

                int tempIndex = i;
                uiSlots[i].ConfirmButton.onClick.RemoveAllListeners();
                uiSlots[i].ConfirmButton.onClick.AddListener(() => TakeUpgrade(upgrades[tempIndex].UpgradeId));
            }

            //If there are too little upgrades, disable unused UI slots
            for (int i = 0; i < GlobalGameData.UpgradeCount - upgradeCount; i++)
            {
                uiSlots[i].SetActive(false);
            }
        }


        public void OnGainGun(int gunId)
        {
            int upgradePoolLength = upgradesLeft.Length;
            int targetGunUpgradeCount = gunAttachmentUpgrades[gunId].Length;

            Array.Resize(ref upgradesLeft, upgradePoolLength + targetGunUpgradeCount);

            for (int gunAttachmentUpgradeId = 0; gunAttachmentUpgradeId < targetGunUpgradeCount; gunAttachmentUpgradeId++)
            {
                upgradesLeft[gunAttachmentUpgrades[gunId][gunAttachmentUpgradeId].UpgradeId] = gunAttachmentUpgrades[gunId][gunAttachmentUpgradeId];
            }
        }

        /// <summary>
        /// Get up to <paramref name="upgradeCount"/> random upgrades based on upgrades left in <see cref="upgradesLeft"/> based on their <see cref="UpgradeRarity"/>
        /// </summary>
        private UpgradeSO[] GetRandomUpgrades(int upgradeCount)
        {
            UpdateWeight();

            // Clamp in case of little upgrades left
            upgradeCount = Mathf.Min(upgradesLeft.Length, upgradeCount);

            UpgradeSO[] chosenUpgrades = new UpgradeSO[upgradeCount];

            for (int i = 0; i < upgradeCount; i++)
            {
                int rWeight = EzRandom.Range(0, totalWeightLeft);

                for (int i2 = 0; i2 < globalUpgradesList.Length; i2++)
                {
                    if (upgradesLeft[i2] == null) continue;

                    int rarity = (int)upgradesLeft[i2].rarity;
                    // If rolled random number is still more then current to check upgrade, skip it
                    if (rWeight > rarity)
                    {
                        rWeight -= rarity;
                        continue;
                    }
                    else
                    {
                        // Select Upgrade
                        chosenUpgrades[i] = upgradesLeft[i2];

                        // Remove Upgrade from main pool temporarely, also remove weight from totalWeightLeft
                        upgradesLeft[i2] = null;

                        totalWeightLeft -= rarity;

                        break;
                    }
                }
            }

            // Only re add upgarde if its non stackable
            for (int i = 0; i < upgradeCount; i++)
            {
                UpgradeSO targetUpgrade = chosenUpgrades[i];

                upgradesLeft[targetUpgrade.UpgradeId] = targetUpgrade;
            }

            return chosenUpgrades;
        }

        private void UpdateWeight()
        {
            // Set UpgradeSO ids and calculate totalWeight
            int upgradesCount = globalUpgradesList.Length;

            totalWeightLeft = 0;

            for (int i = 0; i < upgradesCount; i++)
            {
                if (upgradesLeft[i] == null) continue;

                totalWeightLeft += (int)upgradesLeft[i].rarity;
            }
        }

        public void TakeUpgrade(int upgradeId)
        {
            // Disable Upgrade Screen
            deathBehavior.EndUpgradeMenus();;

            // If Upgrade was non stackable, remove it
            if (globalUpgradesList[upgradeId].stackable == false)
            {
                upgradesLeft[upgradeId] = null;
            }
            globalUpgradesList[upgradeId].ApplyUpgrade(GunManager.Instance, PlayerDataLibrary.LocalInstance);












            ///DEBUG
            ///DEBUG
            ///DEBUG
            ///DEBUG
            ///DEBUG
            ///DEBUG
            ///DEBUG
            ///DEBUG
            ///DEBUG
            ///DEBUG





            GunManager.Instance.UpdateAttachentDEBUG();

            //PlayerDataLibrary.LocalInstance.GunHandler.UpdateGunData();

            MatchManager.Instance.OnEndUpgradePhase_ServerRPC();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                PlayerDataLibrary.LocalInstance.HealthHandler.DealDamage(float.MaxValue, true, default, default, out _);
            }
        }
#endif
    }
}