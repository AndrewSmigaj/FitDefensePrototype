using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Animations.Rigging;

public enum HandType
{
    Left, 
    Right
};

public class Hand : MonoBehaviour
{
    public HandType type = HandType.Left;
    public bool isHidden { get; private set; } = false;

    public bool hideOnTrackingLoss = true;
    public InputAction trackedAction = null;

    public InputAction gripAction = null;
    public InputAction triggerAction = null;
    public Animator handAnimator = null;
    public bool enableGripAnimations = true;
    public float poseAnimationSpeed = 15.0f;
    Coroutine m_poseAnimationCoroutine = null;

    int m_gripAmountParameter = 0;
    int m_pointAmountParameter = 0;

    bool m_isCurrentlyTracked = false;

    List<Renderer> m_currentRenderers = new List<Renderer>();

    Collider[] m_colliders = null;
    public bool isCollisionEnabled { get; private set; } = true;

    public XRBaseInteractor interactor = null;
    

    public Transform grabAttachment = null;
    public GameObject handVisual = null;

    Transform m_currentGrabbedAttach = null;
    HandPose m_currentPose = null;
    Transform m_currentPoseHandAttach = null;
    Vector3 m_restorePosition = Vector3.zero;
    Quaternion m_restoreRotation = Quaternion.identity;

    float m_poseAnimationTarget = 0.0f;
    Rig m_poseAnimationRig = null;
    FingerPose[] m_fingers = null;

    private void Awake()
    {
        if (interactor == null)
        {
            interactor = GetComponentInParent<XRBaseInteractor>();
        }

        if(m_poseAnimationRig == null)
        {
            m_poseAnimationRig = GetComponentInChildren<Rig>();
        }
    }

    private void OnEnable()
    {
        interactor?.onSelectEntered.AddListener(OnGrab);
        interactor?.onSelectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        interactor?.onSelectEntered.RemoveListener(OnGrab);
        interactor?.onSelectExited.RemoveListener(OnRelease);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_colliders = GetComponentsInChildren<Collider>().Where(childCollider => !childCollider.isTrigger).ToArray();
        trackedAction.Enable();
        m_gripAmountParameter = Animator.StringToHash("GripAmount");
        m_pointAmountParameter = Animator.StringToHash("PointAmount");
        gripAction.Enable();
        triggerAction.Enable();

        m_fingers = GetComponentsInChildren<FingerPose>();
        m_poseAnimationRig.weight = 0.0f;

        Hide();
    }

    void UpdateAnimations()
    {
        float pointAmount = triggerAction.ReadValue<float>();
        handAnimator.SetFloat(m_pointAmountParameter, enableGripAnimations ? pointAmount : 0);

        float gripAmount = gripAction.ReadValue<float>();
        handAnimator.SetFloat(m_gripAmountParameter, enableGripAnimations ? Mathf.Clamp01(gripAmount + pointAmount) : 0);
    }

    // Update is called once per frame
    void Update()
    {
        float isTracked = trackedAction.ReadValue<float>();
        if(isTracked == 1.0f && !m_isCurrentlyTracked)
        {
            m_isCurrentlyTracked = true;
            Show();
        }
        else if(isTracked == 0 && m_isCurrentlyTracked)
        {
            m_isCurrentlyTracked = false;
            if(hideOnTrackingLoss) Hide();
        }

        if(isHidden && !hideOnTrackingLoss)
        {
            Show();
        }

        UpdateAnimations();
        SyncRigToPose();
    }

    public void Show()
    {
        foreach (Renderer renderer in m_currentRenderers)
        {
            renderer.enabled = true;
        }
        isHidden = false;
        EnableCollisions(true);
    }

