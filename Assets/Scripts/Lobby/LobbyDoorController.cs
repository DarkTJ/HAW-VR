using UnityEngine;

public class LobbyDoorController : MonoBehaviour
{
    private GameObject _lastDoor;
    private GameObject _lastTeleportPoint;
    
    private LayerMask _layerMask;

    private void Awake()
    {
        _layerMask = LayerMask.GetMask("LobbyDoor");
    }

    public void OpenDoor(float lookYRotation)
    {
        Vector3 forward = Quaternion.Euler(0, lookYRotation, 0) * Vector3.forward;
        Ray ray = new Ray(Vector3.zero, forward);
        
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, 100, _layerMask))
        {
            return;
        }

        if (_lastDoor)
        {
            _lastDoor.SetActive(true);
            _lastTeleportPoint.SetActive(false);
            _lastTeleportPoint.transform.parent = _lastDoor.transform;
        }

        _lastDoor = hitInfo.collider.gameObject;
        _lastTeleportPoint = _lastDoor.GetComponentInChildren<LobbyTeleportPoint>(true).gameObject;
        
        _lastTeleportPoint.transform.parent = null;
        _lastTeleportPoint.SetActive(true);
        _lastDoor.SetActive(false);
    }
}
