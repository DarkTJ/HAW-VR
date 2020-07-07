using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;


using UnityEngine.Android;

using agora_gaming_rtc;

public class QuestInterfaceKonferenz : MonoBehaviour
{
    // PLEASE KEEP THIS App ID IN SAFE PLACE
    // Get your own App ID at https://dashboard.agora.io/

    [SerializeField]
    private Transform[] _videoPositions;

    private string appId = "13d82353ae3a4c99972f58f3a9924fa4";

    private string roomName = "HAW-VR";

    private int numUsers = 0; //0
    private bool connected;


    private static QuestInterfaceKonferenz _AgoraInstance;

    public static QuestInterfaceKonferenz Instance { get { return _AgoraInstance; } }


    private void Awake()
    {
        if (_AgoraInstance != null && _AgoraInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _AgoraInstance = this;
        }
    }



    // Use this for initialization
    private ArrayList permissionList = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
#if (UNITY_2018_3_OR_NEWER)
        permissionList.Add(Permission.Microphone);
#endif
    }

    private void CheckPermission()
    {

        foreach (string permission in permissionList)
        {
            if (Permission.HasUserAuthorizedPermission(permission))
            {
                if (!connected)
                    onJoinRoomClicked();
            }
            else
            {
                Permission.RequestUserPermission(permission);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

        CheckPermission();

    }


    private void onJoinRoomClicked()
    {
        if (!connected)
        {
            connected = true;
            loadEngine();
        }
        join(roomName);
        onSceneHelloVideoLoaded();
    }



    public void onLeaveButtonClicked()
    {

        if (connected)
        {
            leave();
            unloadEngine();
            connected = false;
        }
    }




    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            if (IRtcEngine.QueryEngine() != null)
            {
                IRtcEngine.QueryEngine().DisableVideo();
            }
        }
        else
        {
            if (IRtcEngine.QueryEngine() != null)
            {
                IRtcEngine.QueryEngine().EnableVideo();
            }
        }
    }

    void OnApplicationQuit()
    {
        IRtcEngine.Destroy();
    }


    // load agora engine
    public void loadEngine()
    {
        // start sdk
        Debug.Log("initializeEngine");
        if (mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.getEngine(appId);

        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    // unload agora engine
    public void unloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }

    public void join(string channel)
    {
        Debug.Log("calling join (channel = " + channel + ")");
        if (mRtcEngine == null)
            return;

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
        mRtcEngine.OnUserJoined = onUserJoined;
        mRtcEngine.OnUserOffline = onUserOffline;

        // enable video
        mRtcEngine.EnableVideo();

        // allow camera output callback
        mRtcEngine.EnableVideoObserver();

        // join channel
        mRtcEngine.JoinChannel(channel, null, 0);


        Debug.Log("initializeEngine done");
    }



    public void leave()
    {
        Debug.Log("calling leave");

        if (mRtcEngine == null)
            return;

        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();

    }

    public string getSdkVersion()
    {
        return IRtcEngine.GetSdkVersion();
    }

    // accessing GameObject in Scnene1
    // set video transform delegate for statically created GameObject
    public void onSceneHelloVideoLoaded()
    {
        GameObject go = GameObject.Find("VideoSpawn");
        if (ReferenceEquals(go, null))
        {
            Debug.Log("BBBB: failed to find VideoQuad");
            return;
        }
        VideoSurface o = go.GetComponent<VideoSurface>();
        //  o.mAdjustTransfrom += onTransformDelegate;
    }

    // instance of agora engine
    public IRtcEngine mRtcEngine;

    // implement engine callbacks

    public uint mRemotePeer = 1; // insignificant. only record one peer

    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
        //GameObject textVersionGameObject = GameObject.Find ("VersionText");
        //DebugQuest.Instance.Log(textVersionGameObject.GetComponent<Text> ().text = "Version : " + getSdkVersion ());
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void onUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid);
        // this is called in main thread

        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        numUsers++;
        // create a GameObject and assigne to this new user
        go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        if (!ReferenceEquals(go, null))
        {
            go.name = uid.ToString();

            // configure videoSurface
            VideoSurface o = go.AddComponent<VideoSurface>();
            o.SetForUser(uid);
            //o.mAdjustTransfrom += onTransformDelegate; ///
            o.SetEnable(true);

            go.transform.position = _videoPositions[numUsers].position;
            go.transform.rotation = _videoPositions[numUsers].rotation;
            go.transform.localScale = _videoPositions[numUsers].localScale;
            //float r = Random.Range (-5.0f, 5.0f); ///
           // var videoQuadPos = _videoPositions[numUsers].position;
            //o.transform.position = videoQuadPos + new Vector3(numUsers * 0, 0, 0); //y wie x 0,95f
                                                                                   //            o.transform.localScale = new Vector3(0.80f, (0.28125f), (0.450f));//ohen die 5en  0.09475f  0.5f  0.094924f
        }

        // mRemotePeer = uid;
    }

    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid);
        // this is called in main thread
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Destroy(go);
        }
        numUsers--;
    }

    // delegate: adjust transfrom for game object 'objName' connected with user 'uid'
    // you could save information for 'uid' (e.g. which GameObject is attached)
    private void onTransformDelegate(uint uid, string objName, ref Transform otherTransform)
    {
        if (uid == 0)
        {
            otherTransform.position = new Vector3(0f, 2f, 0f);
            otherTransform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
            otherTransform.Rotate(0f, 1f, 0f);
        }
        else
        {
            otherTransform.Rotate(0.0f, 1.0f, 0.0f);
        }
    }
}