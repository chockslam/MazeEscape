using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventObedient
{
    void SetEventObserver(Observer<GameEvent> eventManager);
}