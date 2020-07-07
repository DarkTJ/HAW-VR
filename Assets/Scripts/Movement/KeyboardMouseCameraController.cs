using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMouseCameraController : MonoBehaviour
{
    public float movementSpeed = 5f;    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(
            movementSpeed * Time.deltaTime * (Input.GetAxis("Vertical") * transform.forward.x + 
            (System.Convert.ToSingle(Input.GetKey(KeyCode.E)) - System.Convert.ToSingle(Input.GetKey(KeyCode.Q))) * transform.up.x + 
                Input.GetAxis("Horizontal") * transform.right.x),
            movementSpeed * (System.Convert.ToSingle(Input.GetKey(KeyCode.E)) - System.Convert.ToSingle(Input.GetKey(KeyCode.Q))) * Time.deltaTime * transform.up.y +
                movementSpeed * Input.GetAxis("Vertical") * Time.deltaTime * transform.forward.y + 
                movementSpeed * Input.GetAxis("Horizontal") * Time.deltaTime * transform.right.y,
            movementSpeed * Input.GetAxis("Vertical") * Time.deltaTime * transform.forward.z +
                movementSpeed * (System.Convert.ToSingle(Input.GetKey(KeyCode.E)) - System.Convert.ToSingle(Input.GetKey(KeyCode.Q))) * Time.deltaTime * transform.up.z + 
                movementSpeed * Input.GetAxis("Horizontal") * Time.deltaTime * transform.right.z
                , Space.World);

    }
}
