using System;
using UnityEngine;
public class PortalCamera : MonoBehaviour
{
    [SerializeField] private Camera portalCam;
    private Camera mainCam;

    [SerializeField] private Transform destinationPortal;
    [SerializeField] private Material portalMat;
    [SerializeField] private int recursionLimit = 10;
    [SerializeField] private Gradient fadeGradient;
    private RenderTexture outputTexture;
    private new Renderer renderer;


    void OnValidate()
    {

        if (portalCam == null) portalCam = transform.GetComponentInChildren<Camera>();

        // Get sibling portal
        // Get index, swap it off of 0 or 1, then get that child
        if (destinationPortal == null)
        {
            destinationPortal = transform.parent.GetChild(Mathf.Abs(transform.GetSiblingIndex() - 1));
        }
    }

    void Start()
    {
        mainCam = PlayerManager.Instance.mainCam;
        portalCam.enabled = false;

        // Set up textures
        outputTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        renderer = GetComponent<Renderer>();
        renderer.material = new Material(portalMat);
        renderer.material.mainTexture = outputTexture;

        // My portalCam outputs to my texture - this allows for multiple portals pointing to a single destination
        portalCam.targetTexture = outputTexture;
    }

    void Update()
    {
        if(renderer.isVisible)
        {
            for (int i = recursionLimit; i > 0; i--)
            {
                RenderCamera(i);
            }
        }
    }

    void RenderCamera(int iteration)
    {

        // Position my portalCam relative to my destination portal

        // Reset position
        portalCam.transform.position = mainCam.transform.position;
        portalCam.transform.rotation = mainCam.transform.rotation;

        for (int i = 0; i < iteration; i++)
        {
            // Position the camera
            Vector3 relativePos = transform.InverseTransformPoint(portalCam.transform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            portalCam.transform.position = destinationPortal.TransformPoint(relativePos);

            // Rotate the camera
            Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * portalCam.transform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            portalCam.transform.rotation = destinationPortal.rotation * relativeRot;
        }

        // Matrix4x4 localToWorldMatrix = mainCam.transform.localToWorldMatrix;
        // Matrix4x4[] matrices = new Matrix4x4[recursionLimit];
        // for (int i = 0; i < recursionLimit; i++)
        // {
        //     localToWorldMatrix = destinationPortal.localToWorldMatrix * transform.worldToLocalMatrix * localToWorldMatrix;
        //     matrices[recursionLimit - i - 1] = localToWorldMatrix;
        // }

        // for (int i = 0; i < recursionLimit; i++)
        // {
        //     portalCam.transform.SetPositionAndRotation(matrices[i].GetColumn(3), matrices[i].rotation);
        //     // Set camera oblique view frustrum

        // }

        renderer.material.color = fadeGradient.Evaluate((float)iteration / (float)recursionLimit);
        Debug.Log(iteration / recursionLimit + ", " + fadeGradient.Evaluate(iteration / recursionLimit));

        SetObliqueProjection();
        portalCam.Render();
    }
    
    private void SetObliqueProjection()
    {
        Plane p = new Plane(-destinationPortal.forward, destinationPortal.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCam.worldToCameraMatrix)) * clipPlaneWorldSpace;

        Matrix4x4 obliqueProj = mainCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCam.projectionMatrix = obliqueProj;

        // Transform clipPlane = transform;
        // int dot = Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - portalCam.transform.position));

        // Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        // Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        // float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal);
        // Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

        // portalCam.projectionMatrix = mainCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }

    public void SetRenderTexture(RenderTexture renderTexture)
    {
        portalCam.targetTexture = renderTexture;
    }

    void OnDestroy()
    {
        outputTexture.Release();

        Destroy(outputTexture);
    }
}
