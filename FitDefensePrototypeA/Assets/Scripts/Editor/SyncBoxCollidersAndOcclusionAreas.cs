using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SyncBoxCollidersAndOcclusionAreas 
{
    [MenuItem("GameObject/Sync Occlusion Areas and Colliders")]
    static void Sync()
    {
        List<OcclusionArea> areasInScene = new List<OcclusionArea>();

        foreach (OcclusionArea area in Resources.FindObjectsOfTypeAll(typeof(OcclusionArea)) as OcclusionArea[])
        {
            if (!EditorUtility.IsPersistent(area.transform.root.gameObject) && !(area.gameObject.hideFlags == HideFlags.NotEditable || area.gameObject.hideFlags == HideFlags.HideAndDontSave))
                areasInScene.Add(area);
        }

        foreach(OcclusionArea area in areasInScene)
        {
            BoxCollider collider = area.GetComponent<BoxCollider>();
            if(collider != null)
            {
                collider.center = area.center;
                collider.size = area.size;
            }
        }
    }
}
