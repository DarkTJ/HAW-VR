using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArtNet.Packets;

public class ArtNetRecorder : MonoBehaviour
{

    public Button startButton;

    public Button save1Button;
    public Button save2Button;
    public Button save3Button;
   

    int saveNumber = 1;

    public string[] savePath = { "/save1.json", "/save2.json", "/save3.json" };

    public Text statusText;

    bool recording = false;

    [SerializeField] ArtNetDmxPacket recievedDMX;

    [System.Serializable]
    public class DMXDataPacketRecorder
    {
        public List<ArtNetDmxPacket> m_data = new List<ArtNetDmxPacket>();
    }

    DMXDataPacketRecorder rec = new DMXDataPacketRecorder();

    // Start is called before the first frame update
    void Start()
    {
        save1Button.onClick.AddListener(() => SwitchSave(1));
        save2Button.onClick.AddListener(() => SwitchSave(2));
        save3Button.onClick.AddListener(() => SwitchSave(3));
        startButton.onClick.AddListener(RecordButton);

        

        startButton.image.color = Color.green;
        save1Button.image.color = Color.gray;
        Debug.Log(Application.persistentDataPath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RecordButton()
    {
        Button[] saveButtons = { save1Button, save2Button, save3Button };
        if (recording == true)
        {       // stop Recording
            recording = false;
            startButton.image.color = Color.green;

            //save buttons wieder entsperren
            for (int i = 0; i < 3; i++)
            {
                saveButtons[i].interactable = true;
            }
            //save m_data to File
            Debug.Log(rec);
            
            string potion = JsonUtility.ToJson(rec);
            Debug.Log(potion);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePath[saveNumber -1], potion);

            Debug.Log(rec.m_data[2]);
            string portion2 = JsonUtility.ToJson(rec.m_data[2]);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePath[saveNumber], portion2);


            // TODO CLEAR DATA 


        }
        else  //start Recording
        {
            recording = true;
            startButton.image.color = Color.red;
            
            //save buttons sperren
            for (int i = 0; i < 3; i++)
            {
                saveButtons[i].interactable = false;
            }
        }
    }


    void SwitchSave(int saveNr)
    {
        Button[] saveButtons = { save1Button, save2Button, save3Button };

        saveNumber = saveNr;

        for (int i = 0; i < 3; i++)
        {
            saveButtons[i].image.color = Color.white;
        }
        saveButtons[saveNr - 1].image.color = Color.gray;
        
    }


    public void DatatoRecord(ArtNetDmxPacket dmxPacket)
    {

        if (recording == true)
        {
            //einfach mal hoffen das record und playback speed stimmt :D
            rec.m_data.Add(dmxPacket);
            recievedDMX = dmxPacket;

            Debug.Log("added:" + recievedDMX);
        }
        
    

    }

   
}
