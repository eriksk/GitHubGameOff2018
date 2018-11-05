
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static IEnumerable<Transform> Children(this Transform transform)
    {
        for(var i = 0; i < transform.childCount; i++)
            yield return transform.GetChild(i);
    }

    public static IEnumerable<Transform> ChildrenDeep(this Transform transform)
    {
        for(var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            if(child.childCount > 0)
            {
                var grandChildren = child.ChildrenDeep();
                foreach(var grandChild in grandChildren)
                    yield return grandChild;
            }

            yield return child;
        }
    }
}