using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;



namespace FirePixel.Networking
{
    public class UpgradeManager : NetworkBehaviour
    {
        [SerializeField] private UpgradeSO[] upgrades;

        [SerializeField] private GameObject upgradeUIParent;
        [SerializeField] private UpgradeUISlot[] uiSlots;


        [SerializeField] private UpgradeSO[] upgradesLeft;
        [SerializeField] private int totalWeightLeft;

        [SerializeField] private List<UpgradeSO> takenUpgrades;
        public List<UpgradeSO> Upgrades => takenUpgrades;


        private void Awake()
        {
            // Set UpgradeSO ids and calculate totalWeight
            int upgradesLeftCount = upgrades.Length;

            for (int i = 0; i < upgradesLeftCount; i++)
            {
                upgrades[i].upgradeId = i;
                totalWeightLeft += (int)upgrades[i].rarity;
            }

            upgradesLeft = new UpgradeSO[upgradesLeftCount];
            Array.Copy(upgrades, upgradesLeft, upgradesLeftCount);


            //TEMP
            //TEMP
            //TEMP
            //TEMP
            Invoke(nameof(CreateUpgradeUI), 0.5f);
        }

        public void CreateUpgradeUI()
        {
            Cursor.lockState = CursorLockMode.None;
            upgradeUIParent.SetActive(true);

            UpgradeSO[] upgrades = GetRandomUpgrades(GlobalGameData.UpgradeCount);
            
            // Enable and setup up UI slots for found upgrades
            int upgradeCount = upgrades.Length;
            for (int i = 0; i < upgradeCount; i++)
            {
                uiSlots[i].SetActiveAndUpdateUI(upgrades[i].upgradeSprite, upgrades[i].upgradeName, upgrades[i].rarityColor);

                int tempIndex = i;
                uiSlots[i].ConfirmButton.onClick.RemoveAllListeners();
                uiSlots[i].ConfirmButton.onClick.AddListener(() => TakeUpgrade(upgrades[tempIndex].upgradeId));
            }

            //If there are too little upgrades, disable unused UI slots
            for (int i = 0; i < GlobalGameData.UpgradeCount - upgradeCount; i++)
            {
                uiSlots[i].SetActive(false);
            }
        }


        /// <summary>
        /// Get up to <paramref name="upgradeCount"/> random upgrades based on upgrades left in <see cref="upgradesLeft"/> based on their <see cref="UpgradeRarity"/>
        /// </summary>
        private UpgradeSO[] GetRandomUpgrades(int upgradeCount)
        {
            // Clamp in case of little upgrades left
            upgradeCount = Mathf.Min(upgradesLeft.Length, upgradeCount);

            UpgradeSO[] chosenUpgrades = new UpgradeSO[upgradeCount];


            for (int i = 0; i < upgradeCount; i++)
            {
                int rWeight = EzRandom.Range(0, totalWeightLeft);

                for (int i2 = 0; i2 < upgradesLeft.Length; i2++)
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

                        // Remove Upgrade from pool temporarely, also remove weight from totalWeightLeft
                        upgradesLeft[i2] = null;

                        totalWeightLeft -= rarity;

                        break;
                    }
                }
            }

            // Re Add Temporarely removed upgrades if they were stackable
            for (int i = 0; i < upgradeCount; i++)
            {
                UpgradeSO targetUpgrade = chosenUpgrades[i];

                upgradesLeft[targetUpgrade.upgradeId] = targetUpgrade;

                totalWeightLeft += (int)targetUpgrade.rarity;
            }

            return chosenUpgrades;
        }



        public void TakeUpgrade(int upgradeId)
        {
            // Disable Upgrade Screen
            Cursor.lockState = CursorLockMode.Locked;
            upgradeUIParent.SetActive(false);

            // If Upgrade was non stackable, remove it
            if (upgradesLeft[upgradeId].stackable == false)
            {
                totalWeightLeft -= (int)upgradesLeft[upgradeId].rarity; 

                upgradesLeft[upgradeId] = null;
            }

            takenUpgrades.Add(upgrades[upgradeId]);

            TakeUpgrade_ServerRPC(upgradeId);



            //TEMP
            //TEMP
            //TEMP
            //TEMP
            Invoke(nameof(CreateUpgradeUI), 0.5f);
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void TakeUpgrade_ServerRPC(int upgradeId)
        {
            TakeUpgrade_ClientRPC(upgradeId);
        }

        [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void TakeUpgrade_ClientRPC(int upgradeId)
        {

        }
    }
}