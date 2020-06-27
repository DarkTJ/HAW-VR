using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReferences : MonoBehaviour
{
    public static Camera PlayerCamera { get; private set; }
    public static Transform PlayerObject { get; private set; }
    public static OVRScreenFade ScreenFade { get; private set; }
    public static RoomController RoomController { get; private set; }
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
       
    }
    
    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) 
    {
        SetReferences();
    }

    private static void SetReferences()
    {
        PlayerCamera = Camera.main;
        PlayerObject = OVRManager.instance.transform;
        ScreenFade = OVRManager.instance.GetComponentInChildren<OVRScreenFade>();
        RoomController = GameObject.Find("Room Controller").GetComponent<RoomController>();
    }
}
