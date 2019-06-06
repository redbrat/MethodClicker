using System;
using UnityEngine;

namespace Vis.MethodClicker
{
    [Serializable]
    public class MethodClickerPointer
    {
        [NonSerialized]
        public string ButtonText;

        [NonSerialized]
        public int ButtonTextSize = 14;
        [NonSerialized]
        public bool RichText = true;

        [NonSerialized]
        public GUIStyle Style;

        [NonSerialized]
        public Color ContentColor = Color.black;
        [NonSerialized]
        public Color BackgroundColor = Color.white;

        [NonSerialized]
        public FontStyle FontStyle = FontStyle.Normal;

        [NonSerialized]
        public float ButtonX;
        [NonSerialized]
        public float ButtonY;
        [NonSerialized]
        public float ButtonHeight;
        [NonSerialized]
        public float ButtonWidth;
    }
}
