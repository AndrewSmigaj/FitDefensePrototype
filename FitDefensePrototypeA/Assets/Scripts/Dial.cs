using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dial : MonoBehaviour
{
    Vector3 m_startRotation;

    MeshRenderer m_meshRenderer = null;

    private void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    public void StartTurn()
    {
        m_startRotation = transform.localEulerAngles;
        m_meshRenderer.material.SetColor("_Color", Color.red);
    }

    public void StopTurn()
    {
        m_meshRenderer.material.SetColor("_Color", Color.white);
    }

    public void DialUpdate(float angle)
    {
        Vector3 angles = m_startRotation;
        angles.z += angle;
        transform.localEulerAngles = angles;
    }
}
