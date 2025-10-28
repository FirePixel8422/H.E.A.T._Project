using FirePixel.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MatchUIHandler : MonoBehaviour
{
    public static MatchUIHandler Instance {get; private set; }


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

    private void OnUpdate()
    {
        if (MatchManager.Instance.MatchState != MatchState.FightingPhase) return;

        roundTimeTextObj.text = IntToClockString(Mathf.FloorToInt(NetworkManager.Singleton.ServerTime.TimeAsFloat - roundStartTime));
    }

    public string IntToClockString(int time)
    {
        string s = time.ToString("D4");
        return $"{s[..2]}:{s.Substring(2, 2)}";
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
}
