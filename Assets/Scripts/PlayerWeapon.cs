using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField]
    Transform playerTransform = null;

    [SerializeField] 
    float restDistanceFromPlayer = 2f;

    bool m_isConnectedToPlayer = true;

    Rigidbody2D m_rigidBody;

    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (m_isConnectedToPlayer)
        {
            Vector3 newDirection = Vector3.Normalize(Input.mousePosition - playerTransform.position);

            transform.position = playerTransform.position + (newDirection * restDistanceFromPlayer);

            Vector3 lookTarget = playerTransform.position + (newDirection * 100f);
            transform.right = lookTarget - transform.position;
            transform.Rotate(0f, 0f, 90f); //because sprite faces down by default
        }
    }
}
