using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Il2CppSystem.Collections.Generic;
using Object = UnityEngine.Object;
using PL = DiscoTranslatorFinalCut.PluginLoader;
using Lang = DiscoTranslatorFinalCut.Translator.TranslatorLanguages;
using DiscoTranslatorFinalCut.Translator.Audio;
using DiscoTranslatorFinalCut.Translator.Images;
using DiscoTranslatorFinalCut.Translator.Exporter;
using LocalizationCustomSystem;
using UnityEngine.SceneManagement;

namespace DiscoTranslatorFinalCut.Translator.UI
{
    class UIModMenu : MonoBehaviour
    {
        public static GameObject obj = null;
        private static bool Initialized = false;
        private PanelMover panelMover;
        private static Scene m_Scene;
        private static string currentLanguage;
        private enum UIElementStatus { Normal, Hover};

        private static GameObject uiCanvas = null;
        private GameObject uiPanel;
        private GameObject uiPanelHeader;
        private GameObject uiPanelBody;
        private GameObject uiPanelBodyContents;
        private GameObject uiPanelBodyContents_layer1;
        private GameObject uiPanelBodyContents_layer1_1;
        private GameObject uiPanelBodyContents_layer1_2;
        private GameObject uiPanelBodyContents_layer2;
        private GameObject uiPanelBodyContents_layer3;
        private GameObject uiPanelBodyContents_layer4;
        private GameObject uiPanelFooter;
        private GameObject uiPanelFooterContents;
        private GameObject uiPanelPlay;
        private GameObject uiPanelPause;
        private GameObject uiPanelStop;
        private GameObject uiPanelVolume;
        private GameObject uiPanelTimer;

        private GameObject uiLabelHead;
        private GameObject uiLabelTimer;
        private GameObject uiLabelLanguageToggler;
        private GameObject uiLabelAudioReplacerToggler;
        private GameObject uiLabelAudioWidgetToggler;
        private GameObject uiLabelImageReplacerToggler;
        public GameObject uiLabelStatusMessage;

        private GameObject uiButtonClose;
        private GameObject uiButtonPlay;
        private GameObject uiButtonPause;
        private GameObject uiButtonStop;
        private GameObject uiButtonCatalog;
        private GameObject uiButtonImages;
        private GameObject uiButtonReload;
        private GameObject uiButtonAudio;

        private GameObject uiSliderVolume;
        //private GameObject uiSliderClipTime;

        private GameObject uiToggleLanguageEnable;
        private GameObject uiToggleAudioRelacer;
        private GameObject uiToggleAudioWidget;
        private GameObject uiToggleImageReplacer;
        
        private static Texture2D cursorTexture;
        private static Texture2D cursorDefault;
        private static Texture2D cursorPointer;

        private static CursorMode cursorMode = CursorMode.Auto;

        private static Vector2 hotSpot = Vector2.zero;

        private static int Delayer = 0;

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);

