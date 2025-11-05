using FirePixel.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerDataLibrary : SmartNetworkBehaviour
{
    public static PlayerDataLibrary LocalInstance { get; private set; }

    [Header("Allow this script to be used outside of network environment")]
    public bool overrideIsOwner;

    public PlayerStatsBlock Stats;

    public PlayerInput Input { get; private set; }
    public GunHandler GunHandler { get; private set; }
    public PlayerHealthHandler HealthHandler { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerHotBarHandler HotBarHandler { get; private set; }
    public PlayerHUDHandler HudHandler { get; private set; }
    public UtilityHandler UtilityHandler { get; private set; }
    public RagDollController RagDollController { get; private set; }

    public Rigidbody Rigidbody { get; private set; }


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

        Rigidbody = GetComponent<Rigidbody>();

        if (overrideIsOwner)
        {
            LocalInstance = this;
            Stats = PlayerManager.Instance.PlayerStats[OwnerClientGameId];
        }

        IngameMenuBehavior.OnMenuToggled += ToggleInput;
    }

    private void ToggleInput(bool state)
    {
        Input.enabled = !state;
    }

    public override void OnNetworkSystemsSetup()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
        Stats = PlayerManager.Instance.PlayerStats[OwnerClientGameId];
    }

    public override void OnDestroy()
    {
        IngameMenuBehavior.OnMenuToggled -= ToggleInput;
    }
}