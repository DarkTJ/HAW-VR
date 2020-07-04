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
    private static SceneLoader _instance;

    private static int _targetSceneIndex = -1;
    private static bool _isLoadingScene;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
        
        // Always keep this object alive
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static void SetTargetScene(int index)
    {
        _targetSceneIndex = index;
    }

    /// <summary>
    /// Loads the scene at the target scene index. 
    /// </summary>
    /// <param name="callingBehaviour">MonoBehaviour that calls this method.</param>
    public static void LoadTargetScene(MonoBehaviour callingBehaviour)
    {
        LoadScene(callingBehaviour, _targetSceneIndex);
    }

    /// <summary>
    /// Loads the scene at the given index.
    /// </summary>
    /// <param name="callingBehaviour">MonoBehaviour that calls this method.</param>
    /// <param name="index">Scene index to load.</param>
    public static void LoadScene(MonoBehaviour callingBehaviour, int index)
    {
        if (_isLoadingScene)
        {
            return;
        }
        _isLoadingScene = true;
        
        _targetSceneIndex = index;
        UIManager.Instance.HideUI();
        Fade(callingBehaviour, 0, 1, () =>
        {
            MultiplayerRoomHandler.Instance.LeaveRoom();
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

    /// <summary>
    /// Fades the screen.
    /// </summary>
    /// <param name="callingBehaviour">MonoBehaviour that calls this method.</param>
    /// <param name="startAlpha">Fade start alpha</param>
    /// <param name="targetAlpha">Fade target alpha</param>
    /// <param name="callback">Callback to invoke after fading</param>
    public static void Fade(MonoBehaviour callingBehaviour, float startAlpha, float targetAlpha, Action callback = null)
    {
        callingBehaviour.StartCoroutine(C_Fade(startAlpha, targetAlpha, callback));
    }
    
    private static IEnumerator C_Fade(float start, float target, Action callback)
    {
        OVRScreenFade screenFade = SceneReferences.ScreenFade;
        
        float t = 0;
        while (t < 1)
        {
            screenFade.SetFadeLevel(Mathf.Lerp(start, target, t));
            t += Time.deltaTime;
            yield return null;
        }
        
        screenFade.SetFadeLevel(target);
        callback?.Invoke();
    }
}
