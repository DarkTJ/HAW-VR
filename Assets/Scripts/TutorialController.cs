﻿using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    private LocalizationManager _localizationManager;
    private Camera _cam;
    
    private OVRScreenFade _screenFade;
    
    [SerializeField]
    private Canvas _tutorialCanvas;
    private TextMeshProUGUI _text;

    [SerializeField] 
    private string _tutorialTextKey = "tutorial_";

    private bool _isTriggerDown, _isStickPushed;
    
    public void SetupAndStart(OVRScreenFade screenFade)
    {
        _screenFade = screenFade;
        
        _tutorialCanvas.gameObject.SetActive(false);
        _text = _tutorialCanvas.GetComponentInChildren<TextMeshProUGUI>();
        
        _cam = Camera.main;
        
        _localizationManager = LocalizationManager.Instance;
        InputManager.Instance.CurrentlyUsedController.OnTriggerDown += OnTriggerDown;
        InputManager.Instance.CurrentlyUsedController.OnStickMove += OnStickMove;

        StartCoroutine(Tutorial());
    }
    
    private void OnTriggerDown()
    {
        _isTriggerDown = true;
    }

    private void OnStickMove(Vector2 axis)
    {
        if (axis.y > 0.8f)
        {
            _isStickPushed = true;
        }
    }

    private IEnumerator Tutorial()
    {
        Vector3 pos = _tutorialCanvas.transform.position;
        pos.z = _cam.transform.position.z + 2;
        _tutorialCanvas.transform.position = pos;
        _tutorialCanvas.gameObject.SetActive(true);

        // Show text 1 - 3 and listen to trigger
        for (int i = 1; i < 4; i++)
        {
            _isTriggerDown = false;
            _text.text = _localizationManager.GetLocalizedText(_tutorialTextKey + i);
            yield return new WaitUntil(() => _isTriggerDown);
        }
        
        // Text 4 asks for the stick, so wait for the stick to be pushed
        _text.text = _localizationManager.GetLocalizedText(_tutorialTextKey + 4);
        yield return new WaitUntil(() => _isStickPushed);
        _isStickPushed = false;
        
        // Show text 5 - 11 and listen to trigger
        for (int i = 5; i < 12; i++)
        {
            _isTriggerDown = false;
            _text.text = _localizationManager.GetLocalizedText(_tutorialTextKey + i);
            yield return new WaitUntil(() => _isTriggerDown);
        }
        _tutorialCanvas.gameObject.SetActive(false);

        FadeIn();
    }

    public void FadeIn()
    {
        StartCoroutine(C_FadeIn());
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
