using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField]
    Transform playerTransform = null;

    [SerializeField]
    float fireSpeed = 20f;

    [SerializeField]
    float recallSpeed = 20f;

    [SerializeField]
    LayerMask recallLayer;

    [SerializeField] 
    float restDistanceFromPlayer = 2f;

    bool m_isConnectedToPlayer = true;
    bool m_isRecalling         = false;

    Rigidbody2D m_rigidBody;

    int m_initialLayerNum;
    int m_recallLayerNum;

    float m_lastTimeFired = Mathf.NegativeInfinity;

    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();

        m_initialLayerNum = gameObject.layer;
        m_recallLayerNum  = Mathf.RoundToInt(Mathf.Log(recallLayer.value, 2)); //convert layer mask to int
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
        else if (!m_isRecalling)
        {
            if (m_rigidBody.velocity.sqrMagnitude > 0.001f)
            {
                Vector3 moveDirection = m_rigidBody.velocity.normalized;
                moveDirection.z = 0f;

                Vector3 lookTarget = transform.position + (moveDirection * 100f);
                transform.right = lookTarget - transform.position;
                transform.Rotate(0f, 0f, 90f); //because sprite faces down by default

                const float smallAmountOfTime = 0.3f;
                if ((playerTransform.position - transform.position).magnitude < restDistanceFromPlayer
                    && Time.time > m_lastTimeFired + smallAmountOfTime)
                {
                    OnPickedUp();
                }
            }
        }
    }

    public void Fire()
    {
        if (!m_isConnectedToPlayer)
        {
            return;
        }
        
        m_isConnectedToPlayer = false;
        m_lastTimeFired       = Time.time;

        Vector3 newDirection = Vector3.Normalize(Input.mousePosition - playerTransform.position);

        Vector3 lookTarget = playerTransform.position + (newDirection * 100f);
        transform.right = lookTarget - transform.position;
        transform.Rotate(0f, 0f, 90f); //because sprite faces down by default

        m_rigidBody.AddForce(newDirection * fireSpeed, ForceMode2D.Impulse);
    }

    public void BeginRecall()
    {
        if (m_isRecalling || m_isConnectedToPlayer)
        {
            return;
        }

        StartCoroutine(RecallRoutine());
    }

    IEnumerator RecallRoutine()
    {
        m_isRecalling    = true;
        gameObject.layer = m_recallLayerNum;

        while ((playerTransform.position - transform.position).magnitude > restDistanceFromPlayer)
        {
            Vector3 recallDirection = Vector3.Normalize(playerTransform.position - transform.position);

            m_rigidBody.velocity = recallDirection * recallSpeed;

            transform.right = transform.position - playerTransform.position;
            transform.Rotate(0f, 0f, 90f); //because sprite faces down by default

            yield return null;
        }

        m_isRecalling = false;
        OnPickedUp();
    }

    void OnPickedUp()
    {
        gameObject.layer      = m_initialLayerNum;
        m_rigidBody.velocity  = Vector2.zero;
        m_isConnectedToPlayer = true;
    }
}
