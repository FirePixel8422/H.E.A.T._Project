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

        [SerializeField] private GameObject upgradeUIParent;
        [SerializeField] private UpgradeUISlot[] uiSlots;

        [Header("AR, Glock, UMP, AWS")]
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
            Cursor.lockState = CursorLockMode.None;
            upgradeUIParent.SetActive(true);

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
                upgradesLeft[upgradePoolLength + gunAttachmentUpgradeId] = gunAttachmentUpgrades[gunId][gunAttachmentUpgradeId];
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                //TEMP
                //TEMP
                //TEMP
                //TEMP
                CreateUpgradeUI();
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

                if (targetUpgrade.stackable == false)
                {
                    upgradesLeft[targetUpgrade.UpgradeId] = targetUpgrade;
                }
            }

            UpdateWeight();

            return chosenUpgrades;
        }


        private void UpdateWeight()
        {
            // Set UpgradeSO ids and calculate totalWeight
            int upgradesCount = globalUpgradesList.Length;

            for (int i = 0; i < upgradesCount; i++)
            {
                if (upgradesLeft[i] == null) continue;

                totalWeightLeft += (int)upgradesLeft[i].rarity;
            }
        }



        public void TakeUpgrade(int upgradeId)
        {
            // Disable Upgrade Screen
            Cursor.lockState = CursorLockMode.Locked;
            upgradeUIParent.SetActive(false);

            // If Upgrade was non stackable, remove it
            if (globalUpgradesList[upgradeId].stackable == false)
            {
                totalWeightLeft -= (int)globalUpgradesList[upgradeId].rarity; 

                //upgradesLeft[upgradeId] = null;
            }
            globalUpgradesList[upgradeId].ApplyUpgrade(GunManager.Instance, PlayerDataLibrary.LocalInstance);

            TakeUpgrade_ServerRPC(upgradeId);


            PlayerDataLibrary.LocalInstance.GunHandler.UpdateGunData();
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