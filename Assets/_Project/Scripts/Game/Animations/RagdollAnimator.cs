using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RagdollAnimator : MonoBehaviour
{
    public Animator Animator;
    public Transform AnimationRoot;
    public SkinnedMeshRenderer Renderer;
    
    public float PinForce = 1f;

    [HideInInspector]
    public Transform Ragdoll;

    private List<RagdollNodePair> _pairs;

    void Start()
    {
        Ragdoll = CopyToRagdoll(AnimationRoot);
        Ragdoll.name = "Ragdoll";
        Ragdoll.SetParent(transform);
        Ragdoll.localPosition = AnimationRoot.localPosition;
        Ragdoll.localRotation = AnimationRoot.localRotation;

        CleanRagdollObjects(AnimationRoot);
        UseAsBones(Ragdoll, AnimationRoot);

        _pairs = CreatePairs();
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

            Debug.Log("Paired: " + rigidbody.gameObject.name);

            if(ragdollNodes[i].name != animationNodes[i].name)
            {
                Debug.LogError("WRONG PAIR: " + ragdollNodes[i].name + " AND " + animationNodes[i].name);
            }

            pairs.Add(new RagdollNodePair()
            {
                Rigidbody = rigidbody,
                RagdollPart = ragdollNodes[i],
                AnimationPart = animationNodes[i]
            });
        }

        return pairs;
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        // TODO: ALL OF THIS IS VERY WRONG
        
        foreach(var pair in _pairs)
        {
            var ragdollPos = pair.RagdollPart.localPosition;
            var animPos = pair.AnimationPart.localPosition;
            
            var ragdollRot = pair.RagdollPart.localRotation;
            var animRot = pair.AnimationPart.localRotation;


            var mag = 1f; //Quaternion.Dot(ragdollRot, animRot);
            var angle = Quaternion.Slerp(animRot, ragdollRot, 0.5f) * Vector3.right;

            pair.Rigidbody.AddTorque(
                angle * 
                mag *
                PinForce * 
                Time.fixedDeltaTime
            );
            

            var directionToTarget = (animPos - ragdollPos).normalized;
            var magnitude = Mathf.Pow(Vector3.Distance(animPos, ragdollPos), 0.5f);

            pair.Rigidbody.AddForce(
                directionToTarget * 
                magnitude * 
                pair.Rigidbody.mass *
                PinForce * 0.003f * 
                Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
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
        return copy;
    }
}

public class RagdollNodePair
{
    public Transform RagdollPart, AnimationPart;
    public Rigidbody Rigidbody;
}