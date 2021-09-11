using System;
using System.IO;
using UnityEngine; // This has built-in Il2CppType which conflicts with UnhollowerRuntimeLib.Il2CppType (Doesn't contain .Of<>)
using DiscoTranslatorFinalCut.Helpers.UI;
using UnhollowerBaseLib;
using UnityEngine.UI;
using UnityEngine.U2D;
using Object = UnityEngine.Object;
using PL = DiscoTranslatorFinalCut.PluginLoader;
using Lang = DiscoTranslatorFinalCut.Translator.TranslatorLanguages;
using DiscoTranslatorFinalCut.Translator;

namespace DiscoTranslatorFinalCut.Translator.UI
{
    public static class UIManager
    {
        public static Il2CppSystem.Object HTMLString2Color(Il2CppSystem.String htmlcolorstring)
        {
            Color32 color = new Color32();

            #region[DevNote]
            // Unity ref: https://docs.unity3d.com/ScriptReference/ColorUtility.TryParseHtmlString.html
            // Note: Color strings can also set alpha: "#7AB900" vs. w/alpha "#7AB90003" 
            //ColorUtility.TryParseHtmlString(htmlcolorstring, out color); // Unity's Method, This may have been stripped
            #endregion

            color = htmlcolorstring.HexToColor().Unbox<Color32>();
            //log.LogMessage("HexString: " + htmlcolorstring  + "RGBA(" + color.r.ToString() + "," + color.g.ToString() + "," + color.b.ToString() + "," + color.a.ToString() + ")");

            return color.BoxIl2CppObject();
        }

        public static Texture2D CreateDefaultTexture(Il2CppSystem.String htmlcolorstring)
        {
            Color32 color = HTMLString2Color(htmlcolorstring).Unbox<Color32>();

            // Make a new sprite from a texture
            Texture2D SpriteTexture = new Texture2D(1, 1);
            SpriteTexture.SetPixel(0, 0, color);
            SpriteTexture.Apply();

            return SpriteTexture;
        }

        public static Texture2D CreateTextureFromFile(Il2CppSystem.String FilePath)
        {
            // Load a PNG or JPG file from disk to a Texture2D
            Texture2D Tex2D;
            Il2CppStructArray<byte> FileData;

            PL.log.LogInfo(PL.PREFIX + "Search image on folder : " + FilePath);

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(265, 198);
                var filename = Path.GetFileNameWithoutExtension(FilePath);
                Tex2D.name = filename;
                //Tex2D.LoadRawTextureData(FileData); // This is Broke. Unhollower/Texture2D doesn't like it...
                Tex2D.LoadImage(FileData, false);
                Tex2D.Apply();
                return Tex2D;
            }
            else
            {
                PL.log.LogError(PL.PREFIX + "File not exist!");
            }
            return null;
        }

