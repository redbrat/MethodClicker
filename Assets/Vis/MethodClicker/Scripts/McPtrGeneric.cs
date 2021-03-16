using System;

[Serializable]
public class McPtr<T> : McPtr
{
    [NonSerialized]
    public T Value1;
    [NonSerialized]
    public string Value1Label;
}

[Serializable]
public class McPtr<T1, T2> : McPtr
{
    [NonSerialized]
    public T1 Value1;
    [NonSerialized]
    public T2 Value2;
    [NonSerialized]
    public string Value1Label;
    [NonSerialized]
    public string Value2Label;
}

[Serializable]
public class McPtr<T1, T2, T3> : McPtr
{
    [NonSerialized]
    public T1 Value1;
    [NonSerialized]
    public T2 Value2;
    [NonSerialized]
    public T3 Value3;
    [NonSerialized]
    public string Value1Label;
    [NonSerialized]
    public string Value2Label;
    [NonSerialized]
    public string Value3Label;
}

[Serializable]
public class McPtr<T1, T2, T3, T4> : McPtr
{
    [NonSerialized]
    public T1 Value1;
    [NonSerialized]
    public T2 Value2;
    [NonSerialized]
    public T3 Value3;
    [NonSerialized]
    public T4 Value4;
    [NonSerialized]
    public string Value1Label;
    [NonSerialized]
    public string Value2Label;
    [NonSerialized]
    public string Value3Label;
    [NonSerialized]
    public string Value4Label;
}

[Serializable]
public class McPtr<T1, T2, T3, T4, T5> : McPtr
{
    [NonSerialized]
    public T1 Value1;
    [NonSerialized]
    public T2 Value2;
    [NonSerialized]
    public T3 Value3;
    [NonSerialized]
    public T4 Value4;
    [NonSerialized]
    public T5 Value5;
    [NonSerialized]
    public string Value1Label;
    [NonSerialized]
    public string Value2Label;
    [NonSerialized]
    public string Value3Label;
    [NonSerialized]
    public string Value4Label;
    [NonSerialized]
    public string Value5Label;
}

[Serializable]
public class McPtr<T1, T2, T3, T4, T5, T6> : McPtr
{
    [NonSerialized]
    public T1 Value1;
    [NonSerialized]
    public T2 Value2;
    [NonSerialized]
    public T3 Value3;
    [NonSerialized]
    public T4 Value4;
    [NonSerialized]
    public T5 Value5;
    [NonSerialized]
    public T6 Value6;
    [NonSerialized]
    public string Value1Label;
    [NonSerialized]
    public string Value2Label;
    [NonSerialized]
    public string Value3Label;
    [NonSerialized]
    public string Value4Label;
    [NonSerialized]
    public string Value5Label;
    [NonSerialized]
    public string Value6Label;
}

[Serializable]
public class McPtr<T1, T2, T3, T4, T5, T6, T7> : McPtr
{
    [NonSerialized]
    public T1 Value1;
    [NonSerialized]
    public T2 Value2;
    [NonSerialized]
    public T3 Value3;
    [NonSerialized]
    public T4 Value4;
    [NonSerialized]
    public T5 Value5;
    [NonSerialized]
    public T6 Value6;
    [NonSerialized]
    public T7 Value7;
    [NonSerialized]
    public string Value1Label;
    [NonSerialized]
    public string Value2Label;
    [NonSerialized]
    public string Value3Label;
    [NonSerialized]
    public string Value4Label;
    [NonSerialized]
    public string Value5Label;
    [NonSerialized]
    public string Value6Label;
    [NonSerialized]
    public string Value7Label;
}