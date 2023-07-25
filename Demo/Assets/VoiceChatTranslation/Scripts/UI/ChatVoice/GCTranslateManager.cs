using System;
using System.Collections.Generic;
using System.Linq;
using FrostweepGames.Plugins.GoogleCloud.Translation;
using UnityEngine;

public class GCTranslateManager : MonoBehaviourSingleton<GCTranslateManager>
{
    [SerializeField]
    private LangLoader langLoader;

    public SpeechController speechController;
    public TranslationController translationController;
    public LangLoader LangLoader => langLoader;

}