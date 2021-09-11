using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using DiscoTranslatorFinalCut.Translator.Ext;
using DiscoTranslatorFinalCut.Translator.Images;
using DiscoTranslatorFinalCut.Translator.Audio;
using DiscoTranslatorFinalCut.Translator.Exporter;
using DiscoTranslatorFinalCut.Translator.UI;
using DiscoTranslatorFinalCut.Translator.Utility;
using PL = DiscoTranslatorFinalCut.PluginLoader;
using Lang = DiscoTranslatorFinalCut.Translator.TranslatorLanguages;
using LocalizationCustomSystem;

namespace DiscoTranslatorFinalCut.Translator
{
    class TranslatorManager : MonoBehaviour
    {
        #region[Declarations]
        #region[DiscoTranslatorFinalCut]

        // DiscoTranslatorFinalCut Base
        public static GameObject obj = null;
        public static TranslatorManager instance;
        private static bool initialized = false;
        //private bool Executed = false;
        public static string currentLanguage = "";
        public static string currentLanguageCode = "";
        public static string currentLanguagePath;
        public static int currentLanguageIndex = 0;
        public static Dictionary<string, string> LanguageFolders = new Dictionary<string, string>();
        public static bool isCustomLanguage = false;

        public static bool EnableTranslation { get; set; } = true;

        #endregion
        #endregion

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);

