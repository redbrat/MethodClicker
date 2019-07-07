using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// It's a pointer for editor to know wich function to invoke when
/// user clicks on a button defined by that pointer. By default the
/// next void() function is invoked. That behaviour can be overrriden 
/// by adding MethodClicker attribute.
/// </summary>
[Serializable]
public class McPtr
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
    public float ButtonHeight = 22;
    [NonSerialized]
    public float ButtonWidth;

    [NonSerialized]
    public float PaddingTop = 0f;
    [NonSerialized]
    public float PaddingLeft = 0f;
    [NonSerialized]
    public float PaddingBottom = 0f;
    [NonSerialized]
    public float PaddingRight = 0f;

    public object ArbitraryData;
#if UNITY_EDITOR
    public Action<Rect, SerializedProperty> ArbitraryGuiCode;
#endif
    public Func<float, float> ArbitraryGetPropertyHeightOverride;
}
