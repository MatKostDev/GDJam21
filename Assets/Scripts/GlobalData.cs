using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
    static readonly int s_playerLayer      = LayerMask.NameToLayer("Player");
    static readonly int s_environmentLayer = LayerMask.NameToLayer("Environment");

    static readonly LayerMask s_playerEnvironmentMask = LayerMask.GetMask("Environment", "Player");

    public static int PlayerLayer
    {
        get => s_playerLayer;
    }

    public static int EnvironmentLayer
    {
        get => s_environmentLayer;
    }

    public static LayerMask PlayerEnvironmentMask
    {
        get => s_playerEnvironmentMask;
    }
}
