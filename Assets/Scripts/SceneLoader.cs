using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads and changes the active scenes.
/// Also calls the MultiplayerRoomHandler to change the multiplayer rooms.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Static accessible instance of the SceneLoader (Singleton pattern)
    /// </summary>
    public static SceneLoader Instance { get; private set; }

    private static int _targetSceneIndex = -1;
    private static bool _isLoadingScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        
        // Always keep this object alive
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(int index)
    {
        if (_isLoadingScene)
        {
            return;
        }
        _isLoadingScene = true;
        
        _targetSceneIndex = index;
        MultiplayerRoomHandler.Instance.LeaveRoom();
        Fade(0, 1, () =>
        {
            SceneManager.LoadScene(index);
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        _isLoadingScene = false;
        
        if (_targetSceneIndex == -1)
        {
            return;
        }
        
        MultiplayerRoomHandler.Instance.JoinRoom(_targetSceneIndex);
        _targetSceneIndex = -1;
    }

    public void Fade(float startAlpha, float targetAlpha, Action callback = null)
    {
        StartCoroutine(C_Fade(startAlpha, targetAlpha, callback));
    }
    
    private IEnumerator C_Fade(float start, float target, Action callback)
    {
        OVRScreenFade _screenFade = SceneReferences.ScreenFade;
        
        float t = 0;
        while (t < 1)
        {
            _screenFade.SetFadeLevel(Mathf.Lerp(start, target, t));
            t += Time.deltaTime;
            yield return null;
        }
        
        _screenFade.SetFadeLevel(target);
        callback?.Invoke();
    }
}
