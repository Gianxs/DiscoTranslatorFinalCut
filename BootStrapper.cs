using System;
using HarmonyLib;
using UnityEngine;
using PL = DiscoTranslatorFinalCut.PluginLoader;

namespace DiscoTranslatorFinalCut
{
    class BootStrapper : MonoBehaviour
    {
        private static GameObject disco = null;
        internal static GameObject Create(string name)
        {
            var obj = new GameObject(name);
            DontDestroyOnLoad(obj);
            new BootStrapper(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<BootStrapper>()).Pointer);
            return obj;
        }
        public BootStrapper(IntPtr intPtr) : base(intPtr) { }

        public static void Awake()
        {
            //PL.log.LogMessage("Bootstrapper Awake() Fired!");
        }

        [HarmonyPostfix]
        public static void Update()
        {
            try
            {
                if (disco == null)
                {
                    PL.log.LogMessage(PL.PREFIX + "Bootstrapper Update() Fired!");
                    PL.log.LogMessage(" ");
                    PL.log.LogMessage(PL.PREFIX + "Bootstrapping DiscoTranslatorFinalCut...");
                    try
                    {
                        disco = Main.Create("TranslatorComponentGO");
                        if (disco != null) { PL.log.LogMessage(PL.PREFIX + "DiscoTranslatorFinalCut Bootstrapped!"); }
                    }
                    catch (Exception e)
                    {
                        PL.log.LogMessage(PL.PREFIX + "ERROR Bootstrapping DiscoTranslatorFinalCut: " + e.Message);
                        PL.log.LogMessage(" ");
                    }
                }
                
            } catch(Exception e)
            {
                PL.log.LogMessage(PL.PREFIX + "ERROR Bootstrapping DiscoTranslatorFinalCut: " + e.Message);
            }
        }
    }
}
