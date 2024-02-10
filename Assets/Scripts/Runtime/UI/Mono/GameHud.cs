using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHud : UIElement
{
    [SerializeField] Image Healthbar;
    [SerializeField] Joystick joystick;

    float m_maxHealth;
    float m_health;

    public void Reset(float maxHealth)
    {
        m_maxHealth = maxHealth;
        m_health = maxHealth;
        Healthbar.fillAmount = 1.0f;
    }

    public void TakeDamage(float damage) 
    {
        m_health -= damage;
        Healthbar.fillAmount = m_health / m_maxHealth;
    }

    public Joystick GetJoystick() 
    {
        return joystick;
    }
}
