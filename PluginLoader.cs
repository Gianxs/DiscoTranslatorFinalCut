using System;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using DiscoTranslatorFinalCut.Helpers;
using DiscoTranslatorFinalCut.Helpers.UI;
using DiscoTranslatorFinalCut.Translator;
using DiscoTranslatorFinalCut.Translator.Images;
using DiscoTranslatorFinalCut.Translator.Audio;
using DiscoTranslatorFinalCut.Translator.UI;
using DiscoTranslatorFinalCut.Translator.Utility;
using UnityEngine.EventSystems;
using UnityEngine;
using UnhollowerRuntimeLib;
using HarmonyLib;

namespace DiscoTranslatorFinalCut
{
    [BepInPlugin("gianxs.DiscoTranslatorFinalCut", "Disco Translator Final Cut", "1.0.0.0")]
    public class PluginLoader : BasePlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "DiscoTranslatorFinalCut",
            BASE_PLUGIN_PATH = MODNAME,
            DUMPED_IMG_PATH = "OriginalImages",
            DUMPED_IMG_ATLAS_PATH = "AtlasTextures",
            DUMPED_IMG_NONATLAS_PATH = "nonAtlasTextures",
            DUMPED_IMG_OTHERS_PATH = "OtherTextures",
            //DUMPED_IMG_BUNDLE_PATH = "BundleTextures",
            DUMPED_LANG_CATALOG_PATH = "Catalog",
            DUMPED_AUDIO_FILES_PATH = "OriginalAudioFiles",
            LANG_TRANSLATIONS_PATH = "Translation",
            IMPORTED_IMG_BASE_PATH = "Images",
            IMPORTED_IMG_NEW_PATH = "New",
            IMPORTED_IMG_REPLACED_PATH = "Replace",
            IMPORTED_AUDIO_BASE_PATH = "Audio",
            IMPORTED_AUDIO_NEW_PATH = "New",
            IMPORTED_AUDIO_REPLACED_PATH = "Replace",
            AUTHOR = "Gianxs",
            GUID = "com." + AUTHOR + "." + MODNAME,
            PREFIX = "[" + MODNAME + "] ",
            FULLNAME = "Disco Translator Final Cut",
            VERSION = "1.1.0.0";
        public static ManualLogSource log;
        public static string LanguageSaved;
        #endregion
        public PluginLoader()
        {
            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;
            Application.runInBackground = true;
            log = Log;
        }
        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs e) =>
            log.LogError("\r\n\r\nUnhandled Exception:" + (e.ExceptionObject as Exception).ToString());

        public static string GetStrPrefs(string key)
        {
            if(PlayerPrefs.GetString(key) != null)
            {
                return PlayerPrefs.GetString(key);
            }
            return null;
        }

        public static bool GetBoolPrefs(string key)
        {
            var prefs = PlayerPrefs.GetInt(key);
            if (prefs == 2) //2 = Disabled
            {
                return false;
            }
            return true;
        }

        public override void Load()
        {
            #region[Try to create base folders structure if not exist]
            try
            {
                HarmonyLib.Harmony harmony = HarmonyLib.Harmony.CreateAndPatchAll(typeof(TranslatorHook), "gianxs.DiscoTranslatorFinalCut");
                //HarmonyLib.Harmony.CreateAndPatchAll(typeof(TranslatorPatch), harmony.ToString());

                FoldersManager.CreatePluginFolders();

                TranslatorManager.EnableTranslation = GetBoolPrefs("EnableDiscoTranslation");
                AudioManager.EnableAudioWidget = GetBoolPrefs("EnableAudioWidget");
                AudioManager.EnableAudioImport = GetBoolPrefs("EnableAudioImport");
                ImageManager.EnableTextureImport = GetBoolPrefs("EnableTextureImport");

            } catch(Exception e)
            {
                log.LogFatal(PREFIX + "Plugin can't be loaded! => " + e.Message);
                return;
            }
            #endregion

            #region[Register DiscoTranslatorFC in Il2Cpp]
            log.LogMessage(PREFIX + "Registering C# Type's in Il2Cpp");
            try
            {
                //DiscoTranslatorFinalCut
                ClassInjector.RegisterTypeInIl2Cpp<BootStrapper>();
                ClassInjector.RegisterTypeInIl2Cpp<Main>();
                ClassInjector.RegisterTypeInIl2Cpp<TranslatorManager>();
                ClassInjector.RegisterTypeInIl2Cpp<TranslatorLanguages>(); 
                ClassInjector.RegisterTypeInIl2Cpp<ImageSwapper>();
                ClassInjector.RegisterTypeInIl2Cpp<AudioSwapper>();
                //ClassInjector.RegisterTypeInIl2Cpp<AudioLoader>();

                // UI
                ClassInjector.RegisterTypeInIl2Cpp<UIControls>();
                ClassInjector.RegisterTypeInIl2Cpp<PanelMover>();
                ClassInjector.RegisterTypeInIl2Cpp<UIModMenu>();
                //ClassInjector.RegisterTypeInIl2Cpp<TooltipGUI>();
            }
            catch
            {
                log.LogError(PREFIX + "FAILED to Register Il2Cpp Type!");
            }
            #endregion

            #region[Harmony Patching]
            try
            {
                //log.LogMessage(" ");
                log.LogMessage(PREFIX + "Inserting Harmony Hooks...");

                var harmony = new Harmony("gianxs.DiscoTranslatorFinalCut.il2cpp");

                #region[Enable/Disable Harmony Debug Log]
                //Harmony.DEBUG = true; (Old)
                //HarmonyFileLog.Enabled = true;
                #endregion
                
                #region[Update() Hook - Only Needed for Bootstrapper]
                var originalUpdate = AccessTools.Method(typeof(UnityEngine.UI.Scrollbar), "Update");
                log.LogMessage(PREFIX + "Original Method: " + originalUpdate.DeclaringType.Name + "." + originalUpdate.Name);
                var postUpdate = AccessTools.Method(typeof(BootStrapper), "Update");
                log.LogMessage(PREFIX + "Postfix Method: " + postUpdate.DeclaringType.Name + "." + postUpdate.Name);
                harmony.Patch(originalUpdate, postfix: new HarmonyMethod(postUpdate));

                #endregion

                // IDragHandler
                var originalOnDrag = AccessTools.Method(typeof(UnityEngine.EventSystems.EventTrigger), "OnDrag");
                //log.LogMessage("   Original Method: " + originalOnDrag.DeclaringType.Name + "." + originalOnDrag.Name);
                var postOnDrag = AccessTools.Method(typeof(PanelMover), "OnDrag");
                //log.LogMessage("   Postfix Method: " + postOnDrag.DeclaringType.Name + "." + postOnDrag.Name);
                harmony.Patch(originalOnDrag, postfix: new HarmonyMethod(postOnDrag));

                /*
                #region[IBeginDragHandler, IDragHandler, IEndDragHandler Hooks]

                // These are required since UnHollower doesn't support Interfaces yet - Only needed if you need these events.

                // IBeginDragHandler
                var originalOnBeginDrag = AccessTools.Method(typeof(UnityEngine.EventSystems.EventTrigger), "OnBeginDrag");
                //log.LogMessage("   Original Method: " + originalOnBeginDrag.DeclaringType.Name + "." + originalOnBeginDrag.Name);
                var postOnBeginDrag = AccessTools.Method(typeof(WindowDragHandler), "OnBeginDrag");
                //log.LogMessage("   Postfix Method: " + postOnBeginDrag.DeclaringType.Name + "." + postOnBeginDrag.Name);
                harmony.Patch(originalOnBeginDrag, postfix: new HarmonyMethod(postOnBeginDrag));

                // IDragHandler
                var originalOnDrag = AccessTools.Method(typeof(UnityEngine.EventSystems.EventTrigger), "OnDrag");
                //log.LogMessage("   Original Method: " + originalOnDrag.DeclaringType.Name + "." + originalOnDrag.Name);
                var postOnDrag = AccessTools.Method(typeof(WindowDragHandler), "OnDrag");
                //log.LogMessage("   Postfix Method: " + postOnDrag.DeclaringType.Name + "." + postOnDrag.Name);
                harmony.Patch(originalOnDrag, postfix: new HarmonyMethod(postOnDrag));

                // IEndDragHandler
                var originalOnEndDrag = AccessTools.Method(typeof(UnityEngine.EventSystems.EventTrigger), "OnEndDrag");
                //log.LogMessage("   Original Method: " + originalOnEndDrag.DeclaringType.Name + "." + originalOnEndDrag.Name);
                var postOnEndDrag = AccessTools.Method(typeof(WindowDragHandler), "OnEndDrag");
                //log.LogMessage("   Postfix Method: " + postOnEndDrag.DeclaringType.Name + "." + postOnEndDrag.Name);
                harmony.Patch(originalOnEndDrag, postfix: new HarmonyMethod(postOnEndDrag));

                #endregion
                */
                log.LogMessage(PREFIX + "Runtime Hooks's Applied");
                //log.LogMessage(" ");
            }
            catch
            {
                log.LogError(PREFIX + "FAILED to Apply Hooks's!");
            }
            #endregion

            log.LogMessage(PREFIX + "Initializing Il2CppTypeSupport..."); // Helps with AssetBundles
            Il2CppTypeSupport.Initialize();

            #region[Bootstrap The Main DiscoTranslatorFinalCut GameObject]
            #region[DevNote]
            // If you create your main object here, only Awake(), OnEnabled() get fired. But if you try to create the trainer in either of 
            // those it doesn't get created properly as the object get's destroyed right away. Bootstrapping the GameObject like this allows
            // for it to inherit Unity MonoBehavior Events like OnGUI(), Update(), etc without a Harmony Patch. The only patch needed 
            // is for the Bootstrapper. You'll see. The Trainer has an EventTest function, Press 'Tab', and watch the BepInEx Console.
            #endregion
            BootStrapper.Create("BootStrapperGO");
            #endregion
        }
    }
}
