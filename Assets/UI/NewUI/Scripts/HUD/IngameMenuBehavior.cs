using FirePixel.Networking;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class IngameMenuBehavior : MonoBehaviour
{
    public GameObject[] ingameMenuScreens;
    public GameObject globalVolume;
    private bool menuIsOpen;
    public static Action OnMenuToggled { get; set; }


    public void OnOpenOrClose(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!menuIsOpen)
            {
                Cursor.lockState = CursorLockMode.None;

                menuIsOpen = true;

                ingameMenuScreens[0].SetActive(true);
                globalVolume.SetActive(true);
            }
            else
            {
                menuIsOpen = false;

                ingameMenuScreens[0].SetActive(false);
                ingameMenuScreens[1].SetActive(false);
                globalVolume.SetActive(false);
            }
            OnMenuToggled?.Invoke();
        }
    }
    public void ResumeGame()
    {
        menuIsOpen = false;

        ingameMenuScreens[0].SetActive(false);
        globalVolume.SetActive(false);
    }
    public void IngameSettings()
    {
        ingameMenuScreens[0].SetActive(false);
        ingameMenuScreens[1].SetActive(true);
    }
    public void Leavematch()
    {
        ClientManager.Instance.DisconnectClient_ServerRPC(ClientManager.LocalClientGameId);
    }
    public void GoBack()
    {
        ingameMenuScreens[1].SetActive(false);
        ingameMenuScreens[0].SetActive(true);
    }
}
