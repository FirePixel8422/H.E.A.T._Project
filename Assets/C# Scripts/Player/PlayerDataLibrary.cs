using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerDataLibrary : NetworkBehaviour
{
    public static PlayerDataLibrary LocalInstance { get; private set; }

    [Header("Allow this script to be used outside of network environment")]
    [SerializeField] private bool overrideIsOwner;

    public PlayerInput input;
    public GunHandler gunHandler;
    public PlayerHealthHandler healthHandler;
    public PlayerStatsHandler statsHandler;
    public PlayerController controller;
    public PlayerHotBarHandler hotBarHandler;
    public PlayerHUDHandler hudHandler;
    public UtilityHandler utilityHandler;
    public RagDollController ragDollController;


    private void Start()
    {
        input = GetComponent<PlayerInput>();
        gunHandler = GetComponent<GunHandler>();
        healthHandler = GetComponent<PlayerHealthHandler>();
        statsHandler = GetComponent<PlayerStatsHandler>();
        controller = GetComponent<PlayerController>();
        hotBarHandler = GetComponent<PlayerHotBarHandler>();
        hudHandler = GetComponent<PlayerHUDHandler>();
        utilityHandler = GetComponent<UtilityHandler>();

        ragDollController = GetComponentInChildren<RagDollController>(true);

        if (overrideIsOwner)
        {
            LocalInstance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
    }
}