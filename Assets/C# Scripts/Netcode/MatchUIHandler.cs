using FirePixel.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MatchUIHandler : MonoBehaviour
{
    public static MatchUIHandler Instance { get; private set; }

    [SerializeField] private GameObject matchUI;
    [SerializeField] string matchSceneName = "Luke Test Scene";

    [SerializeField] private TextMeshProUGUI[] scoreTextObj;
    private int[] roundPoints;

    [SerializeField] private TextMeshProUGUI roundTimeTextObj;

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

    public void AddPoint(int clientGameId)
    {
        roundPoints[clientGameId] += 1;

        scoreTextObj[clientGameId].text = roundPoints[clientGameId].ToString();
    }

    public void OnMatchStart()
    {
        roundStartTime = NetworkManager.Singleton.ServerTime.TimeAsFloat;
    }


    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
