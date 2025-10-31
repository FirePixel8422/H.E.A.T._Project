using System;
using System.Linq;
using UnityEngine;




public class SpawnPointHandler : MonoBehaviour
{
#pragma warning disable UDR0001
    public static SpawnPointHandler Instance;
#pragma warning restore UDR0001
    private void Awake()
    {
        Instance = this;
    }


    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool pinToGround;


    // Get playerCount Random SpawnsPoints in a random order of current scene
    public static (bool, Vector3[], Quaternion[]) GetShuffledSpawnPoints()
    {
        if (Instance == null || Instance.spawnPoints.Length < 2)
        {
            DebugLogger.LogWarning("No or too little SpawnPoints in this fighting scene found. Place a SpawnPointHandler and assign at least 2 spawnPoints if you want a network player in this scene.");
            return (false, null, null);
        }

        int totalSpawns = Instance.spawnPoints.Length;
        int playerLimit = GlobalGameData.MaxPlayers;
        int count = Mathf.Min(totalSpawns, playerLimit);

        Vector3[] positions = new Vector3[count];
        Quaternion[] rotations = new Quaternion[count];

        for (int i = 0; i < count; i++)
        {
            positions[i] = Instance.spawnPoints[i].position;
            rotations[i] = Instance.spawnPoints[i].rotation;
        }

        // Fisher-Yates shuffle (uniform)
        for (int i = count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (positions[i], positions[j]) = (positions[j], positions[i]);
            (rotations[i], rotations[j]) = (rotations[j], rotations[i]);
        }

        return (true, positions, rotations);
    }



    private void OnDrawGizmos()
    {
        foreach (Transform t in transform)
        {
            if (spawnPoints.Contains(t) == false)
            {
                Array.Resize(ref spawnPoints, spawnPoints.Length + 1);
                spawnPoints[^1] = t;
            }
        }

        int spawnPointCount = spawnPoints.Length;
        for (int i = 0; i < spawnPointCount; i++)
        {
            if (spawnPoints[i] == null) continue;

            spawnPoints[i].gameObject.name = "SpawnPoint " + (i + 1).ToString();

            if (pinToGround)
            {
                if (Physics.SphereCast(spawnPoints[i].position + Vector3.up * 1f, 0.25f, Vector3.down, out RaycastHit hit))
                {
                    Vector3 pos = spawnPoints[i].position;
                    pos.y = hit.point.y;

                    spawnPoints[i].position = pos;
                }
            }

            float smallCubeSize = 0.5f;

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(spawnPoints[i].position + Vector3.up * smallCubeSize, Vector3.one);
            
            Gizmos.color = new Color(0.25f, 1, 0.25f);
            Gizmos.DrawWireCube(spawnPoints[i].position + spawnPoints[i].forward + Vector3.up * smallCubeSize, Vector3.one * smallCubeSize);
        }
    }
}