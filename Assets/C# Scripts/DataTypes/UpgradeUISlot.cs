using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class UpgradeUISlot
{
    [SerializeField] private GameObject parentObj;
    [SerializeField] private Image image;
    [SerializeField] private Image rarity;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button confirmButton;
    public Button ConfirmButton => confirmButton;


    /// <summary>
    /// Toggle Active state of parent of UI slot
    /// </summary>
    public void SetActive(bool state)
    {
        parentObj.SetActiveSmart(state);
    }


    /// <summary>
    /// Calls <see cref="SetActive(bool)"/> and Updates UI Image and name
    /// </summary>
    public void SetActiveAndUpdateUI(Sprite upgradeSprite, string name, string description, Color rarityColor)
    {
        SetActive(true);

        image.sprite = upgradeSprite;
        rarity.color = rarityColor;
        this.name.text = name;
        this.description.text = description;
    }
}