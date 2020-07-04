using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class XRInputDeviceController : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

#elif UNITY_ANDROID
    public InputDevice controller;
    
    private Vector2 _controllerStickAxis = Vector2.zero;

    private bool _isStickMoving,
        _isTriggerDown,
        _isGripDown,
        _isMenuButtonDown;

    public event Action OnAnyKeyDown;
    public event Action<Vector2> OnStickMove;
    public event Action OnStickRelease;
    public event Action OnTriggerDown;    
    public event Action OnTrigger; 
    public event Action OnTriggerUp;
    public event Action OnGripDown;
    public event Action OnGrip; 
    public event Action OnGripUp;
    
    /// <summary>
    /// Only works on the left controller.
    /// </summary>
    public event Action OnMenuButtonDown;
    
    /// <summary>
    /// Only works on the left controller.
    /// </summary>
    public event Action OnMenuButtonUp;
    
    public Vector3 Position => controller.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos) ? pos + OVRManager.instance.transform.position : Vector3.zero;
    public Quaternion Rotation => controller.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot) ? rot : Quaternion.identity;

    private void Awake()
    {
        enabled = false;
    }

    public void SetDevice(InputDevice device)
    {
        controller = device;
        enabled = true;
    }

    public InputDevice GetDevice()
    {
        return controller;
    }

    public void ResetDevice()
    {
        enabled = false;
    }

    public bool HasDevice()
    {
        // This behaviour is only enabled if a controller is assigned
        return enabled;
    }
    
    // Update is called once per frame
    private void Update()
    {

        Vector2 stickAxis;
        bool buttonState;

        // STICK
        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out stickAxis))
        {
            if (_controllerStickAxis != stickAxis)
            {
                _controllerStickAxis = stickAxis;
                StickMove();
            }
        }

        // TRIGGER
        if (controller.TryGetFeatureValue(CommonUsages.triggerButton, out buttonState))
        {
            if (_isTriggerDown != buttonState)
            {
                _isTriggerDown = buttonState;
                Trigger();
            }
        }

        // GRIP
        if (controller.TryGetFeatureValue(CommonUsages.gripButton, out buttonState))
        {
            if (_isGripDown != buttonState)
            {
                _isGripDown = buttonState;
                Grip();
            }
        }

        // MENU BUTTON (LEFT)
        if (controller.TryGetFeatureValue(CommonUsages.menuButton, out buttonState))
        {
            if (_isMenuButtonDown != buttonState)
            {
                _isMenuButtonDown = buttonState;
                MenuButton();
            }
        }
    }

    private void AnyKeyDown()
    {
        OnAnyKeyDown?.Invoke();
    }
    
    private void StickMove()
    {
        if (_controllerStickAxis.x < 0.01f && _controllerStickAxis.y < 0.01f)
        {
            OnStickRelease?.Invoke();
            _isStickMoving = false;
        }
        else
        {
            // If this is the first time moving the stick at this moment
            if (!_isStickMoving)
            {
                AnyKeyDown();
                _isStickMoving = true;
            }
            
            OnStickMove?.Invoke(_controllerStickAxis);
        }
    }

    private void Trigger()
    {
        if (_isTriggerDown)
        {
            OnTriggerDown?.Invoke();
            StartCoroutine(WhileTrigger());
            AnyKeyDown();
        }
        else
        {
            OnTriggerUp?.Invoke();
        }
    }

    private IEnumerator WhileTrigger()
    {
        while (_isTriggerDown)
        {
            OnTrigger?.Invoke();
            yield return null;
        }
    }

    private void Grip()
    {
        if (_isGripDown)
        {
            OnGripDown?.Invoke();
            StartCoroutine(WhileGrip());
            AnyKeyDown();
        }
        else
        {
            OnGripUp?.Invoke();
        }
    }
    
    private IEnumerator WhileGrip()
    {
        while (_isGripDown)
        {
            OnGrip?.Invoke();
            yield return null;
        }
    }

    private void MenuButton()
    {
        if (_isMenuButtonDown)
        {
            OnMenuButtonDown?.Invoke();
            AnyKeyDown();
        }
        else
        {
            OnMenuButtonUp?.Invoke();
        }
    }
#endif
}
