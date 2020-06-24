using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(TutorialController))]
public class StartManager : MonoBehaviour
{
    [Header("Debug")] 
    [SerializeField] 
    private bool _forceTutorial = false;
    
    [Header("Will be overriden at runtime.")]
    [SerializeField] 
    private SystemLanguage _language;
    
    [Space]
    
    [SerializeField]
    private OVRScreenFade _screenFade;
    
    private TutorialController _tutorial;
    
    private void Awake()
    {
        _tutorial = GetComponent<TutorialController>();
        
#if !UNITY_EDITOR
        _language = Application.systemLanguage;
#endif
    }
    
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SessionSetupController.Instance.isReady);
        
        string localizedTextFileName;
        
        switch (Application.systemLanguage) {
            case SystemLanguage.German:
                localizedTextFileName = "localizedText_de.json";
                break;
            default:
            case SystemLanguage.English:
                localizedTextFileName = "localizedText_en.json";
                break;
        }

        LocalizationManager.Instance.LoadLocalizedTextFile(localizedTextFileName);
        
        _screenFade.SetFadeLevel(1);
        
        // Enable fading from now on
        _screenFade.fadeOnStart = true;
        
        // Loading
        while (!LocalizationManager.Instance.IsReady) {
            yield return null;
        }
        
        if (PlayerPrefs.HasKey("username") && !_forceTutorial)
        {
            StartCoroutine(C_FadeIn());
        }
        else
        {
            _tutorial.SetupAndStart(_screenFade);
        }
    }
    
    private IEnumerator C_FadeIn()
    {
        float t = 0;
        while (t < 1)
        {
            _screenFade.SetFadeLevel(1 - t);
            t += Time.deltaTime;
            yield return null;
        }
        
        _screenFade.SetFadeLevel(0);
    }
}
