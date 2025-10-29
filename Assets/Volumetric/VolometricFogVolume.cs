using UnityEngine;

[ExecuteAlways]
public class VolumetricFogVolume : MonoBehaviour
{
    public Material fogMaterial;

    [Header("Fog")]
    public Color fogColor = new Color(0.7f, 0.8f, 1.0f);
    public float density = 0.5f;
    public float scattering = 0.8f;
    public float extinction = 1.0f;
    [Range(-0.9f, 0.9f)] public float anisotropy = 0.0f;
    public int steps = 48;
    public float maxDistance = 100.0f;

    [Header("Height")]
    public bool useHeightFog = true;
    public float fogHeight = 0.0f;
    public float fogFalloff = 1.0f;

    [Header("Noise")]
    public float noiseScale = 0.1f;
    public float noiseStrength = 0.25f;
    public float temporalJitter = 0.5f;

    void Update()
    {
        if (fogMaterial == null) return;

        fogMaterial.SetColor("_FogColor", fogColor);
        fogMaterial.SetFloat("_Density", density);
        fogMaterial.SetFloat("_Scattering", scattering);
        fogMaterial.SetFloat("_Extinction", extinction);
        fogMaterial.SetFloat("_Anisotropy", anisotropy);
        fogMaterial.SetInt("_StepCount", steps);
        fogMaterial.SetFloat("_MaxDistance", maxDistance);
        fogMaterial.SetFloat("_FogHeight", useHeightFog ? fogHeight : -10000);
        fogMaterial.SetFloat("_FogFalloff", fogFalloff);
        fogMaterial.SetFloat("_NoiseScale", noiseScale);
        fogMaterial.SetFloat("_NoiseStrength", noiseStrength);
        fogMaterial.SetFloat("_TemporalJitter", temporalJitter);
    }
}

