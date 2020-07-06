using UnityEngine;

public class BackToLobbyButton : UIButton
{
    public override void OnClick(Vector3 hitPoint)
    {
        base.OnClick(hitPoint);
        FMODEventManager.Instance.PlaySound_BackButton();
        
        UIManager.Instance.OnBackToLobbyButton();
        SceneLoader.LoadScene(0);
    }
}
