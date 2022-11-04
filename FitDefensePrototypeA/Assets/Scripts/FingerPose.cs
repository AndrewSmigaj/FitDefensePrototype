using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FingerId
{
    Thumb, 
    Index,
    Middle,
    Ring,
    Pinky
}

public class FingerPose : MonoBehaviour
{
    public FingerId finger;
}
