using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using DiscoTranslatorFinalCut.Translator.UI;
using DiscoTranslatorFinalCut.Translator.Exporter;
using DiscoTranslatorFinalCut.Translator.Utility;
using DiscoTranslatorFinalCut.Helpers.UI;
using PL = DiscoTranslatorFinalCut.PluginLoader;

namespace DiscoTranslatorFinalCut.Translator.Images
{
    public static class ImageManager
    {
        static bool inizialized = false;
        public static bool EnableTextureImport = false;

        public static Dictionary<string, Texture2D> ImportedGenericTextures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> ImportedLanguageTextures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> ImportedSceneTextures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> ImportedSkillsTextures = new Dictionary<string, Texture2D>();
        public static void SceneTextures(bool Load)
        {
            var ActiveScene = SceneManager.GetActiveScene();
            if (ActiveScene.name == "Initialize") return;
            
            var db = ImportedSceneTextures;
            var Textures = Resources.FindObjectsOfTypeAll<Texture2D>();

            foreach (var tex in Textures)
            {
                if (tex == null) continue;
                if (tex.name == null) continue;

                if (ImportedLanguageTextures.ContainsKey(tex.name)) continue;

                string filename;
                if (TranslatorManager.currentLanguagePath != null && TranslatorManager.isCustomLanguage) {
                    var path1 = Path.Combine(TranslatorManager.currentLanguagePath, "images");
                    filename = Path.Combine(path1, tex.name + ".png");
                    if (EnableTextureImport && TranslatorManager.EnableTranslation && TranslatorManager.currentLanguagePath != null && Directory.Exists(path1) && File.Exists(filename))
                    {
                        if (!db.ContainsKey(tex.name) && Load)
                        {
                            if (tex.name.Contains("hat_mullen"))
                            {
                                PL.log.LogWarning(tex.name + " loading...");
                            }
                            LoadTextureToList(tex, db, path1);
                        }

                        if (db.ContainsKey(tex.name) && !Load)
                        {
                            UnLoadTextureFromList(tex, db, path1);
                        }
                    }
                }
                
                var path2 = FoldersManager.GetPath("IIRP"); //Images\Replace
                filename = Path.Combine(path2, tex.name + ".png");
                if (EnableTextureImport && Directory.Exists(path2) && File.Exists(filename))
                {
                    if (!db.ContainsKey(tex.name) && Load)
                    {
                        LoadTextureToList(tex, db, path2);
                    }

                    if (db.ContainsKey(tex.name) && !Load)
                    {
                        UnLoadTextureFromList(tex, db, path2);
                    }
                }
            }

            if (!Load) ImportedSceneTextures.Clear(); //if all is done correctly is alerady empty.

            return;
        }

        public static void TranslatedTextures(bool Load)
        {
            if (!EnableTextureImport || !TranslatorManager.EnableTranslation) return;
            if (TranslatorManager.currentLanguagePath == null || !TranslatorManager.isCustomLanguage) return;

            var ActiveScene = SceneManager.GetActiveScene();
            
            if (ActiveScene.name == "Initialize" || ActiveScene.name == "Lobby")
            {
                var db = ImportedLanguageTextures;
                var path = Path.Combine(TranslatorManager.currentLanguagePath, "images");

                if (!Directory.Exists(path))
                {
                    PL.log.LogWarning(PL.PREFIX + "path is null!");
                    return;
                }

                string[] images = Directory.GetFiles(path);

                if (images.Length <= 0) return;
                var Textures = Resources.FindObjectsOfTypeAll<Texture2D>();

                //PL.log.LogWarning("Language Images found : " + images.Length.ToString());

                foreach (var tex in Textures)
                {
                    if (tex == null) continue;
                    if (tex.name == null) continue;

                    if (!db.ContainsKey(tex.name) && Load)
                    {
                        LoadTextureToList(tex, db, path);
                    }

                    if (db.ContainsKey(tex.name) && !Load)
                    {
                        UnLoadTextureFromList(tex, db, path);
                    }
                }
            }
            return;            
        }

        public static void LoadTextureToList(Texture2D tex, Dictionary<string, Texture2D> db, string path)
        {
            if (!Directory.Exists(Path.Combine(path, "original"))) Directory.CreateDirectory(Path.Combine(path, "original"));
            string oldName = tex.name;
            tex.name = ExtendedFunctions.checkAtlas(path, tex.name);
            if (Directory.GetFiles(path, tex.name + ".png", SearchOption.TopDirectoryOnly).Length == 1)
            {
                if (!File.Exists(Path.Combine(path, "original", oldName + ".png")))
                {
                    File.WriteAllBytes(Path.Combine(path, "original", oldName + ".png"), ImageConversion.EncodeToPNG(ExporterManager.DuplicateTexture(tex)));
                }

                var filename = Directory.GetFiles(path, tex.name + ".png", SearchOption.TopDirectoryOnly)[0];

                //Replace texture...
                if (tex.name == Path.GetFileNameWithoutExtension(filename))
                {
                    try
                    {
                        tex.LoadImage(File.ReadAllBytes(filename), false);
                        tex.Apply();
                    }
                    catch //(Exception e)
                    {
                        //PL.log.LogWarning(e.Message);
                        //Ignore this Readable Error/Warning, it works! O_o, who cares!
                    }
                    db.Add(oldName, tex);
                    PL.log.LogInfo(PL.PREFIX + "Texture replaced... " + oldName + " with " + tex.name);
                    tex.name = oldName;
                }
            }
        }

