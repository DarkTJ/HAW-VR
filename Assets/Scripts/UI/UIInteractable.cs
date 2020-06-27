using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIInteractable : MonoBehaviour
{
    [SerializeField]
    protected Color _normalColor = Color.white,
    _hoverColor = Color.white,
    _pressedColor = Color.white,
    _disabledColor = new Color(0.5f, 0.5f, 0.5f);

    protected BoxCollider _collider;

    protected RectTransform _rectTransform;
    protected bool _isHovered;

    public bool isEnabled = true;

    public abstract void SetState(bool state);

    public abstract void OnPointerEnter();

    public abstract void OnPointerLeave();

    public abstract void OnClick(Vector3 hitPoint);

    public abstract void OnClickUp();
}
