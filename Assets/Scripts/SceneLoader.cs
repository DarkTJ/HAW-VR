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
    public static void LoadTargetScene()
    {
        LoadScene(_targetSceneIndex);
    }

    /// <summary>
    /// Loads the scene at the given index.
    /// </summary>
    /// <param name="index">Scene index to load.</param>
    public static void LoadScene(int index)
    {
        if (_isLoadingScene)
        {
            return;
        }
        _isLoadingScene = true;
        
        _targetSceneIndex = index;
        UIManager.Instance.HideUI();
        
        SceneReferences.ScreenFader.FadeOut(() =>
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
}
