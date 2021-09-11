using System;
using HarmonyLib;
using DiscoTranslatorFinalCut.Translator.Utility;
using PL = DiscoTranslatorFinalCut.PluginLoader;

namespace DiscoTranslatorFinalCut.Translator
{
    public static class TranslatorHook
    {
        public static bool EnableImageHook = true;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(I2.Loc.LocalizationManager), nameof(I2.Loc.LocalizationManager.GetTranslation))]
        static bool GetTermTranslationPrefix(string Term, ref string __result)
        {
            if (TranslatorManager.TryGetTranslation(Term, out string Translation))
            {
                if (Translation != null && Translation.Length > 0)
                {
                    try
                    {
                        Translation = ExtendedFunctions.PregReplace(Translation, "[A-Z]{1}[0-9]+:", "");
                    }
                    catch (Exception e)
                    {
                        PL.log.LogWarning(PL.PREFIX + "Translation PregReplace failed on TranslatorHook.cs => " + e.Message);
                    }
                    __result = Translation;
                    return false;
                }
            }
            return true;
        }
    }
}
