using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIPointer))]
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Static accessible instance of the UIManager (Singleton pattern)
    /// </summary>
    public static UIManager Instance { get; private set; }
    
    [SerializeField]
    private GameObject _menuCanvas, textFieldCanvas;

    [SerializeField]
    private UIInteractable[] _disableInLobby;

    private TextMeshProUGUI _textField;

    private bool _isShowingMenu;

    private UIPointer _uiPointer;
    private Keyboard _keyboard;
    
    public event Action<string> OnTextSubmit;

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

        _textField = textFieldCanvas.GetComponentInChildren<TextMeshProUGUI>();
        _uiPointer = GetComponent<UIPointer>();
        _keyboard = GetComponentInChildren<Keyboard>();
        
        
        InputManager.Instance.LeftController.OnMenuButtonDown += OnMenuButton;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        _menuCanvas.SetActive(false);
        textFieldCanvas.SetActive(false);
        
        _uiPointer.Disable();
        SetUIInteractableState(_disableInLobby, false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 0)
        {
            SetUIInteractableState(_disableInLobby, false);
        }
        else
        {
            SetUIInteractableState(_disableInLobby, true);
        }
    }

    private static void SetUIInteractableState(UIInteractable[] uiInteractables, bool state)
    {
        foreach (UIInteractable i in uiInteractables)
        {
            i.SetState(state);
        }
    }

    private void OnMenuButton()
    {
        if (_isShowingMenu)
        {
            _menuCanvas.SetActive(false);
            _uiPointer.Disable();
        }
        else
        {
            PlaceUI(_menuCanvas.transform);
            _uiPointer.Enable();
        }
        _isShowingMenu = !_isShowingMenu;
    }

    public void ShowKeyboard(bool hideMenu = true)
    {
        _uiPointer.Enable();
        
        PlaceUI(_keyboard.transform);
        _keyboard.transform.Translate(Vector3.down);
        PlaceUI(textFieldCanvas.transform);

        if (hideMenu)
        {
            _menuCanvas.SetActive(false);
        }
    }

    public void HideKeyboard(bool disablePointer = true)
    {
        _keyboard.gameObject.SetActive(false);
        textFieldCanvas.SetActive(false);

        if (disablePointer)
        {
            _uiPointer.Disable();
        }
    }

    private void PlaceUI(Transform uiObject)
    {
        Transform camTransform = SceneReferences.PlayerCamera.transform;
        
        Vector3 targetPosition = camTransform.position + (camTransform.forward * 3);
        uiObject.position = targetPosition;

        Quaternion rot = Quaternion.LookRotation(camTransform.forward);
        uiObject.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);

        uiObject.gameObject.SetActive(true);
    }

    public void SubmitText()
    {
        OnTextSubmit?.Invoke(_textField.text);
        _textField.text = "";
    }

    public void HideUI()
    {
        _isShowingMenu = false;
        _menuCanvas.SetActive(false);
        _keyboard.gameObject.SetActive(false);
        textFieldCanvas.SetActive(false);
        _uiPointer.Disable();
    }
}
