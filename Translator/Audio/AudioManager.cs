using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DiscoTranslatorFinalCut.Translator.Utility;
using PL = DiscoTranslatorFinalCut.PluginLoader;

namespace DiscoTranslatorFinalCut.Translator.Audio
{
    public class AudioManager : MonoBehaviour
    {
        //public static bool EnableLanguageAudioImport = false;
        //public static bool EnableGenericAudioImport = false;

        public static bool EnableAudioImport = false;
        public static bool EnableAudioWidget = false;

        public static Dictionary<string, AudioClip> ImportedSceneClips = new Dictionary<string, AudioClip>();
        public static bool MusicIsInPause = false;
        public static AudioSource ActiveMusic;
        public static bool ActiveMusicFounded = false;
        public static string[] filters;

        public static void GetActiveMusic()
        {
            if (ActiveMusicFounded) return;

            var audiofilters = GetFilters();

            var allAudioSources = Resources.FindObjectsOfTypeAll<AudioSource>();
            foreach (var source in allAudioSources)
            {
                if(source != null && source.isPlaying && source != ActiveMusic && source.clip.name != "silence")
                {
                    if (source.clip != null)
                    {
                        foreach (var filter in audiofilters)
                        {
                            if (source.clip.name.Contains(filter))
                            {
                                //PL.log.LogWarning(source.clip.name + " is playing now and is the Active Music.");
                                ActiveMusic = source;
                                ActiveMusicFounded = true;
                                break;
                            }
                        }
                        break;
                    }
                    else
                    {
                        source.Stop();
                    }
                }
            }

            if (!ActiveMusicFounded)
            {
                var allAudioClips = Resources.FindObjectsOfTypeAll<AudioClip>();
                foreach (var clip in allAudioClips)
                {
                    if (clip != null && clip.loadType == AudioClipLoadType.Streaming && clip.frequency == 48000 && clip.loadState == AudioDataLoadState.Loaded)
                    {
                        foreach (var filter in audiofilters)
                        {
                            if (clip.name.Contains(filter))
                            {
                                try
                                {
                                    var tmpSource = new GameObject().AddComponent<AudioSource>();                                    
                                    tmpSource.clip = Instantiate(clip);
                                    tmpSource.clip.name = clip.name;

                                    Destroy(clip);

                                    ActiveMusic = tmpSource;
                                    ActiveMusic.loop = false;
                                    ActiveMusic.Play();
                                    ActiveMusicFounded = true;
                                    //PL.log.LogWarning(tmpSource.clip.name + " is playing now and is the Active Music.");
                                }
                                catch(Exception e)
                                {
                                    PL.log.LogError("Error in Track audio clip for UI Menu. " + e.Message);
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            return;
        }

        public static void SetActiveMusic(AudioSource source)
        {
            var audiofilters = GetFilters();

            foreach (var filter in audiofilters)
            {
                if (source.clip.name.Contains(filter))
                {
                    //PL.log.LogWarning(source.clip.name + " is playing now and is the Active Music.");
                    ActiveMusic = source;
                    ActiveMusicFounded = true;
                    break;
                }
            }
            return;
        }

        public static string[] GetFilters()
        {
            if(filters == null)
            {
                string filterstring = "-music|-theme|-song|-cafe|protorave|-rock|-credits|-dance";
                filters = filterstring.Split('|');
            }
            return filters;
        }

        public static void ToggleMusic()
        {
            if (ActiveMusic == null) return;
            var allAudioSources = Resources.FindObjectsOfTypeAll<AudioSource>();
            foreach (var source in allAudioSources)
            {
                if (source != null && source.clip != null && source == ActiveMusic) //(source.clip.name == "main-menu-music" || source.clip.name == "main-menu-music_replace")
                {
                    if (MusicIsInPause)
                    {
                        source.UnPause();
                        //PL.log.LogInfo(PL.PREFIX + " UnPaused Audio Source : " + source.name + " with audioclip : " + source.clip.name);
                    }
                    else if (source.isPlaying)
                    {
                        source.Stop();
                        //PL.log.LogInfo(PL.PREFIX + " Stopped Audio Source : " + source.name + " with audioclip : " + source.clip.name);
                    }
                    else if (!source.isPlaying)
                    {
                        source.Play();
                        //PL.log.LogInfo(PL.PREFIX + " Play Audio Source : " + source.name + " with audioclip : " + source.clip.name);
                    }
                    MusicIsInPause = false;
                    break;
                }
            }
        }

        public static void PlayMusic()
        {
            if (ActiveMusic == null) return;
            var allAudioSources = Resources.FindObjectsOfTypeAll<AudioSource>();
            foreach (var source in allAudioSources)
            {
                if (source != null && source.clip != null && source == ActiveMusic)
                {
                    if (MusicIsInPause)
                    {
                        source.UnPause();
                    }
                    else if (source.isPlaying)
                    {
                        source.Stop();
                        source.Play();
                    }
                    else
                    {
                        source.Play();
                    }
                    MusicIsInPause = false;
                    break;
                }
            }
        }

        public static void StopMusic()
        {
            if (ActiveMusic == null) return;
            var allAudioSources = Resources.FindObjectsOfTypeAll<AudioSource>();
            
            foreach (var source in allAudioSources)
            {
                if (source != null && source.clip != null && source.isPlaying && source == ActiveMusic)
                {
                    if(MusicIsInPause)
                    {
                        source.UnPause();
                    }
                    MusicIsInPause = false;
                    source.Stop();
                    break;
                }
            }
        }

        public static void PauseMusic()
        {
            if (ActiveMusic == null) return;
            var allAudioSources = Resources.FindObjectsOfTypeAll<AudioSource>();
            foreach (var source in allAudioSources)
            {
                if (source != null && source.clip != null)
                {
                    if (source == ActiveMusic)
                    {
                        if(MusicIsInPause)
                        {
                            source.UnPause();
                            MusicIsInPause = false;
                        }
                        else
                        {
                            source.Pause();
                            MusicIsInPause = true;
                        }
                        break;
                    }
                }
            }
        }

        public static float GetMusicVolume()
        {
            if (ActiveMusic == null) return 0f;

            var allAudioSources = Resources.FindObjectsOfTypeAll<AudioSource>();
            float ret = 0f;
            foreach (var source in allAudioSources)
            {
                if (source != null && source.clip != null && source == ActiveMusic)
                {
                    ret = source.volume;
                    return ret;
                }
            }
            return ret;
        }

        public static void SetMusicVolume(float volume)
        {
            if (ActiveMusic == null) return;

            var allAudioSources = Resources.FindObjectsOfTypeAll<AudioSource>();
            foreach (var source in allAudioSources)
            {
                if (source != null && source.clip != null && source == ActiveMusic)
                {
                    source.volume = volume;
                    return;
                }
            }
            return;
        }

        public static string[] GetMusicInfo()
        {
            var allAudioSources = Resources.FindObjectsOfTypeAll<AudioSource>();
            string Timer = "00:00";
            string Total = "00:00";
            string[] ret = new string[2];

            if (ActiveMusic == null) GetActiveMusic();

            if(ActiveMusic != null) {
                foreach (var source in allAudioSources)
                {
                    if (source != null && source.clip != null && source == ActiveMusic)
                    {
                        ret[0] = TimeFormatter(source.time);
                        ret[1] = TimeFormatter(source.clip.length);
                        return ret;
                    }
                }
            }

            ret[0] = Timer;
            ret[1] = Total;
            return ret;
        }

        private static string TimeFormatter(float time)
        {
            if (ActiveMusic == null) return "00:00";

            if (time < 60)
            {
                int seconds = (int)time;
                if(seconds < 10)
                {
                    return "00:0" + seconds.ToString();
                }
                else
                {
                    return "00:" + seconds.ToString();
                }
            }else 
            {
                if(time > 3600)
                {
                    PL.log.LogError("Max clip duration exceeded 59:59");
                    return "59:59";
                }

                int minutes = (int)(time / 60);
                int seconds = (int)(((time / 60) - (int)(time / 60)) * 60);

                if(minutes < 10)
                {
                    if(seconds < 10)
                    {
                        return "0" + minutes.ToString() + ":" + "0" + seconds.ToString();
                    }
                    else
                    {
                        return "0" + minutes.ToString() + ":" + seconds.ToString();
                    }
                }
                else
                {
                    if (seconds < 10)
                    {
                        return minutes.ToString() + ":" + "0" + seconds.ToString();
                    }
                    else
                    {
                        return minutes.ToString() + ":" + seconds.ToString();
                    }
                }
            }
        }

        public static void SceneClips(bool Load)
        {
            var ActiveScene = SceneManager.GetActiveScene();
            if (ActiveScene.name == "Initialize") return;

            if(!Load) ActiveMusicFounded = false;

            var db = ImportedSceneClips;
            var Clips = Resources.FindObjectsOfTypeAll<AudioClip>();

            foreach (var clip in Clips)
            {
                if (clip == null) continue;
                if (clip.name == null) continue;
                if (clip.name.Contains("_old")) continue;

                if (clip.name.Contains("_replace") && Load) continue;
                //else if(clip.name.Contains("_replace")) PL.log.LogWarning(clip.name + " in foreach...");

                string filename;
                if (TranslatorManager.currentLanguagePath != null)
                {
                    var path1 = Path.Combine(TranslatorManager.currentLanguagePath, "audio");
                    if(Directory.Exists(path1))
                    {
                        if(EnableAudioImport && TranslatorManager.EnableTranslation && Load && TranslatorManager.currentLanguagePath != null && TranslatorManager.isCustomLanguage)
                        {
                            filename = Path.Combine(path1, clip.name + ".wav");
                            if (!db.ContainsKey(clip.name + "_old") && File.Exists(filename))
                            {
                                //PL.log.LogWarning("path1 load " + clip.name);
                                LoadClipToList(clip, db, path1);
                            }
                        }
                            
                        if(!Load)
                        {
                            clip.name = clip.name.Replace("_replace", "");
                            filename = Path.Combine(path1, clip.name + ".wav");
                            if (db.ContainsKey(clip.name + "_old") && File.Exists(filename))
                            {
                                //PL.log.LogWarning("path1 unload " + clip.name);
                                UnLoadClipFromList(clip, db);
                            }
                        }
                    }
                }
                else
                {
                    if(!Load)
                    {
                        clip.name = clip.name.Replace("_replace", "");
                        foreach (var element in TranslatorManager.LanguageFolders)
                        {
                            var path1 = element.Value;
                            filename = Path.Combine(path1, clip.name + ".wav");
                            if (db.ContainsKey(clip.name + "_old") && File.Exists(filename))
                            {
                                //PL.log.LogWarning("path1 unload " + clip.name);
                                UnLoadClipFromList(clip, db);
                            }
                        }
                    }
                }

                var path2 = FoldersManager.GetPath("IARP"); //Audio\Replace
                if (Directory.Exists(path2))
                {
                    if(EnableAudioImport && Load)
                    {
                        filename = Path.Combine(path2, clip.name + ".wav");
                        if (!db.ContainsKey(clip.name + "_old") && File.Exists(filename))
                        {
                            //PL.log.LogWarning("path2 load " + clip.name);                        
                            LoadClipToList(clip, db, path2);
                        }
                    }

                    if(!Load)
                    {
                        clip.name = clip.name.Replace("_replace", "");
                        filename = Path.Combine(path2, clip.name + ".wav");
                        if (db.ContainsKey(clip.name + "_old") && File.Exists(filename))
                        {
                            //PL.log.LogWarning("path2 unload " + clip.name);
                            UnLoadClipFromList(clip, db);
                        }
                    }
                }
            }

            if (!Load) db.Clear();
            //ShowClipsInDB(db);

            return;
        }

        public static void LoadClipToList(AudioClip clip, Dictionary<string, AudioClip> db, string path)
        {
            //PL.log.LogWarning("LoadClipToList called, clip : " + clip.name + " at path : " + path);
            var path_replace = Directory.GetFiles(path, clip.name + ".wav")[0];
            //PL.log.LogWarning("path_replace : " + path_replace);
            if (!db.ContainsKey(Path.GetFileNameWithoutExtension(path_replace) + "_old"))
            {
                var request = GetAudioClip(path_replace, clip);
                request.Wait();
                if (request.IsCompleted)
                {
                    var audio = request.Result;
                    if (audio != null)
                    {
                        audio.name = Path.GetFileNameWithoutExtension(path_replace) + "_replace";
                        UnityEngine.Object.DontDestroyOnLoad(audio);

                        if (audio.name == clip.name + "_replace")
                        {
                            var source = GetSourceFromClipName(clip.name);
                            if (source != null)
                            {
                                if (source.isPlaying)
                                {
                                    try
                                    {
                                        if (ActiveMusic == null || (ActiveMusic != null && ActiveMusic.clip != null && ActiveMusic.clip.name != null && ActiveMusic.clip.name != source.clip.name))
                                        {
                                            SetActiveMusic(source);
                                        }
                                    }
                                    catch(Exception e)
                                    {
                                        PL.log.LogError("Failed to set ActiveMusic : " + e.Message);
                                    }
                                    
                                    source.clip.name += "_old";
                                    source.Stop();
                                    source.clip = audio;
                                    source.Play();
                                }
                                else
                                {
                                    source.clip.name += "_old";
                                    source.clip = audio;
                                }
                            }
                            else
                            {
                                clip.name += "_old";
                            }

                            db.Add(clip.name, clip);
                            PL.log.LogInfo(PL.PREFIX + "AudioClip : " + clip.name.Replace("_old", "") + " replaced.");
                        }
                    }
                }
            }
        }

        public static void UnLoadClipFromList(AudioClip clip, Dictionary<string, AudioClip> db)
        {
            //PL.log.LogWarning("UnLoadClipFromList called... Clipname : " + clip.name);
            AudioClip oldClip;
            db.TryGetValue(clip.name + "_old", out oldClip);
            if (oldClip != null)
            {
                oldClip.name = oldClip.name.Replace("_old", "");
                //PL.log.LogWarning(oldClip.name + " ready to be recovered");

                try
                {
                    var source = GetSourceFromClipName(oldClip.name);
                    if (source != null)
                    {
                        if (source.isPlaying)
                        {
                            source.Stop();
                            source.clip = oldClip;

                            if(!AudioSwapper.SceneChanged)
                            {
                                SetActiveMusic(source);
                            }
                            
                            source.Play();
                        }
                        else
                        {
                            source.clip = oldClip;
                        }
                    }

                    Destroy(clip);
                    PL.log.LogInfo("AudioClip Recovered : " + oldClip.name);
                }
                catch (Exception e)
                {
                    PL.log.LogError(PL.PREFIX + "Can't Recover default AudioClip : " + e.Message);
                }

                db.Remove(oldClip.name + "_old");
            }
        }

        public static async Task<AudioClip> GetAudioClip(string filePath, AudioClip failback)
        {
            var www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV);

            var result = www.SendWebRequest();

            while (!result.isDone) { 
                await Task.Delay(10); 
            }

            if (www.isNetworkError)
            {
                PL.log.LogError(PL.PREFIX + " " + www.error);
                return failback;
            }
            else
            {
                return DownloadHandlerAudioClip.GetContent(www);
            }
        }

        public static AudioSource GetSourceFromClipName(string name)
        {
            var Sources = Resources.FindObjectsOfTypeAll<AudioSource>();
            AudioSource ret = null;
            foreach(var source in Sources)
            {
                if(source.clip != null && source.clip.name != null && source.clip.name == name)
                {
                    ret = source;
                    break;
                }
            }
            return ret;
        }

        public static void ShowClipsInDB(Dictionary<string, AudioClip> db)
        {
            PL.log.LogWarning(PL.PREFIX + "Clips db contain : ");
            var clipsname = "";
            var nclip = 0;
            foreach (var clip in db)
            {
                nclip++;
                clipsname += clip.Key;
                if (nclip < db.Count) clipsname += ", ";
            }
            PL.log.LogWarning(PL.PREFIX + clipsname);
        }
    }
}
