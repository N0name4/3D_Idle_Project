using UnityEngine.AI;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Enemy enemy;
    private NavMeshAgent agent;
    private Transform player;

    private float patrolTimer;
    private float attackTimer;

    private RoomNode currentRoom;

    public void Init(RoomNode room)
    {
        currentRoom = room;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("Enemy 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        if (agent != null && enemy != null)
            agent.speed = enemy.data.moveSpeed;

        MoveToRandomPointInRoom();
    }

    void Update()
    {
        if (player == null || currentRoom == null || enemy == null || enemy.data == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float attackRange = enemy.data.attackRange;
        float keepDistance = attackRange * 0.8f; // 원하는 거리 유지 비율 (예: 80%)

        if (currentRoom.IsInside(player.position))
        {
            // 너무 가까우면 멈추고 공격만
            if (distance <= attackRange)
            {
                agent.ResetPath(); // 멈춤

                attackTimer += Time.deltaTime;
                if (attackTimer >= enemy.data.attackCooldown)
                {
                    attackTimer = 0f;
                    Attack();
                }
            }
            // 공격 범위보다 멀지만 시야에 있으면 접근 (거리를 두고)
            else if (distance > keepDistance)
            {
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                Vector3 desiredPos = player.position - dirToPlayer * keepDistance;

                agent.SetDestination(desiredPos);
            }
        }
        else
        {
            // 순찰 로직
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= 3f)
            {
                patrolTimer = 0f;
                MoveToRandomPointInRoom();
            }
        }
    }


    void MoveToRandomPointInRoom()
    {
        float half = currentRoom.size / 2f;
        Vector3 center = new Vector3(currentRoom.pos.x, 0, currentRoom.pos.y);
        Vector3 randPos = center + new Vector3(Random.Range(-half + 0.5f, half - 0.5f), 0, Random.Range(-half + 0.5f, half - 0.5f));
        agent.SetDestination(randPos);
    }

    void Attack()
    {
        if (player == null || enemy == null || enemy.data == null) return;

        Player playerComponent = player.GetComponent<Player>();
        if (playerComponent == null) return;

        bool playerDead = playerComponent.condition.TakeDamage(enemy.data.atk);
        Debug.Log($"{enemy.characterName} ▶ {playerComponent.characterName} 에게 공격! 남은 HP: {playerComponent.condition.currentHp}");

        if (playerDead)
        {
            playerComponent.OnDefeated();
        }
    }
}
