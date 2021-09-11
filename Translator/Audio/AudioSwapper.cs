using System;
using UnityEngine;
using PL = DiscoTranslatorFinalCut.PluginLoader;
using UnityEngine.SceneManagement;

namespace DiscoTranslatorFinalCut.Translator.Audio
{
    class AudioSwapper : MonoBehaviour
    {
        public static GameObject obj = null;
        private bool Inizialized = false;
        public static Scene m_Scene;
        Scene f_Scene;
        public static bool SceneChanged = false;
        int Timer = 0;
        int numClips = 0;
        int totClips = 0;

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);

            new AudioSwapper(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<AudioSwapper>()).Pointer);

            return obj;
        }

        public AudioSwapper(IntPtr ptr) : base(ptr) { }

        public void Inizialize()
        {
            m_Scene = SceneManager.GetActiveScene();
            f_Scene = m_Scene;
            Inizialized = true;
            PL.log.LogInfo(PL.PREFIX + "Audio Swapper Inizialized...");
        }

        public void Awake() { }

        public void Start() { }

        public void Update()
        {
            if (!TranslatorManager.EnableTranslation || !AudioManager.EnableAudioImport) enabled = false;

            if (!Inizialized)
            {
                Inizialize();
            }

            if (SceneChanged)
            {
                if (SceneChanged && Timer >= 0)
                {
                    AudioManager.SceneClips(true); //Load New
                    SceneChanged = false;
                    Timer = 0;
                    numClips = Resources.FindObjectsOfTypeAll<AudioClip>().Count;
                    return;
                }
            }
            else
            {
                if (totClips != numClips)
                {
                    AudioManager.SceneClips(true); //Load New
                    Timer = 0;
                    numClips = totClips;
                }

                m_Scene = SceneManager.GetActiveScene();
                totClips = Resources.FindObjectsOfTypeAll<AudioClip>().Count;
            }

            Timer++;

            if (m_Scene.buildIndex != f_Scene.buildIndex && m_Scene.isLoaded)
            {
                SceneChanged = true;
                Timer = 0;
                f_Scene = m_Scene;
                AudioManager.SceneClips(false);
            }
            return;
        }

        public void OnDisable()
        {
            Inizialized = false;
        }
    }
}
