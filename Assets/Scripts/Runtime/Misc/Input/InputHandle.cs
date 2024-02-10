public class InputHandle
{
    public UnityEngine.Vector2 MovementDirection { get; private set; }
    public bool IsInputActive { get; private set; }

    Joystick m_joystick;

    public void SetJoystick(Joystick joystick)
    {
        m_joystick = joystick;
    }

    public void HandleInput()
    {
#if UNITY_EDITOR && UNITY_ANDROID
        HandleKeyboardInput();
#endif
        HandleTouchInput();
    }

    public void HandleKeyboardInput()
    {
        float horizontal = UnityEngine.Input.GetAxisRaw("Horizontal");
        float vertical = UnityEngine.Input.GetAxisRaw("Vertical");

        MovementDirection = new UnityEngine.Vector2(horizontal, vertical);
        IsInputActive = MovementDirection.magnitude > 0.01f;
    }

    public void HandleTouchInput()
    {
        if (m_joystick != null)
        {
            MovementDirection = new UnityEngine.Vector2(-m_joystick.Horizontal, -m_joystick.Vertical);
            IsInputActive = MovementDirection.magnitude > 0.1f;
        }
    }

    public void DeactivateInput() 
    {
        IsInputActive = false;
        MovementDirection = UnityEngine.Vector2.zero;
    }

}
