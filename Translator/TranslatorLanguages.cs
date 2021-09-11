using System;
using System.Collections.Generic;
using UnityEngine;
using PL = DiscoTranslatorFinalCut.PluginLoader;

namespace DiscoTranslatorFinalCut.Translator
{
    class TranslatorLanguages : MonoBehaviour
    {
        public static GameObject obj = null;
        static Dictionary<string, Dictionary<string, string>> DiscoLanguages;

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);

            new TranslatorLanguages(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<TranslatorLanguages>()).Pointer);

            return obj;
        }

        public TranslatorLanguages(IntPtr ptr) : base(ptr) { }

        public void Awake() {
            PopulateDiscoLanguages();
        }

        static void PopulateDiscoLanguages()
        {
            DiscoLanguages = new Dictionary<string, Dictionary<string, string>>();
            DiscoLanguages["English"] = new Dictionary<string, string>();
            DiscoLanguages["English"]["DISCOFC_HOTKEYS_MESSAGE"] = "HotKeys: (Shortcut works only on Main Menu except for the UI toggle key U)";
            DiscoLanguages["English"]["DISCOFC_SHOW_HIDE_UI"] = "Toggle Plugin UI";
            DiscoLanguages["English"]["DISCOFC_EXPORT_CATALOG"] = "Export Language Catalog";
            DiscoLanguages["English"]["DISCOFC_EXPORT_LOADED_IMAGES"] = "Export All Loaded Images";
            DiscoLanguages["English"]["DISCOFC_EXPORT_LOADED_AUDIO"] = "Export All Loaded Audio Files";
            DiscoLanguages["English"]["DISCOFC_RELOAD_ALL_TEXT"] = "Reload All Text (Qx2)";
            DiscoLanguages["English"]["DISCOFC_RELOAD_ALL_TEXT_LONG"] = "Reload All Text (switch x2 the language to see the change)";
            DiscoLanguages["English"]["DISCOFC_TOGGLE_INTRO_MUSIC"] = "Toggle intro Music";
            DiscoLanguages["English"]["DISCOFC_CLOSE_UI_FROM_UI"] = "Close";
            DiscoLanguages["English"]["DISCOFC_OPTIONS_TOGGLE_TRANSLATOR"] = "Translator";
            DiscoLanguages["English"]["DISCOFC_OPTIONS_TOGGLE_AUDIO_REPLACE"] = "Audio Replacer";
            DiscoLanguages["English"]["DISCOFC_OPTIONS_TOGGLE_IMAGES_REPLACE"] = "Images Replacer";
            DiscoLanguages["English"]["DISCOFC_OPTIONS_TOGGLE_AUDIO_WIDGET"] = "Audio Widget";
            DiscoLanguages["English"]["DISCOFC_WELCOME_STATUS_MESSAGE"] = "Welcome in Disco Translator Final Cut v1.0";
            DiscoLanguages["English"]["DISCOFC_BUTTON_PLAY_STATUS_MESSAGE"] = "Play Active Audio";
            DiscoLanguages["English"]["DISCOFC_BUTTON_STOP_STATUS_MESSAGE"] = "Stop Active Audio";
            DiscoLanguages["English"]["DISCOFC_BUTTON_PAUSE_STATUS_MESSAGE"] = "Stop Active Audio";
            DiscoLanguages["English"]["DISCOFC_BUTTON_VOLUME_STATUS_MESSAGE"] = "Active Audio Volume";

            DiscoLanguages["Italian"] = new Dictionary<string, string>();
            DiscoLanguages["Italian"]["DISCOFC_HOTKEYS_MESSAGE"] = "Scorciatoie: (Le Shortcut funzionano solo nel menu principale eccetto per il tasto U per l'UI della mod).";
            DiscoLanguages["Italian"]["DISCOFC_SHOW_HIDE_UI"] = "Mostra/Nascondi l'UI della mod";
            DiscoLanguages["Italian"]["DISCOFC_EXPORT_CATALOG"] = "Esporta catalogo lingue";
            DiscoLanguages["Italian"]["DISCOFC_EXPORT_LOADED_IMAGES"] = "Esporta immagini caricate";
            DiscoLanguages["Italian"]["DISCOFC_EXPORT_LOADED_AUDIO"] = "Esporta audio caricati";
            DiscoLanguages["Italian"]["DISCOFC_RELOAD_ALL_TEXT"] = "Ricarica testi (Qx2)";
            DiscoLanguages["Italian"]["DISCOFC_RELOAD_ALL_TEXT_LONG"] = "Ricarica testi (Premi Q due volte per vedere i cambiamenti)";
            DiscoLanguages["Italian"]["DISCOFC_TOGGLE_INTRO_MUSIC"] = "Stop/Play musica intro";
            DiscoLanguages["Italian"]["DISCOFC_CLOSE_UI_FROM_UI"] = "Chiudi";
            DiscoLanguages["Italian"]["DISCOFC_OPTIONS_TOGGLE_TRANSLATOR"] = "Traduttore";
            DiscoLanguages["Italian"]["DISCOFC_OPTIONS_TOGGLE_AUDIO_REPLACE"] = "Importa Audio";
            DiscoLanguages["Italian"]["DISCOFC_OPTIONS_TOGGLE_IMAGES_REPLACE"] = "Importa Img";
            DiscoLanguages["Italian"]["DISCOFC_OPTIONS_TOGGLE_AUDIO_WIDGET"] = "Widget Audio";
            DiscoLanguages["Italian"]["DISCOFC_WELCOME_STATUS_MESSAGE"] = "Benvenuto in Disco Translator Final Cut v1.0";
            DiscoLanguages["Italian"]["DISCOFC_BUTTON_PLAY_STATUS_MESSAGE"] = "Play Musica attiva";
            DiscoLanguages["Italian"]["DISCOFC_BUTTON_STOP_STATUS_MESSAGE"] = "Ferma Musica";
            DiscoLanguages["Italian"]["DISCOFC_BUTTON_PAUSE_STATUS_MESSAGE"] = "Metti in pausa";
            DiscoLanguages["Italian"]["DISCOFC_BUTTON_VOLUME_STATUS_MESSAGE"] = "Modifica Volume";
        }

        public static string GetTerm(string key)
        {
            var MyLanguage = TranslatorManager.currentLanguage;

            if(!DiscoLanguages.ContainsKey(MyLanguage)) MyLanguage = "English";
            if (!DiscoLanguages[MyLanguage].ContainsKey(key))
            {
                PL.log.LogWarning(PL.PREFIX + "Language code key : " + key + " not found in the Mod Language Dictionary!");
                return "LANGUAGE_CODE_MISSING";
            }
            return DiscoLanguages[MyLanguage][key];
        }
    }
}
