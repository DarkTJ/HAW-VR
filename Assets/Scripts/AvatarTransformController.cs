using System.Collections;
using UnityEngine;

public class AvatarTransformController : MonoBehaviour
{
    private Transform _avatarTransform, _copyTransform;
    
    public void SetAvatarTransform(Transform avatar)
    {
        StopAllCoroutines();
        _avatarTransform = avatar;
        StartCoroutine(C_CopyTransform());
    }

    private IEnumerator C_CopyTransform()
    {
        _copyTransform = SceneReferences.PlayerCamera.transform;
        while (true)
        {
            _avatarTransform.position = _copyTransform.position;
            _avatarTransform.rotation = Quaternion.Euler(0, _copyTransform.rotation.eulerAngles.y, 0);
            yield return null;
        }
    }
}
