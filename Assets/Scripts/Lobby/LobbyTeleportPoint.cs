using UnityEngine;

public class LobbyTeleportPoint : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        SceneLoader.LoadTargetScene();
    }
}
