using UnityEngine;
using UnityEngine.Events;



public class SoundCallbackManager : MonoBehaviour
{
    public static SoundCallbackManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public AudioSystem HipAudioSystem;
    public AudioSystem AdsAudioSystem;
    public AudioSystem WeaponEquipAudioSystem;
}