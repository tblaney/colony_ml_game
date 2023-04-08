using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightHandler : MonoBehaviour, IHandler
{
    public static HighlightHandler Instance;

    public void Initialize()
    {
        Instance = this;
    }
    public GameObject NewHighlightObject(MeshRenderer mesh)
    {
        GameObject obj = new GameObject();
        obj.name = "Highlighter, " + mesh.gameObject.name;
        obj.layer = 10;

        obj.transform.position = mesh.transform.position;
        obj.transform.rotation = mesh.transform.rotation;
        SetGlobalScale(obj.transform, mesh.transform.lossyScale);

        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        MeshFilter filter = obj.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh.GetComponent<MeshFilter>().sharedMesh;
        meshRenderer.sharedMaterials = mesh.sharedMaterials;

        return obj;
    }

    public static void SetGlobalScale (Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3 (globalScale.x/transform.lossyScale.x, globalScale.y/transform.lossyScale.y, globalScale.z/transform.lossyScale.z);
    }
}
