#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class CustomGuiCodeExample : MonoBehaviour
{
    public int GeneratedNumber;
    public McPtr Click;
    void GenerateSomeNumber()
    {
        GeneratedNumber = Random.Range(0, 1000);
#if UNITY_EDITOR

        Click.ArbitraryData = GeneratedNumber;
        Click.ArbitraryGetPropertyHeightOverride = originalHeight => originalHeight + 32;
        Click.ArbitraryGuiCode = (rect, serializedProperty) =>
        {
            var boxRect = rect;
            boxRect.height = 32;
            EditorGUI.HelpBox(boxRect, string.Format("Generated number is {0}!", (int)Click.ArbitraryData), MessageType.Info);
        };
#endif
    }
}
