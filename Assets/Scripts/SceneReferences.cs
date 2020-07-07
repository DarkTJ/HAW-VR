using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReferences : MonoBehaviour
{
    private static SceneReferences _instance;

    public static int CurrentSceneIndex { get; private set; }
    public static Camera PlayerCamera { get; private set; }
    public static Transform PlayerObject { get; private set; }
    public static ScreenFader ScreenFader { get; private set; }
    public static RoomController RoomController { get; private set; }
    public static AvatarTransformController AvatarTransformController { get; private set; }
    
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
        CurrentSceneIndex = 0;
        SetReferences();
    }
    
    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) 
    {
        CurrentSceneIndex = scene.buildIndex;
        SetReferences();
    }

    private static void SetReferences()
    {
        PlayerCamera = Camera.main;
        
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        PlayerObject = PlayerCamera.transform;
#elif UNITY_ANDROID
        PlayerObject = OVRManager.instance.transform;
#endif
        ScreenFader = PlayerCamera.GetComponent<ScreenFader>();

        RoomController = GameObject.Find("Room Controller").GetComponent<RoomController>();
        AvatarTransformController = GameObject.Find("Avatar Transform Controller").GetComponent<AvatarTransformController>();
    }
}
