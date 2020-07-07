using System;
using UnityEngine;

public class BackToLobbyButton : UIButton
{
    private UIManager _uiManager;
    
    private void Start()
    {
        _uiManager = GetComponentInParent<UIManager>();
    }
    
    public override void OnClick(Vector3 hitPoint)
    {
        base.OnClick(hitPoint);
        FMODEventManager.Instance.PlaySound_BackButton();
        
        _uiManager.OnBackToLobbyButton();
        SceneLoader.LoadScene(0);
    }

    public override void OnPointerEnter()
    {
        base.OnPointerEnter();
        FMODEventManager.Instance.PlaySound_ButtonHover();
    }
}
