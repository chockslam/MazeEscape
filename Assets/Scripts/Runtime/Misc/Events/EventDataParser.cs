using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventDataParser
{
    public static int ExtractInt(IEventData eventData)
    {
        int intValue;

        if (eventData is StringEventData stringData)
        {
            if (!int.TryParse(stringData.StringValue, out intValue))
                intValue = 0;
        }
        else if (eventData is IntEventData intData)
        {
            intValue = intData.IntValue;
        }
        else if (eventData is FloatEventData floatData)
        {
            intValue = (int)floatData.FloatValue;
        }
        else if (eventData is BoolEventData boolData)
        { 
            intValue = boolData.BoolValue ? 1 : 0;
        }
        else
        {
            Debug.LogError($"Cannot parse {eventData.GetType()} into int");
            intValue = 0;
        }

        return intValue;
    }

    public static float ExtractFloat(IEventData eventData)
    {
        float floatValue;

        if (eventData is StringEventData stringData)
        {
            if (!float.TryParse(stringData.StringValue, out floatValue))
                floatValue = 0f;
        }
        else if (eventData is IntEventData intData)
        {
            floatValue = intData.IntValue;
        }
        else if (eventData is FloatEventData floatData)
        {
            floatValue = floatData.FloatValue;
        }
        else if (eventData is BoolEventData boolData)
        {
            floatValue = boolData.BoolValue ? 1f : 0f;
        }
        else
        {
            Debug.LogError($"Cannot parse {eventData.GetType()} into float");
            floatValue = 0f;
        }

        return floatValue;
    }

}
