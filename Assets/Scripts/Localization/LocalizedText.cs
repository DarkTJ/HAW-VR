using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour {

	[SerializeField]
	private string _key;
	private TextMeshProUGUI _text;
	
	private void Awake() {
		_text = GetComponent<TextMeshProUGUI>();
	}

	private IEnumerator Start() {
		yield return new WaitUntil(() => LocalizationManager.Instance.IsReady);
		LoadText();
	}

	public void LoadText() {
		_text.text = LocalizationManager.Instance.GetLocalizedText(_key);
	}

	public void LoadText(string key) {
		_key = key;
		LoadText();
	}

	public void Empty() {
		_text.text = "";
	}
}