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

    [SerializeField]
    AnimationClip deathAnim;

    [Header("Sounds")]
    [SerializeField]
    AudioClip fireSound;

    [SerializeField]
    AudioClip recallSound;

    [SerializeField]
    AudioClip dieSound;

    Rigidbody2D m_rigidbody;
    Animator    m_animator;
    AudioSource m_audioSource;

    SpriteRenderer m_renderer;

    Camera m_mainCamera;

    static bool s_isDead;

    bool m_isFiring;

    public static bool IsDead
    {
        get => s_isDead;
    }

    void Awake()
    {
        m_rigidbody   = GetComponent<Rigidbody2D>();
        m_animator    = GetComponent<Animator>();
        m_renderer    = GetComponent<SpriteRenderer>();
        m_audioSource = GetComponent<AudioSource>();
        m_mainCamera  = Camera.main;

        s_isDead = false;

        weapon.onCollected += OnWeaponPickedUp;
    }

    void Update()
    {
        if (s_isDead || Time.timeScale == 0f)
        {
            return;
        }

        Vector3 mousePos = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        if (!m_isFiring)
        {
            m_renderer.flipX = mousePos.x > transform.position.x;
        }

        Vector2 moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveAxis.sqrMagnitude > 1f)
        {
            moveAxis = moveAxis.normalized;
        }

        m_rigidbody.velocity = moveAxis * moveSpeed;

        if (Input.GetButtonDown("Fire1") && weapon.IsConnectedToPlayer)
        {
            OnFireWeapon();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (weapon.BeginRecall())
            {
                m_audioSource.PlayOneShot(recallSound);
            }
        }
    }

    public void TakeDamage()
    {
        if (s_isDead)
        {
            return;
        }

        m_animator.Play(deathAnim.name);

        m_audioSource.PlayOneShot(dieSound);

        s_isDead = true;

        m_rigidbody.velocity = Vector2.zero;

        StatTracker.IncrementPlayerDeath();

        StopAllCoroutines();

        Invoke(nameof(RestartLevel), 2f);
    }

    void RestartLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex, LoadSceneMode.Single);
    }

    void OnFireWeapon()
    {
        if (m_isFiring)
        {
            return;
        }

        m_isFiring = true;

        StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {
        m_animator.Play(fireAnim.name);

        m_audioSource.PlayOneShot(fireSound);

        weapon.IsPreFiring = true;

        //yield return new WaitForSeconds(0.3f);

        weapon.Fire();

        yield return new WaitForSeconds(0.5f);

        m_animator.Play(idleEmptyAnim.name);

        m_isFiring = false;
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
