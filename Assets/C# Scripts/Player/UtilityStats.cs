using UnityEngine;



[System.Serializable]
public class UtilityStats
{
    public Throwable utilityPrefab;

    [SerializeField] private int maxCharges = 2;
    private int chargesLeft;

    public bool IsUsable => chargesLeft > 0;

    public virtual void Use()
    {
        chargesLeft -= 1;
    }
}