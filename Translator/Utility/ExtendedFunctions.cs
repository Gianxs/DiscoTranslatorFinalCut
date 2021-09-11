using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Il2CppSystem.Text.RegularExpressions;

namespace DiscoTranslatorFinalCut.Translator.Utility
{
    static class ExtendedFunctions
    {
        static Regex regEx;

        public static String PregReplace(this String input, string pattern, string replacements)
        {
            regEx = new Regex(pattern);
            input = regEx.Replace(input, replacements);
            return input;
        }

        public static string checkAtlas(string path, string name)
        {
            if (name.Contains("sactx-") && name.Length > 9 && !File.Exists(Path.Combine(path, name + ".png")))
            {
                var sactxAtlases = Directory.GetFiles(path, "sactx-*");

                if (sactxAtlases.Length > 0)
                {
                    foreach (var atlas in sactxAtlases)
                    {
                        string NewTexVer = "";
                        string OldTexVer = "";
                        string tmpTexName = "";
                        string tmpImgName = "";
                        string atlasName = Path.GetFileNameWithoutExtension(atlas);

                        if (name.Length > 9 && atlasName.Length > 9)
                        {
                            OldTexVer = name.Substring(Math.Max(0, name.Length - 8));
                            NewTexVer = atlasName.Substring(Math.Max(0, atlasName.Length - 8));
                            tmpTexName = name.Substring(0, Math.Max(0, name.Length - 9));
                            tmpImgName = atlasName.Substring(0, Math.Max(0, atlasName.Length - 9));
                        }

                        if (tmpImgName == tmpTexName && tmpImgName != "")
                        {
                            //PL.log.LogWarning(OldTexVer + " | " + NewTexVer + " | " + name);
                            return atlasName;
                        }

                        if (tmpTexName == atlasName)
                        {
                            //PL.log.LogWarning(OldTexVer + " | " + NewTexVer + " | " + name);
                            return atlasName;
                        }

                        if (OldTexVer != NewTexVer && (tmpImgName + "-" + OldTexVer) == name)
                        {
                            //PL.log.LogWarning(OldTexVer + " | " + NewTexVer + " | " + name);
                            return tmpImgName + "-" + NewTexVer;
                        }
                    }
                }
            }
            return name;
        }
    }
}
