using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
	[SerializeField]
	private float _fadeDuration = 1.0f;

	[SerializeField]
	private Color _fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);

	[SerializeField]
	private bool _fadeOnStart = true;

	private static bool _fadeOnStartStatic;
	
	[SerializeField]
	private int _renderQueue = 5000;
	
	private MeshRenderer _fadeRenderer;
	private MeshFilter _fadeMesh;
	private Material _fadeMaterial;
    private bool _isFading;

    public float CurrentAlpha { get; private set; }

    private void Start()
	{
		// create the fade material
		_fadeMaterial = new Material(Shader.Find("Oculus/Unlit Transparent Color"));
		_fadeMesh = gameObject.AddComponent<MeshFilter>();
		_fadeRenderer = gameObject.AddComponent<MeshRenderer>();

		var mesh = new Mesh();
		_fadeMesh.mesh = mesh;

		Vector3[] vertices = new Vector3[4];

		float width = 2f;
		float height = 2f;
		float depth = 1f;

		vertices[0] = new Vector3(-width, -height, depth);
		vertices[1] = new Vector3(width, -height, depth);
		vertices[2] = new Vector3(-width, height, depth);
		vertices[3] = new Vector3(width, height, depth);

		mesh.vertices = vertices;

		int[] tri = new int[6];

		tri[0] = 0;
		tri[1] = 2;
		tri[2] = 1;

		tri[3] = 2;
		tri[4] = 3;
		tri[5] = 1;

		mesh.triangles = tri;

		Vector3[] normals = new Vector3[4];

		normals[0] = -Vector3.forward;
		normals[1] = -Vector3.forward;
		normals[2] = -Vector3.forward;
		normals[3] = -Vector3.forward;

		mesh.normals = normals;

		Vector2[] uv = new Vector2[4];

		uv[0] = new Vector2(0, 0);
		uv[1] = new Vector2(1, 0);
		uv[2] = new Vector2(0, 1);
		uv[3] = new Vector2(1, 1);

		mesh.uv = uv;
		
		_fadeOnStartStatic = _fadeOnStart;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public void SetFadeOnStart(bool state)
	{
		_fadeOnStart = state;
		_fadeOnStartStatic = state;
	}

	
	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		Debug.Log("SCREEN FADER SCENE LOADED ############################");
		Debug.Log("SCREEN FADER " + _fadeOnStartStatic + " ############################");
		if (_fadeOnStartStatic)
		{
			Debug.Log("SCREEN FADER FADE IN ############################");
			FadeIn();
		}
	}

	/// <summary>
	/// Cleans up the fade material
	/// </summary>
	private void OnDestroy()
	{
		if (_fadeRenderer != null)
			Destroy(_fadeRenderer);

		if (_fadeMaterial != null)
			Destroy(_fadeMaterial);

		if (_fadeMesh != null)
			Destroy(_fadeMesh);
	}
	
    public void SetAlpha(float alpha)
    {
        CurrentAlpha = alpha;
        SetMaterialAlpha();
    }
    
    public void FadeIn(float duration = -1)
    {
	    FadeIn(null, duration);
    }
	
    public void FadeIn(Action callback, float duration = -1)
    {
	    StartCoroutine(C_Fade(1, 0, duration, callback));
    }
	
    public void FadeOut(float duration = -1)
    {
	    FadeOut(null, duration);
    }
	
    public void FadeOut(Action callback, float duration = -1)
    {
	    StartCoroutine(C_Fade(0, 1, duration, callback));
    }

    private IEnumerator C_Fade(float startAlpha, float endAlpha, float duration = -1, Action callback = null)
    {
	    if (duration == -1)
	    {
		    duration = _fadeDuration;
	    }
	    
	    float t = 0;
	    while (t < 1)
	    {
		    CurrentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
		    SetMaterialAlpha();

		    t += Time.deltaTime / duration;
		    yield return null;
	    }

	    CurrentAlpha = endAlpha;
	    SetMaterialAlpha();
	    
	    callback?.Invoke();
    }
    
    private void SetMaterialAlpha()
    {
		Color color = _fadeColor;
		color.a = CurrentAlpha;
		_isFading = color.a > 0;
        if (_fadeMaterial != null)
        {
            _fadeMaterial.color = color;
			_fadeMaterial.renderQueue = _renderQueue;
			_fadeRenderer.material = _fadeMaterial;
			_fadeRenderer.enabled = _isFading;
        }
    }
}
