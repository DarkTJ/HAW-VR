using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardButton : UIButton
{
    public float xSizeMultiplier = 1;

    private char _character;
    private TextMeshProUGUI _text;

    [SerializeField]
    private bool _isSpacebar,_isBackspace;
    
    private TextMeshProUGUI _targetTextField;
    delegate void FMODSoundDelegate();
    FMODSoundDelegate _PlayFMODClickSoundRef;

    protected override void Awake()
    {
        base.Awake();
        _text = GetComponentInChildren<TextMeshProUGUI>();

        if (_isSpacebar)
        {
            _character = ' ';
            _text.text = "";
            _PlayFMODClickSoundRef = FMODEventManager.Instance.PlaySound_KeyboardTypeKey;
        }
        else if (_isBackspace)
        {
            _text.text = "del";
            _PlayFMODClickSoundRef = FMODEventManager.Instance.PlaySound_KeyboardEraseButton;
        }
        else
        {
            _character = gameObject.name[0];
            _character = char.ToLower(_character);
        
            _text.text = _character.ToString();
            _PlayFMODClickSoundRef = FMODEventManager.Instance.PlaySound_KeyboardTypeKey;
        }

    }

    public void SetFontSize(float size)
    {
        _text.fontSize = size;
    }

    public void SetTargetTextfield(TextMeshProUGUI textfield)
    {
        _targetTextField = textfield;
    }
    
    public override void OnClick(Vector3 hitPoint)
    {
        base.OnClick(hitPoint);

        _PlayFMODClickSoundRef();

        if (_isBackspace)
        {
            _targetTextField.text = _targetTextField.text.Substring(0, _targetTextField.text.Length - 1);
        }
        else
        {
            _targetTextField.text += _character;
        }
    }
}
