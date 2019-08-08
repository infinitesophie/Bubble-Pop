using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    public delegate void ShootBubble();

    public static event ShootBubble OnShootBubble;

    public static void ShoottheBubble()
    {
        if (OnShootBubble != null)
            OnShootBubble();
    }
}