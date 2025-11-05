using FirePixel.Networking;
using System;
using UnityEngine;

public class IngameMenuBehavior : MonoBehaviour
{
    [Header("Menu Screens")]
    public GameObject mainMenuScreen;
    public GameObject settingsMenuScreen;

    [Header("Other References")]
    public GameObject globalVolume;

    private bool menuIsOpen;
    public static Action<bool> OnMenuToggled { get; set; }

    private void OnEnable() => UpdateScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterUpdate(OnUpdate);

    private void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            print("kanus");
            OnOpenOrClose();
        }
    }

    public void OnOpenOrClose()
    {
        if (!menuIsOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            menuIsOpen = true;

            mainMenuScreen.SetActive(true);
            globalVolume.SetActive(true);
        }
        else
        {
            menuIsOpen = false;
            Cursor.lockState = CursorLockMode.Locked;

            mainMenuScreen.SetActive(false);
            settingsMenuScreen.SetActive(false);
            globalVolume.SetActive(false);
        }

        OnMenuToggled?.Invoke(menuIsOpen);
    }

    public void ResumeGame()
    {
        menuIsOpen = false;
        mainMenuScreen.SetActive(false);
        globalVolume.SetActive(false);
    }

    public void IngameSettings()
    {
        mainMenuScreen.SetActive(false);
        settingsMenuScreen.SetActive(true);
    }

    public void LeaveMatch()
    {
        ClientManager.Instance.DisconnectClient_ServerRPC(ClientManager.LocalClientGameId);
    }

    public void GoBack()
    {
        settingsMenuScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
    }
}
