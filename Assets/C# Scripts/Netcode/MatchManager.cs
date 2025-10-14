using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;


namespace FirePixel.Networking
{
    public class MatchManager : NetworkBehaviour
    {
        public static MatchManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }


        [Tooltip("Retrieve MatchData")]
        public MatchSettings settings;

        [Tooltip("Default Match Settings, used when no saved settings are found")]
        [SerializeField] private MatchSettings defaultMatchSettings;

        [Header("Where is UI Parent for all UI that holds components for settings")]
        [SerializeField] private RectTransform UITransform;


        [Header("Match Handling")]
        [SerializeField] private MatchState matchState;

        [SerializeField] private float matchStartDelay;
        [SerializeField] private float matchEndDelay;


        private const string SaveDataPath = "SaveData/CreateLobbySettings.fpx";


        private async void Start()
        {
            await SetupMatchSettings();
        }

        /// <summary>
        /// Sync _matchSettings to server
        /// </summary>
        public async override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                await SaveSettingsAsync(settings);
            }
            else
            {
                RequestSyncMatchSettings_ServerRPC(NetworkManager.LocalClientId);
            }
        }


        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        public void OnPlayerDeath_ServerRPC(int diedPlayerGameId)
        {
            OnPlayerDeath_ClientRPC(diedPlayerGameId);
        }

        [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void OnPlayerDeath_ClientRPC(int diedPlayerGameId)
        {
            matchState = MatchState.UgradePhase;

            PlayerDataLibrary.LocalInstance.Input.enabled = false;
            PlayerDataLibrary.LocalInstance.Rigidbody.isKinematic = true;
            Cursor.lockState = CursorLockMode.None;

            if (ClientManager.LocalClientGameId == diedPlayerGameId)
            {
                UpgradeManager.Instance.CreateUpgradeUI();
            }
        }

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        public void OnEndUpgradePhase_ServerRPC()
        {
            OnEndUpgradePhase_ClientRPC(NetworkManager.ServerTime.TimeAsFloat);
        }
        [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void OnEndUpgradePhase_ClientRPC(float serverTime)
        {
            matchState = MatchState.FightingPhase;

            float lagCompensation = NetworkManager.ServerTime.TimeAsFloat - serverTime;
            float respawnDelay = Mathf.Clamp(matchStartDelay - lagCompensation, 0, float.MaxValue);

            Invoke(nameof(RespawnLocalPlayer), respawnDelay);
        }

        private void RespawnLocalPlayer()
        {
            PlayerManager.Instance.TryRequestLocalPlayerSpawn();
        }


        #region Load/Save Setup Match Settings

        private void UpdateMatchSettingsData(int sliderId, int value)
        {
            settings.SetIntData(sliderId, value);
        }

        /// <summary>
        /// Load saved MatchSettings, or load default if that doesnt exist.
        /// </summary>
        private async Task SetupMatchSettings()
        {
            settings = await LoadSettingsFromFileAsync();

            UIComponentGroup[] UIInputHandlers = UITransform.GetComponentsInChildren<UIComponentGroup>(true);
            int UIhandlerCount = UIInputHandlers.Length;

            for (int i = 0; i < UIhandlerCount; i++)
            {
                int dataIndex = i;
                UIInputHandlers[i].Init(settings.GetSavedInt(dataIndex));

                UIInputHandlers[i].OnValueChanged += (value) => UpdateMatchSettingsData(dataIndex, value);
            }
        }


        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void RequestSyncMatchSettings_ServerRPC(ulong clientNetworkId)
        {
            SyncMatchSettings_ClientRPC(settings, NetworkIdRPCTargets.SendToTargetClient(clientNetworkId));
        }

        [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void SyncMatchSettings_ClientRPC(MatchSettings _settings, NetworkIdRPCTargets rpcTargets)
        {
            if (rpcTargets.IsTarget == false) return;

            settings = _settings;
        }


        private async Task<MatchSettings> LoadSettingsFromFileAsync()
        {
            (bool succes, MatchSettings loadedMatchSettings) = await FileManager.LoadInfoAsync<MatchSettings>(SaveDataPath);

            return succes ? loadedMatchSettings : defaultMatchSettings;
        }
        private async Task SaveSettingsAsync(MatchSettings data)
        {
            await FileManager.SaveInfoAsync(data, SaveDataPath);
        }

        #endregion
    }
}