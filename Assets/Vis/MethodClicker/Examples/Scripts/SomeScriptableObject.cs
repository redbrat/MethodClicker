using UnityEngine;

namespace Vis.MethodClicker.Examples
{
    [CreateAssetMenu(fileName = "test", menuName = "test/test", order = 0)]
    public class SomeScriptableObject : ScriptableObject
    {
        public string Text1 = "This...";
        public string Text2 = "object...";
        public string Text3 = "is...";
        public string Text4 = "very...";
        public string Text5 = "scriptable!";

        [SerializeField]
        private MethodClickerPointer By_the_way_you_can_press_this_button = new MethodClickerPointer() { ButtonHeight = 20 };

        public string Text6 = "The End.";

        private void SomethingScripable() => Debug.Log("It's working.");
    }
}
