using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Extension Methods in C# are additional methods that can be applied to an
 * existing type without the need to create a new derived type.
 * They are special static methods that can be called as if they were instance
 * methods of the existng type without having to modify that type.
 */
public static class ExtensionMethods
{
    //represent the dot product is close enough to constitute facing in the same direction
    const float DOT_THRESHOLD = 0.5f;

    //the key word "this" indicates the type that'll be extended.
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        /**
         * We'll use a vector operation -- the dot product.
         * For 2 normalized vectors, the dot product will return a value between [-1,1]
         * based on close they're to each other.
         * 0°= 1, 90°= 0, 180°= -1
         **/
        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= DOT_THRESHOLD;
    }
}
