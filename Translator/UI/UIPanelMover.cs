using System;
using UnityEngine;
using PL = DiscoTranslatorFinalCut.PluginLoader;

namespace DiscoTranslatorFinalCut.Translator.UI
{
    public class PanelMover : MonoBehaviour
    {
        private float maX;
        private float maY;
        private float goX;
        private float goY;
        private float deltaX;
        private float deltaY;
        private float width;
        private float height;

        public static GameObject obj = null;
        public GameObject ComponentToMove;
        public GameObject AreaToMove;
        RectTransform rt;
        public bool Stop = true;
        private bool Release = true;

        public PanelMover(IntPtr ptr) : base(ptr) { }

        internal static GameObject Create(string name)
        {
            obj = new GameObject(name);
            DontDestroyOnLoad(obj);
            obj.AddComponent<PanelMover>();

            return obj;
        }

        public void Awake()
        {
            //PL.log.LogMessage("PanelMover Awake() Fired!");
        }

        public void Start()
        {
            maX = Input.mousePosition.x;
            maY = Input.mousePosition.y;
            goX = ComponentToMove.GetComponent<RectTransform>().position.x;
            goY = ComponentToMove.GetComponent<RectTransform>().position.y;
            deltaX = maX - goX;
            deltaY = maY - goY;
            Stop = false;
            Release = true;
        }

        public void OnDrag()
        {
            rt = AreaToMove.GetComponent<RectTransform>();
            width = rt.rect.width;
            height = rt.rect.height;
            maX = Input.mousePosition.x;
            maY = Input.mousePosition.y;
            goX = AreaToMove.GetComponent<RectTransform>().position.x;
            goY = AreaToMove.GetComponent<RectTransform>().position.y;

            if (!Release || (maX < (goX + (width / 2)) && maX > (goX - (width / 2))) && (maY < (goY + (height / 2)) && maY > (goY - (height / 2))))
            {
                if (Input.GetMouseButton(0))
                {
                    Release = false;
                }
                else
                {
                    Release = true;
                }

                ComponentToMove.transform.position = new Vector2(maX - deltaX, maY - deltaY);
            }
        }
    }
}