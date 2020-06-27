using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReferences : MonoBehaviour
{
    private static SceneReferences _instance;
    
    public static Camera PlayerCamera { get; private set; }
    public static Transform PlayerObject { get; private set; }
    
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public static OVRScreenFade ScreenFade { get; private set; }
#elif UNITY_ANDROID
    public static OVRScreenFade ScreenFade { get; private set; }
#endif
    public static RoomController RoomController { get; private set; }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
        
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SetReferences();
    }
    
    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) 
    {
        SetReferences();
    }

    private static void SetReferences()
    {
        PlayerCamera = Camera.main;
        
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        PlayerObject = PlayerCamera.transform;
        ScreenFade = null;
#elif UNITY_ANDROID
        PlayerObject = OVRManager.instance.transform;
        ScreenFade = OVRManager.instance.GetComponentInChildren<OVRScreenFade>();
#endif
        RoomController = GameObject.Find("Room Controller").GetComponent<RoomController>();
    }
}
