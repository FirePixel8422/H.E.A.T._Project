using Unity.Netcode;
using UnityEngine;


namespace FirePixel.Networking
{
    public class PlayerManager : SmartNetworkBehaviour
    {
        public static PlayerManager Instance {get; private set;}
        private void Awake()
        {
            Instance = this;
        }


        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private float spawnFreezeTime;

        [SerializeField] private PlayerStatsBlock defaultPlayerStats;
        private NetworkStruct<PlayerStatsBlock>[] playerStats;
        public PlayerStatsBlock LocalPlayerStats => playerStats[LocalClientGameId].Value;

        private Vector3[] playerSpawnPositions;
        private Quaternion[] playerSpawnRotations;
        private bool spawnPointsActive;


        public override void OnNetworkSystemsSetup()
        {
            NetworkManager.SceneManager.OnLoadEventCompleted += (_, _, _, _) => MatchManager.Instance.RespawnLocalPlayer();

            playerStats = new NetworkStruct<PlayerStatsBlock>[GlobalGameData.MaxPlayers];

            playerStats[LocalClientGameId].Value = defaultPlayerStats;

            for (int i = 0; i < GlobalGameData.MaxPlayers; i++)
            {
                if (i == LocalClientGameId)
                {
                    playerStats[LocalClientGameId].OnValueChanged += (PlayerStatsBlock stats) => SendPlayerStatsChange_ServerRPC(LocalClientGameId, stats);
                }
            }
        }

        #region Sync PlayerStatsBlock

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void SendPlayerStatsChange_ServerRPC(int clientGameId, PlayerStatsBlock newValue)
        {
            ReceivePlayerStatsChange_ClientRPC(clientGameId, newValue, GameIdRPCTargets.SendToOppositeClient(clientGameId));
        }

        [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void ReceivePlayerStatsChange_ClientRPC(int clientGameId, PlayerStatsBlock newValue, GameIdRPCTargets rpcTargets)
        {
            if (rpcTargets.IsTarget == false) return;

            playerStats[clientGameId].SilentValue = newValue;
        }

        #endregion

        /// <summary>
        /// Send a request to the server to spawn a player, which it will if there are spawnpoints in the scene
        /// </summary>
        public void TryRequestLocalPlayerSpawn()
        {
            if (IsServer)
            {
                (spawnPointsActive, playerSpawnPositions, playerSpawnRotations) = SpawnPointHandler.GetShuffledSpawnPoints();
            }

            SpawnPlayer_ServerRPC(NetworkManager.LocalClientId);
        }


        /// <summary>
        /// Spawn player and destroy all unneeded components on other clients side for your player
        /// </summary>

        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void SpawnPlayer_ServerRPC(ulong ownerNetworkId)
        {
            if (spawnPointsActive == false) return;

            // 0 for host, 1 for any other
            int arrayId = ownerNetworkId == 0 ? 0 : 1;
            Vector3 pos = playerSpawnPositions[arrayId];
            Quaternion rot = playerSpawnRotations[arrayId];

            NetworkObject spawnedPlayer = NetworkObject.InstantiateAndSpawn(playerPrefab, NetworkManager, ownerNetworkId, position: pos, rotation: rot);

            Invoke(nameof(EnablePlayerInput), spawnFreezeTime);
        }

        private void EnablePlayerInput()
        {
            if (PlayerDataLibrary.LocalInstance == null) return;

            PlayerDataLibrary.LocalInstance.Input.enabled = true;
        }
    }
}