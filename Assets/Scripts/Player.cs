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

    [Header("Animations")]
    [SerializeField]
    AnimationClip idleEmptyAnim;

    [SerializeField]
    AnimationClip idleCrystalAnim;

    [SerializeField]
    AnimationClip fireAnim;

    Rigidbody2D m_rigidbody;
    Animator    m_animator;

    SpriteRenderer m_renderer;

    Camera m_mainCamera;

    static bool s_isDead;

    public static bool IsDead
    {
        get => s_isDead;
    }

    void Awake()
    {
        m_rigidbody  = GetComponent<Rigidbody2D>();
        m_animator   = GetComponent<Animator>();
        m_renderer   = GetComponent<SpriteRenderer>();
        m_mainCamera = Camera.main;

        s_isDead = false;

        weapon.onCollected += OnWeaponPickedUp;
    }

    void Update()
    {
        if (s_isDead)
        {
            return;
        }

        Vector3 mousePos = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        m_renderer.flipX = mousePos.x > transform.position.x;

        Vector2 moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveAxis.sqrMagnitude > 1f)
        {
            moveAxis = moveAxis.normalized;
        }

        m_rigidbody.velocity = moveAxis * moveSpeed;

        if (Input.GetButtonDown("Fire1"))
        {
            if (weapon.Fire())
            {

            }
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

    void OnWeaponPickedUp()
    {
        if (s_isDead)
        {
            return;
        }

        m_animator.Play(idleCrystalAnim.name);
    }
}
