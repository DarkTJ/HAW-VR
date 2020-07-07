using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(BoxCollider))]
public abstract class UISlider : UIInteractable
{
    protected Slider _slider;

    [SerializeField]
    protected Image _colorFeedbackImage;

    protected virtual void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _slider = GetComponent<Slider>();
        _colorFeedbackImage.color = isEnabled ? _normalColor : _disabledColor;
    }

    public override void SetState(bool state)
    {
        isEnabled = state;
        _colorFeedbackImage.color = state ? _normalColor : _disabledColor;
    }

    public override void OnPointerEnter()
    {
        _colorFeedbackImage.color = _hoverColor;
        _isHovered = true;
        FMODEventManager.Instance.PlaySound_ButtonHover();
    }

    public override void OnPointerLeave()
    {
        _colorFeedbackImage.color = _normalColor;
        _isHovered = false;
    }

    public override void OnClick(Vector3 hitPoint)
    {
        _colorFeedbackImage.color = _pressedColor;
    }

    public abstract void OnDrag(Vector3 hitPoint);

    public override void OnClickUp()
    {
        _colorFeedbackImage.color = _isHovered ? _hoverColor : _normalColor;
    }
}