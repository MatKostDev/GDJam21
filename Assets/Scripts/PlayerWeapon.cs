using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeapon : MonoBehaviour
{
    public UnityAction onCollected;

    [SerializeField]
    Transform playerTransform = null;

    [SerializeField]
    float fireSpeed = 20f;

    [SerializeField]
    float recallSpeed = 20f;

    [SerializeField]
    LayerMask recallLayer;

    [SerializeField]
    float recallDelay = 0.5f;

    [SerializeField] 
    float restDistanceFromPlayer = 2f;

    [SerializeField]
    BoxCollider2D enemyCollider = null;

    [Header("Animations")]
    [SerializeField]
    AnimationClip idleAnim;

    [SerializeField]
    AnimationClip shatterAnim;

    [Header("Sounds")]
    [SerializeField]
    AudioClip shatterSound;

    [SerializeField]
    AudioClip repairSound;

    const float NORMAL_COLLIDER_WIDTH  = 0.22f;
    const float NORMAL_COLLIDER_HEIGHT = 0.05f;

    const float RECALLING_COLLIDER_WIDTH  = 0.22f;
    const float RECALLING_COLLIDER_HEIGHT = 0.25f;

    bool m_isConnectedToPlayer = true;
    bool m_isRecalling         = false;
    bool m_isBroken            = false;

    bool m_isPreFiring;

    Rigidbody2D    m_rigidBody;
    SpriteRenderer m_renderer;
    Animator       m_animator;
    AudioSource    m_audioSource;

    int m_initialLayerNum;
    int m_recallLayerNum;

    float m_lastTimeFired = Mathf.NegativeInfinity;

    Camera m_mainCamera;

    public bool IsRecalling
    {
        get => m_isRecalling;
    }

    public bool IsConnectedToPlayer
    {
        get => m_isConnectedToPlayer;
    }

    public Vector3 LookDirection
    {
        get => transform.right;
    }

    public bool IsPreFiring
    {
        get => m_isPreFiring;
        set => m_isPreFiring = value;
    }

    public bool IsBroken
    {
        get => m_isBroken;
    }

    void Awake()
    {
        m_rigidBody   = GetComponent<Rigidbody2D>();
        m_renderer    = GetComponent<SpriteRenderer>();
        m_animator    = GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
        m_mainCamera  = Camera.main;

        m_initialLayerNum = gameObject.layer;
        m_recallLayerNum  = Mathf.RoundToInt(Mathf.Log(recallLayer.value, 2)); //convert layer mask to int

        enemyCollider.size = new Vector2(NORMAL_COLLIDER_WIDTH, NORMAL_COLLIDER_HEIGHT);
    }

    void Update()
    {
        if (Player.IsDead)
        {
            m_rigidBody.velocity = Vector2.zero;
            return;
        }

        m_renderer.enabled = true;

        if (m_isBroken)
        {
            if ((playerTransform.position - transform.position).magnitude < 0.7f)
            {
                OnRepaired();
            }

            return;
        }

        if (m_isConnectedToPlayer)
        {
            if (m_isPreFiring)
            {
                transform.position = playerTransform.position;
            }
            else
            {
                m_renderer.enabled = false;

                Vector3 mousePos = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;

                Vector3 newDirection = Vector3.Normalize(mousePos - playerTransform.position);

                transform.position = playerTransform.position + (newDirection * restDistanceFromPlayer);

                Vector3 lookTarget = playerTransform.position + (newDirection * 100f);

                FaceDirection(lookTarget - transform.position);
            }
        }
        else if (!m_isRecalling)
        {
            if (m_rigidBody.velocity.sqrMagnitude > 0.001f)
            {
                Vector3 moveDirection = m_rigidBody.velocity.normalized;
                moveDirection.z = 0f;

                Vector3 lookTarget = transform.position + (moveDirection * 100f);

                FaceDirection(lookTarget - transform.position);

                const float smallAmountOfTime = 0.3f;
                if ((playerTransform.position - transform.position).magnitude < 0.5f
                    && Time.time > m_lastTimeFired + smallAmountOfTime)
                {
                    OnPickedUp();
                }
            }
        }
    }

    public bool Fire()
    {
        if (!m_isConnectedToPlayer)
        {
            return false;
        }
        
        m_isConnectedToPlayer = false;
        m_lastTimeFired       = Time.time;

        Vector3 mousePos = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 newDirection = Vector3.Normalize(mousePos - playerTransform.position);

        Vector3 lookTarget = playerTransform.position + (newDirection * 100f);

        FaceDirection(lookTarget - transform.position);

        transform.position = playerTransform.position;

        m_rigidBody.AddForce(newDirection * fireSpeed, ForceMode2D.Impulse);
        
        FindObjectOfType<ScreenShake>().ApplyShake(1.2f, 0.2f);

        m_isPreFiring = false;

        return true;
    }

    public bool BeginRecall()
    {
        if (m_isRecalling || m_isConnectedToPlayer || m_isBroken)
        {
            return false;
        }

        FindObjectOfType<ScreenShake>().ApplyShake(1.2f, 0.35f);

        StartCoroutine(RecallRoutine());

        return true;
    }

    public void OnBroken()
    {
        if (m_isBroken)
        {
            return;
        }

        m_isBroken            = true;
        m_isConnectedToPlayer = false;

        m_rigidBody.velocity = Vector2.zero;

        m_animator.Play(shatterAnim.name);
        m_audioSource.PlayOneShot(shatterSound);

        FindObjectOfType<ScreenShake>().ApplyShake(5f, 0.3f);
    }

    IEnumerator RecallRoutine()
    {
        m_isRecalling    = true;
        gameObject.layer = m_recallLayerNum;

        enemyCollider.size = new Vector2(RECALLING_COLLIDER_WIDTH, RECALLING_COLLIDER_HEIGHT);

        m_rigidBody.velocity = Vector2.zero;

        Vector3 startRightDirection = transform.right;

        float recallWaitTimer = 0f;
        while (recallWaitTimer < recallDelay)
        {
            recallWaitTimer += Time.deltaTime;

            Vector3 desiredDirection = transform.position - playerTransform.position;

            transform.right = Vector3.Lerp(startRightDirection, desiredDirection, recallWaitTimer / recallDelay);

            yield return null;
        }

        while ((playerTransform.position - transform.position).magnitude > 0.5f)
        {
            Vector3 recallDirection = Vector3.Normalize(playerTransform.position - transform.position);

            m_rigidBody.velocity = recallDirection * recallSpeed;

            FaceDirection(transform.position - playerTransform.position);

            yield return null;
        }

        m_isRecalling = false;

        enemyCollider.size = new Vector2(NORMAL_COLLIDER_WIDTH, NORMAL_COLLIDER_HEIGHT);
        OnPickedUp();
    }

    void OnPickedUp()
    {
        gameObject.layer      = m_initialLayerNum;
        m_rigidBody.velocity  = Vector2.zero;
        m_isConnectedToPlayer = true;

        onCollected?.Invoke();
    }

    void OnRepaired()
    {
        m_isBroken = false;
        OnPickedUp();

        m_animator.Play(idleAnim.name);
        m_audioSource.PlayOneShot(repairSound);
    }

    void FaceDirection(Vector3 a_direction)
    {
        transform.right = a_direction;
        //transform.Rotate(0f, 0f, -90f); //because sprite faces up by default
    }
}
