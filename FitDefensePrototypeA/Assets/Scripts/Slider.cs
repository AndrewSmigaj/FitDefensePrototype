using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : MonoBehaviour
{
    public Material activeMaterial = null;

    public Transform startPosition = null;
    public Transform endPosition = null;

    MeshRenderer m_meshRenderer = null;
    Material m_prevMaterial = null;

    private void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    public void OnSlideStart()
    {
        m_prevMaterial = m_meshRenderer.sharedMaterial;
        m_meshRenderer.sharedMaterial = activeMaterial;
    }

    public void OnSlideStop()
    {
        m_meshRenderer.sharedMaterial = m_prevMaterial;
    }

    public void UpdateSlider(float percent)
    {
        transform.position = Vector3.Lerp(startPosition.position, endPosition.position, percent);
    }
}
