using UnityEngine;



[CreateAssetMenu(fileName = "UtilitySO", menuName = "ScriptableObjects/Utility")]
public class UtilitySO : ScriptableObject
{
    [SerializeField] private UtilityStats stats;
    public UtilityStats Stats => stats;
}