            new UIModMenu(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<UIModMenu>()).Pointer);

            return obj;
        }

        public UIModMenu(IntPtr ptr) : base(ptr) { }

        public void Initialize()
        {
            cursorDefault = UIManager.FindTexture("curs 1");
            cursorPointer = UIManager.FindTexture("curs-interact");
            currentLanguage = TranslatorManager.currentLanguage;

            m_Scene = SceneManager.GetActiveScene();

            if (uiToggleLanguageEnable != null) uiToggleLanguageEnable.GetComponent<Toggle>().enabled = true;
            if (uiToggleAudioRelacer != null) uiToggleAudioRelacer.GetComponent<Toggle>().enabled = true;
            if (uiToggleImageReplacer != null) uiToggleImageReplacer.GetComponent<Toggle>().enabled = true;
            if (uiToggleAudioWidget != null) uiToggleAudioWidget.GetComponent<Toggle>().enabled = true;

            if (uiToggleLanguageEnable != null) uiToggleLanguageEnable.GetComponent<Toggle>().SetIsOnWithoutNotify(TranslatorManager.EnableTranslation);
            if (uiToggleAudioRelacer != null) uiToggleAudioRelacer.GetComponent<Toggle>().SetIsOnWithoutNotify(AudioManager.EnableAudioImport);
            if (uiToggleImageReplacer != null) uiToggleImageReplacer.GetComponent<Toggle>().SetIsOnWithoutNotify(ImageManager.EnableTextureImport);
            if (uiToggleAudioWidget != null) uiToggleAudioWidget.GetComponent<Toggle>().SetIsOnWithoutNotify(AudioManager.EnableAudioWidget);

            Initialized = true;
        }

        public void Awake()
        {
            try
            {
                //Main Container
                uiCanvas = UIManager.CreateUICanvas(obj.name);
                if (uiCanvas != null)
                {
                    uiCanvas.SetActive(false);
                }
                //Main Container

                //Main Panel
                var uiPanelwidth = 512;
                var uiPanelheight = 512;
                uiPanel = UIManager.CreateUIPanel(uiCanvas, uiPanelwidth.ToString(), uiPanelheight.ToString(), null);
                uiPanel.name = "UIMainMenuPanelGO";
                uiPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                //Main Panel

                //Position rules : -X move to left , +Y move to up, +X move to rigth, -Y move to down
                //Every object spawn inside the parent at middle-center.

                /*Parent
                /******************\
                /                  \
                /##################\
                /   Children Here  \
                /##################\
                /                  \
                /******************\
                */

                //Main Panel Header
                var uiPanelHeaderwidth = 512;
                var uiPanelHeaderheight = 54;
                var tex = ResourcesManager.loadTexture(0, 0, "uiPanelHeader.png");
                tex.name = "uiPanelHeaderTexture";
                var sprite = UIManager.CreateSpriteFrmTexture(tex);
                sprite.name = "uiPanelHeaderSprite";
                uiPanelHeader = UIManager.CreateUISubPanel(uiPanel, uiPanelHeaderheight.ToString(), uiPanelHeaderwidth.ToString(), null);
                uiPanelHeader.name = "UIMainMenuPanelHeaderGO";
                uiPanelHeader.GetComponent<Image>().color = UIManager.HTMLString2Color("#1B242BFF").Unbox<Color32>();
                uiPanelHeader.GetComponent<Image>().sprite = sprite;
                var r = uiPanelHeader.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x, r.position.y + 256 - 27);
                //Main Panel Header

                //Main Panel Body
                var uiPanelBodywidth = 512;
                var uiPanelBodyheight = 406;
                tex = ResourcesManager.loadTexture(0, 0, "uiPanelBody.png");
                tex.name = "uiPanelBodyTexture";
                sprite = UIManager.CreateSpriteFrmTexture(tex);
                sprite.name = "uiPanelBodySprite";
                uiPanelBody = UIManager.CreateUISubPanel(uiPanel, uiPanelBodyheight.ToString(), uiPanelBodywidth.ToString(), null);
                uiPanelBody.name = "UIMainMenuPanelBodyGO";
                uiPanelBody.GetComponent<Image>().color = UIManager.HTMLString2Color("#2C3642FF").Unbox<Color32>();
                uiPanelBody.GetComponent<Image>().sprite = sprite;
                //Main Panel Body

                //Main Panel Footer
                var uiPanelFooterwidth = 512;
                var uiPanelFooterheight = 54;
                tex = ResourcesManager.loadTexture(0, 0, "uiPanelFooter.png");
                tex.name = "uiPanelFooterTexture";
                sprite = UIManager.CreateSpriteFrmTexture(tex);
                sprite.name = "uiPanelFooterSprite";
                uiPanelFooter = UIManager.CreateUISubPanel(uiPanel, uiPanelFooterheight.ToString(), uiPanelFooterwidth.ToString(), null);
                uiPanelFooter.name = "UIMainMenuPanelFooterGO";
                uiPanelFooter.GetComponent<Image>().color = UIManager.HTMLString2Color("#1B242BFF").Unbox<Color32>();
                uiPanelFooter.GetComponent<Image>().sprite = sprite;
                r = uiPanelFooter.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x, r.position.y - 256 + 27);
                //Main Panel Footer

                //Main Panel Header Label
                uiLabelHead = UIManager.CreateUIText(uiPanelHeader, null, "#FFFFFFFF", PL.FULLNAME);
                uiLabelHead.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, uiPanelHeaderwidth - 30);
                uiLabelHead.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, uiPanelHeaderheight - 30);
                r = uiLabelHead.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x + 15, r.position.y);
                var t = uiLabelHead.GetComponent<Text>();
                t.name = "uiPanelHeaderLabel";
                t.AssignDefaultFont();
                t.alignment = TextAnchor.MiddleLeft;
                t.resizeTextForBestFit = true;
                //Main Panel Header Label

                //Main Panel Header Close Button
                tex = ResourcesManager.loadTexture(0, 0, "uiCloseButtonNormalTexture.png");
                tex.name = "uiCloseButtonNormalTexture";
                sprite = UIManager.CreateSpriteFrmTexture(tex);
                sprite.name = "uiCloseButtonNormalSprite";
                uiButtonClose = UIManager.CreateUIButton(uiPanelHeader, null);
                uiButtonClose.name = "uiButtonCloseButton";
                var b = uiButtonClose.GetComponent<Button>();
                b.interactable = true;
                var i = uiButtonClose.GetComponent<Image>();
                i.name = "uiCloseButtonNormalImage";
                i.color = UIManager.HTMLString2Color("#FFFFFFFF").Unbox<Color32>();
                i.sprite = sprite;
                i.enabled = true;
                t = uiButtonClose.GetComponentInChildren<Text>();
                t.name = "uiPanelHeaderCloseText";
                t.text = " ";
                r = uiButtonClose.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 28);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
                r.position = new Vector2(r.position.x + (uiPanelHeaderwidth / 2) - 38, r.position.y);
                //Main Panel Header Close Button

                //Main Panel Footer Sub Panel
                var uiPanelFooterSubPanelwidth = 492;
                var uiPanelFooterSubPanelheight = 34;
                uiPanelFooterContents = UIManager.CreateUISubPanel(uiPanelFooter, uiPanelFooterSubPanelheight.ToString(), uiPanelFooterSubPanelwidth.ToString(), null);
                uiPanelFooterContents.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                //Main Panel Footer Sub Panel

                //Main Panel Footer Sub Panel Play Panel
                var uiPanelPlaywidth = 34;
                var uiPanelPlayheight = 34;
                uiPanelPlay = UIManager.CreateUISubPanel(uiPanelFooterContents, uiPanelPlayheight.ToString(), uiPanelPlaywidth.ToString(), null);
                uiPanelPlay.name = "UIMainMenuPanelPanelPlayGO";
                uiPanelPlay.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                r = uiPanelPlay.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x - (uiPanelFooterSubPanelwidth/2) + 17, r.position.y);
                //Main Panel Footer Sub Panel Play Panel

                //Main Panel Footer Sub Panel Pause Panel
                var uiPanelPausewidth = 34;
                var uiPanelPauseheight = 34;
                uiPanelPause = UIManager.CreateUISubPanel(uiPanelFooterContents, uiPanelPauseheight.ToString(), uiPanelPausewidth.ToString(), null);
                uiPanelPause.name = "UIMainMenuPanelPanelPauseGO";
                uiPanelPause.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                r = uiPanelPause.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x - (uiPanelFooterSubPanelwidth / 2) + 17 + 34 + 10, r.position.y);
                //Main Panel Footer Sub Panel Pause Panel

                //Main Panel Footer Sub Panel Stop Panel
                var uiPanelStopwidth = 34;
                var uiPanellStopheight = 34;
                uiPanelStop = UIManager.CreateUISubPanel(uiPanelFooterContents, uiPanellStopheight.ToString(), uiPanelStopwidth.ToString(), null);
                uiPanelStop.name = "UIMainMenuPanelPanelStopGO";
                uiPanelStop.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                r = uiPanelStop.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x - (uiPanelFooterSubPanelwidth / 2) + 17 + 34 + 34 + 20, r.position.y);
                //Main Panel Footer Sub Panel Stop Panel

                //Main Panel Footer Sub Panel Volume Panel
                var uiPanelVolumewidth = 228;
                var uiPanellVolumeheight = 48;
                uiPanelVolume = UIManager.CreateUISubPanel(uiPanelFooterContents, uiPanellVolumeheight.ToString(), uiPanelVolumewidth.ToString(), null);
                uiPanelVolume.name = "UIMainMenuPanelPanelVolumeGO";
                uiPanelVolume.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                //Main Panel Footer Sub Panel Volume Panel

                //Main Panel Footer Sub Panel Timer Panel
                var uiPanelTimerwidth = 122;
                var uiPanellTimerheight = 34;
                uiPanelTimer = UIManager.CreateUISubPanel(uiPanelFooterContents, uiPanellTimerheight.ToString(), uiPanelTimerwidth.ToString(), null);
                uiPanelTimer.name = "UIMainMenuPanelPanelTimerGO";
                uiPanelTimer.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                r = uiPanelTimer.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x + (uiPanelFooterSubPanelwidth / 2) - 61, r.position.y);
                //Main Panel Footer Sub Panel Timer Panel

                //uiButtonPlay
                tex = ResourcesManager.loadTexture(0, 0, "uiPlayButtonNormalTexture.png");
                tex.name = "uiPlayButtonNormalTexture";
                sprite = UIManager.CreateSpriteFrmTexture(tex);
                sprite.name = "uiPlayButtonNormalSprite";
                uiButtonPlay = UIManager.CreateUIButton(uiPanelPlay, null);
                uiButtonPlay.name = "uiButtonPlayButton";
                b = uiButtonPlay.GetComponent<Button>();
                b.interactable = true;
                i = uiButtonPlay.GetComponent<Image>();
                i.name = "uiPlayButtonNormalImage";
                i.color = UIManager.HTMLString2Color("#FFFFFFFF").Unbox<Color32>();
                i.sprite = sprite;
                i.enabled = true;
                t = uiButtonPlay.GetComponentInChildren<Text>();
                t.name = "uiPanelFooterPlayText";
                t.text = " ";
                r = uiButtonPlay.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 34);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 34);
                //uiButtonPlay

                //uiButtonPause
                tex = ResourcesManager.loadTexture(0, 0, "uiPauseButtonNormalTexture.png");
                tex.name = "uiPauseButtonNormalTexture";
                sprite = UIManager.CreateSpriteFrmTexture(tex);
                sprite.name = "uiPauseButtonNormalSprite";
                uiButtonPause = UIManager.CreateUIButton(uiPanelPause, null);
                uiButtonPause.name = "uiButtonPauseButton";
                b = uiButtonPause.GetComponent<Button>();
                b.interactable = true;
                i = uiButtonPause.GetComponent<Image>();
                i.name = "uiPauseButtonNormalImage";
                i.color = UIManager.HTMLString2Color("#FFFFFFFF").Unbox<Color32>();
                i.sprite = sprite;
                i.enabled = true;
                t = uiButtonPause.GetComponentInChildren<Text>();
                t.name = "uiPanelFooterPauseText";
                t.text = " ";
                r = uiButtonPause.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 34);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 34);
                //uiButtonPause

                //uiButtonStop
                tex = ResourcesManager.loadTexture(0, 0, "uiStopButtonNormalTexture.png");
                tex.name = "uiStopButtonNormalTexture";
                sprite = UIManager.CreateSpriteFrmTexture(tex);
                sprite.name = "uiStopButtonNormalSprite";
                uiButtonStop = UIManager.CreateUIButton(uiPanelStop, null);
                uiButtonStop.name = "uiButtonStopButton";
                b = uiButtonStop.GetComponent<Button>();
                b.interactable = true;
                i = uiButtonStop.GetComponent<Image>();
                i.name = "uiStopButtonNormalImage";
                i.color = UIManager.HTMLString2Color("#FFFFFFFF").Unbox<Color32>();
                i.sprite = sprite;
                i.enabled = true;
                t = uiButtonStop.GetComponentInChildren<Text>();
                t.name = "uiPanelFooterStopText";
                t.text = " ";
                r = uiButtonStop.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 34);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 34);
                //uiButtonStop

                //uiSliderVolume
                var sliderBg = ResourcesManager.loadTexture(0, 0, "uiSliderBackground.png");
                var sliderSpriteBg = UIManager.CreateSpriteFrmTexture(sliderBg);
                var sliderFill = ResourcesManager.loadTexture(0, 0, "uiSliderFiller.png");
                var sliderSpriteFill = UIManager.CreateSpriteFrmTexture(sliderFill);
                var sliderKnob = ResourcesManager.loadTexture(0, 0, "uiSliderKnob.png");
                var sliderSpriteKnob = UIManager.CreateSpriteFrmTexture(sliderKnob);
                sliderSpriteKnob.name = "uiSliderKnob";
                uiSliderVolume = UIManager.CreateUISlider(uiPanelVolume, sliderSpriteBg, sliderSpriteFill, sliderSpriteKnob);
                uiSliderVolume.name = "uiSliderVolume";
                r = uiSliderVolume.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 228);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 34);
                var s = uiSliderVolume.GetComponent<Slider>();
                s.interactable = true;
                s.minValue = 0f;
                s.maxValue = 1f;
                //uiSliderVolume

                //uiLabelTimer
                uiLabelTimer = UIManager.CreateUIText(uiPanelTimer, null, "#FFFFFFFF", "00:00 / 00:00");
                uiLabelTimer.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, uiPanelTimerwidth);
                uiLabelTimer.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, uiPanellTimerheight);
                t = uiLabelTimer.GetComponent<Text>();
                t.name = "uiPanelFooterLabelTimer";
                t.AssignDefaultFont();
                t.alignment = TextAnchor.MiddleCenter;
                t.resizeTextForBestFit = true;
                //uiLabelTimer

                //Main Panel Body Contents
                var uiPanelBodyContentswidth = 492;
                var uiPanelBodyContentsheight = 400;
                uiPanelBodyContents = UIManager.CreateUISubPanel(uiPanelBody, uiPanelBodyContentsheight.ToString(), uiPanelBodyContentswidth.ToString(), null);
                uiPanelBodyContents.name = "UIMainMenuPanelBodyContentsGO";
                uiPanelBodyContents.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                //Main Panel Body Contents

                //Main Panel Body Contents_layer1
                var uiPanelBodyContents_layer1width = 492;
                var uiPanelBodyContents_layer1height = 100;
                uiPanelBodyContents_layer1 = UIManager.CreateUISubPanel(uiPanelBodyContents, uiPanelBodyContents_layer1height.ToString(), uiPanelBodyContents_layer1width.ToString(), null);
                uiPanelBodyContents_layer1.name = "UIMainMenuPanelBodyContents_layer1GO";
                uiPanelBodyContents_layer1.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                r = uiPanelBodyContents_layer1.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x, r.position.y + (uiPanelBodyContentsheight / 2) - 50);
                //Main Panel Body Contents_layer1

                //uiPanelBodyContents_layer1_1
                var uiPanelBodyContents_layer1_1width = 492;
                var uiPanelBodyContents_layer1_1height = 50;
                uiPanelBodyContents_layer1_1 = UIManager.CreateUISubPanel(uiPanelBodyContents_layer1, uiPanelBodyContents_layer1_1height.ToString(), uiPanelBodyContents_layer1_1width.ToString(), null);
                uiPanelBodyContents_layer1_1.name = "UIMainMenuPanelBodyContents_layer1_1GO";
                uiPanelBodyContents_layer1_1.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                r = uiPanelBodyContents_layer1_1.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x, r.position.y + (uiPanelBodyContents_layer1height / 2) - 25);
                //uiPanelBodyContents_layer1_1

                //uiPanelBodyContents_layer1_2
                var uiPanelBodyContents_layer1_2width = 492;
                var uiPanelBodyContents_layer1_2height = 50;
                uiPanelBodyContents_layer1_2 = UIManager.CreateUISubPanel(uiPanelBodyContents_layer1, uiPanelBodyContents_layer1_2height.ToString(), uiPanelBodyContents_layer1_2width.ToString(), null);
                uiPanelBodyContents_layer1_2.name = "UIMainMenuPanelBodyContents_layer1_2GO";
                uiPanelBodyContents_layer1_2.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                r = uiPanelBodyContents_layer1_2.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x, r.position.y + (uiPanelBodyContents_layer1height / 2) - 75);
                //uiPanelBodyContents_layer1_2

                //uiToggleLanguageEnable
                var offtex = ResourcesManager.loadTexture(0, 0, "uiToggleOFF.png");
                offtex.name = "uiToggleOFFTexture";
                var offsprite = UIManager.CreateSpriteFrmTexture(offtex);
                offsprite.name = "uiToggleOFFSprite";
                var ontex = ResourcesManager.loadTexture(0, 0, "uiToggleON.png");
                ontex.name = "uiToggleONTexture";
                var onsprite = UIManager.CreateSpriteFrmTexture(ontex);
                onsprite.name = "uiToggleOFFSprite";
                uiToggleLanguageEnable = UIManager.CreateUIToggle(uiPanelBodyContents_layer1_1, offsprite, onsprite);
                r = uiToggleLanguageEnable.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 62);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 34);
                r.position = new Vector2(r.position.x - (uiPanelBodyContents_layer1_1width / 2) + 51, r.position.y);
                //uiToggleLanguageEnable

                //uiLabelLanguageToggler
                uiLabelLanguageToggler = UIManager.CreateUIText(uiPanelBodyContents_layer1_1, null, "#FFFFFFFF", Lang.GetTerm("DISCOFC_OPTIONS_TOGGLE_TRANSLATOR"));
                r = uiLabelLanguageToggler.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 144);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);
                r.position = new Vector2(r.position.x - (uiPanelBodyContents_layer1_1width / 2) + 164, r.position.y);
                t = uiLabelLanguageToggler.GetComponent<Text>();
                t.name = "uiLabelLanguageToggler";
                t.AssignDefaultFont();
                t.alignment = TextAnchor.MiddleLeft;
                t.fontStyle = FontStyle.Bold;
                t.fontSize = 18;
                //uiLabelLanguageToggler

                //uiToggleAudioRelacer
                uiToggleAudioRelacer = UIManager.CreateUIToggle(uiPanelBodyContents_layer1_1, offsprite, onsprite);
                r = uiToggleAudioRelacer.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 62);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 34);
                r.position = new Vector2(r.position.x - (uiPanelBodyContents_layer1_1width / 2) + 287, r.position.y);
                //uiToggleImageReplacer

                //uiLabelAudioReplacerToggler
                uiLabelAudioReplacerToggler = UIManager.CreateUIText(uiPanelBodyContents_layer1_1, null, "#FFFFFFFF", Lang.GetTerm("DISCOFC_OPTIONS_TOGGLE_AUDIO_REPLACE"));
                r = uiLabelAudioReplacerToggler.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 144);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);
                r.position = new Vector2(r.position.x + (uiPanelBodyContents_layer1_1width / 2) - 92, r.position.y);
                t = uiLabelAudioReplacerToggler.GetComponent<Text>();
                t.name = "uiLabelAudioReplacerToggler";
                t.AssignDefaultFont();
                t.alignment = TextAnchor.MiddleLeft;
                t.fontStyle = FontStyle.Bold;
                t.fontSize = 18;
                //uiLabelAudioReplacerToggler

                //uiToggleImageReplacer
                uiToggleImageReplacer = UIManager.CreateUIToggle(uiPanelBodyContents_layer1_2, offsprite, onsprite);
                r = uiToggleImageReplacer.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 62);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 34);
                r.position = new Vector2(r.position.x - (uiPanelBodyContents_layer1_2width / 2) + 51, r.position.y);
                //uiToggleImageReplacer

                //uiLabelImageReplacerToggler
                uiLabelImageReplacerToggler = UIManager.CreateUIText(uiPanelBodyContents_layer1_2, null, "#FFFFFFFF", Lang.GetTerm("DISCOFC_OPTIONS_TOGGLE_IMAGES_REPLACE"));
                r = uiLabelImageReplacerToggler.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 144);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);
                r.position = new Vector2(r.position.x - (uiPanelBodyContents_layer1_2width / 2) + 164, r.position.y);
                t = uiLabelImageReplacerToggler.GetComponent<Text>();
                t.name = "uiLabelImageReplacerToggler";
                t.AssignDefaultFont();
                t.alignment = TextAnchor.MiddleLeft;
                t.fontStyle = FontStyle.Bold;
                t.fontSize = 18;
                //uiLabelImageReplacerToggler

                //uiToggleAudioWidget
                uiToggleAudioWidget = UIManager.CreateUIToggle(uiPanelBodyContents_layer1_2, offsprite, onsprite);
                r = uiToggleAudioWidget.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 62);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 34);
                r.position = new Vector2(r.position.x - (uiPanelBodyContents_layer1_2width / 2) + 287, r.position.y);
                //uiToggleAudioWidget

                //uiLabelAudioWidgetToggler
                uiLabelAudioWidgetToggler = UIManager.CreateUIText(uiPanelBodyContents_layer1_2, null, "#FFFFFFFF", Lang.GetTerm("DISCOFC_OPTIONS_TOGGLE_AUDIO_WIDGET"));
                r = uiLabelAudioWidgetToggler.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 144);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);
                r.position = new Vector2(r.position.x + (uiPanelBodyContents_layer1_2width / 2) - 92, r.position.y);
                t = uiLabelAudioWidgetToggler.GetComponent<Text>();
                t.name = "uiLabelAudioWidgetToggler";
                t.AssignDefaultFont();
                t.alignment = TextAnchor.MiddleLeft;
                t.fontStyle = FontStyle.Bold;
                t.fontSize = 18;
                //uiLabelAudioReplacerToggler

                //Main Panel Body Contents_layer2
                var uiPanelBodyContents_layer2width = 492;
                var uiPanelBodyContents_layer2height = 240;
                uiPanelBodyContents_layer2 = UIManager.CreateUISubPanel(uiPanelBodyContents, uiPanelBodyContents_layer2height.ToString(), uiPanelBodyContents_layer2width.ToString(), null);
                uiPanelBodyContents_layer2.name = "UIMainMenuPanelBodyContents_layer2GO";
                uiPanelBodyContents_layer2.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                r = uiPanelBodyContents_layer2.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x, r.position.y + (uiPanelBodyContentsheight / 2) - 220);
                //Main Panel Body Contents_layer2

                //uiButtonCatalog
                var texNormal = ResourcesManager.loadTexture(0, 0, "uiMenuButtonNormalTexture.png");
                texNormal.name = "uiMenuButtonNormalTexture";
                var spriteNormal = UIManager.CreateSpriteFrmTexture(texNormal);
                spriteNormal.name = "uiMenuButtonNormalSprite";
                uiButtonCatalog = UIManager.CreateUIButton(uiPanelBodyContents_layer2, null);
                uiButtonCatalog.name = "uiButtonCatalogButton";
                b = uiButtonCatalog.GetComponent<Button>();
                b.interactable = true;
                i = uiButtonCatalog.GetComponent<Image>();
                i.name = "uiMenuButtonNormalImage";
                i.color = UIManager.HTMLString2Color("#FFFFFFFF").Unbox<Color32>();
                i.sprite = spriteNormal;
                i.enabled = true;
                t = uiButtonCatalog.GetComponentInChildren<Text>();
                t.name = "uiMenuButtonNormalText";
                t.text = Lang.GetTerm("DISCOFC_EXPORT_CATALOG");
                t.AssignDefaultFont();
                t.alignment = TextAnchor.MiddleCenter;
                t.fontStyle = FontStyle.Bold;
                t.fontSize = 18;
                t.color = Color.white;
                r = uiButtonCatalog.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);
                r.position = new Vector2(r.position.x, r.position.y + (uiPanelBodyContents_layer2height / 2) - 25 -5);
                //uiButtonCatalog

                //uiButtonImages
                texNormal = ResourcesManager.loadTexture(0, 0, "uiMenuButtonNormalTexture.png");
                texNormal.name = "uiMenuButtonNormalTexture";
                spriteNormal = UIManager.CreateSpriteFrmTexture(texNormal);
                spriteNormal.name = "uiMenuButtonNormalSprite";
                uiButtonImages = UIManager.CreateUIButton(uiPanelBodyContents_layer2, null);
                uiButtonImages.name = "uiButtonImagesButton";
                b = uiButtonImages.GetComponent<Button>();
                b.interactable = true;
                i = uiButtonImages.GetComponent<Image>();
                i.name = "uiButtonImagesNormalImage";
                i.color = UIManager.HTMLString2Color("#FFFFFFFF").Unbox<Color32>();
                i.sprite = spriteNormal;
                i.enabled = true;
                t = uiButtonImages.GetComponentInChildren<Text>();
                t.name = "uiButtonImagesNormalText";
                t.text = Lang.GetTerm("DISCOFC_EXPORT_LOADED_IMAGES");
                t.AssignDefaultFont();
                t.alignment = TextAnchor.MiddleCenter;
                t.fontStyle = FontStyle.Bold;
                t.fontSize = 18;
                t.color = Color.white;
                r = uiButtonImages.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);
                r.position = new Vector2(r.position.x, r.position.y + (uiPanelBodyContents_layer2height / 2) - 10 - 25 - 50 - 5);
                //uiButtonImages

                //uiButtonReload
                texNormal = ResourcesManager.loadTexture(0, 0, "uiMenuButtonNormalTexture.png");
                texNormal.name = "uiMenuButtonNormalTexture";
                spriteNormal = UIManager.CreateSpriteFrmTexture(texNormal);
                spriteNormal.name = "uiMenuButtonNormalSprite";
                uiButtonReload = UIManager.CreateUIButton(uiPanelBodyContents_layer2, null);
                uiButtonReload.name = "uiButtonReloadButton";
                b = uiButtonReload.GetComponent<Button>();
                b.interactable = true;
                i = uiButtonReload.GetComponent<Image>();
                i.name = "uiButtonReloadNormalImage";
                i.color = UIManager.HTMLString2Color("#FFFFFFFF").Unbox<Color32>();
                i.sprite = spriteNormal;
                i.enabled = true;
                t = uiButtonReload.GetComponentInChildren<Text>();
                t.name = "uiButtonReloadNormalText";
                t.text = Lang.GetTerm("DISCOFC_RELOAD_ALL_TEXT");
                t.AssignDefaultFont();
                t.alignment = TextAnchor.MiddleCenter;
                t.fontStyle = FontStyle.Bold;
                t.fontSize = 18;
                t.color = Color.white;
                r = uiButtonReload.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);
                r.position = new Vector2(r.position.x, r.position.y + (uiPanelBodyContents_layer2height / 2) -10 - 25 - 50 - 10 - 50 - 5);
                //uiButtonReload

                //uiButtonAudio
                texNormal = ResourcesManager.loadTexture(0, 0, "uiMenuButtonNormalTexture.png");
                texNormal.name = "uiMenuButtonNormalTexture";
                spriteNormal = UIManager.CreateSpriteFrmTexture(texNormal);
                spriteNormal.name = "uiMenuButtonNormalSprite";
                uiButtonAudio = UIManager.CreateUIButton(uiPanelBodyContents_layer2, null);
                uiButtonAudio.name = "uiButtonAudioButton";
                b = uiButtonAudio.GetComponent<Button>();
                b.interactable = true;
                i = uiButtonAudio.GetComponent<Image>();
                i.name = "uiButtonAudioNormalImage";
                i.color = UIManager.HTMLString2Color("#FFFFFFFF").Unbox<Color32>();
                i.sprite = spriteNormal;
                i.enabled = true;
                t = uiButtonAudio.GetComponentInChildren<Text>();
                t.name = "uiButtonAudioNormalText";
                t.text = Lang.GetTerm("DISCOFC_EXPORT_LOADED_AUDIO");
                t.AssignDefaultFont();
                t.alignment = TextAnchor.MiddleCenter;
                t.fontStyle = FontStyle.Bold;
                t.fontSize = 18;
                t.color = Color.white;
                r = uiButtonAudio.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);
                r.position = new Vector2(r.position.x, r.position.y + (uiPanelBodyContents_layer2height / 2) - 10 - 25 - 50 - 10 - 50 - 10 - 50 - 5);
                //uiButtonAudio

                //Main Panel Body Contents_layer3
                var uiPanelBodyContents_layer3width = 492;
                var uiPanelBodyContents_layer3height = 24;
                uiPanelBodyContents_layer3 = UIManager.CreateUISubPanel(uiPanelBodyContents, uiPanelBodyContents_layer3height.ToString(), uiPanelBodyContents_layer3width.ToString(), null);
                uiPanelBodyContents_layer3.name = "UIMainMenuPanelBodyContents_layer3GO";
                uiPanelBodyContents_layer3.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                r = uiPanelBodyContents_layer3.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x, r.position.y + (uiPanelBodyContentsheight / 2) - 352);
                //Main Panel Body Contents_layer3

                //Main Panel Body Contents_layer4
                var uiPanelBodyContents_layer4width = 492;
                var uiPanelBodyContents_layer4height = 36;
                uiPanelBodyContents_layer4 = UIManager.CreateUISubPanel(uiPanelBodyContents, uiPanelBodyContents_layer4height.ToString(), uiPanelBodyContents_layer4width.ToString(), null);
                uiPanelBodyContents_layer4.name = "UIMainMenuPanelBodyContents_layer4GO";
                uiPanelBodyContents_layer4.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                r = uiPanelBodyContents_layer4.GetComponent<RectTransform>();
                r.position = new Vector2(r.position.x, r.position.y + (uiPanelBodyContentsheight / 2) - 382);
                //Main Panel Body Contents_layer4

                //uiLabelStatusMessage
                uiLabelStatusMessage = UIManager.CreateUIText(uiPanelBodyContents_layer4, null, "#FFFFFFFF", Lang.GetTerm("DISCOFC_WELCOME_STATUS_MESSAGE"));
                r = uiLabelStatusMessage.GetComponent<RectTransform>();
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 450);
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 26);
                t = uiLabelStatusMessage.GetComponent<Text>();
                t.name = "uiLabelStatusMessage";
                t.AssignDefaultFont();
                t.alignment = TextAnchor.MiddleCenter;
                t.fontSize = 18;
                //uiLabelStatusMessage



                /*
                var Fonts = Resources.FindObjectsOfTypeAll<Font>();
                foreach(var font in Fonts)
                {
                    PL.log.LogWarning("Font : " + font.name);
                    foreach(var subfont in font.fontNames)
                    {
                        PL.log.LogWarning("SubfontNames : " + subfont);
                    }
                }
                */
            }
            catch (Exception e)
            {
                Destroy(uiCanvas);
                enabled = false;
                PL.log.LogFatal(PL.PREFIX + " Can't create UI Menu! " + e.Message);
            }
        }

        public void Update()
        {
            if (!Initialized) { Initialize(); }

            if (uiCanvas == null) return;

            if (EventSystem.current != null && Input.GetKeyDown(KeyCode.U) && Event.current.type == EventType.KeyDown)
            {
                if (!uiCanvas.active)
                {
                    uiCanvas.SetActive(true);
                    uiLabelStatusMessage.GetComponent<Text>().text = Lang.GetTerm("DISCOFC_WELCOME_STATUS_MESSAGE");
                }
                else
                {
                    uiCanvas.SetActive(false);
                }
                Event.current.Use();
            }

            if (!uiCanvas.active) return;
            m_Scene = SceneManager.GetActiveScene();

            if (TranslatorManager.currentLanguage != currentLanguage)
            {
                uiLabelLanguageToggler.GetComponent<Text>().text = Lang.GetTerm("DISCOFC_OPTIONS_TOGGLE_TRANSLATOR");
                uiLabelAudioReplacerToggler.GetComponent<Text>().text = Lang.GetTerm("DISCOFC_OPTIONS_TOGGLE_AUDIO_REPLACE");
                uiLabelImageReplacerToggler.GetComponent<Text>().text = Lang.GetTerm("DISCOFC_OPTIONS_TOGGLE_IMAGES_REPLACE");
                uiLabelAudioWidgetToggler.GetComponent<Text>().text = Lang.GetTerm("DISCOFC_OPTIONS_TOGGLE_AUDIO_WIDGET");
                uiButtonCatalog.GetComponentInChildren<Text>().text = Lang.GetTerm("DISCOFC_EXPORT_CATALOG");
                uiButtonImages.GetComponentInChildren<Text>().text = Lang.GetTerm("DISCOFC_EXPORT_LOADED_IMAGES");
                uiButtonReload.GetComponentInChildren<Text>().text = Lang.GetTerm("DISCOFC_RELOAD_ALL_TEXT");
                uiButtonAudio.GetComponentInChildren<Text>().text = Lang.GetTerm("DISCOFC_EXPORT_LOADED_AUDIO");
                uiLabelStatusMessage.GetComponent<Text>().text = Lang.GetTerm("DISCOFC_WELCOME_STATUS_MESSAGE");

                currentLanguage = TranslatorManager.currentLanguage;
            }

            if (Input.GetMouseButton(0) && uiCanvas != null)
            {
                if (panelMover != null)
                {
                    panelMover.OnDrag();
                }
                else
                {
                    panelMover = PanelMover.Create("MainPanelMoverGO").GetComponent<PanelMover>();
                    panelMover.ComponentToMove = uiPanel;
                    panelMover.AreaToMove = uiPanelHeader;
                    panelMover.Start();
                }
            }
            else
            {
                if (panelMover != null)
                {
                    panelMover.Stop = true;
                    panelMover.Start();
                }
            }

            if (AudioManager.EnableAudioWidget)
            {
                if (!uiPanelFooterContents.active) uiPanelFooterContents.SetActive(true);
                Delayer++;
                if (Delayer > 60)
                {
                    AudioManager.GetActiveMusic();
                    UpdateTimer(uiLabelTimer);
                    Delayer = 0;
                }else if(Delayer == 1)
                {
                    uiSliderVolume.GetComponent<Slider>().value = AudioManager.GetMusicVolume();
                }

                if (isPointerInsideObject(uiButtonPlay, "button", Lang.GetTerm("DISCOFC_BUTTON_PLAY_STATUS_MESSAGE"), uiLabelStatusMessage.GetComponent<Text>()))
                {
                    if (uiCanvas.active && Input.GetMouseButtonUp(0))
                    {
                        AudioManager.PlayMusic();
                        UpdateTimer(uiLabelTimer);
                        PL.log.LogInfo(PL.PREFIX + " Play Audio");
                    }
                    return;
                }

                if (isPointerInsideObject(uiButtonPause, "button", Lang.GetTerm("DISCOFC_BUTTON_PAUSE_STATUS_MESSAGE"), uiLabelStatusMessage.GetComponent<Text>()))
                {
                    if (uiCanvas.active && Input.GetMouseButtonUp(0))
                    {
                        AudioManager.PauseMusic();
                        UpdateTimer(uiLabelTimer);
                        PL.log.LogInfo(PL.PREFIX + " Toggle Pause Audio");
                    }
                    return;
                }

                if (isPointerInsideObject(uiButtonStop, "button", Lang.GetTerm("DISCOFC_BUTTON_STOP_STATUS_MESSAGE"), uiLabelStatusMessage.GetComponent<Text>()))
                {
                    if (uiCanvas.active && Input.GetMouseButtonUp(0))
                    {
                        AudioManager.StopMusic();
                        UpdateTimer(uiLabelTimer);
                        PL.log.LogInfo(PL.PREFIX + " Stop Audio");
                    }
                    return;
                }

                if (isPointerInsideObject(uiSliderVolume, "slider", Lang.GetTerm("DISCOFC_BUTTON_VOLUME_STATUS_MESSAGE"), uiLabelStatusMessage.GetComponent<Text>()))
                {
                    if (uiCanvas.active && Input.GetMouseButton(0))
                    {
                        float minpos = uiSliderVolume.GetComponent<RectTransform>().transform.position.x - (uiSliderVolume.GetComponent<RectTransform>().rect.width / 2);
                        float maxpos = uiSliderVolume.GetComponent<RectTransform>().transform.position.x + (uiSliderVolume.GetComponent<RectTransform>().rect.width / 2);
                        float fractions = 1f / (maxpos - minpos);

                        if ((Input.mousePosition.x > minpos || Input.mousePosition.x < maxpos) && (Input.mousePosition.x - minpos > 0f))
                        {
                            try
                            {
                                uiSliderVolume.GetComponent<Slider>().value = fractions * (Input.mousePosition.x - minpos);
                                AudioManager.SetMusicVolume(uiSliderVolume.GetComponent<Slider>().value);
                            }
                            catch (Exception e)
                            {
                                PL.log.LogError(PL.PREFIX + " Can't set Music Volume! " + e.Message);
                            }
                        }
                        else
                        {
                            if (Input.mousePosition.x < minpos)
                            {
                                uiSliderVolume.GetComponent<Slider>().value = 0f;
                            }
                            else if (Input.mousePosition.x > maxpos)
                            {
                                uiSliderVolume.GetComponent<Slider>().value = 1f;
                            }
                        }
                    }
                    return;
                }
            }
            else
            {
                if (uiPanelFooterContents.active) uiPanelFooterContents.SetActive(false);
            }

            if(m_Scene.name == "Lobby")
            {
                if (!uiToggleLanguageEnable.GetComponent<Toggle>().enabled)
                {
                    uiToggleLanguageEnable.GetComponent<Toggle>().enabled = true;
                }
                if (!uiToggleAudioRelacer.GetComponent<Toggle>().enabled)
                {
                    uiToggleAudioRelacer.GetComponent<Toggle>().enabled = true;
                }
                if (!uiToggleImageReplacer.GetComponent<Toggle>().enabled)
                {
                    uiToggleImageReplacer.GetComponent<Toggle>().enabled = true;
                }

                if (isPointerInsideObject(uiToggleLanguageEnable, "toggle"))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (TranslatorManager.EnableTranslation)
                        {
                            uiToggleLanguageEnable.GetComponent<Toggle>().SetIsOnWithoutNotify(false);
                            if (TranslatorManager.isCustomLanguage)
                            {
                                if (AudioManager.EnableAudioImport)
                                {
                                    uiToggleAudioRelacer.GetComponent<Toggle>().SetIsOnWithoutNotify(false);
                                    AudioManager.SceneClips(false);
                                    AudioManager.EnableAudioImport = false;
                                    PlayerPrefs.SetInt("EnableAudioImport", 2); //2 is false;
                                    PL.log.LogInfo("Audio Importer Disabled");
                                }

                                if (ImageManager.EnableTextureImport)
                                {
                                    uiToggleImageReplacer.GetComponent<Toggle>().SetIsOnWithoutNotify(false);
                                    ImageManager.SceneTextures(false);
                                    ImageManager.TranslatedTextures(false);
                                    ImageManager.EnableTextureImport = false;
                                    PlayerPrefs.SetInt("EnableTextureImport", 2); //2 is false;
                                    PL.log.LogInfo("Texture Importer Disabled");
                                }

                                TranslatorManager.currentLanguageCode = "en";
                                TranslatorManager.currentLanguage = "English";
                                TranslatorManager.currentLanguageIndex = TranslatorManager.GetLanguageIndex(TranslatorManager.currentLanguage);
                                TranslatorManager.LoadTranslation(TranslatorManager.currentLanguage);
                                LocalizationManager.SetLanguage(TranslatorManager.currentLanguage);
                                TranslatorManager.SetSelectedLanguageOptions();
                            }
                            TranslatorManager.EnableTranslation = false;
                            PlayerPrefs.SetInt("EnableDiscoTranslation", 2); //2 is false;
                        }
                        else
                        {
                            uiToggleLanguageEnable.GetComponent<Toggle>().SetIsOnWithoutNotify(true);
                            TranslatorManager.EnableTranslation = true;
                            PlayerPrefs.SetInt("EnableDiscoTranslation", 1); //1 is true;
                        }
                    }
                    return;
                }

                if (isPointerInsideObject(uiToggleAudioRelacer, "toggle"))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (AudioManager.EnableAudioImport)
                        {
                            uiToggleAudioRelacer.GetComponent<Toggle>().SetIsOnWithoutNotify(false);
                            AudioManager.SceneClips(false);
                            AudioManager.EnableAudioImport = false;
                            PlayerPrefs.SetInt("EnableAudioImport", 2); //2 is false;
                            PL.log.LogInfo("Audio Importer Disabled");
                        }
                        else
                        {
                            uiToggleAudioRelacer.GetComponent<Toggle>().SetIsOnWithoutNotify(true);
                            AudioManager.EnableAudioImport = true;
                            AudioSwapper.obj.GetComponent<AudioSwapper>().enabled = true;
                            AudioManager.SceneClips(true);
                            PlayerPrefs.SetInt("EnableAudioImport", 1); //1 is true;
                            PL.log.LogInfo("Audio Importer Enabled");
                        }

                        if (AudioManager.EnableAudioWidget)
                        {
                            uiSliderVolume.GetComponent<Slider>().value = AudioManager.GetMusicVolume();
                        }
                    }
                    return;
                }

                if (isPointerInsideObject(uiToggleImageReplacer, "toggle"))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (ImageManager.EnableTextureImport)
                        {
                            uiToggleImageReplacer.GetComponent<Toggle>().SetIsOnWithoutNotify(false);
                            ImageManager.SceneTextures(false);
                            ImageManager.TranslatedTextures(false);
                            ImageManager.EnableTextureImport = false;
                            PlayerPrefs.SetInt("EnableTextureImport", 2); //2 is false;
                            PL.log.LogInfo("Texture Importer Disabled");
                        }
                        else
                        {
                            uiToggleImageReplacer.GetComponent<Toggle>().SetIsOnWithoutNotify(true);
                            ImageManager.EnableTextureImport = true;
                            ImageSwapper.obj.GetComponent<ImageSwapper>().enabled = true;
                            ImageManager.SceneTextures(true);
                            ImageManager.TranslatedTextures(true);
                            PlayerPrefs.SetInt("EnableTextureImport", 1); //1 is true;
                            PL.log.LogInfo("Texture Importer Enabled");
                        }
                    }
                    return;
                }
            }
            else
            {
                if(uiToggleLanguageEnable.GetComponent<Toggle>().enabled)
                {
                    uiToggleLanguageEnable.GetComponent<Toggle>().enabled = false;
                }
                if(uiToggleAudioRelacer.GetComponent<Toggle>().enabled)
                {
                    uiToggleAudioRelacer.GetComponent<Toggle>().enabled = false;
                }
                if(uiToggleImageReplacer.GetComponent<Toggle>().enabled)
                {
                    uiToggleImageReplacer.GetComponent<Toggle>().enabled = false;
                }
            }

            if (isPointerInsideObject(uiToggleAudioWidget, "toggle"))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    if (AudioManager.EnableAudioWidget)
                    {
                        uiToggleAudioWidget.GetComponent<Toggle>().SetIsOnWithoutNotify(false);
                        AudioManager.EnableAudioWidget = false;
                        PlayerPrefs.SetInt("EnableAudioWidget", 2); //2 is false;
                    }
                    else
                    {
                        uiToggleAudioWidget.GetComponent<Toggle>().SetIsOnWithoutNotify(true);
                        AudioManager.EnableAudioWidget = true;
                        PlayerPrefs.SetInt("EnableAudioWidget", 1); //1 is true;
                    }
                }
                return;
            }

            if (isPointerInsideObject(uiButtonCatalog, "text_button"))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    TranslatorManager.ExportCatalog();
                }
                return;
            }

            if (isPointerInsideObject(uiButtonImages, "text_button"))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    ExporterManager.ExportImages();
                }
                return;
            }

            if (isPointerInsideObject(uiButtonReload, "text_button"))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    TranslatorManager.ReloadAllSources();
                }
                return;
            }

            if (isPointerInsideObject(uiButtonAudio, "text_button"))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    ExporterManager.AudioExporter();
                }
                return;
            }

            if (isPointerInsideObject(uiButtonClose, "button"))
            {
                if (Input.GetMouseButtonUp(0))
                {
                    uiCanvas.SetActive(false);
                    //PL.log.LogInfo(PL.PREFIX + " Close UI");
                }
                return;
            }

            if (cursorTexture != cursorDefault)
            {
                cursorTexture = cursorDefault;
                Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            }
        }

        public static void UpdateTimer(GameObject obj)
        {
            string[] AudioData = AudioManager.GetMusicInfo();
            obj.GetComponent<Text>().text = AudioData[0] + " / " + AudioData[1];
        }

        public static bool isPointerInsideObject(GameObject obj, string obj_desc, string status_message=null, Text target=null)
        {
            if (!uiCanvas.active) return false;

            var maX = Input.mousePosition.x;
            var maY = Input.mousePosition.y;
            var goX = obj.GetComponent<RectTransform>().position.x;
            var goY = obj.GetComponent<RectTransform>().position.y;

            var rt = obj.GetComponent<RectTransform>();
            var width = rt.rect.width;
            var height = rt.rect.height;

            if ((maX < (goX + (width / 2)) && maX > (goX - (width / 2))) && (maY < (goY + (height / 2)) && maY > (goY - (height / 2))))
            {
                cursorTexture = cursorPointer;
                Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

                if (obj_desc == "button" || obj_desc == "text_button")
                {
                    var b = obj.GetComponent<Button>();
                    var oldtex = b.GetComponent<Image>().sprite.texture;

                    if (oldtex.name.Contains("Normal"))
                    {
                        var texname = b.GetComponent<Image>().sprite.texture.name.Replace("Normal", "Hover");
                        //PL.log.LogWarning("Try to load from resource : " + texname);
                        try
                        {
                            var tex = ResourcesManager.loadTexture(0, 0, texname + ".png");
                            if (tex != null)
                            {
                                oldtex.name = tex.name = texname;
                                var sprite = UIManager.CreateSpriteFrmTexture(tex);
                                b.DoSpriteSwap(sprite);
                            }
                        }
                        catch (Exception e)
                        {
                            PL.log.LogError(PL.PREFIX + " | " + e.Message);
                        }
                    }

                    if(obj_desc == "text_button")
                    {
                        obj.GetComponentInChildren<Text>().color = UIManager.HTMLString2Color("#2693D5FF").Unbox<Color32>(); //new Color(38, 147, 213, 255);
                    }
                }
                if (status_message != null && target != null) target.text = status_message;
                return true;
            }
            else
            {
                cursorTexture = cursorDefault;
                Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

                if (obj_desc == "button" || obj_desc == "text_button")
                {
                    var b = obj.GetComponent<Button>();
                    var oldtex = b.GetComponent<Image>().sprite.texture;

                    if (oldtex.name.Contains("Hover"))
                    {
                        var texname = b.GetComponent<Image>().sprite.texture.name.Replace("Hover", "Normal");
                        //PL.log.LogWarning("Try to load from resource : " + texname);
                        try
                        {
                            var tex = ResourcesManager.loadTexture(0, 0, texname + ".png");
                            if (tex != null)
                            {
                                oldtex.name = tex.name = texname;
                                var sprite = UIManager.CreateSpriteFrmTexture(tex);
                                b.DoSpriteSwap(sprite);
                            }
                        }
                        catch (Exception e)
                        {
                            PL.log.LogError(PL.PREFIX + " | " + e.Message);
                        }
                    }

                    if (obj_desc == "text_button")
                    {
                        obj.GetComponentInChildren<Text>().color = Color.white;
                    }
                }
                if (status_message != null && target != null && target.text == status_message) target.text = "";
            }

            return false;
        }
    }
}
