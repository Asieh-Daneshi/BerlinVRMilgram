using UnityEngine;

//FROM https://github.com/OSVR/Unity-VR-Samples/blob/master/Assets/VRStandardAssets/Scripts/VREyeRaycaster.cs

/* 
- attach it to the "scene"
- as camera set: [CamerRig]->Camera (Head)->Camera (eye)
*/

public class VREyeRaycaster : MonoBehaviour
{
//    public event Action<RaycastHit> OnRaycasthit;                   // This event is called every frame that the user's gaze is over a collider.

    [SerializeField] Transform m_Camera;
    [SerializeField] LayerMask m_ExclusionLayers; // Layers to exclude from the raycast.
    [SerializeField] bool m_ShowDebugRay; // Optionally show the debug ray.
    [SerializeField] float m_DebugRayLength = 50f; // Debug ray length.
    [SerializeField] float m_DebugRayDuration = 0.2f; // How long the Debug ray will remain visible.
    [SerializeField] float m_RayLength = 500f; // How far into the scene the ray is cast.

    VRInteractiveItemBaseclass m_LastInteractible;


    void Update()
    {
        var layerMask = 1 << 8;
        // This would cast rays only against colliders in layer 8.
        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        if (Physics.Raycast(m_Camera.position, m_Camera.forward, out hit, m_RayLength, layerMask))
        {
            /*OR m_ExclusionLayers */

            if (m_ShowDebugRay) Debug.DrawRay(m_Camera.position, m_Camera.forward * m_DebugRayLength, Color.yellow);

            var interactible = hit.collider.GetComponent<VRInteractiveItemBaseclass>();
            if (interactible)
            {
                if (interactible != m_LastInteractible) interactible.FocusIn();
                m_LastInteractible = interactible;
            }
        }
        else
        {
            if (m_ShowDebugRay) Debug.DrawRay(m_Camera.position, m_Camera.forward * m_DebugRayLength, Color.white);
            if (m_LastInteractible)
            {
                m_LastInteractible.FocusOut();
                m_LastInteractible = null;
            }
        }
    }
}