using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(UIPointer))]
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _menuCanvas, textFieldCanvas;
    
    private TextMeshProUGUI _textField;

    public bool IsShowingMenu { get; private set; }

    private UIPointer _uiPointer;
    private Keyboard _keyboard;

    public event Action OnMenuToggle;
    public event Action<string> OnTextSubmit;

    private void Awake()
    {
        _textField = textFieldCanvas.GetComponentInChildren<TextMeshProUGUI>();
        _uiPointer = GetComponent<UIPointer>();
        _keyboard = GetComponentInChildren<Keyboard>();
    }

    private void Start()
    {
        _menuCanvas.SetActive(false);
        textFieldCanvas.SetActive(false);
        
        _uiPointer.Disable();
    }

    private void OnEnable()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        InputManager.Instance.OnMenuButtonDown += OnMenuButton;
#elif UNITY_ANDROID
        InputManager.Instance.LeftController.OnMenuButtonDown += OnMenuButton;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        InputManager.Instance.OnMenuButtonDown -= OnMenuButton;
#elif UNITY_ANDROID
        InputManager.Instance.LeftController.OnMenuButtonDown -= OnMenuButton;
#endif
    }

    private void OnMenuButton()
    {
        OnMenuToggle?.Invoke();
        if (IsShowingMenu)
        {
            _menuCanvas.SetActive(false);
            _uiPointer.Disable();
        }
        else
        {
            Debug.Log(_menuCanvas.name);
            PlaceUI(_menuCanvas.transform);
            _uiPointer.Enable();
        }
        IsShowingMenu = !IsShowingMenu;
    }

    public void OnBackToLobbyButton()
    {
        OnMenuButton();
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
        IsShowingMenu = false;
        _menuCanvas.SetActive(false);
        _keyboard.gameObject.SetActive(false);
        textFieldCanvas.SetActive(false);
        _uiPointer.Disable();
    }
}
