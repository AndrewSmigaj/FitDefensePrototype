using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Animations.Rigging;

public class HandOld : MonoBehaviour
{
    public enum HandType
    {
        Left,
        Right
    }
    public InputAction inputAction;
    public InputAction triggerAction;
    public InputAction gripAction;

    public List<Renderer> meshRenderers = new List<Renderer>();

    private bool currentlyTracked = false;
    private bool isHidden;
    public HandType handType = HandType.Left;
    public HandPose pose;

    public bool isCollisionEnabled { get; private set; } = false;

    public Collider[] colliders;

    public XRBaseInteractor interactor;

    public Animator handAnimator;

    public bool hideHandsOnTrackingLoss = true;

    public float poseAnimationSpeed = 10.0f;
    public Rig handPoseRig = null;
    public float targetWeight = 0.0f;
    private Coroutine handPosingAnimationCoroutine = null;

    private bool enableGripAnimations = true;

    Vector3 restorePosition = Vector3.zero;
    Quaternion restoreRotation = Quaternion.identity;

    public Transform grabAttachment = null;
    private Transform grabbedAttach = null;
    private Transform handAttach = null;

    public GameObject handVisual = null;

    FingerPose[] fingers = null;
    
    private void Awake()
    {
        
        if(interactor == null)
        {
            interactor = GetComponentInParent<XRBaseInteractor>();
        }
        if(handPoseRig == null)
        {
            handPoseRig = GetComponentInChildren<Rig>();
        }

    }

    private void OnEnable()
    {
        interactor?.selectEntered.AddListener(OnGrab);
        interactor?.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        interactor?.selectEntered.RemoveListener(OnGrab);
        interactor?.selectExited.RemoveListener(OnRelease);
    }
    // Start is called before the first frame updatepublic 
    void Start()
    {


        colliders = GetComponentsInChildren<Collider>().Where(collider => !collider.isTrigger).ToArray();
        fingers = GetComponentsInChildren<FingerPose>();

        inputAction.Enable();
        gripAction.Enable();
        triggerAction.Enable();
        handPoseRig.weight = 0.0f;
        
        if(hideHandsOnTrackingLoss) Hide();
    }

    // Update is called once per frame
    void Update()
    {
        float isTracked = inputAction.ReadValue<float>();



        if(isTracked == 1.0f && !currentlyTracked)
        {
            Show();
            currentlyTracked = true;
        }
        else if(isTracked == 0.0f && currentlyTracked)
        {
            if(hideHandsOnTrackingLoss) Hide();
            currentlyTracked = false;
        }

        //not how we generally want to do this
        float gripPress = gripAction.ReadValue<float>();
        handAnimator.SetFloat("GripAmount", enableGripAnimations? gripPress : 0);

        float triggerPress = triggerAction.ReadValue<float>();
        handAnimator.SetFloat("PointAmount", enableGripAnimations? triggerPress: 0);

        syncRigToPose();
    }


    void Show()
    {
        foreach(Renderer renderer in meshRenderers)
        {
            renderer.enabled = true;
        }
        isHidden = false;
        EnableCollisions(true);
    }

    void Hide()
    {
        meshRenderers.Clear();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
            meshRenderers.Add(renderer);
        }
        isHidden = true;
        EnableCollisions(false);
    }

    void EnableCollisions(bool enabled)
    {
        if (isCollisionEnabled == enabled) return;

        isCollisionEnabled = enabled;
        foreach(Collider collider in colliders)
        {
            collider.enabled = enabled;
        }
    }

    public void OnGrab(SelectEnterEventArgs args)
    {
        //check if whether to hide it or not
        HandControlOld ctrl = args.interactableObject.transform.gameObject.GetComponent<HandControlOld>();

        if (ctrl.fixedAttachment == null)
        {
            grabbedAttach = ctrl.attachTransform.transform;
        }
        else
        {
            grabbedAttach = ctrl.fixedAttachment.transform;
        }

        if(ctrl != null)
        {
            if (ctrl.hideHandOnGrab)
            {
                Hide();
            }
            else
            {
                pose = ctrl.GetHandPose(handType);

                if(pose != null)
                {
                    if(ctrl.fixedAttachment != null)
                    {
                        handAttach = ctrl.fixedAttachment;
                        restorePosition = handVisual.transform.localPosition;
                        restoreRotation = handVisual.transform.localRotation;
                    }

                    RunHandAnimation(1.0f);

                }
            }
        }
        
    }

    public void OnRelease(SelectExitEventArgs args)
    {
        HandControlOld ctrl = args.interactableObject.transform.gameObject.GetComponent<HandControlOld>();
        grabbedAttach = null;
        if (ctrl != null)
        {
            if (ctrl.hideHandOnGrab)
            {
                Show();
            }
            else if(pose != null)
            {
                RunHandAnimation(0.0f);

                if(handAttach != null)
                {
                    handVisual.transform.localRotation = restoreRotation;
                    handVisual.transform.localPosition = restorePosition;
                    handAttach = null;
                }
                pose = null;
            }
        }

    }


    IEnumerator AnimateHandCoroutine()
    {
        enableGripAnimations = false;
        while (!Mathf.Approximately(handPoseRig.weight, targetWeight))
        {
            handPoseRig.weight = Mathf.MoveTowards(handPoseRig.weight, targetWeight, poseAnimationSpeed * Time.deltaTime);
            yield return null;
        }
        if( targetWeight < 0.5f)
        {
            enableGripAnimations = true;
        }
    }

    private void RunHandAnimation(float target)
    {
        if(handPosingAnimationCoroutine != null)
        {
            StopCoroutine(handPosingAnimationCoroutine);
        }
        targetWeight = target;
        handPosingAnimationCoroutine = StartCoroutine(AnimateHandCoroutine());
    }

    void syncFingerTransform(Transform fingerEffectorTransform, Pose pose)
    {
        Vector3 worldPosition = grabbedAttach.TransformPoint(pose.position);
        Quaternion worldRotation = grabbedAttach.rotation * pose.rotation;
        fingerEffectorTransform.SetPositionAndRotation(worldPosition, worldRotation);
    }

    void syncRigToPose()
    {
        if (pose == null || grabAttachment == null) return;

        //get grab attach
        if(handAttach != null)
        {
            HandControlOld.AlignHandLocalTransformToGrabbedAttach(handVisual.transform, grabAttachment, handAttach);
        }



        foreach (FingerPose finger in fingers)
        {
            switch (finger.finger)
            {
                case FingerId.Thumb: syncFingerTransform(finger.transform, pose.thumb);
                    break;
                case FingerId.Index: syncFingerTransform(finger.transform, pose.index);
                    break;
                case FingerId.Middle: syncFingerTransform(finger.transform, pose.middle);
                    break;
                case FingerId.Ring: syncFingerTransform(finger.transform, pose.ring);
                    break;
                case FingerId.Pinky: syncFingerTransform(finger.transform, pose.pinky);
                    break;
            }
        }
    }
}
