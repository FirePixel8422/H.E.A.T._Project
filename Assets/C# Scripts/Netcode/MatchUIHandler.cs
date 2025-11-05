using FirePixel.Networking;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MatchUIHandler : MonoBehaviour
{
    public static MatchUIHandler Instance { get; private set; }

    [SerializeField] private GameObject matchUI;
    [SerializeField] private TextMeshProUGUI roundResultTestObj;

    [SerializeField] string matchSceneName = "Luke Test Scene";

    [SerializeField] private TextMeshProUGUI[] scoreTextObj;
    private int[] roundPoints;

    [SerializeField] private TextMeshProUGUI roundTimeTextObj;

    [SerializeField] private Animator anim;
    [SerializeField] private GameObject upgradeCards;

    private float roundStartTime;



    private void Awake()
    {
        Instance = this;

        roundPoints = new int[GlobalGameData.MaxPlayers];
    }

    private void OnEnable() => UpdateScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterUpdate(OnUpdate);

    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        matchUI.SetActive(scene.name == matchSceneName);
    }

    private void OnUpdate()
    {
        if (MatchManager.Instance.MatchState != MatchState.FightingPhase) return;

        roundTimeTextObj.text = IntToClockString(Mathf.FloorToInt(NetworkManager.Singleton.ServerTime.TimeAsFloat - roundStartTime));
    }

    public string IntToClockString(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }
    public void OnMatchStart()
    {
        roundStartTime = NetworkManager.Singleton.ServerTime.TimeAsFloat;
    }

    public void UpdateMatchState(int winnerClientGameId)
    {
        bool localClientWon = ClientManager.LocalClientGameId == winnerClientGameId;
        roundResultTestObj.text = localClientWon ? "Round Won!" : "Round Lost...";
        roundPoints[winnerClientGameId] += 1;

        if (roundPoints[winnerClientGameId] == 7)
        {
            roundResultTestObj.text = "Player: " + winnerClientGameId + " Won!";
            Invoke(nameof(Leave), 3);

            for (int i = 0; i < GlobalGameData.MaxPlayers; i++)
            {
                string text = roundPoints[i].ToString();

                scoreTextObj[i].text = text;
            }

            return;
        }
        for (int i = 0; i < GlobalGameData.MaxPlayers; i++)
        {
            string text = roundPoints[i].ToString();

            scoreTextObj[i].text = text;
        }

        // Win/Loss animation
        anim.SetInteger("Death", 1);
        if (winnerClientGameId != ClientManager.LocalClientGameId)
        {
            Invoke(nameof(SetUpgradeUIActive), 5f);
        }
    }
    private void Leave()
    {
        ClientManager.Instance.DisconnectClient_ServerRPC(ClientManager.LocalClientGameId);
    }
    private void SetUpgradeUIActive()
    {
        upgradeCards.SetActive(true);
    }

    public void EndUpgradeMenus()
    {
        StartCoroutine(FadeOutUpgrades());
    }
    private IEnumerator FadeOutUpgrades()
    {
        anim.SetInteger("Death", 2);

        yield return new WaitForSeconds(1);
        upgradeCards.SetActive(false);
    }


    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
