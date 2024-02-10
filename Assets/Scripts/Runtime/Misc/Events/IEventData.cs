public interface IEventData
{
    object Value { get; }
}

public class StringEventData : IEventData
{
    public string StringValue { get; }
    public object Value => StringValue;

    public StringEventData(string stringValue)
    {
        StringValue = stringValue;
    }
}

public class IntEventData : IEventData
{
    public int IntValue { get; }
    public object Value => IntValue;

    public IntEventData(int intValue)
    {
        IntValue = intValue;
    }
}

public class FloatEventData : IEventData
{
    public float FloatValue { get; }
    public object Value => FloatValue;

    public FloatEventData(float floatValue)
    {
        FloatValue = floatValue;
    }
}

public class BoolEventData : IEventData
{
    public bool BoolValue { get; }
    public object Value => BoolValue;

    public BoolEventData(bool boolValue)
    {
        BoolValue = boolValue;
    }
}

public class CustomEventData<T> : IEventData
{
    public T CustomValue { get; private set; }
    public object Value => CustomValue;

    public CustomEventData(T customValue)
    {
        CustomValue = customValue;
    }
}