    public void Hide()
    {
        m_currentRenderers.Clear();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            renderer.enabled = false;
            m_currentRenderers.Add(renderer);
        }
        isHidden = true;
        EnableCollisions(false);
    }

    public void EnableCollisions(bool enabled)
    {
        if (isCollisionEnabled == enabled) return;

        isCollisionEnabled = enabled;
        foreach(Collider collider in m_colliders)
        {
            collider.enabled = isCollisionEnabled;
        }
    }

    void OnGrab(XRBaseInteractable grabbedObject)
    {
        HandControl ctrl = grabbedObject.GetComponent<HandControl>();
        if (ctrl.fixedAttachment == null)
        {
            m_currentGrabbedAttach = grabbedObject.GetComponent<XRGrabInteractable>().attachTransform;
        }
        else
        {
            m_currentGrabbedAttach = ctrl.fixedAttachment.transform;
        }
       
        if(ctrl != null)
        {
            if(ctrl.hideHand)
            {
                Hide();
            }
            else
            {
                HandPose pose = ctrl.PoseForHand(type);
                if(pose != null)
                {
                    m_currentPose = pose;
                    if (ctrl.fixedAttachment != null)
                    {
                        m_currentPoseHandAttach = ctrl.fixedAttachment;
                        m_restorePosition = handVisual.transform.localPosition;
                        m_restoreRotation = handVisual.transform.localRotation;
                    }
                    AnimatePoseIn();
                }
            }
        }
    }

    void SyncTransform(Pose pose, Transform finger)
    {
        //Code here is different from videos. Disregard video for this part.
        finger.SetPositionAndRotation(m_currentGrabbedAttach.TransformPoint(pose.position),m_currentGrabbedAttach.rotation * pose.rotation);
    }

    void SyncRigToPose()
    {
        //Code here is slightly different from the videos to avoid early returns when loading a pose without an active XR rig present.
        var selected = interactor?.selectTarget;

        if (grabAttachment == null || m_currentPose == null) return;

        if(m_currentPoseHandAttach != null)
            HandControl.AlignHandToAttachment(handVisual.transform, grabAttachment, m_currentPoseHandAttach);

        foreach (var finger in m_fingers)
        {
            switch (finger.finger)
            {
                case FingerId.Thumb:
                    SyncTransform(m_currentPose.thumb, finger.transform);
                    break;
                case FingerId.Index:
                    SyncTransform(m_currentPose.index, finger.transform);
                    break;
                case FingerId.Middle:
                    SyncTransform(m_currentPose.middle, finger.transform);
                    break;
                case FingerId.Ring:
                    SyncTransform(m_currentPose.ring, finger.transform);
                    break;
                case FingerId.Pinky:
                    SyncTransform(m_currentPose.pinky, finger.transform);
                    break;
            }
        }
    }

    void OnRelease(XRBaseInteractable releasedObject)
    {
        HandControl ctrl = releasedObject.GetComponent<HandControl>();
        m_currentGrabbedAttach = null;
        if(ctrl != null)
        {
            if(m_currentPose != null)
            {
                AnimatePoseOut();

                //restore hand
                if(m_currentPoseHandAttach != null)
                {
                    handVisual.transform.localPosition = m_restorePosition;
                    handVisual.transform.localRotation = m_restoreRotation;
                    m_currentPoseHandAttach = null;
                }
               
                m_currentPose = null;
            }
            else if(ctrl.hideHand)
            {
                Show();
            }
        }
    }

    void AnimatePoseIn()
    {
        m_poseAnimationTarget = 1.0f;
        if (m_poseAnimationCoroutine != null)
        {
            StopCoroutine(m_poseAnimationCoroutine);
        }
        m_poseAnimationCoroutine = StartCoroutine(AnimateHand());
    }

    void AnimatePoseOut()
    {
        m_poseAnimationTarget = 0.0f;
        if (m_poseAnimationCoroutine != null)
        {
            StopCoroutine(m_poseAnimationCoroutine);
        }
        m_poseAnimationCoroutine = StartCoroutine(AnimateHand());
    }

    IEnumerator AnimateHand()
    {
        enableGripAnimations = false;
        while(!Mathf.Approximately(m_poseAnimationTarget, m_poseAnimationRig.weight))
        {
            m_poseAnimationRig.weight = Mathf.MoveTowards(m_poseAnimationRig.weight, m_poseAnimationTarget, poseAnimationSpeed * Time.deltaTime);
            yield return null;
        }
        //allow grip if animating pose out
        if(m_poseAnimationTarget < 0.5f)
            enableGripAnimations = true;
    }
}
