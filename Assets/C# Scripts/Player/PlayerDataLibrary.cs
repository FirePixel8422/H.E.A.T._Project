using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerDataLibrary : NetworkBehaviour
{
    public static PlayerDataLibrary LocalInstance { get; private set; }

    [Header("Allow this script to be used outside of network environment")]
    [SerializeField] private bool overrideIsOwner;


    [SerializeField] private PlayerStatsBlock stats;
    public PlayerStatsBlock Stats => stats;

    public PlayerInput Input { get; private set; }
    public GunHandler GunHandler { get; private set; }
    public PlayerHealthHandler HealthHandler { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerHotBarHandler HotBarHandler { get; private set; }
    public PlayerHUDHandler HudHandler { get; private set; }
    public UtilityHandler UtilityHandler { get; private set; }
    public RagDollController RagDollController { get; private set; }


    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
        GunHandler = GetComponent<GunHandler>();
        HealthHandler = GetComponent<PlayerHealthHandler>();
        Controller = GetComponent<PlayerController>();
        HotBarHandler = GetComponent<PlayerHotBarHandler>();
        HudHandler = GetComponent<PlayerHUDHandler>();
        UtilityHandler = GetComponent<UtilityHandler>();

        RagDollController = GetComponentInChildren<RagDollController>(true);

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