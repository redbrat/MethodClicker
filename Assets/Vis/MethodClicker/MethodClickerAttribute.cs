using UnityEngine;

namespace Vis.MethodClicker
{
    public class MethodClickerAttribute : PropertyAttribute
    {
        public readonly string MethodName;
        public MethodClickerAttribute(string methodName = null) => MethodName = methodName;
    }
}
