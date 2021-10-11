using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    bool m_triggered = false;

    private void OnCollisionEnter2D(Collision2D a_other)
    {
        if (m_triggered)
        {
            return;
        }

        if (a_other.gameObject.layer == GlobalData.PlayerLayer)
        {
            m_triggered = true;
            FindObjectOfType<LevelManager>().LoadNextLevel();
        }
    }
}
