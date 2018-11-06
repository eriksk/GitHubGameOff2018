using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RagdollAnimator : MonoBehaviour
{
    public Animator Animator;
    public Transform AnimationRoot;
    public SkinnedMeshRenderer Renderer;
    
    [Range(0f, 1f)]
    public float Pin = 1f;

    [HideInInspector]
    public Transform Ragdoll;

    public bool Pinned = true;

    private List<RagdollNodePair> _pairs;
    private bool _created;

    public void Create()
    {
        Ragdoll = CopyToRagdoll(AnimationRoot);
        Ragdoll.name = "Ragdoll";
        Ragdoll.SetParent(transform);
        Ragdoll.localPosition = AnimationRoot.localPosition;
        Ragdoll.localRotation = AnimationRoot.localRotation;

        CleanRagdollObjects(AnimationRoot);
        UseAsBones(Ragdoll, AnimationRoot);

        // Super hack
        StartCoroutine(Hax());

        _pairs = CreatePairs();
        _created = true;
    }

    private IEnumerator Hax()
    {
        yield return new WaitForSeconds(0.1f);
        
        AnimationRoot.Find("root").localRotation = Quaternion.Euler(90f, 0f, -90f);
    }

    private List<RagdollNodePair> CreatePairs()
    {
        var pairs = new List<RagdollNodePair>();

        var ragdollNodes = Ragdoll.ChildrenDeep().ToArray();
        var animationNodes = AnimationRoot.ChildrenDeep().ToArray();

        for(var i = 0; i < ragdollNodes.Length; i++)
        {
            var rigidbody = ragdollNodes[i].GetComponent<Rigidbody>();
            if(rigidbody == null) continue;
            if(ragdollNodes[0].name.Contains("foot")) continue;

            if(ragdollNodes[i].name != animationNodes[i].name)
            {
                Debug.LogError("WRONG PAIR: " + ragdollNodes[i].name + " AND " + animationNodes[i].name);
            }
            
            // Allow better ragdolls
            rigidbody.maxAngularVelocity = float.MaxValue;

            pairs.Add(new RagdollNodePair()
            {
                Rigidbody = rigidbody,
                RagdollPart = ragdollNodes[i],
                AnimationPart = animationNodes[i]
            });
        }

        pairs.Reverse();

        return pairs;
    }

    void FixedUpdate()
    {
        if(!_created) return;
        if(Pinned)
        {
            foreach(var pair in _pairs)
            {
                // if(pair.AnimationPart.name == "root") continue;
                RotateTowards(pair.Rigidbody, pair.AnimationPart, pair.RagdollPart);
            }
        }
        else
        {
            foreach(var pair in _pairs)
            {
                pair.Rigidbody.angularDrag = 0f;
            }

        }

    }
    
    private void RotateTowards(Rigidbody body, Transform source, Transform target)
    {
        var targetForward = ((target.transform.up));
        var sourceForward = ((source.transform.up));

		var targetDelta = (-targetForward).normalized;
 
		var angleDiff = Vector3.Angle(sourceForward, targetDelta);
		var cross = Vector3.Cross(sourceForward, targetDelta);
 
        var angleMag = Mathf.Clamp01(Mathf.Abs(angleDiff) / 180f);

        body.angularDrag = angleMag * 20f; // Fixed damping

        var torque = (cross * angleDiff) * body.mass;
        
        body.AddTorque(torque * Pin);
    }

    private void CleanRagdollObjects(Transform target)
    {
        var objectsToRemove = new Component[0]
            .Concat(target.GetComponents<Joint>())
            .Concat(target.GetComponents<Collider>())
            .Concat(target.GetComponents<Rigidbody>())
            .Where(x => x != null)
            .ToArray();

        foreach(var obj in objectsToRemove)
        {
            Destroy(obj);
        }

        for(var i = 0; i < target.childCount; i++)
        {
            CleanRagdollObjects(target.GetChild(i));
        }
    }
    
    private void RemoveComponentsRecursively<T>(Transform target) where T : Component
    {
        var objectsToRemove = new Component[0]
            .Concat(target.GetComponents<T>())
            .Where(x => x != null)
            .ToArray();

        foreach(var obj in objectsToRemove)
        {
            Destroy(obj);
        }

        for(var i = 0; i < target.childCount; i++)
        {
            RemoveComponentsRecursively<T>(target.GetChild(i));
        }
    }

    private void UseAsBones(Transform newRoot, Transform originalRoot)
    {
        Renderer.bones = CloneBones(newRoot.ChildrenDeep().ToArray(), Renderer.bones);
        Renderer.rootBone = newRoot.GetChild(0);
    }

    private Transform[] CloneBones(Transform[] bones, Transform[] sourceBones)
    {
        var matching = new List<Transform>();
        
        for(var i = 0; i < sourceBones.Length; i++)
        {
            var targetName = sourceBones[i].name;
            matching.Add(bones.FirstOrDefault(x => x.name == targetName));
        }

        return matching.ToArray();
    }

    private Transform CopyToRagdoll(Transform target)
    {
        var copy = Instantiate(target);
        
        RemoveComponentsRecursively<VirtualChild>(copy);

        return copy;
    }
}

public class RagdollNodePair
{
    public Transform RagdollPart, AnimationPart;
    public Rigidbody Rigidbody;
}