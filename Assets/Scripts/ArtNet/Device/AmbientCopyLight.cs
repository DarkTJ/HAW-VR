using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientCopyLight : MonoBehaviour
{



    private Light lamp;

    public Color color;


    public GameObject[] objectToSample;
    
    // Start is called before the first frame update
    void Start()
    {
        lamp = this.GetComponent<Light>();

        objectToSample = FindGameObjectsInsideRange(this.transform.position, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        color = Color.black;
        for (int i = 0;i < objectToSample.Length; i++)
        {
            //color = Color.white - (Color.white - color) * (Color.white - objectToSample[i].GetComponentInChildren<Renderer>().material.GetColor("_EmissionColor"));
            //color = color + ((Vector3.Distance(this.transform.position,objectToSample[i].GetComponentInChildren<Renderer>().transform.position)/5f + 0.2f) * objectToSample[i].GetComponentInChildren<Renderer>().material.GetColor("_EmissionColor"));
            color = color + (objectToSample[i].GetComponentInChildren<Renderer>().material.GetColor("_EmissionColor"));
            //Debug.Log( Vector3.Distance(this.transform.position, objectToSample[i].GetComponentInChildren<Renderer>().transform.position) / 5f + 0.2f);
        }

        color = color / objectToSample.Length;
        lamp.color = color;
    }





    //mybe this works ??
    GameObject[] FindGameObjectsInsideRange(Vector3 center,float radius) {
    Collider[] cols  = Physics.OverlapSphere(center, radius);
    var q = cols.Length; // q = how many colliders were found
    //coppy gameobject into array
    GameObject[] gos = new GameObject[q];
        // copy the game objects inside range to it:
    for (int i = 0; i < q; i++)
    {
         gos[i] = cols[i].gameObject;
    }


    return gos; // return the GameObject array
 }
}
