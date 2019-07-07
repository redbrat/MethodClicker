using System.Collections.Generic;
using UnityEngine;

namespace Vis.MethodClicker.Examples
{
    public class ManyExposedThings : MonoBehaviour
    {
        public int SomeId = 141547;
        public string SomeName = "Gubert";
        public long SomeTime = 15432148846435;
        public Texture SomeTexture;
        public bool Alive;
        [MethodClicker("Ressurect")]
        public McPtr _Ressurect = new McPtr() { BackgroundColor = Color.green, ButtonWidth = 120, ButtonHeight = 40, ButtonText = "<b>Ressurect!!!</b>" };
        public bool SomeBool1 = false;
        public bool SomeBool2 = true;
        public bool SomeBool3 = false;
        [TextArea]
        public string LongText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        [MethodClicker("ReadText")]
        public McPtr Read_Text = new McPtr() { BackgroundColor = Color.grey, FontStyle = FontStyle.Bold };
        public long AnotherInt = 78;
        public string ShortText = "Lorem ipsum dolor sit...";
        public List<Object> SomeList = new List<Object>() { null, null, null, null, null, null };
        public int Int1 = 1;
        public int Int2 = 2;
        public int Int3 = 3;
        public McPtr Try____This____Value = new McPtr() { BackgroundColor = Color.yellow };
        public int Int4 = 4;
        public int Int5 = 5;

        public void TrySome()
        {
            Debug.Log(string.Format("The value is {0} - it's very good...", Int4));
        }

        public void Ressurect()
        {
            Alive = true;
        }

        public void ReadText()
        {
            Debug.Log(LongText);
        }
    }
}
