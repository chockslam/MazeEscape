using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour, IEventObedient
{
    [SerializeField] GameHud gameHUD;
    [SerializeField] UIElement gameOverScreen;
    [SerializeField] UIElement loadingScreen;
    [SerializeField] UIElement victory;

    Dictionary<EventType, IUIElement> m_stateToPanelMap;

    Observer<GameEvent> m_playerEventsObserver;

    private void Awake()
    {
        m_stateToPanelMap = new Dictionary<EventType, IUIElement>
        {
            { EventType.Initializing, loadingScreen },
            { EventType.GamePlaying, gameHUD },
            { EventType.GameOver, gameOverScreen },
            { EventType.GameVictory, victory },
            { EventType.GameRestarting, loadingScreen },
        };
    }

    void OnDestroy()
    {
        
        m_playerEventsObserver?.Unsubscribe(HandleEvent);
    }

    public void SetEventObserver(Observer<GameEvent> newPlayerEventsManager)
    {
        m_playerEventsObserver?.Unsubscribe(HandleEvent);

        m_playerEventsObserver = newPlayerEventsManager;
        m_playerEventsObserver.Subscribe(HandleEvent);
    }

    void HandleGameStateChanged(EventType state)
    {
        foreach (var panel in m_stateToPanelMap.Values)
        {
            panel.Hide();
        }

        if (m_stateToPanelMap.TryGetValue(state, out IUIElement panelToShow))
        {
            panelToShow.Show();
        }
        else
        {
            Debug.LogWarning($"No UI panel mapped for state: {state}");
        }
    }

    void HandleEvent(GameEvent gameEvent)
    {
        switch (gameEvent.Type)
        {
            case EventType.PlayerInitialize:
                SetHealthUI(gameEvent.EventDataList);
                break;
            case EventType.PlayerHit:
                UpdateHealthUI(gameEvent.EventDataList);
                break;
            case EventType.GamePlaying:
                HandleGameStateChanged(EventType.GamePlaying);
                break;
            case EventType.GameOver:
                HandleGameStateChanged(EventType.GameOver);
                break;
            case EventType.GameVictory:
                HandleGameStateChanged(EventType.GameVictory);
                break;
            case EventType.Initializing:
                HandleGameStateChanged(EventType.Initializing);
                break;
            case EventType.GameRestarting:
                HandleGameStateChanged(EventType.GameRestarting);
                break;
            default:
                Debug.LogWarning($"Unhandled player event: {gameEvent}");
                break;
        }
    }

    public Joystick GetJoystick() 
    {
        return gameHUD.GetJoystick();
    }

    // Difference of the implementation is purely for the demonstration of robustness of the system
    void SetHealthUI(List<IEventData> eventDataList)
    {
        foreach (var eventData in eventDataList) 
        {
            if (eventData is CustomEventData<PlayerHealthData> playerHealthEvent) 
            {
                gameHUD.Reset(playerHealthEvent.CustomValue.maxHealth);
            }
        }
    }
    //--
    // Difference of the implementation is purely for the demonstration of robustness of the system
    void UpdateHealthUI(List<IEventData> eventDataList)
    {
        foreach (var eventData in eventDataList)
        {
            float damage = EventDataParser.ExtractFloat(eventData);
            gameHUD.TakeDamage(damage);
        }
    }
}
