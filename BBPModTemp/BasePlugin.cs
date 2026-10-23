using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System.IO;
using UnityEngine;

namespace BBCExitOverElevatorGate
{
    [BepInPlugin("ganaisthere.plus.bbcexitoverelevatorgate", "Baldi's Basic Classic Exit Over Elevator Gate", "0.1.0.0")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    public class BasePlugin : BaseUnityPlugin
    {
        public static BasePlugin Instance { get; internal set; }
        public static Harmony harmony = new Harmony("ganaisthere.plus.bbcexitoverelevatorgate");
        public static AssetManager assetMan = new AssetManager();
        public string ModPath => AssetLoader.GetModPath(this);

        //----------------------------------------

        public ConfigEntry<bool> ConfigOverrideSounds;
        public ConfigEntry<bool> ConfigAutoGenerateMissingTexture2D;

        public void Awake()
        {
            ConfigOverrideSounds = Config.Bind
            (
                "General",
                "Override Sound Effects",
                true,
                "If true, this mod will override some related sound effects of the elevator. (Which may affect other mods that replace sound effects, such as \"BB+ texture pack\" mod.)"
            );
            ConfigAutoGenerateMissingTexture2D = Config.Bind
            (
                "General",
                "Auto Generate Missing Texture2D",
                true,
                "If true, the mod will automatically generate the original texture of the missing texture wall in the \"Texture2D\" folder when the texture cannot be found."
            );

            Instance = this;
            harmony.PatchAll();

            AddLocalization("Subtitles_English.json");
            AddSoundObject("Thunk.ogg", "SoundObjects", "Sfx_Thunk", SoundType.Effect, Color.white);
            AddSoundObject("Null.ogg", "SoundObjects");

            string texture2DFolder = Path.Combine(AssetLoader.GetModPath(this), "Textures");
            Directory.CreateDirectory(texture2DFolder);
            string[] allFiles = Directory.GetFiles(texture2DFolder, "*_GateOut.png", SearchOption.TopDirectoryOnly);
            foreach (string file in allFiles)
            {
                string fileName = Path.GetFileName(file);
                Log("Loading File: " + fileName, 1);
                AddTexture2D(fileName, "Textures");
            }
        }

        internal void AddLocalization(string fileNameWithExtension, string chlidPath = null, Language language = Language.English)
        {
            string filePath;
            if (chlidPath == null)
            {
                filePath = Path.Combine(AssetLoader.GetModPath(this), fileNameWithExtension);
            }
            else
            {
                filePath = Path.Combine(AssetLoader.GetModPath(this), chlidPath, fileNameWithExtension);
            }
            if (File.Exists(filePath))
            {
                AssetLoader.LocalizationFromFile(filePath, language);
            }
            else
            {
                Log("File Not Exists: " + filePath);
            }
        }
        internal void AddSoundObject(string fileNameWithExtension, string chlidPath, string subtitle, SoundType type, Color color, float sublength = -1)
        {
            string filePath;
            if (chlidPath == null)
            {
                filePath = Path.Combine(AssetLoader.GetModPath(this), fileNameWithExtension);
            }
            else
            {
                filePath = Path.Combine(AssetLoader.GetModPath(this), chlidPath, fileNameWithExtension);
            }
            if (File.Exists(filePath))
            {
                AudioClip audioClip = AssetLoader.AudioClipFromFile(filePath);
                SoundObject soundObject = ObjectCreators.CreateSoundObject(audioClip, subtitle, type, color, sublength);
                soundObject.name = Path.GetFileNameWithoutExtension(filePath);
                assetMan.Add(Path.GetFileNameWithoutExtension(filePath), soundObject);
            }
            else
            {
                Log("File Not Exists: " + filePath);
            }
        }
        internal void AddSoundObject(string fileNameWithExtension, string chlidPath = null, SoundType type = SoundType.Effect)
        {
            AddSoundObject(fileNameWithExtension, chlidPath, "Nothing", type, Color.white, 0f);
        }
        internal void AddTexture2D(string fileNameWithExtension, string chlidPath = null, bool btwSprite = false, float pixelsPerUnit = 50f)
        {
            string filePath;
            if (chlidPath == null)
            {
                filePath = Path.Combine(AssetLoader.GetModPath(this), fileNameWithExtension);
            }
            else
            {
                filePath = Path.Combine(AssetLoader.GetModPath(this), chlidPath, fileNameWithExtension);
            }
            if (File.Exists(filePath))
            {
                Texture2D texture2D = AssetLoader.TextureFromFile(filePath);
                assetMan.Add(Path.GetFileNameWithoutExtension(filePath), texture2D);
                if (btwSprite)
                {
                    Sprite sprite = AssetLoader.SpriteFromTexture2D(texture2D, pixelsPerUnit);
                    assetMan.Add(Path.GetFileNameWithoutExtension(filePath), sprite);
                }
            }
            else
            {
                Log("File Not Exists: " + filePath);
            }
        }

        internal static void Log(object data, int level = 0)
        {
            if (level == 1)
            {
                Instance.Logger.LogWarning(data);
            }
            else if (level == 2)
            {
                Instance.Logger.LogError(data);
            }
            else
            {
                Instance.Logger.LogInfo(data);
            }
        }
    }
}
