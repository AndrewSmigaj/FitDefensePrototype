using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HandPoseVisualizer : MonoBehaviour
{
    public HandPose pose;
    public Transform attachment;
    public Material material;

    [HideInInspector, SerializeField]
    GameObject thumbProxy;

    [HideInInspector, SerializeField]
    GameObject indexProxy;

    [HideInInspector, SerializeField]
    GameObject middleProxy;

    [HideInInspector, SerializeField]
    GameObject ringProxy;

    [HideInInspector, SerializeField]
    GameObject pinkyProxy;

    GameObject createProxy(string name)
    {
        GameObject proxy = new GameObject(name);
        proxy.transform.parent = transform;
        proxy.SetActive(true);
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(.01f, .01f, .01f);
        cube.transform.parent = proxy.transform;
        cube.SetActive(true);
        cube.GetComponent<MeshRenderer>().sharedMaterial = material;
        return proxy;
    }

    private void OnValidate()
    {
        if (thumbProxy) thumbProxy.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = material;
        if (indexProxy) indexProxy.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = material;
        if (middleProxy) middleProxy.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = material;
        if (ringProxy) ringProxy.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = material;
        if (pinkyProxy) pinkyProxy.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    private void Awake()
    {
        if(material == null)
        {
            material = new Material(Shader.Find("HDRP/Lit"));
            material.name = "default hand visual";
        }
    }

    private void Update()
    {
        if (attachment == null || pose == null) return;

        if (thumbProxy == null) thumbProxy = createProxy("thumb");
        if (indexProxy == null) indexProxy = createProxy("index");
        if (middleProxy == null) middleProxy = createProxy("middle");
        if (ringProxy == null) ringProxy = createProxy("ring");
        if (pinkyProxy == null) pinkyProxy = createProxy("pinky");

        thumbProxy.transform.SetPositionAndRotation(attachment.TransformPoint(pose.thumb.position), attachment.rotation * pose.thumb.rotation);
        indexProxy.transform.SetPositionAndRotation(attachment.TransformPoint(pose.index.position), attachment.rotation * pose.index.rotation);
        middleProxy.transform.SetPositionAndRotation(attachment.TransformPoint(pose.middle.position), attachment.rotation * pose.middle.rotation);
        ringProxy.transform.SetPositionAndRotation(attachment.TransformPoint(pose.ring.position), attachment.rotation * pose.ring.rotation);
        pinkyProxy.transform.SetPositionAndRotation(attachment.TransformPoint(pose.pinky.position), attachment.rotation * pose.pinky.rotation);
    }
}
