using Unity.Netcode;
using UnityEngine;


namespace FirePixel.Networking
{
    public class PlayerManager : NetworkBehaviour
    {
        public static PlayerManager Instance {get; private set;}
        private void Awake()
        {
            Instance = this;
        }


        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private float spawnFreezeTime;

        [SerializeField] private PlayerStatsBlock defaultPlayerStats;
        public PlayerStatsBlock[] PlayerStats;

        private Vector3[] playerSpawnPositions;
        private Quaternion[] playerSpawnRotations;
        private bool spawnPointsActive;


        public override void OnNetworkSpawn()
        {
            NetworkManager.SceneManager.OnLoadEventCompleted += (_, _, _, _) => TryRequestLocalPlayerSpawn();

            PlayerStats = new PlayerStatsBlock[GlobalGameData.MaxPlayers];

            PlayerStats[NetworkManager.LocalClientId == 0 ? 0 : 1] = defaultPlayerStats;
        }

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
            PlayerDataLibrary.LocalInstance.Input.enabled = true;
        }
    }
}