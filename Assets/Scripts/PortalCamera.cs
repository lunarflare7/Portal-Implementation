using UnityEngine;
using UnityEngine.Rendering;

public class PortalCamera : MonoBehaviour
{
    [SerializeField] private Camera portalCam;
    private Camera mainCam;

    [SerializeField] private Transform linkedPortal;
    [SerializeField] private Material portalMat;
    private PortalCamera linkedPortalCam;
    private RenderTexture outputTexture;


    void OnValidate()
    {

        if (portalCam == null) portalCam = transform.GetComponentInChildren<Camera>();

        // Get sibling portal
        // Get index, swap it off of 0 or 1, then get that child
        if (linkedPortal == null)
        {
            linkedPortal = transform.parent.GetChild(Mathf.Abs(transform.GetSiblingIndex() - 1));
        }
    }

    void Start()
    {
        mainCam = PlayerManager.Instance.mainCam;

        // Set up textures
        outputTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        Renderer r = GetComponent<Renderer>();
        r.material = new Material(portalMat);
        r.material.mainTexture = outputTexture;
        linkedPortalCam = linkedPortal.GetComponentInChildren<PortalCamera>();
        linkedPortalCam.SetRenderTexture(outputTexture);   
    }

    void Update()
    {
        RenderCamera();
    }

    void RenderCamera() {

        portalCam.transform.position = mainCam.transform.position;
        portalCam.transform.rotation = mainCam.transform.rotation;

        // Position the camera
        Vector3 relativePos = linkedPortal.transform.InverseTransformPoint(mainCam.transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        portalCam.transform.position = transform.TransformPoint(relativePos);

        // Rotate the camera
        Quaternion relativeRot = Quaternion.Inverse(linkedPortal.rotation) * portalCam.transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        portalCam.transform.rotation = transform.rotation * relativeRot;


        // Set camera oblique view frustrum
        Plane p = new Plane(-transform.forward, transform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCam.worldToCameraMatrix)) * clipPlaneWorldSpace;

        Matrix4x4 obliqueProj = mainCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCam.projectionMatrix = obliqueProj;
    }

    public void SetRenderTexture(RenderTexture renderTexture) {
        portalCam.targetTexture = renderTexture;
    }
}
