using System;
using System.Collections;
using UnityEngine;

public class PlayerControllerStandalone : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    /// <summary>
    /// Static accessible instance of the PlayerControllerStandalone (Singleton pattern)
    /// </summary>
    public static PlayerControllerStandalone Instance { get; private set; }
    
    private Transform _cam;
    
    [SerializeField] 
    private float _playerHeight = 1.6f;
    
    [SerializeField]
    [Range(80, 89.9f)]
    [Tooltip("Defines the speed.")]
    private float _rayRotationAngle = 89;
    private LayerMask _layerMask;

    [SerializeField] 
    private float _mouseSensitivity = 1;

    private Coroutine _movementCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
        
        _cam = GetComponentInChildren<Camera>().transform;
        _layerMask = LayerMask.GetMask("Floor");

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 10, _layerMask))
        {
            transform.position = hitInfo.point + (Vector3.up * _playerHeight);
        }
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
       UIManager.Instance.OnMenuToggle += OnMenuToggle;
       _movementCoroutine = StartCoroutine(C_Movement());
    }

    private void OnMenuToggle()
    {
        Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
        
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
        else
        {
            _movementCoroutine = StartCoroutine(C_Movement());
        }
    }

    private IEnumerator C_Movement()
    {
        while (true)
        {
            Rotation();
            Movement();
            yield return null;
        }
    }
    
    private void Rotation()
    {
        float horizontal = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float vertical = -Input.GetAxis("Mouse Y") * _mouseSensitivity;
        
        transform.Rotate(0, horizontal, 0);
        _cam.Rotate(vertical, 0, 0);
    }

    private void Movement()
    {
        float newRotationAngle = _rayRotationAngle;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            newRotationAngle *= 0.995f;
        }
        
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            dir += transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dir += -transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir += -transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir += transform.right;
        }

        Vector3 rotationAxis = Quaternion.Euler(0, 90, 0) * dir;
        Quaternion rotation = Quaternion.AngleAxis(newRotationAngle, rotationAxis);
        
        Ray ray = new Ray(transform.position, rotation * dir);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10, _layerMask))
        {
            transform.position = hitInfo.point + (Vector3.up * _playerHeight);
        }
    }
#elif UNITY_ANDROID

#endif
}
