using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using PL = DiscoTranslatorFinalCut.PluginLoader;
using DiscoTranslatorFinalCut.Translator.Utility;
using Karambolo.PO;
using PixelCrushers.DialogueSystem;
using UnhollowerBaseLib;

namespace DiscoTranslatorFinalCut.Translator.Exporter
{
    static class ExporterManager
    {
        /*
        private static readonly HashSet<string> ConversationTranslatableFields =
            new HashSet<string>(new string[] { "Title", "Description", "subtask_title_01", "subtask_title_02",
                "subtask_title_03", "subtask_title_04", "subtask_title_05", "subtask_title_06" });
        */
        private static readonly HashSet<string> DialogueTranslatableFields =
            new HashSet<string>(new string[] { "Alternate1", "Alternate2", "Alternate3", "Alternate4", "Dialogue Text", "tooltip1",
                "tooltip10", "tooltip2", "tooltip3", "tooltip4", "tooltip5", "tooltip6", "tooltip7", "tooltip8", "tooltip9" });

        public static Texture2D DuplicateTexture(Texture2D texture)
        {
            var tmp = RenderTexture.GetTemporary(
                    texture.width,
                    texture.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

            Graphics.Blit(texture, tmp);
            var previous = RenderTexture.active;
            RenderTexture.active = tmp;
            var newTexture = new Texture2D(texture.width, texture.height);
            newTexture.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            newTexture.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);
            return newTexture;
        }
        public static void ExportImages()
        {
            TranslatorHook.EnableImageHook = false;

            PL.log.LogInfo(PL.PREFIX + "ExportImages Start...");
            FoldersManager.GetPath("DIP");

            var Textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            var Atlases = Resources.FindObjectsOfTypeAll<SpriteAtlas>();
            var Sprites = Resources.FindObjectsOfTypeAll<Sprite>();
            int Exported = 0;

            var dumpedTex = new List<string>();

            foreach (var sprite in Sprites)
            {
                if (sprite == null) continue;
                if (sprite.texture == null) continue;

                foreach (var atlas in Atlases)
                {
                    if (atlas == null) continue;
                    string atlas_path = FoldersManager.GetPath("DIAP"); //"/AtlasTextures_sprites";
                    string atlas_dirName = "/" + atlas.name;
                    Directory.CreateDirectory(atlas_path + atlas_dirName);

                    var texture = sprite.texture;

                    dumpedTex.Add(texture.name);

                    if (atlas.CanBindTo(sprite))
                    {
                        if (!File.Exists(Path.Combine(atlas_path + atlas_dirName, texture.name + ".png")))
                        {
                            File.WriteAllBytes(Path.Combine(atlas_path + atlas_dirName, texture.name + ".png"), ImageConversion.EncodeToPNG(DuplicateTexture(texture)));
                            Exported++;
                            //PL.log.LogInfo("La sprite " + sprite.name + " (texture name : "+texture.name+") è all'interno dell'atlas : " + atlas.name);
                        }
                    }
                    else
                    {
                        if (!texture.name.Contains("Atlas"))
                        {
                            string nonAtlasTexture_path = FoldersManager.GetPath("DINP"); //"/nonAtlasTextures_sprites";
                            if (!File.Exists(Path.Combine(nonAtlasTexture_path, texture.name + ".png")))
                            {
                                File.WriteAllBytes(Path.Combine(nonAtlasTexture_path, texture.name + ".png"), ImageConversion.EncodeToPNG(DuplicateTexture(texture)));
                                Exported++;
                            }
                        }
                    }
                }
            }

            foreach (var tex in Textures)
            {
                if (!dumpedTex.Contains(tex.name))
                {
                    string OtherTexture_path = FoldersManager.GetPath("DIOP"); //"/OtherTextures";
                    Directory.CreateDirectory(OtherTexture_path);
                    if (!File.Exists(Path.Combine(OtherTexture_path, tex.name + ".png")))
                    {
                        File.WriteAllBytes(Path.Combine(OtherTexture_path, tex.name + ".png"), ImageConversion.EncodeToPNG(DuplicateTexture(tex)));
                        Exported++;
                    }
                }
            }

            /*
            string MyPath = Path.Combine(Application.streamingAssetsPath, "aa", "StandaloneWindows64");
            string[] MyAssets = Directory.GetFiles(MyPath);

            foreach (var asset in MyAssets)
            {
                var bundleLoaded = Il2CppAssetBundleManager.LoadFromFile(asset);
                if (bundleLoaded != null)
                {
                    var names = bundleLoaded.GetAllAssetNames();

                    foreach (var name in names)
                    {
                        if (name != null && (name.Contains(".png")))
                        {
                            var path = FoldersManager.GetPath("DIBP");
                            if (!File.Exists(Path.Combine(path, Path.GetFileNameWithoutExtension(name) + ".png")))
                            {
                                Sprite newSprite = bundleLoaded.LoadAsset<Sprite>(name);

                                if (newSprite == null || newSprite.texture == null) continue;

                                File.WriteAllBytes(Path.Combine(path, Path.GetFileNameWithoutExtension(name) + ".png"), ImageConversion.EncodeToPNG(DuplicateTexture(newSprite.texture)));
                                Exported++;
                            }
                        }
                    }
                }
            }
            */

            PL.log.LogInfo(PL.PREFIX + "ExportImages Done... Total Exported texture : " + Exported.ToString());
            TranslatorHook.EnableImageHook = true;
        }

