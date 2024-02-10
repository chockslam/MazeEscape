using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, IPoolable, IEventObedient
{

    [SerializeField] protected CharacterMovementSettings m_characterMovementSettings;
    
    protected Observer<GameEvent> m_eventObserver;

    public virtual void SetEventObserver(Observer<GameEvent> eventManager)
    {
        m_eventObserver = eventManager;
    }

    // TODO: (Potential improvement) prevent tunelling when FPS is low (using Time.fixedDeltaTime for Physics calculations in fixed update and the movement itself in LateUpdate)
    protected virtual void Move(Vector3 normalizedDirection, float speed)
    {
        Vector3 movement = normalizedDirection * speed * Time.deltaTime;
        float distance = MathHelper.MagnitudeOnXZPlane(movement);

        if (!Physics.SphereCast(transform.position, m_characterMovementSettings.bodyRadius, normalizedDirection, out RaycastHit hit, distance))
        {
            RotateTowards(normalizedDirection);
            transform.position += movement;
        }
        else if (normalizedDirection.x != 0.0f && normalizedDirection.z != 0.0f)
        {
            Vector3 slideDirection = Vector3.ProjectOnPlane(normalizedDirection, hit.normal);
            slideDirection.y = 0.0f;

            RotateTowards(slideDirection);

            bool isCorner = CheckForCorner(transform.position, m_characterMovementSettings.bodyRadius, MathHelper.NormalizeOnXZPlane(slideDirection), distance);
            if (!isCorner)
            {
                Vector3 slideMovement = slideDirection * speed * Time.deltaTime;
                transform.position += slideMovement;
            }
        }
    }

    void RotateTowards(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_characterMovementSettings.rotationSpeed * Time.deltaTime);
        }
    }

    bool CheckForCorner(Vector3 position, float radius, Vector3 direction, float distance)
    {
        Vector3 leftPerpendicular = MathHelper.RotateAroundYAxis(direction, -90f);
        Vector3 rightPerpendicular = MathHelper.RotateAroundYAxis(direction, 90f);

        bool forwardHit = Physics.SphereCast(position, radius, direction, out RaycastHit forwardHitInfo, distance);
        bool leftHit = Physics.SphereCast(position, radius, leftPerpendicular, out RaycastHit leftHitInfo, radius);
        bool rightHit = Physics.SphereCast(position, radius, rightPerpendicular, out RaycastHit rightHitInfo, radius);

        return forwardHit && (leftHit || rightHit);
    }

    
    public abstract void OnObjectSpawn();

    public abstract void OnObjectReturn();
}
