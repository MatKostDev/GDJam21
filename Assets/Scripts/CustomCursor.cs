using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(transform.parent.gameObject);

        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 newPos = Input.mousePosition;
        newPos.z = 100f;
        transform.position = newPos;
    }
}
