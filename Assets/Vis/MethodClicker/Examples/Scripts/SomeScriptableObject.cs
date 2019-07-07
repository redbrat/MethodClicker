using UnityEngine;

namespace Vis.MethodClicker.Examples
{
    public class SomeScriptableObject : ScriptableObject
    {
        public string Text1 = "This...";
        public string Text2 = "object...";
        public string Text3 = "is...";
        public string Text4 = "very...";
        public string Text5 = "scriptable!";

#pragma warning disable
        [SerializeField]
        private McPtr By_the_way_you_can_press_this_button = new McPtr() { ButtonHeight = 20 };
#pragma warning enable

        public string Text6 = "The End.";

        private void SomethingScripable()
        {
            Debug.Log("It's working.");
        }
    }
}
