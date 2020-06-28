using UnityEngine;

public class TextSubmitButton : UIButton
{
    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = UIManager.Instance;
    }

    public override void OnClick(Vector3 hitPoint)
    {
        base.OnClick(hitPoint);
        FMODEventManager.Instance.PlaySound_TriggerPressOK();
        _uiManager.SubmitText();
    }
}
