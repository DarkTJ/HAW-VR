﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SculpturePiece : MonoBehaviour
{
    private MeshRenderer _renderer;
    
    private MeshRenderer _glowRenderer;
    private Material _glowMaterial;
    private float _defaultIntensity, _interactingIntensity;
    private Vector2 _initialSize, _defaultSize, _interactingSize;
    
    private static readonly int ScaleXProperty = Shader.PropertyToID("_ScaleX");
    private static readonly int ScaleYProperty = Shader.PropertyToID("_ScaleY");
    private static readonly int IntensityProperty = Shader.PropertyToID("_Intensity");

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        
        // [1] -> Ignore the object itself
        _glowRenderer = GetComponentsInChildren<MeshRenderer>()[1];
        _glowMaterial = _glowRenderer.material;
    }

    public void Setup(float defaultIntensity, float interactingIntensity, float defaultScaleMultiplier, float interactingScaleMultiplier)
    {
        _defaultIntensity = defaultIntensity;
        _interactingIntensity = interactingIntensity;
        _glowMaterial.SetFloat(IntensityProperty, _defaultIntensity);
        
        Vector3 meshSize = _renderer.bounds.size;
        _initialSize = new Vector2(meshSize.x, meshSize.y);
        _defaultSize = _initialSize * defaultScaleMultiplier;
        _interactingSize = _initialSize * interactingScaleMultiplier;
        _glowMaterial.SetFloat(ScaleXProperty, _defaultSize.x);
        _glowMaterial.SetFloat(ScaleYProperty, _defaultSize.y);
    }


    // DEBUG, ANIMATION PREVIEW
    // private float x;
    // private bool b;
    // private void Update()
    // {
    //     if (!b)
    //     {
    //         x += Time.deltaTime;
    //         if (x > 2)
    //         {
    //             StopAllCoroutines();
    //             StartCoroutine(C_FadeIntensity(_interactingIntensity));
    //             StartCoroutine(C_FadeScale(_interactingSize));
    //             b = !b;
    //             x = 0;
    //         }
    //     }
    //     else
    //     {
    //         x += Time.deltaTime;
    //         if (x > 2)
    //         {
    //             StopAllCoroutines();
    //             StartCoroutine(C_FadeIntensity(_defaultIntensity));
    //             StartCoroutine(C_FadeScale(_defaultSize));
    //             b = !b;
    //             x = 0;
    //         }
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        StopAllCoroutines();
        StartCoroutine(C_FadeIntensity(_interactingIntensity));
        StartCoroutine(C_FadeScale(_interactingSize));
    }

    private void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
        StartCoroutine(C_FadeIntensity(_defaultIntensity));
        StartCoroutine(C_FadeScale(_defaultSize));
    }

    private IEnumerator C_FadeIntensity(float target)
    {
        float start = _glowMaterial.GetFloat(IntensityProperty);
        
        float t = 0;
        while (t < 1)
        {
            _glowMaterial.SetFloat(IntensityProperty, Mathf.Lerp(start, target, t));
            
            t += Time.deltaTime;
            yield return null;
        }

        _glowMaterial.SetFloat(IntensityProperty, target);

    }
    
    private IEnumerator C_FadeScale(Vector2 target)
    {
        Vector2 start = new Vector2(_glowMaterial.GetFloat(ScaleXProperty), _glowMaterial.GetFloat(ScaleYProperty));

        float t = 0;
        while (t < 1)
        {
            Vector2 v = Vector2.Lerp(start, target, t);
            _glowMaterial.SetFloat(ScaleXProperty, v.x);
            _glowMaterial.SetFloat(ScaleYProperty, v.y);
            
            t += Time.deltaTime;
            yield return null;
        }
        
        _glowMaterial.SetFloat(ScaleXProperty, target.x);
        _glowMaterial.SetFloat(ScaleYProperty, target.y);
    }
}
