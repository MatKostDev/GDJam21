using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    float chaseSpeed = 5f;

    [SerializeField]
    float wanderSpeed = 2f;

    [SerializeField]
    float stoppingDistance = 0f;

    [Header("Animations")]
    [SerializeField]
    AnimationClip wanderAnim;

    [SerializeField]
    AnimationClip chaseAnim;

    [SerializeField]
    AnimationClip idleAnim;

    [SerializeField]
    AnimationClip spottedAnim;

    [SerializeField]
    AnimationClip snoozeAnim;

    [SerializeField]
    AnimationClip deathAnim;

    [Header("Sounds")]
    [SerializeField]
    AudioClip dieSound;

    protected PlayerTracker m_playerTracker;

    protected NavMeshAgent m_agent;
    protected NavMeshPath  m_navPath;

    protected Animator       m_animator;
    protected SpriteRenderer m_renderer;
    protected AudioSource    m_audioSource;

    protected bool m_isDead      = false;
    protected bool m_isAttacking = false;
    protected bool m_isSpotting;
    protected bool m_isSnoozin;

    float m_randomDestinationCooldown = 6f;
    float m_randomDestinationTimer;

    protected virtual void Awake()
    {
        m_playerTracker = GetComponent<PlayerTracker>();
        m_agent         = GetComponent<NavMeshAgent>();
        m_animator      = GetComponent<Animator>();
        m_renderer      = GetComponent<SpriteRenderer>();
        m_audioSource   = GetComponent<AudioSource>();

        m_agent.updateRotation = false;
        m_agent.updateUpAxis   = false;

        m_randomDestinationTimer = Random.Range(m_randomDestinationCooldown * 0.5f, m_randomDestinationCooldown);

        m_navPath = new NavMeshPath();

        m_playerTracker.onTargetSpotted += EnemySpotted;
    }

    protected virtual void Update()
    {
        if (Player.IsDead || m_isDead)
        {
            StopInstantly();
            return;
        }

        if (m_isAttacking || m_isSpotting)
        {
            m_renderer.flipX = m_playerTracker.PlayerPosition.x > transform.position.x;

            StopInstantly();
            return;
        }

        if (m_agent.velocity.x > 0.1f)
        {
            m_renderer.flipX = true;
        }
        else if (m_agent.velocity.x < -0.1f)
        {
            m_renderer.flipX = false;
        }

        if (m_playerTracker.IsTracking)
        {
            if (m_agent.velocity.sqrMagnitude < 0.01f)
            {
                m_animator.Play(idleAnim.name);

                m_renderer.flipX = m_playerTracker.PlayerPosition.x > transform.position.x;
            }
            else
            {
                m_animator.Play(chaseAnim.name);
            }

            m_agent.stoppingDistance = stoppingDistance;

            Vector3 playerPosition = m_playerTracker.PlayerPosition;

            const float sampleDistance = 10f;
            //find nearest point on nav mesh from target's current position
            if (NavMesh.SamplePosition(playerPosition, out var navHit, sampleDistance, m_agent.areaMask))
            {
                playerPosition = navHit.position;
            }

            //only use new destination if it's reachable
            if (IsPathValid(playerPosition))
            {
                m_agent.SetPath(m_navPath);
            }
            m_agent.speed = chaseSpeed;
        }
        else
        {
            if (m_isSnoozin && m_agent.velocity.sqrMagnitude < 0.05f 
                || !m_isSnoozin && m_agent.velocity.sqrMagnitude < 0.001f)
            {
                m_animator.Play(snoozeAnim.name);
                m_isSnoozin = true;
            }
            else
            {
                m_animator.Play(wanderAnim.name);
                m_isSnoozin = false;
            }

            m_agent.speed = wanderSpeed;

            if (m_randomDestinationTimer >= m_randomDestinationCooldown)
            {
                m_randomDestinationTimer = 0f;

                const float maxRange = 3f;
                Vector3 randomPosition = new Vector3(
                    Random.Range(-maxRange, maxRange), 
                    Random.Range(-maxRange, maxRange), 
                    0f);

                randomPosition += transform.position;

                if (NavMesh.SamplePosition(randomPosition, out _, 100f, NavMesh.AllAreas))
                {
                    m_agent.SetDestination(randomPosition);
                }
            }

            m_randomDestinationTimer += Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D a_other)
    {
        if (m_isDead)
        {
            return;
        }

        if (a_other.CompareTag("Player"))
        {
            a_other.GetComponent<Player>().TakeDamage();
        }
        else if (a_other.CompareTag("PlayerWeapon"))
        {
            PlayerWeapon weapon = a_other.GetComponent<PlayerWeapon>();
            if (!weapon)
            {
                weapon = a_other.GetComponentInParent<PlayerWeapon>();
            }

            if (weapon.IsConnectedToPlayer)
            {
                return;
            }

            Vector3 weaponForward   = weapon.LookDirection;
            Vector3 weaponDirection = Vector3.Normalize(weapon.transform.position - transform.position);

            if (weapon.IsRecalling || Vector3.Dot(weaponForward, weaponDirection) > 0f)
            {
                TakeDamage();
            }
            else
            {
                weapon.OnBroken();
            }
        }
    }

    protected abstract void TakeDamage();

    protected void GenericTakeDamage()
    {
        if (m_isDead)
        {
            return;
        }

        m_animator.Play(deathAnim.name);
        m_audioSource.PlayOneShot(dieSound);

        StatTracker.IncrementEnemiesKilled();

        m_isDead = true;

        StopAllCoroutines();
        Destroy(gameObject, 2f);
    }

    protected void StopInstantly()
    {
        if (!m_agent)
        {
            return;
        }
        m_agent.ResetPath();
        m_agent.acceleration = 1000f;
        m_agent.speed        = 0f;
    }

    bool IsPathValid(Vector3 a_targetPosition)
    {
        m_agent.CalculatePath(a_targetPosition, m_navPath);

        if (m_navPath.status == NavMeshPathStatus.PathComplete)
        {
            return true;
        }

        return false;
    }

    void EnemySpotted()
    {
        StartCoroutine(EnemySpottedRoutine());
    }

    IEnumerator EnemySpottedRoutine()
    {
        m_isSpotting = true;
        StopInstantly();

        m_animator.Play(spottedAnim.name);

        yield return new WaitForSeconds(0.4f);

        m_isSpotting = false;

        m_animator.Play(idleAnim.name);
    }
}
