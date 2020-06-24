using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SculptureController : MonoBehaviour
{
    [Header("Sculpture Piece Settings")]
    private SculpturePiece[] _pieces;

    [SerializeField] private float _defaultGlowIntensity = 1,
        _interactingGlowIntensity = 2,
        _defaultGlowScaleMultiplier = 2f,
        _interactingGlowScaleMultiplier = 4f;

    private void Awake()
    {
        _pieces = GetComponentsInChildren<SculpturePiece>();
    }

    private void Start()
    {
        foreach (SculpturePiece piece in _pieces)
        {
            piece.Setup(_defaultGlowIntensity, 
                _interactingGlowIntensity, 
                _defaultGlowScaleMultiplier, 
                _interactingGlowScaleMultiplier);
        }
    }
}
