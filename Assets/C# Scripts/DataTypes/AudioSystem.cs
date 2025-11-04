using UnityEngine;

[System.Serializable]
public class AudioSystem
{
    [SerializeField] private AudioSource source;

    [SerializeField] private AudioClip[] clips;


    public void PlayRandom()
    {
        source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}