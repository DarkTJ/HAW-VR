using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public abstract class UIButton : UIInteractable
{
    private Image _image;

    protected virtual void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        _image = GetComponent<Image>();
        _image.color = isEnabled ? _normalColor : _disabledColor;
    }

    public override void SetState(bool state)
    {
        isEnabled = state;
        _image.color = state ? _normalColor : _disabledColor;
    }

    public override void OnPointerEnter()
    {
        _image.color = _hoverColor;
        _isHovered = true;
        FMODEventManager.Instance.PlaySound_ButtonHover();
    }

    public override void OnPointerLeave()
    {
        _image.color = _normalColor;
        _isHovered = false;
    }

    public override void OnClick(Vector3 hitPoint)
    {
        _image.color = _pressedColor;
    }

    public override void OnClickUp()
    {
        _image.color = _isHovered ? _hoverColor : _normalColor;
    }
}