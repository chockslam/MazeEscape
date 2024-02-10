using System;
using UnityEngine;

public sealed class Player : Character, IPoolable
{

    [SerializeField] CharacterStats m_stats;

    InputHandle m_inputHandler;

    float m_health;
    bool m_isDead = false;
    bool m_hasWon = false;
    Vector3 m_mazeEndPosition;

    void Update()
    {
        if (m_isDead)
            return;

        HandleInput();

        if (MathHelper.DistanceOnXZPlane(transform.position, m_mazeEndPosition) < 0.5f)
        {
            m_hasWon = true;
            m_eventObserver.NotifySubscribers(new GameEvent(EventType.GameVictory, null));
        }
    }

    public override void OnObjectSpawn()
    {
        m_health = m_stats.maxHealth;
        m_isDead = false;
        m_hasWon = false;

        ResetPlayerState();
    }

    public override void SetEventObserver(Observer<GameEvent> eventManager)
    {
        base.SetEventObserver(eventManager);

        m_eventObserver.Subscribe(TakeDamage);
        m_eventObserver.NotifySubscribers(new GameEvent(EventType.PlayerInitialize, new() { new CustomEventData<PlayerHealthData>(new PlayerHealthData(m_health)) }));
    }

    public void SetMazeEndPosition(Vector3 endPosition)
    {
        m_mazeEndPosition = endPosition;
    }

    public bool IsDead()
    {
        return m_isDead;
    }

    public bool HasWon() 
    {
        return m_hasWon;
    }

    public void InitializeInputHanlder(Joystick joystick)
    {
        m_inputHandler ??= new InputHandle();
        m_inputHandler.SetJoystick(joystick);
    }

    protected override void Move(Vector3 normalizedDirection, float speed)
    {
        if (m_isDead || m_hasWon)
            return;

        base.Move(normalizedDirection, speed);
    }

    public override void OnObjectReturn()
    {
        m_inputHandler?.DeactivateInput();
    }

    void TakeDamage(GameEvent gameEvent) 
    {
        if (gameEvent.Type != EventType.PlayerHit)
            return;

        foreach (var eventData in gameEvent.EventDataList)
        {
            m_health -= EventDataParser.ExtractFloat(eventData);

            if (m_health <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        m_isDead = true;
        m_eventObserver.NotifySubscribers(new GameEvent(EventType.GameOver, null));
    }


    void HandleInput()
    {     
        m_inputHandler?.HandleInput();

        if (!m_inputHandler?.IsInputActive ?? false)
            return;
            
        Vector3 direction = new Vector3(m_inputHandler.MovementDirection.x, 0f, m_inputHandler.MovementDirection.y);
        Move(direction, m_characterMovementSettings.speed);
    }

    void ResetPlayerState()
    {
        gameObject.SetActive(true);
    }

}
