using UnityEngine;

namespace Vis.MethodClicker.Examples
{
    public class SomeClass : MonoBehaviour
    {
        public int SomeId = 141547;
        public string SomeName = "Gubert";
        public long SomeTime = 15432148846435;
        public Texture SomeTexture;

        public McPtr TestClicker = new McPtr()
        {
            ButtonText = "<b>Hello</b> <i>World</i> ©",
            BackgroundColor = Color.cyan * 0.3f,
            ContentColor = Color.white,
            ButtonX = 60,
            ButtonY = 30,
            ButtonHeight = 60,
            ButtonWidth = 220,
            ButtonTextSize = 22,
            PaddingBottom = 14
        };
        void Test1()
        {
            Debug.Log("Hello World!");
        }

        [SerializeField]
        private McPtr _test2; //If a pointer is private, make sure it's marked with SerializeField attribute!
        void Test2()
        {
            Debug.Log("Test2");
        }

        public McPtr Test___3; //Invokes the nearest next method - Test3 method
        [MethodClicker("Test4")]
        public McPtr Test___4; //Invokes Test4 method as explicitly asked in attribute
        void Test3()
        {
            Debug.Log("Test3");
        }

        void Test4()
        {
            Debug.Log("Test4");
        }
    }
}
