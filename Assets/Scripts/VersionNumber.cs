using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VersionNumber : MonoBehaviour
{
    private Text _text;
    public string conMessage;

    private void Start()
    {
        _text = GetComponent<Text>();
        SetText();
        SaveGameManager.instance.onSave += SetText;
    }

    public void SetText()
    {
        _text.text = "v20240707.1";
    }

    private void OnDestroy()
    {
        if (SaveGameManager.instance) { SaveGameManager.instance.onSave -= SetText; }
    }
}
