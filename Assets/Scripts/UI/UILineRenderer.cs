using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class UILineRenderer : MonoBehaviour
{
    [SerializeField]
    private float _defaultLength = 0.5f;
    
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

#elif UNITY_ANDROID
    private LineRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        transform.position = InputManager.Instance.CurrentlyUsedController.Position;
        transform.rotation = InputManager.Instance.CurrentlyUsedController.Rotation;
    }

    public void Enable()
    {
        _renderer.enabled = true;
        enabled = true;
    }

    public void Disable()
    {
        _renderer.enabled = false;
        enabled = false;
    }

    public void SetTarget(Vector3 targetWorldPos)
    {
        Vector3 start = transform.position;
        Vector3 dir = targetWorldPos - start;

        float step = 1f / (_renderer.positionCount - 1);
        for (int i = 1; i < _renderer.positionCount; i++)
        {
            Vector3 target = start + ((i * step) * dir);
            Vector3 targetLocalPos = transform.InverseTransformPoint(target);
            _renderer.SetPosition(i, targetLocalPos);
        }   
    }

    public void ClearTarget()
    {
        // Calculating in local space
        Vector3 start = Vector3.zero;
        Vector3 dir = Vector3.forward * _defaultLength;

        float step = 1f / (_renderer.positionCount - 1);
        for (int i = 0; i < _renderer.positionCount; i++)
        {
            Vector3 target = start + ((i * step) * dir);
            _renderer.SetPosition(i, target);
        }
    }
#endif
}
