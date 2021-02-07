using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CabinetController : MonoBehaviour
{
    [SerializeField] private int rayLength = 5;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private string excludeLayerName = null;

    private Cabinet cabinetRaycast;
    private Cabinet2 cabinetRaycast2;

    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0;

    [SerializeField] private Image crosshair = null;
    private bool isCrosshairActive;
    private bool doOnce;

    private const string interactableTag = "InteractableCabinet";

    public GameObject rayHit;
    void Start()
    {
        cabinetRaycast = GetComponent<Cabinet>();
    }
    void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        int mask = 1 << LayerMask.NameToLayer(excludeLayerName) | interactableMask.value;

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
        {
            rayHit = hit.collider.gameObject;
            if (hit.collider.CompareTag(interactableTag))
            {
                if (!doOnce)
                {
                    cabinetRaycast = hit.collider.gameObject.GetComponent<Cabinet>();

                    CrosshairChange(true);
                    Debug.Log(hit.collider.name);
                }

                isCrosshairActive = true;
                doOnce = true;

                if (Input.GetKeyDown(interactKey))
                {
                    cabinetRaycast.PlayAnim();
                }
            }
        }
        else
        {
            if (isCrosshairActive)
            {
                CrosshairChange(false);
                doOnce = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rayLength);
    }

    void CrosshairChange(bool on)
    {
        if (on && !doOnce)
        {
            crosshair.color = Color.red;
        }
        else
        {
            crosshair.color = Color.white;
            isCrosshairActive = false;
        }
    }
}
