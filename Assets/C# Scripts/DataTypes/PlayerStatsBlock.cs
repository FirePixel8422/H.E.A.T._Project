using Unity.Netcode;
using UnityEngine;


[System.Serializable]
public class PlayerStatsBlock : INetworkSerializable
{
    [SerializeField] private float maxHealth = 250;
    public float maxHealthMultiplier = 1;
    public float MaxHealth => maxHealth * maxHealthMultiplier;

    public float damageMultiplier = 1;

    public float lifeStealMultiplier = 0;
    public float lifeStealOverflowMultiplier = 0;

    public float agilityMultiplier = 1;
    public float jumpStrengthMultiplier = 1;

    public float heatGenerationMultiplier = 1;
    public float heatDecayMultiplier = 1;

    public float recoilMultiplier = 1;
    public float spreadMultiplier = 1;


    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref maxHealth);
        serializer.SerializeValue(ref maxHealthMultiplier);
        serializer.SerializeValue(ref damageMultiplier);
        serializer.SerializeValue(ref lifeStealMultiplier);
        serializer.SerializeValue(ref lifeStealOverflowMultiplier);
        serializer.SerializeValue(ref agilityMultiplier);
        serializer.SerializeValue(ref jumpStrengthMultiplier);
        serializer.SerializeValue(ref heatGenerationMultiplier);
        serializer.SerializeValue(ref heatDecayMultiplier);
        serializer.SerializeValue(ref recoilMultiplier);
        serializer.SerializeValue(ref spreadMultiplier);
    }
}