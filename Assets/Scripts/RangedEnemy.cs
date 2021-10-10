using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Attack")]
    [SerializeField]
    EnemyProjectile projectilePrefab = null;

    [SerializeField]
    AnimationClip attackAnim;

    [SerializeField]
    float attackCooldown = 2.5f;

    [SerializeField]
    float targetShotLeadAmount = 5f;

    [SerializeField]
    float firstShotWindupTime = 0.4f;

    [SerializeField]
    float delayBetweenShots = 0.2f;

    [SerializeField]
    float windDownDuration = 0.3f;

    float m_lastTimeAttacked;

    void Start()
    {
        m_lastTimeAttacked = -attackCooldown * 0.5f;
    }

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
        if (m_lastTimeAttacked + attackCooldown > Time.time || m_isSpotting)
        {
            return;
        }

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        m_lastTimeAttacked = Time.time;

        StopInstantly();
        m_isAttacking = true;

        m_animator.Play(attackAnim.name);

        yield return new WaitForSeconds(firstShotWindupTime);

        //set up general position and directions
        Vector3 targetPosition = m_playerTracker.PlayerPosition;
        targetPosition.z = 0f;

        Vector3 targetDirection = m_playerTracker.DirectionToPlayer;

        Vector3 targetRightDirection = new Vector3(targetDirection.y, -targetDirection.x, 0f).normalized;

        {
            //fire to the right of the target
            Vector3 firstShotTargetPos = targetPosition + (targetShotLeadAmount * targetRightDirection);
            Vector3 firstShotDirection = Vector3.Normalize(firstShotTargetPos - transform.position);

            var newProjectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            newProjectileObject.GetComponent<EnemyProjectile>().OnFired(firstShotDirection);
        }

        yield return new WaitForSeconds(delayBetweenShots);

        {
            m_animator.Play(attackAnim.name, -1, 0.5f);

            //fire at the target
            var newProjectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            newProjectileObject.GetComponent<EnemyProjectile>().OnFired(targetDirection);
        }

        yield return new WaitForSeconds(delayBetweenShots);

        {
            m_animator.Play(attackAnim.name, -1, 0.5f);

            //fire to the left of the target
            Vector3 thirdShotTargetPos = targetPosition + (targetShotLeadAmount * -targetRightDirection);
            Vector3 thirdShotDirection = Vector3.Normalize(thirdShotTargetPos - transform.position);

            var newProjectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            newProjectileObject.GetComponent<EnemyProjectile>().OnFired(thirdShotDirection);
        }

        yield return new WaitForSeconds(windDownDuration);

        m_isAttacking = false;
    }
}
