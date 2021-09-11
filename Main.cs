using System;
using UnityEngine;
using PL = DiscoTranslatorFinalCut.PluginLoader;
using Lang = DiscoTranslatorFinalCut.Translator.TranslatorLanguages;
using Input = BepInEx.IL2CPP.UnityEngine.Input; //For UnityEngine.Input
using DiscoTranslatorFinalCut.Translator;
using DiscoTranslatorFinalCut.Translator.Images;
using DiscoTranslatorFinalCut.Translator.Audio;
using DiscoTranslatorFinalCut.Translator.Exporter;
using DiscoTranslatorFinalCut.Translator.UI;
using UnityEngine.SceneManagement;

namespace DiscoTranslatorFinalCut
{
    public class Main : MonoBehaviour
    {
        #region[Declarations]
        #region[DiscoTranslatorFinalCut]

        // DiscoTranslatorFinalCut Base
        public static GameObject obj = null;
        public static Main instance;
        private static bool initialized = false;
        Scene m_Scene;

        // Debugging
        private static bool onGuiFired = false;
        private static bool updateFired = false;

        #endregion
        #endregion

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);

            new Main(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<Main>()).Pointer);

            return obj;
        }

        public Main(IntPtr ptr) : base(ptr)
        {
            instance = this;
        }

        private static void Initialize()
        {
            TranslatorManager.Create("TranslatorComponentGO");
            ImageManager.Create("TranslatorImageManagerGO");
            //ImageManager.LoadGenericTextures();
            AudioSwapper.Create("AudioSwapperGO");
            //ImageSwapper.Create("ImageSwapperGO");
            UIModMenu.Create("UIMainMenuCanvasGO");

            #region[Display HotKeys]
            PL.log.LogMessage(" ");
            PL.log.LogMessage(PL.PREFIX + Lang.GetTerm("DISCOFC_HOTKEYS_MESSAGE"));
            PL.log.LogMessage(PL.PREFIX + "   U = " + Lang.GetTerm("DISCOFC_SHOW_HIDE_UI"));
            PL.log.LogMessage(PL.PREFIX + "   C = " + Lang.GetTerm("DISCOFC_EXPORT_CATALOG"));
            PL.log.LogMessage(PL.PREFIX + "   I = " + Lang.GetTerm("DISCOFC_EXPORT_LOADED_IMAGES"));
            PL.log.LogMessage(PL.PREFIX + "   A = " + Lang.GetTerm("DISCOFC_EXPORT_LOADED_AUDIO"));
            PL.log.LogMessage(PL.PREFIX + "   R = " + Lang.GetTerm("DISCOFC_RELOAD_ALL_TEXT_LONG"));
            PL.log.LogMessage(PL.PREFIX + "   S = " + Lang.GetTerm("DISCOFC_TOGGLE_INTRO_MUSIC"));
            PL.log.LogMessage(" ");

            #endregion
            initialized = true;
        }

        public void Awake()
        {
            //PL.log.LogMessage("Main Awake() Fired!");
        }

        public void Start()
        {
            //PL.log.LogMessage("Main Start() Fired!");
        }

        public void OnEnable()
        {
            //PL.log.LogMessage("Main OnEnable() Fired!");
        }

        public void Update()
        {
            if (!updateFired) { PL.log.LogMessage(PL.PREFIX + "Main Update() Fired!"); updateFired = true; }
            if (!initialized) { Initialize(); }

            if (initialized)
            {
                m_Scene = SceneManager.GetActiveScene();
                if(m_Scene.name == "Lobby")
                {
                    if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.S) && Event.current.type == EventType.KeyDown)
                    {
                        if(AudioManager.EnableAudioWidget)
                        {
                            AudioManager.ToggleMusic();
                        }
                        else
                        {
                            PL.log.LogWarning("AudioWidget disabled!");
                        }
                        
                        Event.current.Use();
                    }

                    if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.A) && Event.current.type == EventType.KeyDown)
                    {
                        ExporterManager.AudioExporter();
                        Event.current.Use();
                    }

                    if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.C) && Event.current.type == EventType.KeyDown)
                    {
                        TranslatorManager.ExportCatalog();
                        Event.current.Use();
                    }

                    if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.I) && Event.current.type == EventType.KeyDown)
                    {
                        try
                        {
                            ExporterManager.ExportImages();
                        }
                        catch (Exception e)
                        {
                            PL.log.LogError(PL.PREFIX + "Export Images failed! " + e.Message);
                        }

                        Event.current.Use();
                    }

                    if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.R) && Event.current.type == EventType.KeyDown)
                    {
                        if (TranslatorManager.EnableTranslation)
                        {
                            TranslatorManager.ReloadAllSources();
                            PL.log.LogInfo(PL.PREFIX + "Reload Resources Complete!");
                        }
                        else
                        {
                            PL.log.LogWarning("Mod Translations disabled!");
                        }
                        Event.current.Use();
                    }
                }
            }
        }

        public void OnGUI()
        {
            if (!onGuiFired) { 
                //PL.log.LogMessage("Component OnGUI() Fired!"); 
                onGuiFired = true; 
            }
            //windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)DoMyWindow, "My Window");
        }

        /*
        void DoMyWindow(int windowID)
        {
            GUI.Button(new Rect(10, 20, 100, 20), "Can't drag me");
            // Insert a huge dragging area at the end.
            // This gets clipped to the window (like all other controls) so you can never
            //  drag the window from outside it.
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            //PL.log.LogInfo("DoMyWindow");
        }
        */
    }
}
