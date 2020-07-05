using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR;

public class AvatarController : MonoBehaviour
{
    private Renderer[] _renderers;
    private Graphic[] _graphics;
    private TextMeshProUGUI _nameField;
    
    [Header("Face Textures")] 
    
    [SerializeField]
    [Range(24, 120)]
    private float _animationFps;
    private float _animationWaitTime;

    [SerializeField]
    private Texture2D _idleTexture, _happyTexture;
    
    [FormerlySerializedAs("_idleEyeClosingTextures")] [SerializeField]
    private Texture2D[] _idleBlinkingTextures;

    [SerializeField]
    private Texture2D[] _happyTransitionTextures;

    [SerializeField]
    private Renderer _faceRenderer;

    [SerializeField]
    [Tooltip("How often in a minute is the avatar supposed to blink? The range 10 to 15 is human average.")]
    private Vector2 _idleBlinkingFrequencyRange = new Vector2(10, 15);
    private Vector2 _idleBlinkingWaitRange;

    private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");

    private void Awake()
    {
        _animationWaitTime = 1f / _animationFps;
        _idleBlinkingWaitRange = new Vector2(60f / _idleBlinkingFrequencyRange.x, 60f / _idleBlinkingFrequencyRange.y);

        _renderers = GetComponentsInChildren<Renderer>();
        _graphics = GetComponentsInChildren<Graphic>();
        _nameField = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        IdleFace();
        
        if (PlayerPrefs.HasKey("username"))
        {
            SetName(PlayerPrefs.GetString("username"));
        }
        else
        {
            StartCoroutine(C_WaitForUsername());
        }
    }

    private IEnumerator C_WaitForUsername()
    {
        yield return new WaitUntil(() => PlayerPrefs.HasKey("username"));
        SetName(PlayerPrefs.GetString("username"));
    }

    private void SetName(string name)
    {
        _nameField.text = name;
    }

    public void SetRenderers(bool state)
    {
        foreach (Renderer r in _renderers)
        {
            r.enabled = state;
        }
        
        foreach (Graphic g in _graphics)
        {
            g.enabled = state;
        }

        _nameField.enabled = state;
    }
    
    public void IdleFace()
    {
        StopAllCoroutines();
        StartCoroutine(C_IdleFaceAnimation());
    }
    
    private IEnumerator C_IdleFaceAnimation()
    {
        while (true)
        {
            _faceRenderer.material.SetTexture(EmissionMap, _idleTexture);

            float randomWaitTime = Random.Range(_idleBlinkingWaitRange.x, _idleBlinkingWaitRange.y);
            yield return new WaitForSeconds(randomWaitTime);
            
            for (int i = 0; i < _idleBlinkingTextures.Length; i++)
            {
                _faceRenderer.material.SetTexture(EmissionMap, _idleBlinkingTextures[i]);
                yield return new WaitForSeconds(_animationWaitTime);

            }
            
            // Do not show last texture of _idleEyeClosingTextures twice, therefore Length - 2
            for (int i = _idleBlinkingTextures.Length - 2; i >= 0; i--)
            {
                _faceRenderer.material.SetTexture(EmissionMap, _idleBlinkingTextures[i]);
                yield return new WaitForSeconds(_animationWaitTime);
            }
        }
    }

    /// <summary>
    /// Lets the avatar look happy for an unlimited duration.
    /// </summary>
    public void HappyFace()
    {
        StopAllCoroutines();
        StartCoroutine(C_HappyFaceAnimation());
    }
    
    private IEnumerator C_HappyFaceAnimation()
    {
        while (true)
        {
            for (int i = 0; i < _happyTransitionTextures.Length; i++)
            {
                _faceRenderer.material.SetTexture(EmissionMap, _happyTransitionTextures[i]);
                yield return new WaitForSeconds(_animationWaitTime);
            }
        
            _faceRenderer.material.SetTexture(EmissionMap, _happyTexture);
        }
    }
    
    /// <summary>
    /// Lets the avatar look happy for a defined duration.
    /// </summary>
    public void HappyFace(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(C_HappyFaceAnimation(duration));
    }
    
    private IEnumerator C_HappyFaceAnimation(float duration)
    {
        for (int i = 0; i < _happyTransitionTextures.Length; i++)
        {
            _faceRenderer.material.SetTexture(EmissionMap, _happyTransitionTextures[i]);
            yield return new WaitForSeconds(_animationWaitTime);
        }
        
        _faceRenderer.material.SetTexture(EmissionMap, _happyTexture);
        yield return new WaitForSeconds(duration);
        
        for (int i = _happyTransitionTextures.Length - 1; i >= 0; i--)
        {
            _faceRenderer.material.SetTexture(EmissionMap, _happyTransitionTextures[i]);
            yield return new WaitForSeconds(_animationWaitTime);
        }
        
        IdleFace();
    }
}
