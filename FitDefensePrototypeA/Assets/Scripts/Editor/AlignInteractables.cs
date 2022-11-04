using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public static class AlignInteractables
{
    static bool FilterSourceAndTargetFromSelection(out Transform sourceParent, out Transform sourceAttachment, out Transform targetAttachment)
    {
        var selected = Selection.instanceIDs;
        if (selected == null || selected.Length != 3)
        {
            sourceParent = sourceAttachment = targetAttachment = null;
            Debug.LogWarning("Couldn't align interactables because there weren't enough game objects selected. Select in order: 1. Source Interactable. 2. Source Attach Transform. 3. Destination Attach transform.");
            return false;
        }

        var srcParentGameObject = EditorUtility.InstanceIDToObject(selected[0]) as GameObject;
        var srcAttachmentGameObject = EditorUtility.InstanceIDToObject(selected[1]) as GameObject;
        var targetAttachmentGameObject = EditorUtility.InstanceIDToObject(selected[2]) as GameObject;

        if (srcParentGameObject == null || srcAttachmentGameObject == null || targetAttachmentGameObject == null)
        {
            sourceParent = sourceAttachment = targetAttachment = null;
            Debug.LogWarning("Couldn't align interactables because one of the selections was not a game object. Select in order: 1. Source Interactable. 2. Source Attach Transform. 3. Destination Attach transform.");
            return false;
        }

        sourceParent = srcParentGameObject.transform;
        sourceAttachment = srcAttachmentGameObject.transform;
        targetAttachment = targetAttachmentGameObject.transform;

        return true;
    }

    [MenuItem("XR Hand Poser/Align Interactables by Attach Transform", false, 0)]
    static void AlignAttachTransforms()
    {
        if (FilterSourceAndTargetFromSelection(out Transform sourceParent, out Transform sourceAttachTransform, out Transform targetAttachTransform))
        {
            Undo.RecordObject(sourceParent, "Align attach transform " + sourceParent.name + " with " + targetAttachTransform.name);

            HandControl.AlignHandToAttachment(sourceParent, sourceAttachTransform, targetAttachTransform);
        }
    }
}

#endif
