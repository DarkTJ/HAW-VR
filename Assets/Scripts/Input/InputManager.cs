using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class InputManager : MonoBehaviour
{
    /// <summary>
    /// Static accessible instance of the InputManager (Singleton pattern)
    /// </summary>
    public static InputManager Instance { get; private set; }
    
    [NonSerialized]
    public XRInputDeviceController LeftController, RightController, CurrentlyUsedController;
    
    public event Action OnCurrentlyUsedControllerUpdate;

    public Transform PlayerObject { get; private set; }
    public OVRScreenFade ScreenFade { get; private set; }

    private void OnEnable()
    {
        InputDevices.deviceConnected += DeviceConnected;
        InputDevices.deviceDisconnected += DeviceDisconnected;
    }

    private void OnDisable()
    {
        InputDevices.deviceConnected -= DeviceConnected;
        InputDevices.deviceDisconnected -= DeviceDisconnected;
    }
    
    // This manager needs three controllers to work.
    // Is called when the script is added to an object or the user resets the component.
    private void Reset()
    {
        int numOfControllers = gameObject.GetComponents<XRInputDeviceController>().Length;

        // Add up to three controllers
        for (int i = 0; i < 3 - numOfControllers; i++)
        {
            gameObject.AddComponent<XRInputDeviceController>();
        }
    }

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

        XRInputDeviceController[] controllers = GetComponents<XRInputDeviceController>();
        LeftController = controllers[0];
        RightController = controllers[1];
        CurrentlyUsedController = controllers[2];

        SceneManager.sceneLoaded += OnsceneLoad;
    }

    /// <summary>
    /// Loads references that are not persistent / dont destroy on load
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="loadSceneMode"></param>
    private void OnsceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        PlayerObject = OVRManager.instance.transform;
        ScreenFade = OVRManager.instance.GetComponentInChildren<OVRScreenFade>();
    }
    
    private void Start()
    {
        StartCoroutine(FetchController(LeftController, InputDeviceCharacteristics.Left));
        StartCoroutine(FetchController(RightController, InputDeviceCharacteristics.Right));

        LeftController.OnAnyKeyDown += UpdateCurrentlyUsedControllerLeft;
        RightController.OnAnyKeyDown += UpdateCurrentlyUsedControllerRight;
    }
 
    /// <summary>
    /// Called when a button is clicked on the left controller.
    /// Sets the device of the CurrentlyUsedController to the device of the left controller
    /// </summary>
    private void UpdateCurrentlyUsedControllerLeft()
    {
        if (CurrentlyUsedController.GetDevice() == LeftController.GetDevice())
        {
            return;
        }
        
        CurrentlyUsedController.SetDevice(LeftController.GetDevice());
        OnCurrentlyUsedControllerUpdate?.Invoke();
    }

    /// <summary>
    /// Called when a button is clicked on the right controller.
    /// Sets the device of the CurrentlyUsedController to the device of the right controller
    /// </summary>
    private void UpdateCurrentlyUsedControllerRight()
    {
        if (CurrentlyUsedController.GetDevice() == RightController.GetDevice())
        {
            return;
        }
        
        CurrentlyUsedController.SetDevice(RightController.GetDevice());
        OnCurrentlyUsedControllerUpdate?.Invoke();
    }
    
    private IEnumerator FetchController(XRInputDeviceController device, InputDeviceCharacteristics handedness)
    {
        if (device.HasDevice())
        {
            yield break;
        }
        
        List<InputDevice> controllers = new List<InputDevice>();
        InputDeviceCharacteristics characteristicsLeft = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | handedness;
        InputDevices.GetDevicesWithCharacteristics(characteristicsLeft, controllers);
        if (controllers.Count > 0)
        {
            device.SetDevice(controllers[0]);
            yield break;
        }

        // Keep searching for the controller if it has not been found yet
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FetchController(device, handedness));
    }
    
    private void DeviceConnected(InputDevice device)
    {
        UpdateController(device, LeftController, InputDeviceCharacteristics.Left);
        UpdateController(device, RightController, InputDeviceCharacteristics.Right);
    }

    private static void UpdateController(InputDevice device, XRInputDeviceController controller, InputDeviceCharacteristics handedness)
    {
        if (controller.HasDevice())
        {
            return;
        }
        
        InputDeviceCharacteristics desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | handedness;
        if ((device.characteristics & desiredCharacteristics) == desiredCharacteristics)
        {
            controller.SetDevice(device);
        }
    }
    
    private void DeviceDisconnected(InputDevice device)
    {
        if (device == LeftController.GetDevice())
        {
            if (CurrentlyUsedController.GetDevice() == LeftController.GetDevice())
            {
                CurrentlyUsedController.ResetDevice();
            }
            
            LeftController.ResetDevice();

        }
        else if (device == RightController.GetDevice())
        {
            if (CurrentlyUsedController.GetDevice() == RightController.GetDevice())
            {
                CurrentlyUsedController.ResetDevice();
            }
            
            RightController.ResetDevice();
        }
    }
}