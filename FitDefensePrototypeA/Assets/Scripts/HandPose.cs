using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Pose
{
    public static readonly Pose empty = new Pose() { position = Vector3.zero, rotation = Quaternion.identity};

    public Vector3 position;
    public Quaternion rotation;
}

[CreateAssetMenu(menuName = "XR/Create New Hand Pose")]
public class HandPose : ScriptableObject
{
    public Pose thumb = Pose.empty;
    public Pose index = Pose.empty;
    public Pose middle = Pose.empty;
    public Pose ring = Pose.empty;
    public Pose pinky = Pose.empty;
}
