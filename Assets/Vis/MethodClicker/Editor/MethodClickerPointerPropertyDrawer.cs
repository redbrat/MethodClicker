using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Vis.MethodClicker.Editor
{
    [CustomPropertyDrawer(typeof(MethodClickerPointer))]
    public class MethodClickerPointerPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return base.CreatePropertyGUI(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (GUI.Button(position, property.name))
            {
                //Debug.Log($"attribute = {attribute}");
                //Debug.Log($"fieldInfo = {fieldInfo}");
                //Debug.Log($"fieldInfo.FieldType = {fieldInfo.FieldType}");
                //Debug.Log($"fieldInfo.ReflectedType = {fieldInfo.ReflectedType}");
                //Debug.Log($"fieldInfo attrs count = {fieldInfo.CustomAttributes.Count()}");
                Debug.Log($"fieldInfo.DeclaringType = {fieldInfo.DeclaringType}");
                var parent = property.serializedObject.targetObject;
                if (fieldInfo.CustomAttributes.Any() && fieldInfo.CustomAttributes.Count(customAttributeData => typeof(MethodClickerAttribute).IsAssignableFrom(customAttributeData.AttributeType)) > 0)
                    invokeMethod(property.name, fieldInfo.GetValue(parent), parent, fieldInfo.GetCustomAttribute<MethodClickerAttribute>());
                else
                    invokeMethod(property.name, fieldInfo.GetValue(parent), parent);
            }
        }

        private void invokeMethod(string fieldName, object pointerObject, Object parentObject, MethodClickerAttribute attributeObject = null)
        {
            var methodName = attributeObject?.MethodName;
            if (string.IsNullOrEmpty(methodName))
                methodName = findNextMethod(fieldName, fieldInfo.DeclaringType);
            if (string.IsNullOrEmpty(methodName))
                return;
            
            //    //var member = findType(parent.GetType(), attribute.MethodName);

            //    //var members = parent.GetType().GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            //    //var reachedNext = false;
            //    //for (int i = 0; i < members.Length; i++)
            //    //{
            //    //    var currentMember = members[i];
            //    //    if (!reachedNext)
            //    //    {
            //    //        if (currentMember is FieldInfo && (currentMember as FieldInfo).GetValue(parent) == pointer)
            //    //            reachedNext = true;
            //    //        else if (currentMember is PropertyInfo && (currentMember as PropertyInfo).GetValue(parent) == pointer)
            //    //            reachedNext = true;
            //    //    }
            //    //    else if (currentMember is MethodInfo)
            //    //    {
            //    //        var method = currentMember as MethodInfo;
            //    //        tryInvoke(parent, method);
            //    //        return;
            //    //    }
            //    //}
            //}
            //else
            //{
                var method = findMethod(parentObject.GetType(), methodName);
                if (method == null)
                    Debug.LogError($"[MethodClicker]: Can't find method/function with name \"{methodName}\"!");
                else
                    tryInvoke(parentObject, method);

                //var members = parent.GetType().GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy | BindingFlags.);
                //for (int i = 0; i < members.Length; i++)
                //{
                //    Debug.Log($"Member {i + 1}: {members[i].Name}");
                //}
                //Debug.Log($"attribute method = {attribute.MethodName}");
                //var method = parent.GetType().GetMethod(attribute.MethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy);
                //if (method == null)
                //{
                //    Debug.LogError($"[MethodClicker]: Can't find method/function with name \"{attribute.MethodName}\"!");
                //    return;
                //}
                //tryInvoke(parent, method);
                return;
            //}

            //var editors = Resources.FindObjectsOfTypeAll<UnityEditor.Editor>();
            //for (int i = 0; i < editors.Length; i++)
            //{
            //    Debug.Log($"{i+1}: {editors[i].GetType().FullName}. target = {(editors[i] as UnityEditor.Editor).target.GetType().FullName}");
            //}

            //var inspectorWindow = Resources.FindObjectsOfTypeAll<UnityEditor.Editor>().Where(w => w.GetType().FullName == "UnityEditor.InspectorWindow").First();

            //enumerateMembers(inspectorWindow.GetType(), 0);
            //var inspectorPath = AssetDatabase.GetAssetPath(inspectorWindow);
            //Debug.Log($"inspectorPath = {inspectorPath}");
            //var assets = AssetDatabase.LoadAllAssetsAtPath(inspectorPath);
            //Debug.Log($"assets.Length = {assets.Length}");
            //for (int i = 0; i < assets.Length; i++)
            //{
            //    Debug.Log($"{i + 1}: {assets[i].name} ({assets[i].GetType().FullName})");
            //}
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
                Debug.LogError($"[MethodClicker] Cannot locate script with class {className} declared! Try to put your classes to scripts with corresponding name.");
                return null;
            }

            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetsWithSimilarNames[0]);

            var pointerRegex = new Regex($@"(\bpublic\b|\bprivate\b|\bprotected\b|binternal\b)?\s*({nameof(MethodClickerPointer)})\s*({fieldName})\s*");
            var index = pointerRegex.Match(script.text).Index;
            if (index < 0)
            {
                Debug.LogError($"[MethodClicker] Cannot locate pointer with name {fieldName} within a class {className}!");
                return null;
            }

            var afterGoing = script.text.Substring(index);

            var voidMethodRegex = new Regex($@"(\bpublic\b|\bprivate\b|\bprotected\b|binternal\b)?\s*\b(void)\b\s+([\w\d_]+)\s*\((.*)\)\s*");
            var match = voidMethodRegex.Match(afterGoing);
            if (!match.Success)
            {
                Debug.LogError($"[MethodClicker] Cannot locate parameterless and returning void method or function after pointer with name {fieldName} within a class {className}!");
                return null;
            }

            var methodNameGroup = match.Groups[3];
            Debug.Log($"Method that goes right after pointer is void {methodNameGroup}()");



            Debug.Log($"index = {index}");
            //Debug.Log($"halves.length = {halves.Length}");
            Debug.Log($"text = {script.text}");

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
                Debug.LogError($"[MethodClicker]: Target method/function must have no parameters!");
                return;
            }
            if (method.ReturnType != typeof(void))
            {
                Debug.LogError($"[MethodClicker]: Target method/function must return void!");
                return;
            }

            method.Invoke(parent, null);
        }

        private void enumerateMembers(Type type, int indentLevel)
        {
            if (indentLevel > 7)
                return;
            var members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public);
            var indent = "";
            for (int i = 0; i < indentLevel; i++)
                indent += "    ";
            for (int i = 0; i < members.Length; i++)
            {
                Debug.Log($"{indent}member {i + 1}: {members[i].Name} ({members[i].GetType().FullName})");
                enumerateMembers(members[i].GetType(), indentLevel + 1);
            }
        }
    }
}