        public static void UnLoadTextureFromList(Texture2D tex, Dictionary<string, Texture2D> db, string path)
        {
            var opath = Path.Combine(path, "original");
            if (!Directory.Exists(opath))
            {
                PL.log.LogFatal(PL.PREFIX + "Can't recover original texture files, original folder missing!");
                db.Clear();
                return;
            }

            if (Directory.GetFiles(opath, tex.name + ".png", SearchOption.TopDirectoryOnly).Length == 1)
            {
                var filename = Directory.GetFiles(opath, tex.name + ".png", SearchOption.TopDirectoryOnly)[0];
                try
                {
                    tex.LoadImage(File.ReadAllBytes(filename), false);
                    tex.Apply();
                }
                catch //(Exception e)
                {
                    //PL.log.LogWarning(e.Message);
                    //Ignore this Readable Error/Warning, it works! O_o, who cares!
                }
                db.Remove(tex.name);
                PL.log.LogInfo(PL.PREFIX + "Texture recovered... " + tex.name);
            }
        }

        public static void LoadGenericTextures()
        {
            if (!EnableTextureImport) return;
           
            var ActiveScene  = SceneManager.GetActiveScene();
            
            if (ActiveScene.name == "Initialize")
            {
                if (!inizialized) PL.log.LogInfo(PL.PREFIX + "Importing Generic Images -> Start...");

                var path = FoldersManager.GetPath("IIBP"); //Images
                if (!Directory.Exists(path))
                {
                    PL.log.LogError(PL.PREFIX + "Can't create folder for images!");
                    return;
                }

                var path1 = FoldersManager.GetPath("IIRP"); //Images\Replace
                var path2 = FoldersManager.GetPath("IINP"); //Images\New

                var db = ImportedGenericTextures;
                string[] images = Directory.GetFiles(path1);
                string[] images2 = Directory.GetFiles(path2);
                var Textures = Resources.FindObjectsOfTypeAll<Texture2D>();

                //PL.log.LogWarning("LoadGenericTextures! Replace Count : " + images.Length.ToString());

                if (images.Length > 0)
                {
                    foreach (var tex in Textures)
                    {
                        if (tex == null) continue;
                        if (tex.name == null) continue;

                        if (tex.name.Contains("portrait_"))
                        {
                            continue;
                        }

                        tex.name = ExtendedFunctions.checkAtlas(path1, tex.name);

                        if (!db.ContainsKey(tex.name))
                        {
                            if (Directory.GetFiles(path1, tex.name + ".png").Length == 1)
                            {
                                //PL.log.LogInfo("Texture found : " + tex.name);
                                var filename = Path.GetFileNameWithoutExtension(Directory.GetFiles(path1, tex.name + ".png")[0]);

                                //Replace texture...
                                if (tex.name == Path.GetFileNameWithoutExtension(filename))
                                {
                                    try
                                    {
                                        tex.LoadImage(File.ReadAllBytes(filename), false);
                                        tex.Apply();
                                    }
                                    catch //(Exception e)
                                    {
                                        //PL.log.LogWarning(e.Message);
                                        //Ignore this Readable Error/Warning, it works! O_o, who cares!
                                    }
                                    db.Add(tex.name, tex);
                                    PL.log.LogInfo(PL.PREFIX + "Texture replaced... " + tex.name);
                                }
                            }
                        }
                    }
                }

                if (images2.Length > 0)
                {
                    foreach (var image in images2)
                    {
                        if (!db.ContainsKey(image))
                        {
                            var tex = UIManager.CreateTextureFromFile(image);
                            if (tex != null)
                            {
                                tex.name = Path.GetFileNameWithoutExtension(image);
                                PL.log.LogInfo(PL.PREFIX + "Importing new texture... " + tex.name);
                                Sprite sprite = UIManager.CreateSpriteFrmTexture(tex);
                                sprite.name = tex.name;
                                PL.log.LogInfo(PL.PREFIX + "Sprite Created with name : " + sprite.name);
                                db.Add(image, tex);
                            }
                            else
                            {
                                PL.log.LogError(PL.PREFIX + "Texture creation failed! Texture is null!");
                            }
                        }
                    }
                }

                if (!inizialized) PL.log.LogInfo(PL.PREFIX + "Importing Generic Images -> Done...");
                inizialized = true;
            }
            return;
        }
    }
}
