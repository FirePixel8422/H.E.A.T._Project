using FirePixel.Networking;
using System;
using UnityEngine;


public class IngameMenuBehavior : MonoBehaviour
{
    public GameObject[] ingameMenuScreens;
    public GameObject globalVolume;
    private bool menuIsOpen;
    public static Action<bool> OnMenuToggled { get; set; }



    private void OnEnable() => UpdateScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => UpdateScheduler.UnRegisterUpdate(OnUpdate);
    private void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnOpenOrClose();
        }
    }


    public void OnOpenOrClose()
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
            Cursor.lockState = CursorLockMode.Locked;

            ingameMenuScreens[0].SetActive(false);
            ingameMenuScreens[1].SetActive(false);
            globalVolume.SetActive(false);
        }
        OnMenuToggled?.Invoke(menuIsOpen);
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
