using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using PL = DiscoTranslatorFinalCut.PluginLoader;

namespace DiscoTranslatorFinalCut.Translator.Images
{
    class ImageSwapper : MonoBehaviour
    {
        public static GameObject obj = null;
        private bool Inizialized = false;
        public static Scene m_Scene;
        Scene f_Scene;
        public static bool SceneChanged = false;
        int Timer = 0;
        int numTex = 0;
        int totTex = 0;

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);

            new ImageSwapper(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<ImageSwapper>()).Pointer);

            return obj;
        }

        public ImageSwapper(IntPtr ptr) : base(ptr) { }

        public void Inizialize()
        {
            m_Scene = SceneManager.GetActiveScene();
            f_Scene = m_Scene;
            Inizialized = true;
            PL.log.LogInfo(PL.PREFIX + "Image Swapper Inizialized...");
        }

        public void Awake() { }

        public void Start() { }

        public void Update()
        {
            if (!TranslatorManager.EnableTranslation || !ImageManager.EnableTextureImport) enabled = false;

            if (!Inizialized)
            {
                Inizialize();
            }

            if (SceneChanged)
            {
                if (SceneChanged && Timer >= 5)
                {
                    ImageManager.SceneTextures(true); //Load New
                    SceneChanged = false;
                    Timer = 0;
                    numTex = Resources.FindObjectsOfTypeAll<Texture2D>().Count;
                    return;
                }
            }
            else
            {
                if (Timer > 60 && totTex != numTex)
                {
                    ImageManager.SceneTextures(true); //Load New
                    Timer = 0;
                    numTex = totTex;
                }

                m_Scene = SceneManager.GetActiveScene();

                if (Timer == 60)
                {
                    totTex = Resources.FindObjectsOfTypeAll<Texture2D>().Count;
                }

            }

            Timer++;

            if (m_Scene.buildIndex != f_Scene.buildIndex && m_Scene.isLoaded)
            {
                SceneChanged = true;
                Timer = 0;
                f_Scene = m_Scene;
                PL.log.LogInfo(PL.PREFIX + "Scene changed to : " + m_Scene.name);
                ImageManager.SceneTextures(false); //Unload
            }
            return;
        }

        public void OnDisable()
        {
            Inizialized = false;
        }
    }
}
