using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 100f;

    [SerializeField]
    PlayerWeapon weapon = null;

    Rigidbody2D m_rigidbody;

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveAxis.sqrMagnitude > 1f)
        {
            moveAxis = moveAxis.normalized;
        }

        m_rigidbody.velocity = moveAxis * moveSpeed;
    }
}
