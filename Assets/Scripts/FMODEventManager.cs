using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEventManager : MonoBehaviour
{
    public static FMODEventManager Instance { get; private set; }
    // UI Sounds
    [FMODUnity.EventRef]
    public string ButtonHover_Event = "";
    [FMODUnity.EventRef]
    public string TriggerPress_Event = "";
    [FMODUnity.EventRef]
    public string TriggerPressOK_Event = "";
    [FMODUnity.EventRef]
    public string QuitGameButton_Event = "";
    [FMODUnity.EventRef]
    public string SettingsButton_Event = "";
    [FMODUnity.EventRef]
    public string CancelButton_Event = "";
    [FMODUnity.EventRef]
    public string KeyboardEraseButton_Event = "";
    [FMODUnity.EventRef]
    public string KeyboardHoverKey_Event = "";
    [FMODUnity.EventRef]
    public string KeyboardTypeKey_Event = "";
    [FMODUnity.EventRef]
    public string BackButton_Event = "";
    [FMODUnity.EventRef]
    public string PortalTransport_Event = "";
    [FMODUnity.EventRef]
    public string PortalSaber_Event = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Always keep this object alive
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        InputManager.Instance.OnMainButtonDown += PlaySound_TriggerPress;
    }


    void OnDisable()
    {
        InputManager.Instance.OnMainButtonDown -= PlaySound_TriggerPress;
    }


    public void PlaySound_ButtonHover()
    {
        FMODUnity.RuntimeManager.PlayOneShot(ButtonHover_Event);
    }

    public void PlaySound_TriggerPress()
    {
        FMODUnity.RuntimeManager.PlayOneShot(TriggerPress_Event);
    }

    public void PlaySound_TriggerPressOK()
    {
        FMODUnity.RuntimeManager.PlayOneShot(TriggerPressOK_Event);
    }

    public void PlaySound_QuitGameButton()
    {
        FMODUnity.RuntimeManager.PlayOneShot(QuitGameButton_Event);
    }

    public void PlaySound_SettingsButton()
    {
        FMODUnity.RuntimeManager.PlayOneShot(SettingsButton_Event);
    }

    public void PlaySound_CancelButton()
    {
        FMODUnity.RuntimeManager.PlayOneShot(CancelButton_Event);
    }

    public void PlaySound_KeyboardEraseButton()
    {
        FMODUnity.RuntimeManager.PlayOneShot(KeyboardEraseButton_Event);
    }

    public void PlaySound_KeyboardHoverKey()
    {
        FMODUnity.RuntimeManager.PlayOneShot(KeyboardHoverKey_Event);
    }

    public void PlaySound_KeyboardTypeKey()
    {
        FMODUnity.RuntimeManager.PlayOneShot(KeyboardTypeKey_Event);
    }

    public void PlaySound_BackButton()
    {
        FMODUnity.RuntimeManager.PlayOneShot(BackButton_Event);
    }

    public void PlaySound_PortalTransport()
    {
        FMODUnity.RuntimeManager.PlayOneShot(PortalTransport_Event);
    }

    public void PlaySound_PortalSaber()
    {
        FMODUnity.RuntimeManager.PlayOneShot(PortalSaber_Event);
    }
}