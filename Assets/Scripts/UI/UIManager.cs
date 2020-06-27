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

    private void SetUIInteractableState(UIInteractable[] UIInteractables, bool state)
    {
        foreach (UIInteractable i in UIInteractables)
        {
            i.SetState(state);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InputManager.Instance.LeftController.OnMenuButtonDown += OnMenuButton;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        InputManager.Instance.LeftController.OnMenuButtonDown -= OnMenuButton;
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
        
        Vector3 pos = uiObject.position;
        Vector3 targetPosition = camTransform.position + (camTransform.forward * 3);
        targetPosition.y = pos.y;
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
}
