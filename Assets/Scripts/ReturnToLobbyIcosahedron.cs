using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToLobbyIcosahedron : MonoBehaviour
{
    [SerializeField]
    private float _defaultGlowIntensity = 1,
        _interactingGlowIntensity = 2,
        _defaultGlowScaleMultiplier = 2f,
        _interactingGlowScaleMultiplier = 4f,
        _rotationSpeed = 0.5f;
    
    [SerializeField]
    private Color _glowColor = Color.white;
    
    private MeshRenderer _renderer;
    
    private MeshRenderer _glowRenderer;
    private Material _glowMaterial;
    private Vector2 _initialSize, _defaultSize, _interactingSize;
    private Vector3 _rotation;

    private static readonly int ScaleXProperty = Shader.PropertyToID("_ScaleX");
    private static readonly int ScaleYProperty = Shader.PropertyToID("_ScaleY");
    private static readonly int IntensityProperty = Shader.PropertyToID("_Intensity");

    /// <summary>
    /// Bool shared within the class to verify that the player only interacts with one piece at a time.
    /// </summary>
    private static bool _isAnyPieceInteractedWith;
    
    /// <summary>
    /// Bool to verify that the player interacts with this piece.
    /// Can only be true if _isAnyPieceInteractedWith was false before.
    /// </summary>
    private bool _isThisPieceInteractedWith;
    
    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        
        // [1] -> Ignore this object, [0] would be equal to  GetComponent<MeshRenderer>()
        _glowRenderer = GetComponentsInChildren<MeshRenderer>()[1];
        _glowMaterial = _glowRenderer.material;
        _glowMaterial.color = _glowColor;
        
        _glowMaterial.SetFloat(IntensityProperty, _defaultGlowIntensity);
        
        Vector3 meshSize = _renderer.bounds.size;
        _initialSize = new Vector2(meshSize.x, meshSize.y);
        _defaultSize = _initialSize * _defaultGlowScaleMultiplier;
        _interactingSize = _initialSize * _interactingGlowScaleMultiplier;
        _glowMaterial.SetFloat(ScaleXProperty, _defaultSize.x);
        _glowMaterial.SetFloat(ScaleYProperty, _defaultSize.y);
        
        float x = Random.Range(-_rotationSpeed, _rotationSpeed);
        float y = Random.Range(-_rotationSpeed, _rotationSpeed);
        float z = Random.Range(-_rotationSpeed, _rotationSpeed);
        _rotation = new Vector3(x, y, z);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        InputManager.Instance.OnMainButtonDown += OnTriggerDown;
#elif UNITY_ANDROID
        InputManager.Instance.CurrentlyUsedController.OnTriggerDown += OnTriggerDown;
#endif
    }
    
    private void Update()
    {
        transform.Rotate(_rotation);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Only allow interaction if the player is not interacting with any other piece already
        if (_isAnyPieceInteractedWith)
        {
            return;
        }
        
        _isAnyPieceInteractedWith = true;
        _isThisPieceInteractedWith = true;

        StopAllCoroutines();
        StartCoroutine(C_FadeIntensity(_interactingGlowIntensity));
        StartCoroutine(C_FadeScale(_interactingSize));
    }

    private void OnTriggerExit(Collider other)
    {
        // Can only be true if this piece was the only one the player interacted with
        if (!_isThisPieceInteractedWith)
        {
            return;
        }
        
        _isAnyPieceInteractedWith = false;
        _isThisPieceInteractedWith = false;
        
        StopAllCoroutines();
        StartCoroutine(C_FadeIntensity(_defaultGlowIntensity));
        StartCoroutine(C_FadeScale(_defaultSize));
    }

    /// <summary>
    /// Is called when the controller trigger is down. Do not confuse with unity event functions above.
    /// </summary>
    private void OnTriggerDown()
    {
        if (!_isThisPieceInteractedWith)
        {
            return;
        }
        
        _isAnyPieceInteractedWith = false;
        
        SceneLoader.LoadScene(this, 0);
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