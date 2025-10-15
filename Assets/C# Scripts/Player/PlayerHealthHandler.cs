using FirePixel.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealthHandler : NetworkBehaviour, IDamagable
{
    [SerializeField] private float cHealth = 250;
    [SerializeField] private TextMeshProUGUI healthTextObj;
    [SerializeField] private Slider healthSlider;
    public float MaxHealth
    {
        get => stats.MaxHealth;
    }
    public float CurrentHealth
    {
        get => cHealth;
        set
        {
            cHealth = value;

            if (IsOwner || PlayerDataLibrary.LocalInstance.overrideIsOwner)
            {
                DebugLogger.Log("Updating health: " + value);

                healthTextObj.text = Mathf.FloorToInt(value).ToString();
                healthSlider.value = value / MaxHealth;
            }
        }
    }

    public void GainLifeStealHealth(float toHeal, float overFlowMultiplier)
    {
        float healthAwayFromMax = MaxHealth - CurrentHealth;
        float overflow = Mathf.Clamp(toHeal - healthAwayFromMax, 0, float.MaxValue);

        CurrentHealth += toHeal + (overflow * overFlowMultiplier);
    }

    private NetworkStateMachine stateMachine;
    /// <summary>
    /// The Local players hudHandler
    /// </summary>
    private PlayerHUDHandler hudHandler;

    private PlayerStatsBlock stats;


    private void Awake()
    {
        stateMachine = GetComponent<NetworkStateMachine>();
        hudHandler = GetComponent<PlayerHUDHandler>();

        stats = GetComponent<PlayerDataLibrary>().Stats;
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }


    #region Take Damage, Update Health

    /// <summary>
    /// Make this player take damage locally and send to other clients. if health hits 0, die and send to other clients.
    /// </summary>
    public void DealDamage(float damage, bool headShot, Vector3 hitPoint, Vector3 hitDir, out HitTypeResult hitTypeResult)
    {
        bool dead = RecieveDamage(damage);

        hitTypeResult = IDamagable.CalculateHitType(headShot, dead);

        if (dead)
        {
            OnDeath(hitPoint, hitDir);
            return;
        }

        SendDamage_ServerRPC(GameIdRPCTargets.SendToOppositeOfLocalClient(), damage, hitTypeResult);
    }

    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void SendDamage_ServerRPC(GameIdRPCTargets rpcTargets, float damage, HitTypeResult hitType)
    {
        RecieveDamage_ClientRPC(rpcTargets, damage, hitType);
    }
    [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void RecieveDamage_ClientRPC(GameIdRPCTargets rpcTargets, float damage, HitTypeResult hitType)
    {
        if (rpcTargets.IsTarget == false) return;

        RecieveDamage(damage);

        hudHandler.OnDamageRecieved(damage / MaxHealth, hitType);
    }

    private bool RecieveDamage(float damage)
    {
        CurrentHealth -= damage;

        // If player health falls below 0, Call OnDeath
        if (CurrentHealth <= 0)
        {
            return true;
        }
        return false;
    }

    #endregion


    #region Death

    // Flag state machine as dead and notify other clients
    private void OnDeath(Vector3 hitPoint, Vector3 hitDir)
    {
        stateMachine.Die(hitDir, hitPoint, 0.25f);

        Die();
        OnDeath_ServerRPC(GameIdRPCTargets.SendToOppositeOfLocalClient(), NetworkObject.GetOwnerClientGameId());
    }

    /// <summary>
    /// Notify Server client has died and update game state on server
    /// </summary>
    [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void OnDeath_ServerRPC(GameIdRPCTargets rpcTargets, int deadPlayerGameId)
    {
        OnDeath_ClientRPC(rpcTargets);

        PlayerHealthHandler[] players = this.FindObjectsOfType<PlayerHealthHandler>();
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].IsSpawned == false) continue;

            players[i].NetworkObject.Despawn(gameObject);
        }

        MatchManager.Instance.OnPlayerDeath_ServerRPC(deadPlayerGameId);
    }
    [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    private void OnDeath_ClientRPC(GameIdRPCTargets rpcTargets)
    {
        if (rpcTargets.IsTarget == false) return;

        Die();
    }

    private void Die()
    {
        
    }

    #endregion
}