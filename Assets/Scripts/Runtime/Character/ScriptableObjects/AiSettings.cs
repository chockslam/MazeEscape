using UnityEngine;

[CreateAssetMenu(fileName = "AiSettings", menuName = "Character/AI Settings", order = 0)]
public class AiSettings : ScriptableObject
{
    public float stoppingDistance = 0.1f;
    public float detectionRadius = 1f;
    public float chaseSpeed = 2f;
    public float damage = 20f;
    public float damageCoolDownInSeconds = 3.0f;
    public float fieldOfViewAngle = 45.0f;
    public float searchDuration = 3.5f;
}