        public static void ExportAll(string directory)
        {
            PL.log.LogInfo(PL.PREFIX + "Export Catalog start...");
            var gen = new POGenerator();

            try
            {
                var languageSources = Resources.FindObjectsOfTypeAll<I2.Loc.LanguageSourceAsset>();

                foreach (var source in languageSources)
                {
                    var fullName = source.name;

                    POCatalog catalog = null;

                    PL.log.LogInfo(PL.PREFIX + "I2.Loc.LanguageSourceAsset -> Name = " + fullName);

                    var Languages = source.mSource.GetLanguages();

                    foreach (var language in Languages)
                    {
                        int index = source.mSource.GetLanguageIndex(language);
                        if (index < 0) continue;
                        catalog = LanguageSourceToCatalog(source, index);
                        var catalogPath = Path.Combine(directory, fullName + ".po");
                        using (var file = File.Create(catalogPath))
                        using (var writer = new StreamWriter(file))
                        {
                            gen.Generate(writer, catalog);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PL.log.LogError(PL.PREFIX + "Export LanguageSourceAsset Failed : " + e.Message);
            }

            try
            {
                // Manually generate dialogue catalog for additional infos
                // Actor, conversant, conversation context

                var db = Resources.FindObjectsOfTypeAll<DialogueDatabase>()[0];

                var dialogueCatalog = GetDialogueCatalog(db);
                var dialogueCatalogPath = Path.Combine(directory, "DialoguesLockitEnglish.po");
                PL.log.LogInfo(PL.PREFIX + "I2.Loc.LanguageSourceAsset -> Name = DialoguesLockitEnglish.po");

                using (var file = File.Create(dialogueCatalogPath))
                using (var writer = new StreamWriter(file))
                {
                    gen.Generate(writer, dialogueCatalog);
                }
            }
            catch (Exception e)
            {
                PL.log.LogInfo(PL.PREFIX + " " + e.Message);
            }

            /*
            try
            {
                var languageSources = Resources.FindObjectsOfTypeAll<I2.Loc.LanguageSourceAsset>();

                foreach (var source in languageSources)
                {
                    var fullName = source.name;

                    var shortName = fullName.Replace("Languages", "");
                    POCatalog catalog = null;
                    if (shortName == "Dialogue" || shortName == "ButtonsImages" || shortName == "NamesLockit" || shortName == "ImagesLockitEnglish" || shortName == "FontsLockitEnglish")
                        continue;

                    int engIndex = source.mSource.GetLanguageIndex("English");
                    if (engIndex != -1 && fullName.Contains("English"))
                    {
                        PL.log.LogInfo("I2.Loc.LanguageSourceAsset -> Name = " + fullName);
                        catalog = LanguageSourceToCatalog(source, engIndex);

                        var catalogPath = Path.Combine(directory, shortName + ".po");

                        using (var file = File.Create(catalogPath))
                        using (var writer = new StreamWriter(file))
                        {
                            gen.Generate(writer, catalog);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PL.log.LogError("Export LanguageSourceAsset Failed : " + e.Message);
            }
            */
            PL.log.LogInfo("Export Catalog done.");
        }

        public static POCatalog LanguageSourceToCatalog(I2.Loc.LanguageSourceAsset languageSource, int languageIndex)
        {
            var catalog = InitCatalog();

            foreach (var term in languageSource.mSource.mTerms)
            {
                string key = term.Term;
                string source = term.Languages[languageIndex];

                if (string.IsNullOrWhiteSpace(source) || source == "\"")
                    continue;

                var entry = new POSingularEntry(new POKey(source, contextId: key));
                catalog.Add(entry);
            }

            return catalog;
        }

        public static IDictionary<int, Actor> GetActors(DialogueDatabase db)
        {
            var ret = new Dictionary<int, Actor>();

            foreach (var actor in db.actors)
            {
                ret[actor.id] = actor;
            }
            return ret;
        }

        public static POCatalog GetDialogueCatalog(this DialogueDatabase db)
        {
            var actors = GetActors(db);

            var catalog = InitCatalog();

            foreach (var conversation in db.conversations)
            {
                foreach (var dialogue in conversation.dialogueEntries)
                {
                    foreach (var field in dialogue.fields)
                    {
                        if (string.IsNullOrWhiteSpace(field.value))
                            continue;
                        //PL.log.LogWarning(field.title + " | " + field.value);
                        //if (field.title.Contains("ELECTROCHEMISTRY"))
                        if (!DialogueTranslatableFields.Contains(field.title))
                        {
                            continue;
                        }
                        //if (!ConversationTranslatableFields.Contains(field.title))
                        //continue;

                        string key = $"{field.title}/{Field.LookupValue(dialogue.fields, "Articy Id")}";
                        string source = field.value;

                        var entry = new POSingularEntry(new POKey(source, contextId: key))
                        {
                            Comments = new List<POComment>()
                        };

                        entry.Comments.Add(new POTranslatorComment { Text = $"Title = {conversation.Title}" });
                        entry.Comments.Add(new POTranslatorComment { Text = $"Description = {conversation.Description.Replace("\n", "\\n")}" });
                        if (actors.TryGetValue(dialogue.ActorID, out Actor actor))
                            entry.Comments.Add(new POTranslatorComment { Text = $"Actor = {actor.Name}" });
                        if (actors.TryGetValue(dialogue.ConversantID, out Actor conversant))
                            entry.Comments.Add(new POTranslatorComment { Text = $"Conversant = {conversant.Name}" });

                        catalog.Add(entry);
                    }
                }
            }

            return catalog;
        }

        public static POCatalog InitCatalog()
        {
            return new POCatalog
            {
                Encoding = "UTF-8",
                Language = "en_US"
            };
        }
        
        public static void AudioExporter()
        {
            PL.log.LogInfo(PL.PREFIX + "AudioExporter Start...");
            var path = FoldersManager.GetPath("DAFP");

            var AudioClips = Resources.FindObjectsOfTypeAll<AudioClip>();
            
            foreach (var audio in AudioClips)
            {
                if (audio == null) continue;
                if (File.Exists(path + "/" + audio.name + ".wav")) continue;

                if(audio.loadType == AudioClipLoadType.DecompressOnLoad || audio.loadState == AudioDataLoadState.Loaded)
                {
                    try
                    {
                        PL.log.LogInfo(PL.PREFIX + "Saving... " + audio.name + " with loadType = " + audio.loadType.ToString());
                        ExportClipData(path, audio.name, audio);
                    }
                    catch (Exception e)
                    {
                        PL.log.LogError(PL.PREFIX + "Can't save " + audio.name + " => " + e.Message);
                    }
                }else if(audio.loadType == AudioClipLoadType.Streaming)
                {
                    PL.log.LogWarning(PL.PREFIX + "Streaming audio are saved empty. " + audio.name + " skipped.");
                }
                else
                {
                    PL.log.LogWarning(PL.PREFIX + "Cannot get data on compressed samples for audio clip " + audio.name + ". Changing the load type to DecompressOnLoad on the audio clip will fix this.");
                }
            }
            PL.log.LogInfo(PL.PREFIX + "AudioExporter Done...");
        }

        /*
        public static void ExportAudio(AudioClip audio)
        {
            var path = FoldersManager.GetPath("DAFP");
            if (File.Exists(path + "/" + audio.name + ".wav")) return;
            try
            {
                ExportClipData(path, audio.name, audio);
                //PL.log.LogInfo(audio.name + " saved!");
            }
            catch (Exception e)
            {
                PL.log.LogError(PL.PREFIX + "Can't save " + audio.name + " => " + e.Message);
            }
        }
        */
        private static void ExportClipData(string filepath, string filename, AudioClip clip)
        {
            Il2CppStructArray<float> data = new float[clip.samples * clip.channels];
            clip.GetData(data, 0);
            var path = Path.Combine(filepath, filename + ".wav");
            using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            {
                // The following values are based on http://soundfile.sapp.org/doc/WaveFormat/
                var bitsPerSample = (ushort)16;
                var chunkID = "RIFF";
                var format = "WAVE";
                var subChunk1ID = "fmt ";
                var subChunk1Size = (uint)16;
                var audioFormat = (ushort)1;
                var numChannels = (ushort)clip.channels;
                var sampleRate = (uint)clip.frequency;
                var byteRate = (uint)(sampleRate * clip.channels * bitsPerSample / 8);  // SampleRate * NumChannels * BitsPerSample/8
                var blockAlign = (ushort)(numChannels * bitsPerSample / 8); // NumChannels * BitsPerSample/8
                var subChunk2ID = "data";
                var subChunk2Size = (uint)(data.Length * clip.channels * bitsPerSample / 8); // NumSamples * NumChannels * BitsPerSample/8
                var chunkSize = (uint)(36 + subChunk2Size); // 36 + SubChunk2Size
                                                            // Start writing the file.
                WriteString(stream, chunkID);
                WriteInteger(stream, chunkSize);
                WriteString(stream, format);
                WriteString(stream, subChunk1ID);
                WriteInteger(stream, subChunk1Size);
                WriteShort(stream, audioFormat);
                WriteShort(stream, numChannels);
                WriteInteger(stream, sampleRate);
                WriteInteger(stream, byteRate);
                WriteShort(stream, blockAlign);
                WriteShort(stream, bitsPerSample);
                WriteString(stream, subChunk2ID);
                WriteInteger(stream, subChunk2Size);
                foreach (var sample in data)
                {
                    // De-normalize the samples to 16 bits.
                    var deNormalizedSample = (short)0;
                    if (sample > 0)
                    {
                        var temp = sample * short.MaxValue;
                        if (temp > short.MaxValue)
                            temp = short.MaxValue;
                        deNormalizedSample = (short)temp;
                    }
                    if (sample < 0)
                    {
                        var temp = sample * (-short.MinValue);
                        if (temp < short.MinValue)
                            temp = short.MinValue;
                        deNormalizedSample = (short)temp;
                    }
                    WriteShort(stream, (ushort)deNormalizedSample);
                }
            }
        }

        private static void WriteString(Stream stream, string value)
        {
            foreach (var character in value)
                stream.WriteByte((byte)character);
        }

        private static void WriteInteger(Stream stream, uint value)
        {
            stream.WriteByte((byte)(value & 0xFF));
            stream.WriteByte((byte)((value >> 8) & 0xFF));
            stream.WriteByte((byte)((value >> 16) & 0xFF));
            stream.WriteByte((byte)((value >> 24) & 0xFF));
        }

        private static void WriteShort(Stream stream, ushort value)
        {
            stream.WriteByte((byte)(value & 0xFF));
            stream.WriteByte((byte)((value >> 8) & 0xFF));
        }
    }
}
