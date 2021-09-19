using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PL = DiscoTranslatorFinalCut.PluginLoader;
using Lang = DiscoTranslatorFinalCut.Translator.TranslatorLanguages;
using Input = BepInEx.IL2CPP.UnityEngine.Input; //For UnityEngine.Input
using DiscoTranslatorFinalCut.Translator;
using DiscoTranslatorFinalCut.Translator.Images;
using DiscoTranslatorFinalCut.Translator.Audio;
using DiscoTranslatorFinalCut.Translator.Exporter;
using DiscoTranslatorFinalCut.Translator.UI;
using DiscoTranslatorFinalCut.Tools;
using UnhollowerBaseLib;
using UnityEngine.SceneManagement;

namespace DiscoTranslatorFinalCut
{
    internal delegate void getRootSceneObjects(int handle, IntPtr list);
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
        public static bool optionToggle = false;

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
            ImageManager.LoadGenericTextures();
            AudioSwapper.Create("AudioSwapperGO");
            ImageSwapper.Create("ImageSwapperGO");
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
                if (m_Scene.name == "Lobby")
                {
                    if (Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.S) && Event.current.type == EventType.KeyDown)
                    {
                        if (AudioManager.EnableAudioWidget)
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

                    /*
                    if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.N) && Event.current.type == EventType.KeyDown)
                    {
                        
                    }

                    
                    // Dump All Scenes GameObjects (w/ optionToggle True prints components also)
                    if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.N) && Event.current.type == EventType.KeyDown)
                    {
                        DumpAll(GetAllScenesGameObjects());
                        Event.current.Use();
                    }

                    // Dumping Root Scene Objects w/ Values (w/ optionToggle True prints components also)
                    if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.M) && Event.current.type == EventType.KeyDown)
                    {
                        PL.log.LogMessage("Dumping Root Scene Objects w/ values...");
                        SceneDumper.DumpObjects(GetRootSceneGameObjects().ToArray());
                        Event.current.Use();
                    }
                    */
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

        #region[ICalls]

        #region[Get Objects]

        // Resolve the GetRootGameObjects ICall (internal Unity MethodImp functions)
        internal static getRootSceneObjects getRootSceneObjects_iCall = IL2CPP.ResolveICall<getRootSceneObjects>("UnityEngine.SceneManagement.Scene::GetRootGameObjectsInternal");
        private static void GetRootGameObjects_Internal(Scene scene, IntPtr list)
        {
            getRootSceneObjects_iCall(scene.handle, list);
        }

        private static Il2CppSystem.Collections.Generic.List<GameObject> GetRootSceneGameObjects()
        {
            var scene = SceneManager.GetActiveScene();
            var list = new Il2CppSystem.Collections.Generic.List<GameObject>(scene.rootCount);

            GetRootGameObjects_Internal(scene, list.Pointer);

            return list;
        }
        private static Il2CppSystem.Collections.Generic.List<GameObject> GetAllScenesGameObjects()
        {
            Scene[] array = new Scene[SceneManager.sceneCount];
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                array[i] = SceneManager.GetSceneAt(i);
            }

            var allObjectsList = new Il2CppSystem.Collections.Generic.List<GameObject>();
            foreach (var scene in array)
            {
                var list = new Il2CppSystem.Collections.Generic.List<GameObject>(scene.rootCount);
                GetRootGameObjects_Internal(scene, list.Pointer);
                foreach (var obj in list) { allObjectsList.Add(obj); }
            }

            #region[DevNote]
            /*
            The reason these differ are that GetAllScenesObjects doen't get DontDestroyOnLoad objects and it maintains heirarchy so it looks like alot less
            For example: GetAllScenesObjects() doesn't find this Trainer and the games StageLoadManager object.
            */
            //log.LogMessage("AllScenesObject's Count: " + allObjectsList.Count.ToString());
            //log.LogMessage("FindAll<GameObject>() Count: " + GameObject.FindObjectsOfType<GameObject>().Count.ToString());
            #endregion

            return allObjectsList;
        }

