using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 100f;

    [SerializeField]
    PlayerWeapon weapon = null;

    Rigidbody2D m_rigidbody;

    static bool s_isDead;

    public static bool IsDead
    {
        get => s_isDead;
    }

    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();

        s_isDead = false;
    }

    void Update()
    {
        if (s_isDead)
        {
            return;
        }

        Vector2 moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveAxis.sqrMagnitude > 1f)
        {
            moveAxis = moveAxis.normalized;
        }

        m_rigidbody.velocity = moveAxis * moveSpeed;

        if (Input.GetButtonDown("Fire1"))
        {
            weapon.Fire();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            weapon.BeginRecall();
        }
    }

    public void TakeDamage()
    {
        if (s_isDead)
        {
            return;
        }

        s_isDead = true;

        m_rigidbody.velocity = Vector2.zero;

        Invoke(nameof(RestartLevel), 2f);
    }

    void RestartLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex, LoadSceneMode.Single);
    }
}
