using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ArtNet.Packets;

public class ArtNetPlayer : MonoBehaviour
{
    public TextAsset save1;
    public TextAsset save2;
    public TextAsset save3;
    public AudioSource[] track;

    public DmxController dmxcontroller;

    private int length;
    private int progres;

    [SerializeField] ArtNetDmxPacket recievedDMX;
    [System.Serializable]
    public class DMXDataPacketRecorder
    {
        public List<ArtNetDmxPacket> m_data = new List<ArtNetDmxPacket>();
    }

    DMXDataPacketRecorder rec1 = new DMXDataPacketRecorder();
    DMXDataPacketRecorder rec2 = new DMXDataPacketRecorder();
    DMXDataPacketRecorder rec3 = new DMXDataPacketRecorder();
    DMXDataPacketRecorder playback = new DMXDataPacketRecorder();

    public void StartPlayback(int id)
    {

    if (id == 1)
        {
            length = rec1.m_data.Count;  //anzahl pakete, die gespeichert wurden.
            playback = rec1;
        }
        if (id == 2)
        {
            length = rec2.m_data.Count;  //anzahl pakete, die gespeichert wurden.
            playback = rec2;
        }
        if (id == 3)
        {
            length = rec3.m_data.Count;  //anzahl pakete, die gespeichert wurden.
            playback = rec3;
        }

        progres = 0;
        Debug.Log("startPlayback");
        InvokeRepeating("PlayArtNet", 2.0f, 1.0f / 50.0f);   //1/50 ist paketanzahl.
        track[id - 1].Play();
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
    // Start is called before the first frame update
    void Start()
    {
        //open file and save it to a playable List
        var text = save1.text;
        rec1 = JsonUtility.FromJson<DMXDataPacketRecorder>(text);

        StartPlayback(1);


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    void PlayArtNet()
    {

        if (progres <= length)
        {
            dmxcontroller.RecivefromLocalRecorder(playback.m_data[progres]);
            progres += 1;
            Debug.Log("data send: " + progres);
        } else if (progres > length)
        {
            Debug.Log("panik now pls");
            CancelInvoke("PlayArtNet");
        }

        

       
    }
}
