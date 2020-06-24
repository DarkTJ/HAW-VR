using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MovementTrajectory))]
public class MovementController : MonoBehaviour
{
    /// <summary>
    /// Static accessible instance of the MovementController (Singleton pattern)
    /// </summary>
    public static MovementController Instance { get; private set; }

    private InputManager _inputManager;
    
    private MovementTrajectory _trajectory;
    private MovementCircle _circle;
    private Transform _circleTransform;
    private Vector3 _lastTargetPosition;
    private float _lastTargetPositionResetCounter = 0;

    private int _floorLayer, _snapLayer;
    private LayerMask _raycastLayers;

    [SerializeField] 
    private Color _indicatorColor;
    
    [Range(2, 64)]
    [SerializeField]
    private int _indicatorResolution = 20;
    
    private Transform _playerObject;

    [SerializeField]
    private float _fadeDuration = 0.33f;
    
    [SerializeField]
    private OVRScreenFade _screenFade;

    public bool IsMoving { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        
        // Always keep this object alive
        DontDestroyOnLoad(gameObject);
        
        IsMoving = false;
        _trajectory = GetComponent<MovementTrajectory>();
        _circle = GetComponentInChildren<MovementCircle>();
        
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        _floorLayer = LayerMask.NameToLayer("Floor");
        _snapLayer = LayerMask.NameToLayer("TeleportSnapPoint");
        _raycastLayers = LayerMask.GetMask("Floor", "TeleportSnapPoint");
    }

    private void Start()
    {
        _circleTransform = _circle.transform;
        _circle.SetColor(_indicatorColor);
        _circle.SetResolution(_indicatorResolution);
        _circle.enabled = false;
        
        _trajectory.SetColor(_indicatorColor);
        _trajectory.SetResolution(_indicatorResolution);
    }

    private void OnEnable()
    {
        _inputManager = InputManager.Instance;
        _inputManager.CurrentlyUsedController.OnStickMove += OnStickMove;
        _inputManager.CurrentlyUsedController.OnStickRelease += OnStickRelease;
        
        _playerObject = OVRManager.instance.transform;
    }

    private void OnDisable()
    {
        if (!_inputManager) 
            return;
        
        _inputManager.CurrentlyUsedController.OnStickMove -= OnStickMove;
        _inputManager.CurrentlyUsedController.OnStickRelease -= OnStickRelease;
    }

    // // DEBUG
    // private void Update()
    // {
    //     PreviewMovement(transform.position, transform.rotation);
    // }

    private void OnStickMove(Vector2 stickAxis)
    {
        if (stickAxis.y > 0.8f)
        {
            PreviewMovement(InputManager.Instance.CurrentlyUsedController.Position, InputManager.Instance.CurrentlyUsedController.Rotation);
            _lastTargetPositionResetCounter = 0;
        }
        else
        {
            ResetPreview();
            _lastTargetPositionResetCounter += Time.deltaTime;
            if (_lastTargetPositionResetCounter > 0.2f)
            {
                _lastTargetPosition = Vector3.zero;
                _lastTargetPositionResetCounter = 0;
            }
        }
    }

    private void OnStickRelease()
    {
        Move();
    }
    
    private void PreviewMovement(Vector3 raycastOrigin, Quaternion raycastRotation)
    {
        Ray ray = new Ray(raycastOrigin, raycastRotation * Vector3.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, _raycastLayers))
        {
            Quaternion rotation = Quaternion.identity;
            
            int hitLayer = hitInfo.transform.gameObject.layer;
            if (hitLayer == _floorLayer)
            {
                _lastTargetPosition = hitInfo.point;
                rotation = raycastRotation;
            }
            else if (hitLayer == _snapLayer)
            {
                Bounds bounds = hitInfo.collider.bounds;
                Vector3 targetPosition = bounds.center;
                targetPosition.y += bounds.extents.y;
                
                _lastTargetPosition = targetPosition;

                Vector3 forward = _lastTargetPosition - raycastOrigin;
                Quaternion lookRotation = Quaternion.LookRotation(forward);
                
                // Vector3 raycastRotationEuler = raycastRotation.eulerAngles;
                // rotation = Quaternion.Euler(raycastRotationEuler.x, lookRotation.eulerAngles.y, raycastRotationEuler.z);
                rotation = lookRotation;
            }
            
            _circle.EnableLineRenderer();
            _circleTransform.position = _lastTargetPosition;
            
            _trajectory.EnableLineRenderer();
            _trajectory.DrawTrajectory(raycastOrigin, rotation, _lastTargetPosition);

        }
        else
        {
            ResetPreview();
            _lastTargetPosition = Vector3.zero;
        }
    }

    private void ResetPreview()
    {
        _trajectory.DisableLineRenderer();
        _circle.DisableLineRenderer();
    }

    private void Move()
    {
        if (IsMoving || _lastTargetPosition == Vector3.zero)
        {
            return;
        }
        
        StartCoroutine(C_Move());
    }
    
    /// <summary>
    /// Fades to black using the OVRScreenFade object, moves the player and fades back.
    /// </summary>
    /// <returns></returns>
    private IEnumerator C_Move()
    {
        IsMoving = true;

        float t = 0;
        while (t < 1)
        {
            _screenFade.SetFadeLevel(t);
            t += Time.deltaTime / _fadeDuration;
            yield return null;
        }
        _screenFade.SetFadeLevel(1);

        _playerObject.position = _lastTargetPosition;
        _lastTargetPosition = Vector3.zero;
        
        t = 1;
        while (t > 0)
        {
            _screenFade.SetFadeLevel(t);
            t -= Time.deltaTime / _fadeDuration;
            yield return null;
        }
        _screenFade.SetFadeLevel(0);

        IsMoving = false;
    }
}
