using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMovementSettings", menuName = "Character/Character Movement Settings", order = 0)]
public class CharacterMovementSettings : ScriptableObject
{
    public float bodyRadius = 0.1f;
    public float speed = 2.0f;
    public float rotationSpeed = 50.0f;
}