using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : UISlider
{
    private bool _isDragging;

    [SerializeField]
    private float _width;

    public override void OnDrag(Vector3 hitPoint)
    {
        Vector3 localHitPoint = transform.InverseTransformPoint(hitPoint);
        float hitX = localHitPoint.x;
        float hitDistance = hitX - (-_width / 2);
        float pos = hitDistance / _width;
        _slider.value = pos;

        FMODUnity.RuntimeManager.GetBus("bus:/master").setVolume(pos);
    }
}
