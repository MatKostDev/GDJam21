using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 8f;

    Rigidbody2D m_rigidbody;

    public void OnFired(Vector3 a_fireDirection)
    {
        m_rigidbody = GetComponent<Rigidbody2D>();

        m_rigidbody.velocity = a_fireDirection * moveSpeed;

        transform.right = transform.position + a_fireDirection;
    }

    void OnTriggerEnter2D(Collider2D a_other)
    {
        if (a_other.gameObject.layer == GlobalData.EnvironmentLayer)
        {
            TakeDamage();
        }
        else if (a_other.CompareTag("Player"))
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

            if (weapon.IsConnectedToPlayer || weapon.IsBroken)
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

    void TakeDamage()
    {
        Destroy(gameObject);
    }
}
