using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
//using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Vis.MethodClicker.Editor
{
    [CustomPropertyDrawer(typeof(McPtr))]
    public class MethodClickerPointerPropertyDrawer : PropertyDrawer
    {
        //public override VisualElement CreatePropertyGUI(SerializedProperty property)
        //{
        //    return base.CreatePropertyGUI(property);
        //}

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var methodClickerPointer = fieldInfo.GetValue(property.serializedObject.targetObject) as McPtr;
            var result = base.GetPropertyHeight(property, label);

            if (methodClickerPointer.ButtonHeight != default(float))
                result = methodClickerPointer.ButtonHeight;
            if (methodClickerPointer.ButtonY != default(float))
                result += methodClickerPointer.ButtonY;
            if (methodClickerPointer.PaddingTop != default(float))
                result += methodClickerPointer.PaddingTop;
            if (methodClickerPointer.PaddingBottom != default(float))
                result += methodClickerPointer.PaddingBottom;

            if (methodClickerPointer.ArbitraryGetPropertyHeightOverride != null)
                result = methodClickerPointer.ArbitraryGetPropertyHeightOverride.Invoke(result);

            return result;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var methodClickerPointer = fieldInfo.GetValue(property.serializedObject.targetObject) as McPtr;

            var formattedStyle = default(GUIStyle);

            var originalBackgroundColor = GUI.backgroundColor;
            var originalContentColor = GUI.contentColor;

            if (methodClickerPointer.Style != null)
                formattedStyle = methodClickerPointer.Style;
            else
            {
                formattedStyle = new GUIStyle(GUI.skin.button);

                formattedStyle.richText = methodClickerPointer.RichText;
                formattedStyle.fontSize = methodClickerPointer.ButtonTextSize;
                formattedStyle.fontStyle = methodClickerPointer.FontStyle;

                formattedStyle.normal.textColor = methodClickerPointer.ContentColor;
                formattedStyle.hover.textColor = methodClickerPointer.ContentColor;
                formattedStyle.active.textColor = methodClickerPointer.ContentColor;
                formattedStyle.focused.textColor = methodClickerPointer.ContentColor;

                GUI.contentColor = methodClickerPointer.ContentColor;
                GUI.backgroundColor = methodClickerPointer.BackgroundColor;
            }

            var formattedName = string.IsNullOrEmpty(methodClickerPointer.ButtonText) ? formatName(property.name) : methodClickerPointer.ButtonText;

            var formattedPosition = formatPosition(methodClickerPointer, position);

            if (GUI.Button(formattedPosition, formattedName, formattedStyle))
            {
                var parent = property.serializedObject.targetObject;
                var attrs = fieldInfo.GetCustomAttributes(typeof(MethodClickerAttribute), false);
                if (attrs.Length > 0)
                //if (fieldInfo.CustomAttributes.Any() && fieldInfo.CustomAttributes.Count(customAttributeData => typeof(MethodClickerAttribute).IsAssignableFrom(customAttributeData.AttributeType)) > 0)
                    invokeMethod(property.name, parent, attrs[0] as MethodClickerAttribute);
                else
                    invokeMethod(property.name, parent);
            }

            GUI.contentColor = originalContentColor;
            GUI.backgroundColor = originalBackgroundColor;

            if (methodClickerPointer.ArbitraryGuiCode != null)
            {
                var arbitraryCodePosition = position;
                arbitraryCodePosition.y += methodClickerPointer.ButtonHeight;
                arbitraryCodePosition.height -= methodClickerPointer.ButtonHeight;

                methodClickerPointer.ArbitraryGuiCode.Invoke(arbitraryCodePosition, property);
            }
        }

        private Rect formatPosition(McPtr methodClickerPointer, Rect position)
        {
            var result = position;

            if (methodClickerPointer.ButtonX != default(float))
                result.x = methodClickerPointer.ButtonX;
            if (methodClickerPointer.PaddingLeft != default(float))
                result.x = methodClickerPointer.PaddingLeft;
            if (methodClickerPointer.ButtonY != default(float))
                result.y += methodClickerPointer.ButtonY;
            if (methodClickerPointer.PaddingTop != default(float))
                result.y += methodClickerPointer.PaddingTop;
            if (methodClickerPointer.ButtonHeight != default(float))
                result.height = methodClickerPointer.ButtonHeight;
            if (methodClickerPointer.ButtonWidth != default(float))
                result.width = methodClickerPointer.ButtonWidth;

            return result;
        }

        private string formatName(string name)
        {
            var sb = new StringBuilder();

            var lastChar = default(char?);
            for (int i = 0; i < name.Length; i++)
            {
                var currentChar = name[i];
                lastChar = sb.Length == 0 ? null : (char?)sb[sb.Length - 1];
                if (currentChar == '_' && lastChar == null)
                    continue;
                if (lastChar == null)
                    sb.Append(currentChar.ToString().ToUpper());
                else
                {
                    if (currentChar == '_')
                        sb.Append(' ');
                    else
                    {
                        if (char.IsLower(lastChar.Value) && char.IsUpper(currentChar))
                            sb.Append(' ');

                        sb.Append(currentChar);
                    }
                }
            }

            return sb.ToString();
        }

        private void invokeMethod(string fieldName, Object parentObject, MethodClickerAttribute attributeObject = null)
        {
            var methodName = attributeObject == null ? null : attributeObject.MethodName;
            if (string.IsNullOrEmpty(methodName))
                methodName = findNextMethod(fieldName, fieldInfo.DeclaringType);
            if (string.IsNullOrEmpty(methodName))
                return;
            var method = findMethod(parentObject.GetType(), methodName);
            if (method == null)
                Debug.LogError(string.Format("[MethodClicker]: Can't find method/function with name \"{0}\"!", methodName));
            else
                tryInvoke(parentObject, method);
        }

        private string findNextMethod(string fieldName, Type declaringType)
        {
            var classNameRegex = new Regex(@"(\b[A-Za-z_]+[A-Za-z0-9_]*\b)");
            var className = classNameRegex.Match(declaringType.Name).Value;

            var assetsWithSimilarNames = AssetDatabase.FindAssets(className)
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => Path.GetFileNameWithoutExtension(path) == className)
                .ToArray();

            if (assetsWithSimilarNames.Length == 0)
            {
                Debug.LogError(string.Format("[MethodClicker] Cannot locate script with class {0} declared! Try to put your classes to scripts with corresponding name.", className));
                return null;
            }

            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetsWithSimilarNames[0]);

            var pointerRegex = new Regex(string.Format(@"(\bpublic\b|\bprivate\b|\bprotected\b|binternal\b)?\s*({0})\s*({1})\s*", "McPtr", fieldName));
            var match = pointerRegex.Match(script.text);
            if (!match.Success)
            {
                Debug.LogError(string.Format("[MethodClicker] Cannot locate pointer with name {0} within a class {1}!", fieldName, className));
                return null;
            }

            var afterGoing = script.text.Substring(match.Index);

            var voidMethodRegex = new Regex(@"(\bpublic\b|\bprivate\b|\bprotected\b|binternal\b)?\s*\b(void)\b\s+([\w\d_]+)\s*\((.*)\)\s*");
            match = voidMethodRegex.Match(afterGoing);
            if (!match.Success)
            {
                Debug.LogError(string.Format("[MethodClicker] Cannot locate parameterless and returning void method or function after pointer with name {0} within a class {1}!", fieldName, className));
                return null;
            }

            var methodNameGroup = match.Groups[3];
            return methodNameGroup.Value;
        }

        private MethodInfo findMethod(Type type, string methodName)
        {
            if (type == null)
                return null;
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (methods.Count(m => m.Name == methodName) == 0)
                return findMethod(type.BaseType, methodName);
            else
                return methods.Where(m => m.Name == methodName).First();
        }

        private void tryInvoke(Object parent, MethodInfo method)
        {
            if (method.GetParameters().Length > 0)
            {
                Debug.LogError("[MethodClicker]: Target method/function must have no parameters!");
                return;
            }
            if (method.ReturnType != typeof(void))
            {
                Debug.LogError("[MethodClicker]: Target method/function must return void!");
                return;
            }

            method.Invoke(parent, null);
        }
    }
}
