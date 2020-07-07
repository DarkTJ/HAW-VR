using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovementTrajectory))]
public class MovementController : MonoBehaviour
{
    private InputManager _inputManager;
    private ScreenFader _screenFade;
    
    [SerializeField]
    private PlayerControllerStandalone _playerControllerStandalone;

    [SerializeField] 
    private UIManager _uiManager;
    
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
    
    private bool _IsAllowedToMove = true;
    
    private void Awake()
    {
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
        
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        StartCoroutine(C_Check());
#endif
    }
    
    private void OnEnable()
    {
        _uiManager.OnMenuToggle += OnMenuToggle;
        _inputManager = InputManager.Instance;
        
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        _inputManager.OnMainButton += Move;
#elif UNITY_ANDROID
        _inputManager.CurrentlyUsedController.OnStickMove += OnStickMove;
        _inputManager.CurrentlyUsedController.OnStickRelease += Move;
#endif
    }

    private void OnDisable()
    {
        _uiManager.OnMenuToggle -= OnMenuToggle;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        _inputManager.OnMainButton -= Move;
#elif UNITY_ANDROID
        _inputManager.CurrentlyUsedController.OnStickMove -= OnStickMove;
        _inputManager.CurrentlyUsedController.OnStickRelease -= Move;
#endif
    }

    private void OnMenuToggle()
    {
        if (_IsAllowedToMove)
        {
            _IsAllowedToMove = false;
            
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            StopCoroutine(nameof(C_Check));
#elif UNITY_ANDROID
            _inputManager.CurrentlyUsedController.OnStickMove -= OnStickMove;
            _inputManager.CurrentlyUsedController.OnStickRelease -= Move;
#endif
        }
        else
        {
            _IsAllowedToMove = true;
            
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            StartCoroutine(nameof(C_Check));
#elif UNITY_ANDROID
            _inputManager.CurrentlyUsedController.OnStickMove += OnStickMove;
            _inputManager.CurrentlyUsedController.OnStickRelease += Move;
#endif
        }
    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    private IEnumerator C_Check()
    {
        while (true)
        {
            Transform playerCameraTransform = SceneReferences.PlayerCamera.transform;
            PreviewMovement(playerCameraTransform.position + playerCameraTransform.forward, playerCameraTransform.rotation);
            yield return null;
        }
    }
    
    private void PreviewMovement(Vector3 raycastOrigin, Quaternion raycastRotation)
    {
        Ray ray = new Ray(raycastOrigin, raycastRotation * Vector3.forward);
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, 5))
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
    /// Fades to black, moves the player and fades back.
    /// </summary>
    private IEnumerator C_Move(Vector3 target)
    {
        _screenFade = SceneReferences.ScreenFader;
        
        IsMoving = true;
        
        _screenFade.FadeOut(_fadeDuration);
        yield return new WaitForSeconds(_fadeDuration);
        
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        _playerControllerStandalone.Move(target);
#elif UNITY_ANDROID
        SceneReferences.PlayerObject.position = target;
#endif
        
        _lastTargetPosition = Vector3.zero;
        _screenFade.FadeIn(_fadeDuration);
        yield return new WaitForSeconds(_fadeDuration);
  
        IsMoving = false;
    }
}
