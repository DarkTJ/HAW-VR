using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TutorialController))]
public class StartManager : MonoBehaviour
{
    [Header("Debug")] 
    [SerializeField] 
    private bool _forceTutorial = false;
    
    [Header("Will be overriden at runtime.")]
    [SerializeField] 
    private SystemLanguage _language;
    
    private ScreenFader _screenFade;
    
    private TutorialController _tutorial;

    private static bool _hasAppJustStarted = true;
    
    private void Awake()
    {
        _tutorial = GetComponent<TutorialController>();
        
#if !UNITY_EDITOR
        _language = Application.systemLanguage;
#endif
    }
    
    private IEnumerator Start()
    {
        if (!_hasAppJustStarted)
        {
            gameObject.SetActive(false);
            yield break;
        }
        
        _screenFade = SceneReferences.ScreenFader;
        _screenFade.SetAlpha(1);
        
        yield return new WaitUntil(() => MultiplayerSceneSetupController.Instance.IsReady);
        _tutorial.SetControllers(false);

        string localizedTextFileName;
        
        switch (Application.systemLanguage) {
            case SystemLanguage.German:
                localizedTextFileName = "localizedText_de.json";
                break;
            default:
                localizedTextFileName = "localizedText_en.json";
                break;
        }

        LocalizationManager.Instance.LoadLocalizedTextFile(localizedTextFileName);
        
        // Enable fading from now on
        _screenFade.SetFadeOnStart(true);

        // Loading
        while (!LocalizationManager.Instance.IsReady) {
            yield return null;
        }
        
        if (PlayerPrefs.HasKey("username") && !_forceTutorial)
        {
            _screenFade.FadeIn(() =>
            {
                _tutorial.SetControllers(true);
                gameObject.SetActive(false);
            });
        }
        else
        {
            _tutorial.SetupAndStart();
        }
        
        _hasAppJustStarted = false;
    }
}
