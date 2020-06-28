using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLobbyButton : UIButton
{
    public override void OnClick(Vector3 hitPoint)
    {
        base.OnClick(hitPoint);
        FMODEventManager.Instance.PlaySound_BackButton();
        SceneLoader.Instance.LoadScene(0);
    }
}
