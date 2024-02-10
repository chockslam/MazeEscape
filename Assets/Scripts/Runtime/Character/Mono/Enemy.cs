using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] PatrolType patrolType;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask wallMask;
    [SerializeField] AiSettings aiSettings;
    [SerializeField] MeshFilter viewMeshFilter;
    [SerializeField] int fovRayCount = 50;

    public AStarPathfinding m_pathfinder;

    bool playerDetected;
    bool canDamage = true;
    int m_currentPatrolIndex = 0;
    int pathIndex = 0;
    float searchStartTime;
    Player m_player;
    State m_currentState;
    Vector3[] m_patrolPoints;
    Cell[,] m_grid;
    List<Vector3> viewPoints;
    List<Vector3> currentPath = new();
    Mesh viewMesh;
    Vector3 lastSeenPlayerPosition = Vector3.zero;

    void Awake()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        m_pathfinder = new AStarPathfinding();    
        viewPoints = new List<Vector3>();
    }

    void Update()
    {
        switch (m_currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                ChasePlayer();
                break;
            case State.FollowingPath:
                FollowPath();
                break;
            case State.Search:
                SearchForPlayer();
                break;
        }

        DetectPlayer();
    }

    void LateUpdate()
    {
        DrawFOV();
    }

    void SearchForPlayer()
    {
        if (Time.time - searchStartTime < aiSettings.searchDuration)
        {
            if (MathHelper.DistanceOnXZPlane(transform.position, lastSeenPlayerPosition) > aiSettings.stoppingDistance)
            {
                MoveTowards(lastSeenPlayerPosition);
            }
        }
        else
        {
            m_currentState = State.FollowingPath;
            CalculatePathBackToPatrol();
        }
    }

    public override void OnObjectSpawn()
    {
        m_currentState = State.Patrol;
        GeneratePatrolPoints(); 
    }

    public override void OnObjectReturn()
    {
        m_currentState = State.Patrol;
        m_currentPatrolIndex = 0;
    }

    public void SetPlayer(Player player)
    {
        m_player = player;
    }

    public void SetGrid(Cell[,] grid)
    {
        m_grid = grid;
    }

    void GeneratePatrolPoints()
    {
        switch (patrolType)
        {
            case PatrolType.Diagonally:
                GenerateDiagonalPatrol();
                break;
            case PatrolType.AroundCell:
                GenerateAroundCellPatrol();
                break;
            case PatrolType.ZigZag:
                GenerateTrianglePatrol();
                break;
        }
    }

    void GenerateDiagonalPatrol()
    {
        float diagonalDistance = 0.7f; 
        Vector3 cellOrigin = transform.position;

        m_patrolPoints = new Vector3[2];
        m_patrolPoints[0] = cellOrigin + new Vector3(-diagonalDistance / 2, 0, -diagonalDistance / 2);
        m_patrolPoints[1] = cellOrigin + new Vector3(diagonalDistance / 2, 0, diagonalDistance / 2);
    }


    void GenerateAroundCellPatrol()
    {
        float cellSize = 0.7f; // FIXED SIZE
        Vector3 cellOrigin = transform.position;

        m_patrolPoints = new Vector3[4];
        m_patrolPoints[0] = cellOrigin + new Vector3(-cellSize / 2, 0, -cellSize / 2);
        m_patrolPoints[1] = cellOrigin + new Vector3(cellSize / 2, 0, -cellSize / 2);
        m_patrolPoints[2] = cellOrigin + new Vector3(cellSize / 2, 0, cellSize / 2);
        m_patrolPoints[3] = cellOrigin + new Vector3(-cellSize / 2, 0, cellSize / 2);
    }

    void GenerateTrianglePatrol()
    {
        float width = 0.6f;
        float height = 0.8f; 
        Vector3 cellOrigin = transform.position;

        m_patrolPoints = new Vector3[3];
        m_patrolPoints[0] = cellOrigin + new Vector3(-width / 2, 0, height / 2);
        m_patrolPoints[1] = cellOrigin + new Vector3(width / 2, 0, height / 2);
        m_patrolPoints[2] = cellOrigin + new Vector3(-width / 2, 0, -height / 2);
    }

    void Patrol()
    {
        if (m_patrolPoints.Length == 0)
            return; 

        Vector3 targetPoint = m_patrolPoints[m_currentPatrolIndex];
        Vector3 directionToTarget = MathHelper.NormalizeOnXZPlane(targetPoint - transform.position);
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);

        if (distanceToTarget > aiSettings.stoppingDistance)
        {
            Move(directionToTarget, m_characterMovementSettings.speed);
        }
        else
        {
            m_currentPatrolIndex = (m_currentPatrolIndex + 1) % m_patrolPoints.Length; 
        }
    }

    void ChasePlayer()
    {
        Vector3 directionToPlayer = MathHelper.NormalizeOnXZPlane(m_player.transform.position - transform.position);
        
        if(MathHelper.DistanceOnXZPlane(m_player.transform.position, transform.position) > aiSettings.stoppingDistance)
            Move(directionToPlayer, aiSettings.chaseSpeed);

        if (canDamage) 
        {
            m_eventObserver.NotifySubscribers(new GameEvent(EventType.PlayerHit, new() { new FloatEventData(aiSettings.damage) }));
            canDamage = false;
            StartCoroutine(HandleDamageCoolDown());
        }
    }

    IEnumerator HandleDamageCoolDown() 
    {
        yield return new WaitForSeconds(aiSettings.damageCoolDownInSeconds);
        canDamage = true;
        
    }

    void DetectPlayer()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aiSettings.detectionRadius, playerLayer);

        if (!(m_player.IsDead() || m_player.HasWon()))
        {
            foreach (Collider hitCollider in hitColliders)
            {
                Vector3 directionToTarget = MathHelper.NormalizeOnXZPlane(hitCollider.transform.position - transform.position);
                float angle = Vector3.Angle(transform.forward, directionToTarget);
            
                if (angle <= aiSettings.fieldOfViewAngle / 2)
                {
                    if (Physics.Raycast(transform.position, MathHelper.NormalizeOnXZPlane(m_player.transform.position - transform.position), out RaycastHit hit, aiSettings.detectionRadius, wallMask))
                    {
                        return;
                    }

                    m_currentState = State.Chase;
                    return;
                }
            }
        }

        if (m_currentState == State.Chase)
        {
            m_currentState = State.Search;
            lastSeenPlayerPosition = m_player.transform.position;
            searchStartTime = Time.time;
            playerDetected = true;
        }
    }

    void FollowPath()
    {
        if (currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            m_currentState = State.Patrol;
            return;
        }

        Vector3 nextPoint = currentPath[pathIndex];
        MoveTowards(nextPoint);

        if (Vector3.Distance(transform.position, nextPoint) < aiSettings.stoppingDistance)
        {
            pathIndex++;
            if (pathIndex >= currentPath.Count)
            {
                m_currentState = State.Patrol;
            }
        }
    }

    void CalculatePathBackToPatrol()
    {
        Vector3 patrolTarget = m_patrolPoints[0];
        if (m_pathfinder != null)
        {
            currentPath = m_pathfinder.FindPath(m_grid, transform.position, patrolTarget);
            pathIndex = 0;
        }
    }

    void MoveTowards(Vector3 target)
    {
        Move(MathHelper.NormalizeOnXZPlane(target - transform.position), m_characterMovementSettings.speed);
    }

    void DrawFOV()
    {
        viewPoints.Clear(); // Clear the existing list instead of creating a new one

        float stepAngleSize = aiSettings.fieldOfViewAngle / fovRayCount;
        for (int i = 0; i <= fovRayCount; i++)
        {
            float angle = transform.eulerAngles.y - aiSettings.fieldOfViewAngle / 2 + stepAngleSize * i;
            ViewCastInfo viewCast = ViewCast(angle);
            viewPoints.Add(viewCast.point);
        }

        UpdateMeshWithViewPoints(viewPoints); // Refactor mesh updating into a separate method
    }

    void UpdateMeshWithViewPoints(List<Vector3> viewPoints)
    {
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }


    public enum PatrolType
    {
        Diagonally,
        AroundCell,
        ZigZag
    }

    enum State
    { 
        Patrol, 
        Chase, 
        Search,
        FollowingPath
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
        }
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = MathHelper.DirFromAngle(globalAngle, true, transform.eulerAngles.y);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, aiSettings.detectionRadius, wallMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * aiSettings.detectionRadius, aiSettings.detectionRadius, globalAngle);
        }
    }
}
