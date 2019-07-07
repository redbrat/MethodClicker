using UnityEngine;

public class MethodClickerAttribute : PropertyAttribute
{
    public readonly string MethodName;

    public MethodClickerAttribute(string methodName = null)
    {
        MethodName = methodName;
    }
}
