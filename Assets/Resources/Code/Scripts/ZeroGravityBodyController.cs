using EventBusSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class ZeroGravityBodyController : MonoBehaviour, IZeroGravityController
{
    [Header("Bone References")]
    [SerializeField] private Transform headBone;
    private Transform tempHeadBone;
    [SerializeField] private Transform rightArmBone;
    [SerializeField] private Transform leftArmBone;

    [Header("Physics Settings")]
    [SerializeField] private float angularDrag = 0.5f;
    [SerializeField] private float linearDrag = 0.2f;

    private Rigidbody2D mainBodyRb;
    private List<Rigidbody2D> limbRigidbodies = new List<Rigidbody2D>();
    private List<HingeJoint2D> limbJoints = new List<HingeJoint2D>();
    private SpriteSkin spriteSkin;

    private void Awake()
    {
        mainBodyRb = GetComponent<Rigidbody2D>();
        spriteSkin = GetComponentInChildren<SpriteSkin>();
        tempHeadBone = headBone;
        
    }

    public void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    public void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    public void EnableZeroGravityMode()
    {

        //headBone.GetComponent<Rigidbody2D>().simulated = true;
        rightArmBone.GetComponent<Rigidbody2D>().simulated = true;
        leftArmBone.GetComponent<Rigidbody2D>().simulated = true;

        foreach (var rb in limbRigidbodies)
        {
            rb.simulated = true;
            rb.useFullKinematicContacts = false;
        }

        foreach (var joint in limbJoints)
        {
            joint.enabled = true;
        }

        if (spriteSkin != null)
        {
            spriteSkin.enabled = true;
            spriteSkin.autoRebind = true;
        }
    }

    public void DisableZeroGravityMode()
    {

        //headBone.GetComponent<Rigidbody2D>().simulated = false;
        rightArmBone.GetComponent<Rigidbody2D>().simulated = false;
        leftArmBone.GetComponent<Rigidbody2D>().simulated = false;
        ResetBonesPosition();


        foreach (var rb in limbRigidbodies)
        {
            rb.simulated = false;
            rb.useFullKinematicContacts = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void ResetBonesPosition()
    {
        headBone.localPosition = tempHeadBone.localPosition;
        rightArmBone.localPosition = Vector3.zero;
        leftArmBone.localPosition = Vector3.zero;

        headBone.localRotation = tempHeadBone.localRotation;
        rightArmBone.localRotation = Quaternion.identity;
        leftArmBone.localRotation = Quaternion.identity;
    }
}

public interface IZeroGravityController : IGlobalSubscriber
{
    void EnableZeroGravityMode();
    void DisableZeroGravityMode();
}