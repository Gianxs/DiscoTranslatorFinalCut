using BepInEx;
using System.IO;
using Loader = DiscoTranslatorFinalCut.PluginLoader;

namespace DiscoTranslatorFinalCut.Translator.Utility
{
    public class FoldersManager
    {
        public static void CreatePluginFolders()
        {
            if (!Directory.Exists(GetPath("DIP"))) Directory.CreateDirectory(GetPath("DIP"));
            if (!Directory.Exists(GetPath("DIAP"))) Directory.CreateDirectory(GetPath("DIAP"));
            if (!Directory.Exists(GetPath("DINP"))) Directory.CreateDirectory(GetPath("DINP"));
            if (!Directory.Exists(GetPath("DIOP"))) Directory.CreateDirectory(GetPath("DIOP"));
            if (!Directory.Exists(GetPath("DIBP"))) Directory.CreateDirectory(GetPath("DIBP"));
            if (!Directory.Exists(GetPath("DLCP"))) Directory.CreateDirectory(GetPath("DLCP"));
            if (!Directory.Exists(GetPath("LTP"))) Directory.CreateDirectory(GetPath("LTP"));
            if (!Directory.Exists(GetPath("IIBP"))) Directory.CreateDirectory(GetPath("IIBP"));
            if (!Directory.Exists(GetPath("IINP"))) Directory.CreateDirectory(GetPath("IINP"));
            if (!Directory.Exists(GetPath("IIRP"))) Directory.CreateDirectory(GetPath("IIRP"));
            if (!Directory.Exists(GetPath("IABP"))) Directory.CreateDirectory(GetPath("IABP"));
            if (!Directory.Exists(GetPath("IANP"))) Directory.CreateDirectory(GetPath("IANP"));
            if (!Directory.Exists(GetPath("IARP"))) Directory.CreateDirectory(GetPath("IARP"));
            if (!Directory.Exists(GetPath("DAFP"))) Directory.CreateDirectory(GetPath("DAFP"));
        }

        public static string GetPath(string path)
        {
            string ret;
            switch(path)
            {
                case "DIP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.DUMPED_IMG_PATH);
                break;
                case "DIAP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.DUMPED_IMG_PATH, Loader.DUMPED_IMG_ATLAS_PATH);
                break;
                case "DINP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.DUMPED_IMG_PATH, Loader.DUMPED_IMG_NONATLAS_PATH);
                break;
                case "DIOP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.DUMPED_IMG_PATH, Loader.DUMPED_IMG_OTHERS_PATH);
                break;
                case "DIBP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.DUMPED_IMG_PATH, Loader.DUMPED_IMG_BUNDLE_PATH);
                break;
                case "DLCP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.DUMPED_LANG_CATALOG_PATH);
                break;
                case "LTP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.LANG_TRANSLATIONS_PATH);
                break;
                case "IIBP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.IMPORTED_IMG_BASE_PATH);
                break;
                case "IINP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.IMPORTED_IMG_BASE_PATH, Loader.IMPORTED_IMG_NEW_PATH);
                break;
                case "IIRP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.IMPORTED_IMG_BASE_PATH, Loader.IMPORTED_IMG_REPLACED_PATH);
                break;
                case "IABP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.IMPORTED_AUDIO_BASE_PATH);
                break;
                case "IANP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.IMPORTED_AUDIO_BASE_PATH, Loader.IMPORTED_AUDIO_NEW_PATH);
                break;
                case "IARP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.IMPORTED_AUDIO_BASE_PATH, Loader.IMPORTED_AUDIO_REPLACED_PATH);
                break;
                case "DAFP":
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH, Loader.DUMPED_AUDIO_FILES_PATH);
                break;
                default:
                    ret = Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH);
                break;
            }
            if (!Directory.Exists(ret) && ret != Path.Combine(Paths.PluginPath, Loader.BASE_PLUGIN_PATH)) Directory.CreateDirectory(ret);
            return ret;
        }
    }
}
