using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Animations.Rigging;

public class PoseCreatorWindow : EditorWindow
{
    [SerializeField]
    GameObject m_currentHandObject = null;

    Hand m_currentHand = null;

    [SerializeField]
    HandPose m_currentPose = null;

    [SerializeField]
    FingerPose[] m_fingers = null;

    [SerializeField]
    Transform m_attachTransform = null;

    [SerializeField]
    Transform m_grabAttachTransform = null;

    [SerializeField]
    string m_assetPath = "Assets/HandPoses";

    [SerializeField]
    string m_defaultName = "NewHandPose";

    [MenuItem("XR Hand Poser/Pose Creator")]
    static void Init()
    {
        PoseCreatorWindow window =
            (PoseCreatorWindow)EditorWindow.GetWindow(typeof(PoseCreatorWindow));
    }

    void LoadReferences()
    {
        if (m_currentHandObject != null)
        {
            m_currentHand = m_currentHandObject.GetComponentInChildren<Hand>();
            m_fingers = m_currentHand.GetComponentsInChildren<FingerPose>();
        }
    }

    private void OnEnable()
    {
        LoadReferences();
    }

    //loads relative to the hand's attach transform
    //make sure your grabbed object attach transform and hand attach transform are the same in world space.
    void LoadTransform(Pose pose, Transform transform)
    {
        transform.SetPositionAndRotation(m_attachTransform.TransformPoint(pose.position), m_attachTransform.rotation * pose.rotation);
    }

    void LoadPose(HandPose pose)
    {
        if (m_fingers == null) return;

        foreach (var finger in m_fingers)
        {
            switch (finger.finger)
            {
                case FingerId.Thumb:
                    LoadTransform(pose.thumb, finger.transform);
                    break;
                case FingerId.Index:
                    LoadTransform(pose.index, finger.transform);
                    break;
                case FingerId.Middle:
                    LoadTransform(pose.middle, finger.transform);
                    break;
                case FingerId.Ring:
                    LoadTransform(pose.ring, finger.transform);
                    break;
                case FingerId.Pinky:
                    LoadTransform(pose.pinky, finger.transform);
                    break;
            }
        }
    }

    //save in local terms of the attachment transform
    void SaveTransform(Transform transform, ref Pose pose)
    {
        pose.position = m_attachTransform.InverseTransformPoint(transform.position);
        pose.rotation = Quaternion.Inverse(m_attachTransform.rotation) * transform.rotation;
    }

    HandPose CreateNewPose()
    {
        HandPose handPose = ScriptableObject.CreateInstance<HandPose>();
        AssetDatabase.CreateAsset(handPose, m_assetPath + "/" + m_defaultName + ( m_currentHand != null ? " - " + m_currentHand.type.ToString() : "") + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = handPose;
        return handPose;
    }

    void OnGUI()
    {
        if ((m_fingers == null || m_currentHand == null) && m_currentHandObject != null)
        {
            m_currentHand = m_currentHandObject.GetComponentInChildren<Hand>();
            m_fingers = m_currentHand.GetComponentsInChildren<FingerPose>();
        }

        EditorGUILayout.LabelField("Path To Save Hand Poses");
        m_assetPath = EditorGUILayout.TextField(m_assetPath);
        EditorGUILayout.LabelField("Pose Name");
        m_defaultName = EditorGUILayout.TextField(m_defaultName);
        if (GUILayout.Button("Create New Pose"))
        {
            m_currentPose = CreateNewPose();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Hand");
        EditorGUILayout.BeginHorizontal();
        GUI.changed = false;
        m_currentHandObject = EditorGUILayout.ObjectField(m_currentHandObject, typeof(GameObject), true) as GameObject;
        if (GUI.changed && m_currentHandObject != null)
        {
            m_currentHand = m_currentHandObject.GetComponentInChildren<Hand>();
            m_fingers = m_currentHand.GetComponentsInChildren<FingerPose>();
            GUI.changed = false;
        }
        else if(GUI.changed && m_currentHandObject == null)
        {
            m_currentHand = null;
            m_fingers = null;
        }
        EditorGUILayout.EndHorizontal();
        if (m_currentHand != null)
        { 
            if (GUILayout.Button("Reset To Default Pose"))
            {
                foreach(var finger in m_fingers)
                {
                    var constraint = finger.GetComponent<TwoBoneIKConstraint>().data;
                    finger.transform.SetPositionAndRotation(constraint.tip.position, constraint.tip.rotation);
                }
            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Hand Attach Transform");
        EditorGUILayout.BeginHorizontal();
        m_grabAttachTransform = EditorGUILayout.ObjectField(m_grabAttachTransform, typeof(Transform), true) as Transform;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Interactable Attach Transform");
        EditorGUILayout.BeginHorizontal();
        m_attachTransform = EditorGUILayout.ObjectField(m_attachTransform, typeof(Transform), true) as Transform;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Pose");
        EditorGUILayout.BeginHorizontal();
        GUI.changed = false;
        m_currentPose = EditorGUILayout.ObjectField(m_currentPose, typeof(HandPose), false) as HandPose;
        if(GUI.changed && m_currentPose != null)
        {
            LoadPose(m_currentPose);
            GUI.changed = false;
        }
        EditorGUILayout.EndHorizontal();
        if (m_currentPose != null)
        {
            if (GUILayout.Button("Update Pose"))
            {
                foreach (var finger in m_fingers)
                {
                    switch (finger.finger)
                    {
                        case FingerId.Thumb:
                            SaveTransform(finger.transform, ref m_currentPose.thumb);
                            break;
                        case FingerId.Index:
                            SaveTransform(finger.transform, ref m_currentPose.index);
                            break;
                        case FingerId.Middle:
                            SaveTransform(finger.transform, ref m_currentPose.middle);
                            break;
                        case FingerId.Ring:
                            SaveTransform(finger.transform, ref m_currentPose.ring);
                            break;
                        case FingerId.Pinky:
                            SaveTransform(finger.transform, ref m_currentPose.pinky);
                            break;
                    }
                }
                EditorUtility.SetDirty(m_currentPose);
            }
            if (GUILayout.Button("Load Pose"))
            {
                if(m_currentHand != null) LoadPose(m_currentPose);
            }
        }
    }
}
