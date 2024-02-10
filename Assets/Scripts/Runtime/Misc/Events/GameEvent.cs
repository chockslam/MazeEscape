using System.Collections.Generic;

public class GameEvent
{
    public EventType Type { get; private set; }
    public List<IEventData> EventDataList { get; private set; }

    public GameEvent(EventType type, List<IEventData> eventData)
    {
        Type = type;
        EventDataList = eventData;
    }
}