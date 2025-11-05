using UnityEngine;
using System.Collections;

public class AudioCrossfader : MonoBehaviour
{
    public static AudioCrossfader Instance;

    [Header("Audio Sources")]
    public AudioSource sourceA;
    public AudioSource sourceB;

    [Header("Settings")]
    public float fadeDuration = 2f; // seconds

    private AudioSource activeSource;
    private AudioSource inactiveSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        activeSource = sourceA;
        inactiveSource = sourceB;

        activeSource.volume = 1f;
        inactiveSource.volume = 0f;
    }

    /// <summary>
    /// Crossfades to a new clip, optionally playing it in reverse.
    /// </summary>
    public void SwitchAudio(AudioClip newClip, bool reverse = false)
    {
        StopAllCoroutines();
        StartCoroutine(CrossfadeToNewClip(newClip, reverse));
    }

    private IEnumerator CrossfadeToNewClip(AudioClip newClip, bool reverse)
    {
        if (activeSource.clip == newClip && !reverse)
            yield break;

        AudioClip clipToPlay = reverse ? CreateReversedClip(newClip) : newClip;

        inactiveSource.clip = clipToPlay;
        inactiveSource.Play();

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            activeSource.volume = Mathf.Lerp(1f, 0f, t);
            inactiveSource.volume = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        activeSource.Stop();
        (activeSource, inactiveSource) = (inactiveSource, activeSource);
    }

    /// <summary>
    /// Creates a reversed copy of an AudioClip in memory.
    /// </summary>
    private AudioClip CreateReversedClip(AudioClip original)
    {
        float[] data = new float[original.samples * original.channels];
        original.GetData(data, 0);

        // Reverse audio data
        int sampleCount = original.samples * original.channels;
        float[] reversedData = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            reversedData[i] = data[sampleCount - i - 1];
        }

        // Create new clip
        AudioClip reversedClip = AudioClip.Create(
            original.name + "_reversed",
            original.samples,
            original.channels,
            original.frequency,
            false
        );

        reversedClip.SetData(reversedData, 0);
        return reversedClip;
    }
}
