using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControlOld : MonoBehaviour
{
    public bool hideHandOnGrab;
    public HandPose leftHandPose = null;
    public HandPose rightHandPose = null;
    public Transform fixedAttachment;

    //quick hack due to hand script not working with draginteractable
    public Transform attachTransform;

    public static void AlignHandLocalTransformToGrabbedAttach(Transform handVisualLocalTransform, Transform grabAttach, Transform grabbedAttach)
    {
        handVisualLocalTransform.rotation = grabbedAttach.rotation * Quaternion.Inverse(grabAttach.localRotation);
        handVisualLocalTransform.position = grabbedAttach.position + (handVisualLocalTransform.position - grabAttach.position);
    }

    public HandPose GetHandPose(HandOld.HandType type)
    {
        switch (type)
        {
            case HandOld.HandType.Left: return leftHandPose;
            case HandOld.HandType.Right: return rightHandPose;
            default: return null;
        }

    }
}
