using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtilities
{
    public static Vector3 ScaleReturn(this Vector3 l, Vector3 r)
    {
        return new Vector3(l.x * r.x, l.y * r.y, l.z * r.z);
    }
}