        #endregion


        #endregion

        private static string dumpLog = "";
        public static void DumpAll(Il2CppSystem.Collections.Generic.List<GameObject> rootObjects)
        {
            PL.log.LogMessage("Dumping Objects...");

            foreach (GameObject obj in rootObjects)
            {
                dumpLog = "";
                level = 1;
                prevlevel = 0;

                // Dump this object
                PL.log.LogMessage("[GameObject]: " + obj.name);
                dumpLog += "[GameObject]: " + obj.name + "\r\n";

                #region[Get GameObject Components if optionToggle]
                if (optionToggle)
                {
                    PL.log.LogMessage("  [Components]:");
                    dumpLog += "  [Components]:\r\n";

                    var comps = obj.GetGameObjectComponents();
                    foreach (var comp in comps)
                    {
                        PL.log.LogMessage("    " + comp.Name);
                        dumpLog += "    " + comp.Name + "\r\n";
                    }

                    dumpLog += "\r\n";
                }
                #endregion

                // Dump the children
                DisplayChildren(obj.transform);

                // Write the Dump File
                if (dumpLog != "")
                {
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\OBJECT_DUMPS\\")) { Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\OBJECT_DUMPS\\"); }
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\OBJECT_DUMPS\\" + obj.name + "_DUMP.txt", dumpLog);
                }

                PL.log.LogMessage("Dump Complete!");
            }
        }

        private static int level = 0;
        private static int prevlevel = 0;
        private static void DisplayChildren(Transform trans)
        {
            prevlevel = level;

            foreach (var child in trans)
            {
                var t = child.Cast<Transform>();

                // Adjust the indent
                string consoleprefix = "";
                for (int cnt = 0; cnt < level; cnt++) { consoleprefix += "  "; }

                // The Actual Logging
                PL.log.LogMessage(consoleprefix + "[GameObject]: " + t.gameObject.name);
                dumpLog += consoleprefix + "[GameObject]: " + t.gameObject.name + "\r\n";

                #region[Get GameObject Components if optionToggle]
                if (optionToggle)
                {
                    PL.log.LogMessage(consoleprefix + "  [Components]:");
                    dumpLog += consoleprefix + "  [Components]:\r\n";

                    var comps = t.gameObject.GetGameObjectComponents();
                    foreach (var comp in comps)
                    {
                        PL.log.LogMessage(consoleprefix + "    " + comp.Name);
                        dumpLog += consoleprefix + "    " + comp.Name + "\r\n";
                    }

                    dumpLog += "\r\n";
                }
                #endregion

                // Out Inifinate Iterator
                if (t.childCount > 0)
                {
                    level += 1;
                    DisplayChildren(t);
                }
                else { level = prevlevel; }

            }
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

    public static class GameObjectExtensions
    {
        public static bool HasComponent<T>(this GameObject flag) where T : Component
        {
            if (flag == null)
                return false;
            return flag.GetComponent<T>() != null;
        }

        public static List<GameObject> GetParentsChildren(this GameObject parent)
        {
            if (parent == null)
                return null;
            List<GameObject> tmp = new List<GameObject>();

            for (int idx = 0; idx < parent.transform.childCount; idx++)
            {
                tmp.Add(parent.transform.GetChild(idx).gameObject);
            }

            return tmp;
        }

        public static List<Il2CppSystem.Type> GetGameObjectComponents(this GameObject gameObject)
        {
            if (gameObject == null)
                return null;
            List<Il2CppSystem.Type> tmp = new List<Il2CppSystem.Type>();

            var comps = gameObject.GetComponents<Component>();
            foreach (var comp in comps)
            {
                tmp.Add(comp.GetIl2CppType());
            }

            return tmp;
        }

        public static void DumpGameObject(this GameObject obj)
        {
            if (obj == null)
                return;

            Il2CppSystem.Collections.Generic.List<GameObject> tmpList = new Il2CppSystem.Collections.Generic.List<GameObject>();
            tmpList.Add(obj);
            Main.DumpAll(tmpList);
        }

    }
}
