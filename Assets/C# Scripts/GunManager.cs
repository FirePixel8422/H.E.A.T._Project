using FirePixel.Networking;
using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class GunManager : SmartNetworkBehaviour
{
    public static GunManager Instance { get; private set; }


    [Header("Allow this script to be used outside of network environment")]
    [SerializeField] private bool overrideIsOwner;

    [SerializeField] private GunAttachmentSO[] globalAttachmentsList;

    [SerializeField] private GunSO[] baseGuns;
    [SerializeField] private bool[] unlockedGuns;
    public bool[] UnlockedGuns => unlockedGuns;

    [SerializeField] ArrayWrapper<int2>[] attachmentIdsList;
    [SerializeField] private CompleteGunStatsSet[] currentGunStats;

    [SerializeField] private ArrayWrapper<ArrayWrapper<GunAttachmentSO>>[] DEBUG_Attachments;

    public int GunCount => baseGuns.Length;

    private int currentGunId;


    public override void OnNetworkSystemsSetup()
    {
        Instance = this;
        if (overrideIsOwner)
        {
            SetupAttachments(0);
        }
        else
        {
            SetupAttachments(LocalClientGameId);
        }
    }


    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    public void UnlockAttachment_ServerRPC(int playerGameId, int gunId, int attachmentTypeId, int newAttachmentId)
    {
        UnlockAttachment_ClientRPC(playerGameId, gunId, attachmentTypeId, newAttachmentId);
    }
    [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void UnlockAttachment_ClientRPC(int playerGameId, int gunId, int attachmentTypeId, int newAttachmentId)
    {
        attachmentIdsList[gunId].Value[attachmentTypeId][playerGameId] = newAttachmentId;
    }


    private void SetupAttachments(int playerGameId)
    {
        int attachmentCount = globalAttachmentsList.Length;
        for (int i = 0; i < attachmentCount; i++)
        {
            globalAttachmentsList[i].Stats.AttachmentId = i;
        }

        int gunCount = baseGuns.Length;
        attachmentIdsList = new ArrayWrapper<int2>[gunCount];

        unlockedGuns = new bool[gunCount];
        UnlockGun(0);


        for (int i = 0; i < gunCount; i++)
        {
            attachmentIdsList[i].Value = new int2[5];

            Array.Fill(attachmentIdsList[i].Value, new int2(-1, -1));
        }

        currentGunStats = new CompleteGunStatsSet[gunCount];

        CalculateGunBaseStats(playerGameId);
    }

    public void UnlockGun(int gunId)
    {
        unlockedGuns[gunId] = true;

        UpgradeManager.Instance.OnGainGun(gunId);
    }

    public void CalculateGunBaseStats(int playerGameId)
    {
        int gunCount = baseGuns.Length;
        for (int gunId = 0; gunId < gunCount; gunId++)
        {
            GunSO targetGun = baseGuns[gunId];

            CompleteGunStatsSet targetStatsSet = targetGun.BaseStats;

            int attachmentCount = targetGun.BaseAttachmentsCount;
            for (int i = 0; i < attachmentCount; i++)
            {
                IGunAtachment targetAttachment = targetGun.BaseAttachments[i].Stats;

                targetAttachment.ApplyToBaseStats(ref targetStatsSet);

                attachmentIdsList[gunId].Value[(int)targetAttachment.Type][playerGameId] = targetAttachment.AttachmentId;
            }

            targetStatsSet.BakeAllCurves();

            currentGunStats[gunId] = targetStatsSet;
        }
    }
    public void SetupHeatSinks(out GunHeatSink[] heatSinks, Image heatBar, Animator anim)
    {
        heatSinks = new GunHeatSink[GunCount];
        for (int i = 0; i < GunCount; i++)
        {
            heatSinks[i] = new GunHeatSink();
            heatSinks[i].stats = currentGunStats[i].heatSinkStats;
            heatSinks[i].Init(heatBar, anim);
        }
    }

    /// <summary>
    /// Swap gun and get baseGunstats by gunId.
    /// </summary>
    public void SwapGun(
        Transform gunParentTransform, int gunId, int playerGameId, ref GunRefHolder gunRefHolder,
        out GunCoreStats coreStats,
        out GunAudioStats audioStats,
        out GunShakeStats shakeStats,
        out GunSwayStats swayStats,
        out GunADSStats gunADSStats)
    {
        currentGunId = gunId;

        if (gunRefHolder != null)
        {
            gunRefHolder.DestroyGun();
        }

        gunRefHolder = Instantiate(baseGuns[gunId].GunPrefab, gunParentTransform);

        for (int i = 0; i < 5; i++)
        {
            int attachmentId = attachmentIdsList[gunId][i][playerGameId];

            if (attachmentId == -1 || attachmentId >= globalAttachmentsList.Length) continue;

            gunRefHolder.SpawnAttachment(globalAttachmentsList[attachmentId]);

            globalAttachmentsList[attachmentId].Stats.ApplyToBaseStats(ref currentGunStats[gunId]);
        }

        // Initilialize gun After spawning attachments
        gunRefHolder.Init();

        currentGunStats[gunId].GetStatsCopy(out coreStats, out audioStats, out _, out shakeStats, out swayStats, out gunADSStats);
    }

    public int GetNextGunId() 
    {
        for (int i = 0; i < GunCount; i++)
        {
            int nextGunId = (currentGunId + 1 + i) % baseGuns.Length;

            if (unlockedGuns[nextGunId] == false) continue;

            currentGunId = nextGunId;
            return currentGunId;
        }

        DebugLogger.Log("Player Only has 1 gun");
        return currentGunId;
    }


    public string GetCurrentGunName() => baseGuns[currentGunId].name;


    public override void OnDestroy()
    {
        int gunCount = baseGuns.Length;
        for (int gunId = 0; gunId < gunCount; gunId++)
        {
            baseGuns[gunId].BaseStats.Dispose();
        }
        base.OnDestroy();
    }


    public void UpdateAttachentDEBUG()
    {
        int gunCount = GunCount;
        DEBUG_Attachments = new ArrayWrapper<ArrayWrapper<GunAttachmentSO>>[gunCount];

        for (int gunId = 0; gunId < gunCount; gunId++)
        {
            // Initialize level 1
            DEBUG_Attachments[gunId] = new ArrayWrapper<ArrayWrapper<GunAttachmentSO>>
            {
                Value = new ArrayWrapper<GunAttachmentSO>[5]
            };

            for (int attachmentTypeId = 0; attachmentTypeId < 5; attachmentTypeId++)
            {
                // Initialize level 2
                DEBUG_Attachments[gunId].Value[attachmentTypeId] = new ArrayWrapper<GunAttachmentSO>
                {
                    Value = new GunAttachmentSO[2] // two player slots (0/1)
                };

                for (int playerId = 0; playerId < 2; playerId++)
                {
                    int globalAttachmentId = attachmentIdsList[gunId].Value[attachmentTypeId][playerId];
                    if (globalAttachmentId == -1 || globalAttachmentId >= globalAttachmentsList.Length)
                        continue;

                    DEBUG_Attachments[gunId].Value[attachmentTypeId].Value[playerId] = globalAttachmentsList[globalAttachmentId];
                }
            }
        }
    }

}
