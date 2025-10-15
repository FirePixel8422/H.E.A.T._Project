using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class IngameMenuBehavior : MonoBehaviour
{
    public GameObject[] ingameMenuScreens;
    public GameObject globalVolume;
    private bool menuIsOpen;
    public void OnOpenOrClose(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!menuIsOpen)
            {
                menuIsOpen = true;

                ingameMenuScreens[0].SetActive(true);
                globalVolume.SetActive(true);
            }
            else
            {
                menuIsOpen = false;

                ingameMenuScreens[0].SetActive(false);
                globalVolume.SetActive(false);
            }
        }
    }
}
