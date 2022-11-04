using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public Material activeMaterial = null;

    public Transform startOrientation = null;
    public Transform endOrientation = null;

    MeshRenderer m_meshRenderer = null;
    Material m_prevMaterial = null;

    private void Start()
    {
        m_meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void OnLeverPullStart()
    {
        m_prevMaterial = m_meshRenderer.sharedMaterial;
        m_meshRenderer.sharedMaterial = activeMaterial;
    }

    public void OnLeverPullStop()
    {
        m_meshRenderer.sharedMaterial = m_prevMaterial;
    }

    public void UpdateLever(float percent)
    {
        transform.rotation = Quaternion.Slerp(startOrientation.rotation, endOrientation.rotation, percent);
    }
}
