using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Vis.MethodClicker.Editor
{
    [CustomPropertyDrawer(typeof(McPtr), true)]
    public class MethodClickerPointerPropertyDrawer : PropertyDrawer
    {
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

            var valuesCounter = 0;
            var lineHeight = result;
            while (methodClickerPointer.GetType().GetField($"Value{valuesCounter++ + 1}") != null)
                result += lineHeight;
            
            if (methodClickerPointer.ArbitraryGetPropertyHeightOverride != null)
                result = methodClickerPointer.ArbitraryGetPropertyHeightOverride.Invoke(lineHeight);

            return result;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var originalBackgroundColor = GUI.backgroundColor;
            var originalContentColor = GUI.contentColor;
            
            var methodClickerPointer = fieldInfo.GetValue(property.serializedObject.targetObject) as McPtr;

            var formattedName = string.IsNullOrEmpty(methodClickerPointer.ButtonText) ? formatName(property.name) : methodClickerPointer.ButtonText;

            var formattedPosition = formatPosition(methodClickerPointer, position);
            var lineHeight = formattedPosition.height;
            var currentY = formattedPosition.y;

            var valuesCounter = 0;
            while (methodClickerPointer.GetType().GetField($"Value{valuesCounter++ + 1}") != null) {}
            valuesCounter -= 1;

            GUI.backgroundColor = methodClickerPointer.BackgroundColor;
            GUI.contentColor = methodClickerPointer.BackgroundColor;
            if (valuesCounter > 0)
                GUI.Box(new Rect(formattedPosition.x, currentY, formattedPosition.width, lineHeight * valuesCounter), GUIContent.none);
            GUI.backgroundColor = originalBackgroundColor;
            GUI.contentColor = originalContentColor;
            
            for (int i = 0; i < valuesCounter; i++)
            {
                var rect = new Rect(formattedPosition.x, currentY, formattedPosition.width, lineHeight);
                var fieldValue = methodClickerPointer.GetType().GetField($"Value{i + 1}");
                var fieldName = methodClickerPointer.GetType().GetField($"Value{i + 1}Label");
                var name = (string)fieldName.GetValue(methodClickerPointer);
                name = string.IsNullOrEmpty(name) ? null : name;
                if (fieldValue.FieldType == typeof(int))
                    fieldValue.SetValue(methodClickerPointer, EditorGUI.IntField(rect, name ?? fieldValue.Name, (int)fieldValue.GetValue(methodClickerPointer)));
                else if (fieldValue.FieldType == typeof(bool))
                    fieldValue.SetValue(methodClickerPointer, EditorGUI.Toggle(rect, name ?? fieldValue.Name, (bool)fieldValue.GetValue(methodClickerPointer)));
                else if (fieldValue.FieldType == typeof(string))
                    fieldValue.SetValue(methodClickerPointer, EditorGUI.TextField(rect, name ?? fieldValue.Name, (string)fieldValue.GetValue(methodClickerPointer)));
                else if (fieldValue.FieldType == typeof(float))
                    fieldValue.SetValue(methodClickerPointer, EditorGUI.FloatField(rect, name ?? fieldValue.Name, (float)fieldValue.GetValue(methodClickerPointer)));
                else if (fieldValue.FieldType == typeof(Vector2))
                    fieldValue.SetValue(methodClickerPointer, EditorGUI.Vector2Field(rect, name ?? fieldValue.Name, (Vector2)fieldValue.GetValue(methodClickerPointer)));
                else if (fieldValue.FieldType == typeof(Vector3))
                    fieldValue.SetValue(methodClickerPointer, EditorGUI.Vector3Field(rect, name ?? fieldValue.Name, (Vector3)fieldValue.GetValue(methodClickerPointer)));
                else if (fieldValue.FieldType == typeof(Vector4))
                    fieldValue.SetValue(methodClickerPointer, EditorGUI.Vector4Field(rect, name ?? fieldValue.Name, (Vector4)fieldValue.GetValue(methodClickerPointer)));
                else if (fieldValue.FieldType == typeof(Vector2Int))
                    fieldValue.SetValue(methodClickerPointer, EditorGUI.Vector2IntField(rect, name ?? fieldValue.Name, (Vector2Int)fieldValue.GetValue(methodClickerPointer)));
                else if (fieldValue.FieldType == typeof(Vector3Int))
                    fieldValue.SetValue(methodClickerPointer, EditorGUI.Vector3IntField(rect, name ?? fieldValue.Name, (Vector3Int)fieldValue.GetValue(methodClickerPointer)));
                else if (typeof(Enum).IsAssignableFrom(fieldValue.FieldType))
                {
                    if (fieldValue.FieldType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
                        fieldValue.SetValue(methodClickerPointer, EditorGUI.EnumFlagsField(rect, name ?? fieldValue.Name, (Enum)fieldValue.GetValue(methodClickerPointer)));
                    else
                        fieldValue.SetValue(methodClickerPointer, EditorGUI.EnumPopup(rect, name ?? fieldValue.Name, (Enum)fieldValue.GetValue(methodClickerPointer)));
                }
                currentY += lineHeight;
            }

            var formattedStyle = default(GUIStyle);

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

            if (GUI.Button(new Rect(formattedPosition.x, currentY, formattedPosition.width, lineHeight), formattedName, formattedStyle))
            {
                var parent = property.serializedObject.targetObject;
                var attrs = fieldInfo.GetCustomAttributes(typeof(MethodClickerAttribute), false);
                if (attrs.Length > 0)
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

            if (methodClickerPointer.ArbitraryGuiDataChangingCode != null)
            {
                var arbitraryCodePosition = position;
                arbitraryCodePosition.y += methodClickerPointer.ButtonHeight;
                arbitraryCodePosition.height -= methodClickerPointer.ButtonHeight;

                methodClickerPointer.ArbitraryData = methodClickerPointer.ArbitraryGuiDataChangingCode.Invoke(arbitraryCodePosition, property, methodClickerPointer.ButtonHeight, methodClickerPointer.ArbitraryData);
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
                .Where(path => Path.GetFileNameWithoutExtension(path) == className && Path.GetExtension(path) == ".cs")
                .ToArray();

            if (assetsWithSimilarNames.Length == 0)
            {
                Debug.LogError(string.Format("[MethodClicker] Cannot locate script with class {0} declared! Try to put your classes to scripts with corresponding name.", className));
                return null;
            }

            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetsWithSimilarNames[0]);

            // var pointerRegex = new Regex(string.Format(@"(\bpublic\b|\bprivate\b|\bprotected\b|binternal\b)?\s*({0})\s*({1})\s*", "McPtr", fieldName));
            var pointerRegex = new Regex(string.Format(@"(\bpublic\b|\bprivate\b|\bprotected\b|binternal\b)?\s*({0})[^;]*\s+({1})\b", "McPtr", fieldName));
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
