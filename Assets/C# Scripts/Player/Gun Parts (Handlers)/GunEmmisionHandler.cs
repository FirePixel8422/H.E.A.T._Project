using UnityEngine;


[System.Serializable]
public class GunEmmisionHandler
{
    private Material emissionMatInstance;
    private int heatPercentageId;


    public void Init()
    {
        heatPercentageId = Shader.PropertyToID("_HeatPercent");
    }

    public void OnSwapGun(Material _matInstance)
    {
        emissionMatInstance = _matInstance;
    }

    public void UpdateHeatEmission(float percent)
    {
        if (emissionMatInstance == null) return;

        emissionMatInstance.SetFloat(heatPercentageId, percent);
    }
}