            new TranslatorManager(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<TranslatorManager>()).Pointer);

            return obj;
        }

        public TranslatorManager(IntPtr ptr) : base(ptr)
        {
            instance = this;
        }

        private void Initialize()
        {
            initialized = true;
        }

        public void Awake()
        {
            Lang.Create("MODTranslatorLanguagesGO");

            PL.LanguageSaved = PL.GetStrPrefs("LanguageSaved");
            if (PL.LanguageSaved == null) PL.LanguageSaved = PL.GetStrPrefs("I2 Language"); //Default Game PlayerPrefs
            PreloadTranslations();

            if (!EnableTranslation) return;
            if ((LocalizationManager.GetCurrentLanguageName() != PL.LanguageSaved) && PL.LanguageSaved != null && PL.LanguageSaved != "")
            {
                LocalizationManager.SetLanguage(PL.LanguageSaved);
                currentLanguage = LocalizationManager.GetCurrentLanguageName();
                currentLanguageCode = LocalizationManager.GetCurrentLanguageCode();
                LoadTranslation(currentLanguage);
            }
        }

        public void Update()
        {
            if (!EnableTranslation) return;

            if (!initialized) { Initialize(); }

            if (LocalizationManager.GetCurrentLanguageName() != currentLanguage)
            {
                currentLanguage = LocalizationManager.GetCurrentLanguageName();
                currentLanguageCode = LocalizationManager.GetCurrentLanguageCode();
                currentLanguageIndex = GetLanguageIndex(currentLanguage);

                LoadTranslation(currentLanguage);

                PL.log.LogInfo(PL.PREFIX + "currentLanguage : " + currentLanguage + " | Code : " + currentLanguageCode);
            }
        }

        public static string GetLanguageCode(string name)
        {
            return I2.Loc.LocalizationManager.GetLanguageCode(name);
        }

        public static void PreloadTranslations()
        {
            DirectoryInfo translations_dir = new DirectoryInfo(FoldersManager.GetPath("LTP"));

            if (!translations_dir.Exists)
            {
                PL.log.LogError(PL.PREFIX + "Can't find folder for translations!");
                return;
            }

            var EnabledLanguages = I2.Loc.LocalizationManager.GetAllLanguages();

            PL.log.LogInfo(PL.PREFIX + "Preload Translations Start...");
            var dirs = translations_dir.GetDirectories();
            foreach (var dir in dirs)
            {
                if (!dir.Exists) continue;

                var path = dir.FullName;
                PL.log.LogInfo(PL.PREFIX + "Custom Translation folder found... " + dir.Name);

                string[] dirParams = dir.Name.Split('_');
                if (dirParams.Length != 3)
                {
                    PL.log.LogError(PL.PREFIX + "Invalid Folder " + dir);
                    return;
                }

                foreach (var param in dirParams)
                {
                    if (param == null || param == "")
                    {
                        PL.log.LogError(PL.PREFIX + "Loading language from folder error : Invalid Param, language folder.");
                        return;
                    }
                }

                string TranslatedName = dirParams[0];
                string EnglishName = dirParams[1];
                string LanguageCode = dirParams[2];

                //PL.log.LogInfo("CreatingLanguage...");
                if (!EnabledLanguages.Contains(EnglishName))
                {
                    CreateLanguage(path, EnglishName, TranslatedName, LanguageCode);
                    PL.log.LogInfo(PL.PREFIX + "Creating Language : " + EnglishName + " with index = " + GetLanguageIndex(EnglishName).ToString());
                }
                else
                {
                    currentLanguageIndex = GetLanguageIndex(EnglishName);
                    PL.log.LogInfo(PL.PREFIX + "Language Already Exists : " + EnglishName + " with index = " + currentLanguageIndex);
                }
                LanguageFolders.Add(EnglishName, path);
            }

            ImageManager.LoadLanguageTextures();

            EnabledLanguages = I2.Loc.LocalizationManager.GetAllLanguages();

            PL.log.LogWarning(PL.PREFIX + "Languages list : ");
            string alllanguages = "";
            int nlang = 0;
            foreach (var language in EnabledLanguages)
            {
                nlang++;
                alllanguages += language;
                if (nlang < EnabledLanguages.Count) alllanguages += ", ";
            }
            PL.log.LogWarning(PL.PREFIX + alllanguages);
            PL.log.LogInfo(PL.PREFIX + "Preload Translations Done...");
        }

        public static void LoadTranslation(string currentLanguage)
        {
            if(EnableTranslation)
            {
                //ImageManager.TranslatedTextures(false);
                //ImageManager.SceneTextures(false);
                AudioManager.SceneClips(false);
            }

            isCustomLanguage = false;
            currentLanguagePath = null;
            RemoveAllSources();

            if (EnableTranslation && LanguageFolders.ContainsKey(currentLanguage))
            {
                LanguageFolders.TryGetValue(currentLanguage, out currentLanguagePath);

                if (currentLanguagePath == null)
                {
                    PL.log.LogError(PL.PREFIX + "currentLanguagePath is null");
                    return;
                }

                try
                {
                    if (Directory.GetFiles(currentLanguagePath, "*.po").Length > 0)
                    {
                        var CustomLanguageFiles = Directory.GetFiles(currentLanguagePath);

                        foreach (var filePath in CustomLanguageFiles)
                        {
                            if (filePath.Contains("LanguagesNames")) continue;
                            var extension = Path.GetExtension(filePath);

                            if (extension == ".po")
                            {
                                POTranslationSource source = new POTranslationSource(filePath, true);

                                if (Path.GetFileNameWithoutExtension(filePath).ToLower().Contains("dialog"))
                                {
                                    source.EnableStrNo = true;
                                    source.StrNoPrefix = "D";
                                }
                                source.Name = currentLanguage;
                                AddSource(source);

                                PL.log.LogInfo(PL.PREFIX + "file PO loaded: " + filePath + " for language " + source.Name);
                            }
                        }
                    }

                    isCustomLanguage = true;
                    LocalizationManager.SetLanguage(currentLanguage);
                    PlayerPrefs.SetString("LanguageSaved", currentLanguage);

                    if(ImageManager.EnableTextureImport)
                    {
                        //ImageManager.SceneTextures(true);
                        //ImageManager.TranslatedTextures(true);
                    }
                    
                    if(AudioManager.EnableAudioImport)
                    {
                        AudioManager.SceneClips(true);
                    }

                    PL.log.LogInfo(PL.PREFIX + "Language Creation Done...");
                    return;
                }
                catch (Exception e)
                {
                    PL.log.LogFatal(PL.PREFIX + " " + e.Message + " \r\n " + e.StackTrace);
                    return;
                }
            }
        }

        public static int GetLanguageIndex(string Language)
        {
            var LanguageAssets = Resources.FindObjectsOfTypeAll<I2.Loc.LanguageSourceAsset>();
            int index = 0;
            foreach (var LanguageAsset in LanguageAssets)
            {
                if (LanguageAsset.name == "LanguagesNamesLockit")
                {
                    index = LanguageAsset.SourceData.GetLanguageIndex(Language);
                    break;
                }
            }
            return index;
        }

        public static void CreateLanguage(string path, string EnglishName, string TranslatedName, string LanguageCode)
        {
            var LanguageAssets = Resources.FindObjectsOfTypeAll<I2.Loc.LanguageSourceAsset>();

            foreach (var LanguageAsset in LanguageAssets)
            {
                if (LanguageAsset.name == "LanguagesNamesLockit")
                {
                    if (LanguageAsset.SourceData.GetLanguageIndex(EnglishName) <= 0)
                    {
                        LanguageAsset.SourceData.AddLanguage(EnglishName, LanguageCode);
                        currentLanguageIndex = LanguageAsset.SourceData.GetLanguageIndex(EnglishName);
                    }
                }
            }

            try
            {
                var GameOptions = Resources.FindObjectsOfTypeAll<LocalizationSettingsOption>();
                var switchOpt = Resources.FindObjectsOfTypeAll<SwitchableLocalizationSettingsOption>();

                var locopt = new TMPro.TMP_Dropdown.OptionData();
                locopt.text = TranslatedName.ToUpper();

                var locopt2 = new LocalizationOption();
                locopt2.englishLanguageName = EnglishName;
                locopt2.displayedOption = TranslatedName;

                foreach (var option in GameOptions)
                {
                    if (option == null) continue;
                    if (option.dropdown != null)
                    {
                        if (!option.dropdown.options.Contains(locopt))
                        {
                            option.dropdown.options.Add(locopt);
                            if (PL.LanguageSaved == EnglishName)
                            {
                                option.dropdown.SetValueWithoutNotify(currentLanguageIndex);
                                option.dropdown.RefreshShownValue();
                            }
                            //PL.log.LogInfo("Adding " + locopt.text + " to Language Options");
                        }
                    }
                    if (option.languageOptions != null)
                    {
                        var array = option.languageOptions;
                        if (!array.Contains(locopt2))
                        {
                            option.languageOptions.Add(locopt2);
                        }
                    }
                }

                foreach (var option in switchOpt)
                {
                    if (option == null) continue;
                    if (option.dropdown != null)
                    {
                        if (!option.dropdown.options.Contains(locopt))
                        {
                            option.dropdown.options.Add(locopt);
                            if (LocalizationManager.SecondLanguage == EnglishName)
                            {
                                option.dropdown.SetValueWithoutNotify(currentLanguageIndex);
                                option.dropdown.RefreshShownValue();
                            }
                        }
                    }
                    if (option.languageOptions != null)
                    {
                        var array = option.languageOptions;
                        if (!array.Contains(locopt2))
                        {
                            option.languageOptions.Add(locopt2);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PL.log.LogError(PL.PREFIX + "Can't inizialize TMP_Dropdown options properly : " + e.Message);
            }
        }

        public static void SetSelectedLanguageOptions()
        {
            var GameOptions = Resources.FindObjectsOfTypeAll<LocalizationSettingsOption>();
            if (GameOptions != null)
            {
                foreach (var option in GameOptions)
                {
                    if (option == null) continue;
                    if (option.dropdown != null)
                    {
                        if (option.dropdown.value == currentLanguageIndex)
                        {
                            option.dropdown.SetValueWithoutNotify(currentLanguageIndex);
                            option.dropdown.RefreshShownValue();
                            break;
                        }
                    }
                }
            }
        }

        public static void ExportCatalog()
        {
            ExporterManager.ExportAll(FoldersManager.GetPath("DLCP"));
        }

        public static bool TryGetTranslation(string Key, out string Translation)
        {
            Translation = null;
            if (!EnableTranslation)
                return false;

            foreach (var source in Sources)
                if (source.TryGetTranslation(Key, out Translation))
                    return true;

            return false;
        }

        public static void AddSource(ITranslationSource source)
        {
            if (Sources.Contains(source))
                return;

            Sources.Add(source);
        }

        public static void ReloadAllSources()
        {
            foreach (var source in Sources)
            {
                source.Reload();
            }
            LoadTranslation(currentLanguage);
        }

        public static void RemoveAllSources()
        {
            Sources.Clear();
        }

        private static readonly IList<ITranslationSource> Sources = new List<ITranslationSource>();
    }

    public interface ITranslationSource
    {
        string Name { get; }
        bool SourceTranslationAvailable { get; }

        bool TryGetTranslation(string Key, out string Translation);

        void Reload();
    }
}
