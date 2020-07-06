using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovementTrajectory))]
public class MovementController : MonoBehaviour
{
    /// <summary>
    /// Static accessible instance of the MovementController (Singleton pattern)
    /// </summary>
    public static MovementController Instance { get; private set; }

    private InputManager _inputManager;

    private OVRScreenFade _screenFade;
    
    private MovementTrajectory _trajectory;
    private MovementCircle _circle;
    private Transform _circleTransform;
    private Vector3 _lastTargetPosition;
    private float _lastTargetPositionResetCounter = 0;

    private int _floorLayer, _snapLayer;

    [SerializeField] 
    private Color _indicatorColor;
    
    [Range(2, 64)]
    [SerializeField]
    private int _indicatorResolution = 20;

    [SerializeField]
    private float _fadeDuration = 0.33f;
    
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
    }

    private void Start()
    {
        _circleTransform = _circle.transform;
        _circle.SetColor(_indicatorColor);
        _circle.SetResolution(_indicatorResolution);
        _circle.enabled = false;
        
        _trajectory.SetColor(_indicatorColor);
        _trajectory.SetResolution(_indicatorResolution);
        
        _inputManager = InputManager.Instance;
        
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        
#elif UNITY_ANDROID
        _inputManager.CurrentlyUsedController.OnStickMove += OnStickMove;
        _inputManager.CurrentlyUsedController.OnStickRelease += OnStickRelease;
#endif
    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    private void Update()
    {
        Transform playerCameraTransform = SceneReferences.PlayerCamera.transform;
        // PreviewMovement(playerCameraTransform.position, playerCameraTransform.rotation);
    }
    
    private void PreviewMovement(Vector3 raycastOrigin, Quaternion raycastRotation)
    {
        Ray ray = new Ray(raycastOrigin, raycastRotation * Vector3.forward);
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, 100))
        {
            ResetPreview();
            _lastTargetPosition = Vector3.zero;
            return;
        }
            
        int hitLayer = hitInfo.transform.gameObject.layer;
        Quaternion rotation = Quaternion.identity;

        if (hitLayer == _snapLayer)
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
        else
        {
            ResetPreview();
            _lastTargetPosition = Vector3.zero;
            return;
        }
            
        _circle.EnableLineRenderer();
        _circleTransform.position = _lastTargetPosition;
            
        _trajectory.EnableLineRenderer();
        _trajectory.DrawTrajectory(raycastOrigin, rotation, _lastTargetPosition);
    }
    
#elif UNITY_ANDROID
    private void OnStickMove(Vector2 stickAxis)
    {
        if (stickAxis.y > 0.8f)
        {
            PreviewMovement(_inputManager.CurrentlyUsedController.Position, _inputManager.CurrentlyUsedController.Rotation);
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
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, 100))
        {
            ResetPreview();
            _lastTargetPosition = Vector3.zero;
            return;
        }
            
        int hitLayer = hitInfo.transform.gameObject.layer;
        if (hitLayer != _floorLayer && hitLayer != _snapLayer)
        {
            ResetPreview();
            _lastTargetPosition = Vector3.zero;
            return;
        }
            
        Quaternion rotation = Quaternion.identity;
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
#endif
    
    private void ResetPreview()
    {
        _trajectory.DisableLineRenderer();
        _circle.DisableLineRenderer();
    }

    private void Move()
    {
        ResetPreview();
        if (IsMoving || _lastTargetPosition == Vector3.zero)
        {
            return;
        }
        
        StartCoroutine(C_Move(_lastTargetPosition));
    }

    /// <summary>
    /// Teleports the player with a fade to the target position.
    /// Does not work if the player is already currently moving.
    /// </summary>
    /// <param name="target"></param>
    public void Move(Vector3 target)
    {
        if (IsMoving)
        {
            return;
        }
        
        StartCoroutine(C_Move(_lastTargetPosition));
    }
    
    /// <summary>
    /// Fades to black using the OVRScreenFade object, moves the player and fades back.
    /// </summary>
    private IEnumerator C_Move(Vector3 target)
    {
        _screenFade = SceneReferences.ScreenFade;
        
        IsMoving = true;

        float t = 0;
        while (t < 1)
        {
            
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            
#elif UNITY_ANDROID
            _screenFade.SetFadeLevel(t);
#endif
            t += Time.deltaTime / _fadeDuration;
            yield return null;
        }
        
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            
#elif UNITY_ANDROID
        _screenFade.SetFadeLevel(1);
#endif

        SceneReferences.PlayerObject.position = target;
        _lastTargetPosition = Vector3.zero;
        
        t = 1;
        while (t > 0)
        {
            
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            
#elif UNITY_ANDROID
            _screenFade.SetFadeLevel(t);
#endif
            t -= Time.deltaTime / _fadeDuration;
            yield return null;
        }
        
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            
#elif UNITY_ANDROID
            _screenFade.SetFadeLevel(0);
#endif
        IsMoving = false;
    }
}
