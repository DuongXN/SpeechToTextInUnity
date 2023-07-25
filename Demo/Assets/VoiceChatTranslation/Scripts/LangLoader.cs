using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LangLoader : MonoBehaviour
{
    [SerializeField] string defaultLanguage = "English";
    public TextAsset langInfoTextAsset;
    public Dictionary<string, LangInfo> dicLangs;
    string prefName = "Language";
    [SerializeField]
    LangInfo langInfoUsed;
    [SerializeField]
    LangInfo langInfoSelecting;
    public LangInfo LangInfoUsed => langInfoUsed;
    public LangInfo LangInfoSelecting => langInfoSelecting;

    void Awake()
    {
        LoadLang();
        langInfoUsed = GetLangByPref();
        if (langInfoUsed == null && dicLangs != null)
        {
            langInfoUsed = (defaultLanguage==null || defaultLanguage==string.Empty) ? dicLangs["日本"] : dicLangs[defaultLanguage];
        }

        ApplySpeechCodeToRecognition();
    }

    void LoadLang()
    {
        dicLangs = new Dictionary<string, LangInfo>();
        string txt = langInfoTextAsset.text;
        string[] langs = txt.Split(',');
        for (int i = 1; i < langs.Length; i++)
        {
            string l = langs[i];
            string[] langComponents = l.Split(';');
            if (langComponents != null && langComponents.Length == 3)
            {
                LangInfo lc = new LangInfo();
                lc.langName = langComponents[0].Trim();
                lc.speechCode = langComponents[1].Trim();
                lc.transCode = langComponents[2].Trim();
                dicLangs.Add(lc.langName, lc);
            }
        }
    }

    public LangInfo SelectNewLang(string lang)
    {
        if (dicLangs == null) return null;
        if (langInfoUsed != null)
        {
            if (langInfoUsed.langName == lang)
            {
                return null;
            }
            if (dicLangs.ContainsKey(lang))
            {
                return dicLangs[lang];
            }
        }
        return null;
    }

    public void UpdateLangInfoSelecting(LangInfo _langInfo)
    {
        langInfoSelecting = _langInfo;
    }

    public void SaveSelectedOption()
    {
        if (langInfoSelecting != null)
        {
            langInfoUsed = langInfoSelecting;
            SaveToPref(langInfoUsed.langName);
            ApplySpeechCodeToRecognition();

            langInfoSelecting = null;
        }
    }

    void SaveToPref(string lang)
    {
        PlayerPrefs.SetString(prefName, lang.ToString());
        PlayerPrefs.Save();
    }

    LangInfo GetLangByPref()
    {
        if (dicLangs == null) return null;
        string sLang = PlayerPrefs.GetString(prefName);
        if (dicLangs.ContainsKey(sLang))
            return dicLangs[sLang];
        return null;
    }

    void ApplySpeechCodeToRecognition()
    {
        GCTranslateManager.Instance.speechController.SetLanguage(langInfoUsed.speechCode);
    }
}

[Serializable]
public class LangInfo
{
    public string langName;
    public string speechCode;
    public string transCode;
}