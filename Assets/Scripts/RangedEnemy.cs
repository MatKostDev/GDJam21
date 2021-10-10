using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField]
    EnemyProjectile projectilePrefab = null;

    [SerializeField]
    float attackCooldown = 1.5f;

    float m_lastTimeAttacked;

    protected override void Update()
    {
        base.Update();

        if (Player.IsDead || m_isDead)
        {
            return;
        }

        if (m_playerTracker.IsTracking)
        {
            const float rayDistance = 50f;
            if (Physics2D.Raycast(
                transform.position, 
                m_playerTracker.DirectionToPlayer,
                rayDistance,
                GlobalData.PlayerEnvironmentMask).transform.gameObject.layer == GlobalData.PlayerLayer)
            {
                BeginAttack();
            }
        }
    }

    protected override void TakeDamage()
    {
        GenericTakeDamage();
    }

    void BeginAttack()
    {
        if (m_lastTimeAttacked + attackCooldown > Time.time)
        {
            return;
        }

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        m_lastTimeAttacked = Time.time;

        const float startDelay = 0.2f;
        yield return new WaitForSeconds(startDelay);

        StopInstantly();
        m_isAttacking = true;

        var newProjectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        newProjectileObject.GetComponent<EnemyProjectile>().OnFired(m_playerTracker.DirectionToPlayer);

        yield return new WaitForSeconds(0.15f);

        m_isAttacking = false;
    }
}
