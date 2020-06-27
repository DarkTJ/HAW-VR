﻿using System;
using System.Collections;
using System.Collections.Generic;
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
        _screenFade = SceneReferences.ScreenFade;
        _screenFade.SetFadeLevel(1);
        
        yield return new WaitUntil(() => MultiplayerSceneSetupController.Instance.IsReady);
        _tutorial.SetControllers(false);

        string localizedTextFileName;
        
        switch (Application.systemLanguage) {
            case SystemLanguage.German:
                localizedTextFileName = "localizedText_de.json";
                break;
            default:
                localizedTextFileName = "localizedText_en.json";
                break;
        }

        LocalizationManager.Instance.LoadLocalizedTextFile(localizedTextFileName);
        
        // Enable fading from now on
        _screenFade.fadeOnStart = true;
        
        // Loading
        while (!LocalizationManager.Instance.IsReady) {
            yield return null;
        }
        
        if (PlayerPrefs.HasKey("username") && !_forceTutorial)
        {
            StartCoroutine(C_FadeIn());
            _tutorial.SetControllers(true);
        }
        else
        {
            _tutorial.SetupAndStart();
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
