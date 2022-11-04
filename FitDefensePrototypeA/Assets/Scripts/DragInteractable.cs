using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

[Serializable]
public class DragEvent : UnityEvent<float> {}

public class DragInteractable : XRBaseInteractable
{
    public Transform startDragPosition = null;
    public Transform endDragPosition = null;

    [HideInInspector]
    public float dragPercent = 0.0f; //[0,1]

    protected XRBaseInteractor m_interactor = null;

    public UnityEvent onDragStart = new UnityEvent();
    public UnityEvent onDragEnd = new UnityEvent();
    public DragEvent onDragUpdate = new DragEvent();
    //public Transform attachTransform = null;

    Coroutine m_drag = null;


    void StartDrag()
    {
        if(m_drag != null)
        {
            StopCoroutine(m_drag);
        }
        m_drag = StartCoroutine(CalculateDrag());
        onDragStart?.Invoke();
    }

    void EndDrag()
    {
        if (m_drag != null)
        {
            StopCoroutine(m_drag);
            m_drag = null;
            onDragEnd?.Invoke();
        }
    }
    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        //the dot of a to value divided by the dot of the total range
        //gives the normalized 0-1 distance of value between a and b
        return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
    }

    IEnumerator CalculateDrag()
    {
        while(m_interactor != null)
        {
            // get a line in local space
            Vector3 line = startDragPosition.localPosition - endDragPosition.localPosition;

            //convert our interactor position to local space
            Vector3 interactorLocalPosition = startDragPosition.parent.InverseTransformPoint(m_interactor.transform.position);

            //project the interactor position onto the line
            Vector3 projectedPoint = Vector3.Project(interactorLocalPosition, line.normalized);

            //reverse interpolate that position on the line to get a percentage of how far the drag has moved
            dragPercent = InverseLerp(startDragPosition.localPosition, endDragPosition.localPosition, projectedPoint);

            onDragUpdate?.Invoke(dragPercent);

            yield return null;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("Select Entered Slider");
        m_interactor = args.interactor;
        StartDrag();
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs interactor)
    {
        EndDrag();
        m_interactor = null;
        base.OnSelectExited(interactor);
    }
}