        public static Sprite CreateSpriteFrmTexture(Texture2D SpriteTexture)
        {
            // Create a new Sprite from Texture
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), 100.0f, 0, SpriteMeshType.Tight);

            return NewSprite;
        }

        public static GameObject CreateUICanvas(string name)
        {
            PL.log.LogMessage(PL.PREFIX + "   Creating Canvas");

            // Create a new Canvas Object with required components
            GameObject CanvasGO = new GameObject(name);
            Object.DontDestroyOnLoad(CanvasGO);

            Canvas canvas = new Canvas(CanvasGO.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<Canvas>()).Pointer);

            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            UnityEngine.UI.CanvasScaler cs = new UnityEngine.UI.CanvasScaler(CanvasGO.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<UnityEngine.UI.CanvasScaler>()).Pointer);

            cs.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
            cs.referencePixelsPerUnit = 100f;
            cs.referenceResolution = new Vector2(1024f, 788f);

            UnityEngine.UI.GraphicRaycaster gr = new UnityEngine.UI.GraphicRaycaster(CanvasGO.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<UnityEngine.UI.GraphicRaycaster>()).Pointer);

            return CanvasGO;
        }

        public static GameObject CreateUIPanel(GameObject canvas, Il2CppSystem.String height, Il2CppSystem.String width, Sprite BgSprite = null)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;

            PL.log.LogMessage(PL.PREFIX + "   Creating UI Panel");
            GameObject uiPanel = UIControls.CreatePanel(uiResources);
            uiPanel.transform.SetParent(canvas.transform, false);

            RectTransform rectTransform = uiPanel.GetComponent<RectTransform>();

            float size;
            size = Il2CppSystem.Single.Parse(height); // Their is no float support in Unhollower, this avoids errors
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            size = Il2CppSystem.Single.Parse(width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            // You can also use rectTransform.sizeDelta = new Vector2(width, height);

            return uiPanel;
        }

        public static GameObject CreateUISubPanel(GameObject parent, Il2CppSystem.String height, Il2CppSystem.String width, Sprite BgSprite = null)
        {
            UIControls.Resources uiResources = new UIControls.Resources();
            uiResources.background = BgSprite;
            PL.log.LogMessage(PL.PREFIX + "   Creating UI Sub Panel");
            GameObject uiPanel = UIControls.CreatePanel(uiResources);
            uiPanel.transform.SetParent(parent.transform, false);

            RectTransform rectTransform = uiPanel.GetComponent<RectTransform>();

            float size;
            size = Il2CppSystem.Single.Parse(height); // Their is no float support in Unhollower, this avoids errors
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            size = Il2CppSystem.Single.Parse(width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            // You can also use rectTransform.sizeDelta = new Vector2(width, height);

            return uiPanel;
        }

        public static GameObject CreateUIButton(GameObject parent, Sprite NewSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.standard = NewSprite;

            PL.log.LogMessage(PL.PREFIX + "   Creating UI Button");
            GameObject uiButton = UIControls.CreateButton(uiResources);
            uiButton.transform.SetParent(parent.transform, false);

            return uiButton;
        }

        public static GameObject CreateUIToggle(GameObject parent, Sprite BgSprite, Sprite customCheckmarkSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.standard = BgSprite;
            uiResources.checkmark = customCheckmarkSprite;

            PL.log.LogMessage(PL.PREFIX + "   Creating UI Toggle");
            GameObject uiToggle = UIControls.CreateToggle(uiResources);
            uiToggle.transform.SetParent(parent.transform, false);

            return uiToggle;
        }

        public static GameObject CreateUISlider(GameObject parent, Sprite BgSprite, Sprite FillSprite, Sprite KnobSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;
            uiResources.standard = FillSprite;
            uiResources.knob = KnobSprite;

            PL.log.LogMessage(PL.PREFIX + "   Creating UI Slider");
            GameObject uiSlider = UIControls.CreateSlider(uiResources);
            uiSlider.transform.SetParent(parent.transform, false);

            return uiSlider;
        }

        public static GameObject CreateUIInputField(GameObject parent, Sprite BgSprite, Il2CppSystem.String textColor)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.inputField = BgSprite;

            PL.log.LogMessage(PL.PREFIX + "   Creating UI InputField");
            GameObject uiInputField = UIControls.CreateInputField(uiResources);
            uiInputField.transform.SetParent(parent.transform, false);

            var textComps = uiInputField.GetComponentsInChildren<Text>();
            foreach (var text in textComps)
            {
                text.color = HTMLString2Color(textColor).Unbox<Color32>();
            }

            return uiInputField;
        }

        public static GameObject CreateUIDropDown(GameObject parent, Sprite BgSprite, Sprite ScrollbarSprite, Sprite DropDownSprite, Sprite CheckmarkSprite, Sprite customMaskSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            // Set the Background and Handle Image
            uiResources.standard = BgSprite;
            // Set the Scrollbar Background Image
            uiResources.background = ScrollbarSprite;
            // Set the Dropdown Handle Image
            uiResources.dropdown = DropDownSprite;
            // Set the Checkmark Image
            uiResources.checkmark = CheckmarkSprite;
            // Set the Viewport Mask
            uiResources.mask = customMaskSprite;

            PL.log.LogMessage(PL.PREFIX + "   Creating UI DropDown");
            var uiDropdown = UIControls.CreateDropdown(uiResources);
            uiDropdown.transform.SetParent(parent.transform, false);

            return uiDropdown;
        }

        public static GameObject CreateUIImage(GameObject parent, Sprite BgSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;
            PL.log.LogMessage(PL.PREFIX + "   Creating UI Image");
            GameObject uiImage = UIControls.CreateImage(uiResources);
            uiImage.transform.SetParent(parent.transform, false);

            return uiImage;
        }

        public static GameObject CreateUIRawImage(GameObject parent, Sprite BgSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;

            PL.log.LogMessage(PL.PREFIX + "   Creating UI RawImage");
            GameObject uiRawImage = UIControls.CreateRawImage(uiResources);
            uiRawImage.transform.SetParent(parent.transform, false);

            return uiRawImage;
        }

        public static GameObject CreateUIScrollbar(GameObject parent, Sprite ScrollbarSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = ScrollbarSprite;

            PL.log.LogMessage(PL.PREFIX + "   Creating UI Scrollbar");
            GameObject uiScrollbar = UIControls.CreateScrollbar(uiResources);
            uiScrollbar.transform.SetParent(parent.transform, false);

            return uiScrollbar;
        }

        public static GameObject CreateUIScrollView(GameObject parent, Sprite BgSprite, Sprite customMaskSprite, Sprite customScrollbarSprite)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            // These 2 all need to be the same for some reason, I think due to scrollview automation.
            uiResources.background = BgSprite;
            uiResources.knob = BgSprite;

            uiResources.standard = customScrollbarSprite;
            uiResources.mask = customMaskSprite;

            PL.log.LogMessage(PL.PREFIX + "   Creating UI ScrollView");
            GameObject uiScrollView = UIControls.CreateScrollView(uiResources);
            uiScrollView.transform.SetParent(parent.transform, false);

            return uiScrollView;
        }

        public static GameObject CreateUIText(GameObject parent, Sprite BgSprite, Il2CppSystem.String textColor, string LabelText)
        {
            UIControls.Resources uiResources = new UIControls.Resources();

            uiResources.background = BgSprite;

            PL.log.LogMessage(PL.PREFIX + "   Creating UI Text");
            GameObject uiText = UIControls.CreateText(uiResources);
            uiText.transform.SetParent(parent.transform, false);

            Text text = uiText.GetComponent<Text>();
            //uiText.transform.GetChild(0).GetComponent<Text>().font = (Font)Resources.GetBuiltinResource(Font.Il2CppType, "Arial.ttf"); // Invalid Cast
            text.color = HTMLString2Color(textColor).Unbox<Color32>();
            text.text = LabelText;

            return uiText;
        }

        public static Texture2D FindTexture(string name)
        {
            var Textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (Texture2D tex in Textures)
            {
                if (tex == null) continue;
                if (tex.name == name)
                {
                    return tex;
                }
            }
            return null;
        }

        public static Sprite FindSprite(string name)
        {
            var Sprites = Resources.FindObjectsOfTypeAll<Sprite>();
            foreach(var sprite in Sprites)
            {
                if(sprite != null && sprite.name != null && sprite.name == name)
                {
                    return sprite;
                }
                break;
            }
            return null;
        }
    }
}
