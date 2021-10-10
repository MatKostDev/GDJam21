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

    PlayerTracker m_playerTracker;

    NavMeshAgent m_agent;

    float m_randomDestinationCooldown = 3f;
    float m_randomDestinationTimer;

    protected virtual void Awake()
    {
        m_playerTracker = GetComponent<PlayerTracker>();
        m_agent         = GetComponent<NavMeshAgent>();

        m_agent.updateRotation = false;
        m_agent.updateUpAxis   = false;

        m_randomDestinationTimer = Random.Range(m_randomDestinationCooldown * 0.5f, m_randomDestinationCooldown);
    }

    protected virtual void Update()
    {
        if (Player.IsDead)
        {
            m_agent.ResetPath();
            m_agent.acceleration = 1000f;
            m_agent.speed = 0f;
            return;
        }

        if (m_playerTracker.IsTracking)
        {
            if (NavMesh.SamplePosition(m_playerTracker.PlayerPosition, out _, 100f, NavMesh.AllAreas))
            {
                m_agent.SetDestination(m_playerTracker.PlayerPosition);
            }
            m_agent.speed = chaseSpeed;
        }
        else
        {
            m_agent.speed = wanderSpeed;

            if (m_randomDestinationTimer >= m_randomDestinationCooldown)
            {
                m_randomDestinationTimer = 0f;

                const float maxRange = 6f;
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
        Destroy(gameObject);
    }
}
