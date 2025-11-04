using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SettingsBehavior : MonoBehaviour
{
    public static float SensitivityMultiplierHip;
    public static float SensitivityMultiplierADS;

    public AudioMixer mainAudioMixer;

    float savedVolumeMaster;
    float savedVolumeAmbient;
    float savedVolumeMusic;
    float savedVolumeSFX;

    public Slider masterSlider;
    public Slider ambientSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider sensitivitySlider;
    public Slider adssensitvitySlider;

    public TMP_Text masterNumber;
    public TMP_Text ambientNumber;
    public TMP_Text musicNumber;
    public TMP_Text sfxNumber;
    public TMP_Text sensitivityNumber;
    public TMP_Text adssensitivityNumber;

    public void Start()
    {
        savedVolumeMaster = PlayerPrefs.GetFloat("MasterVolume", Mathf.Log10(0.75f) * 20); // default 75%
        mainAudioMixer.SetFloat("MasterMixer", savedVolumeMaster);

        savedVolumeAmbient = PlayerPrefs.GetFloat("AmbientVolume", Mathf.Log10(0.75f) * 20); // default 75%
        mainAudioMixer.SetFloat("AmbientMixer", savedVolumeAmbient);

        savedVolumeMusic = PlayerPrefs.GetFloat("MusicVolume", Mathf.Log10(0.75f) * 20); // default 75%
        mainAudioMixer.SetFloat("MusicMixer", savedVolumeMusic);

        savedVolumeSFX = PlayerPrefs.GetFloat("SFXVolume", Mathf.Log10(0.75f) * 20); // default 75%
        mainAudioMixer.SetFloat("SFXMixer", savedVolumeSFX);

        SensitivityMultiplierHip = PlayerPrefs.GetFloat("Sensitivity", 1);

        SensitivityMultiplierADS = PlayerPrefs.GetFloat("ADSSensitivity", 1);

        float masterlinearVolume = Mathf.Pow(10f, savedVolumeMaster / 20f);
        masterSlider.value = masterlinearVolume;
        float masterlinearText = Mathf.RoundToInt(masterlinearVolume * 100f);
        masterNumber.text = masterlinearText.ToString("N0");

        float ambientlinearVolume = Mathf.Pow(10f, savedVolumeAmbient / 20f);
        ambientSlider.value = ambientlinearVolume;
        float ambientlinearText = Mathf.RoundToInt(ambientlinearVolume * 100f);
        ambientNumber.text = ambientlinearText.ToString("N0");

        float musiclinearVolume = Mathf.Pow(10f, savedVolumeMusic / 20f);
        musicSlider.value = musiclinearVolume;
        float musiclinearText = Mathf.RoundToInt(musiclinearVolume * 100f);
        musicNumber.text = musiclinearText.ToString("N0");

        float sfxlinearVolume = Mathf.Pow(10f, savedVolumeSFX / 20f);
        sfxSlider.value = sfxlinearVolume;
        float sfxlinearText = Mathf.RoundToInt(sfxlinearVolume * 100f);
        sfxNumber.text = sfxlinearText.ToString("N0");

        sensitivitySlider.value = SensitivityMultiplierHip;
        adssensitvitySlider.value = SensitivityMultiplierADS;

        sensitivityNumber.text = SensitivityMultiplierHip.ToString("F1");
        adssensitivityNumber.text = SensitivityMultiplierADS.ToString("F1");
    }
    public void OnMasterVolumeChanged(float value)
    {
        float volumeInDb = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mainAudioMixer.SetFloat("MasterMixer", volumeInDb);
        PlayerPrefs.SetFloat("MasterVolume", volumeInDb);

        float linearVolume = Mathf.RoundToInt(value * 100f);
        masterNumber.text = linearVolume.ToString("N0");
    }
    public void OnAmbientVolumeChanged(float value)
    {
        float volumeInDb = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mainAudioMixer.SetFloat("AmbientMixer", volumeInDb);
        PlayerPrefs.SetFloat("AmbientVolume", volumeInDb);

        float linearVolume = Mathf.RoundToInt(value * 100f);
        ambientNumber.text = linearVolume.ToString("N0");
    }
    public void OnMusicVolumeChanged(float value)
    {
        float volumeInDb = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mainAudioMixer.SetFloat("MusicMixer", volumeInDb);
        PlayerPrefs.SetFloat("MusicVolume", volumeInDb);

        float linearVolume = Mathf.RoundToInt(value * 100f);
        musicNumber.text = linearVolume.ToString("N0");
    }
    public void OnSFXVolumeChanged(float value)
    {
        float volumeInDb = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mainAudioMixer.SetFloat("SFXMixer", volumeInDb);
        PlayerPrefs.SetFloat("SFXVolume", volumeInDb);

        float linearVolume = Mathf.RoundToInt(value * 100f);
        sfxNumber.text = linearVolume.ToString("N0");
    }
    public void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        SensitivityMultiplierHip = value;

        sensitivityNumber.text = value.ToString("F1");
    }
    public void OnADSSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("ADSSensitivity", value);
        SensitivityMultiplierADS = value;

        sensitivityNumber.text = value.ToString("F1");
    }
}
