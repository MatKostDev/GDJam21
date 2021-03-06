using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTracker : MonoBehaviour
{
    public UnityAction onTargetSpotted;

    [SerializeField]
    float detectionRange = 10f;

    static Transform s_playerTransform;
    static Transform s_playerWeaponTransform;

    bool m_isTracking = false;

    float sqrDetectionRange;

    public bool IsTracking
    {
        get => m_isTracking;
    }

    public Vector3 PlayerPosition
    {
        get => s_playerTransform.position;
    }

    public Vector3 DirectionToPlayer
    {
        get => Vector3.Normalize(s_playerTransform.position - transform.position);
    }

    void Awake()
    {
        s_playerTransform       = GameObject.FindGameObjectWithTag("Player").transform;
        s_playerWeaponTransform = GameObject.FindGameObjectWithTag("PlayerWeapon").transform;

        sqrDetectionRange = detectionRange * detectionRange;
    }

    void Update()
    {
        if (m_isTracking)
        {
            return;
        }

        if ((transform.position - s_playerTransform.position).sqrMagnitude < sqrDetectionRange 
            || (transform.position - s_playerWeaponTransform.position).sqrMagnitude < sqrDetectionRange)
        {
            m_isTracking = true;
            onTargetSpotted?.Invoke();
        }
    }
}
