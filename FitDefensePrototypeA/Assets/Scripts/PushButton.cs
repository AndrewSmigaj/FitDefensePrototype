using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class PushButton : MonoBehaviour
{
    public UnityEvent onPressed = new UnityEvent();
    public UnityEvent onReset = new UnityEvent();

    public UnityEvent onInteractionStart = new UnityEvent();
    public UnityEvent onInteractionEnd = new UnityEvent();

    [Min(0.01f)]
    public float depressionDepth = 0.015f;

    [Min(0.0001f)]
    public float pressThreshold = 0.001f;

    [Min(0.0001f)]
    public float resetThreshold = 0.001f;

    [Min(0.01f)]
    public float returnSpeed = 1.0f;

    float m_currentPressDepth = 0.0f;
    float m_yMax = 0.0f; //resting postion
    float m_yMin = 0.0f; //all the way pressed in
    bool m_wasPressed = false;

    List<Collider> m_currentColliders = new List<Collider>();
    XRBaseInteractor m_interactor = null;

    // Start is called before the first frame update
    void Start()
    {
        m_yMax = transform.localPosition.y;
    }

    void SetMinRange()
    {
        m_yMin = m_yMax - depressionDepth;
    }

    void SetHeight(float newHeight)
    {
        Vector3 currentPosition = transform.localPosition;
        currentPosition.y = newHeight;
        currentPosition.y = Mathf.Clamp(currentPosition.y, m_yMin, m_yMax);
        transform.localPosition = currentPosition;
    }

    bool IsPressed()
    {
        return transform.localPosition.y >= m_yMin && transform.localPosition.y <= m_yMin + pressThreshold;
    }

    bool IsReset()
    {
        return transform.localPosition.y >= m_yMax - resetThreshold && transform.localPosition.y <= m_yMax;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_interactor != null)
        {
            float newPressHeight = GetPressDepth(m_interactor.transform.position);
            float deltaHeight = m_currentPressDepth - newPressHeight;
            float newPressedPosition = transform.localPosition.y - deltaHeight;

            SetHeight(newPressedPosition);

            if(!m_wasPressed && IsPressed())
            {
                //we pressed the button!
                onPressed?.Invoke();
                m_wasPressed = true;
            }

            m_currentPressDepth = newPressHeight;
        }
        else
        {
            if(!Mathf.Approximately(transform.localPosition.y, m_yMax))
            {
                float returnHeight = Mathf.MoveTowards(transform.localPosition.y, m_yMax, Time.deltaTime * returnSpeed);
                SetHeight(returnHeight);
            }
        }

        if(m_wasPressed && IsReset())
        {
            onReset?.Invoke();
            m_wasPressed = false;
        }
    }

    float GetPressDepth(Vector3 interactorWorldPosition)
    {
        return transform.parent.InverseTransformPoint(interactorWorldPosition).y;
    }

    private void OnTriggerEnter(Collider other)
    {
        XRBaseInteractor interactor = other.GetComponentInParent<XRBaseInteractor>();
        if(interactor != null && !other.isTrigger)
        {
            m_currentColliders.Add(other);
            if(m_interactor == null)
            {
                m_interactor = interactor;
                SetMinRange();
                m_currentPressDepth = GetPressDepth(m_interactor.transform.position);
                onInteractionStart?.Invoke();
            }
        }
    }

    void EndPress()
    {
        m_currentColliders.Clear();
        m_currentPressDepth = 0.0f;
        m_interactor = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if(m_currentColliders.Contains(other))
        {
            m_currentColliders.Remove(other);
            if(m_currentColliders.Count == 0)
            {
                onInteractionEnd?.Invoke();
                EndPress();
            }
        }
    }
}
