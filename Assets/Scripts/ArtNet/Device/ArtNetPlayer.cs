using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArtNet.Packets;
using FMODUnity;


public class ArtNetPlayer : MonoBehaviour
{
    public ArtNetRecorder recorder;

    public float paketeProSekunde = 10f; //10 pakete pro sekunde //TODO ddas in der json speichern und mit auslesen :D
    //3 Text-Dateien, die den artnet code in djson verpackt haben.

    public string[] savePath = { "/save1.json", "/save2.json", "/save3.json" };
    public Button[] savebuttons;
    public Button playbackButton;
    private int ids;
    //die sound player
    //public AudioSource[] track;
    //
    public StudioEventEmitter[] track;

    public DmxController dmxcontroller;

    //playback variables
    private int length;
    private int progres;
    [SerializeField] ArtNetDmxPacket recievedDMX;
    [System.Serializable]
    public class DMXDataPacketRecorder
    {
        public List<ArtNetDmxPacket> m_data = new List<ArtNetDmxPacket>();
    }

    //Speicherort der ausgepackten json dateien
    DMXDataPacketRecorder rec = new DMXDataPacketRecorder();

    public bool playing = false;



    void Start()
    {
        //dateien öffnen und in die Packetrecorder speichern
        savebuttons[0].onClick.AddListener(() => loadfromFile(1));
        savebuttons[1].onClick.AddListener(() => loadfromFile(2));
        savebuttons[2].onClick.AddListener(() => loadfromFile(3));
        playbackButton.onClick.AddListener(() => playbackbuttonevent());

        //StartPlaZyback(2);

    }

    void loadfromFile(int id)
    {
        if(id == 1)
        {
            rec = JsonUtility.FromJson<DMXDataPacketRecorder>(System.IO.File.ReadAllText(Application.persistentDataPath + savePath[0]));

        }
        if (id == 2)
        {
            rec = JsonUtility.FromJson<DMXDataPacketRecorder>(System.IO.File.ReadAllText(Application.persistentDataPath + savePath[1]));

        }
        if (id == 3)
        {
            rec = JsonUtility.FromJson<DMXDataPacketRecorder>(System.IO.File.ReadAllText(Application.persistentDataPath + savePath[2]));

        }
        ids = id;
        Debug.Log(rec);
    }
    // Update is called once per frame
    void Update()
    {

    }
    void playbackbuttonevent()
    {
        if (playing != false)
        {
            playing = false;
            StopPlayback();
            recorder.lockUnlockButtons(false);
        }
        else
        {
            playing = true;
            StartPlayback();
            recorder.lockUnlockButtons(true);
        }
    }
    public void StartPlayback()
    {


        
        progres = 0;
        length = rec.m_data.Count;

        InvokeRepeating("PlayArtNet", 2.0f, 1.0f / paketeProSekunde);   
        //erst dann die musik starten, wegen synch näh
        track[ids - 1].Play();
    }


    public void StopPlayback()
    {
        track[0].Stop();
        track[1].Stop();
        track[2].Stop();
        if (IsInvoking("PlayArtNet")) {
            CancelInvoke("PlayArtNet");
        }
        
    }
    

    

    void PlayArtNet()
    {

        if (progres < length)
        {
            dmxcontroller.RecivefromLocalRecorder(rec.m_data[progres]);
            progres += 1;
           // Debug.Log("data send: " + progres);
        } else if (progres >= length)

        {
            Debug.Log("panik now pls");
            CancelInvoke("PlayArtNet");
        }

        

       
    }
}
