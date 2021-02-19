using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVDetection : MonoBehaviour
{
    public Transform player;
    public float maxAngle;
    public float maxRadius;
    public float heightMultiplayer = 1.5f;
    public RaycastHit hit;

    public bool isInFov = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (!isInFov)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * maxRadius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * maxRadius);
        }

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }

    public bool inFOV(Transform checkingObject, Transform target, float maxAngle, float maxRadius)
    {
        Vector3 directionBetween = (target.position - checkingObject.position).normalized;
        directionBetween.y *= 0;
        if (Physics.Raycast(checkingObject.position + Vector3.up * heightMultiplayer, (target.position - checkingObject.position).normalized, out hit, maxRadius))
        {
            if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Player")
            //   if(hit.target.CompareTag("Player"))
            {
                float angle = Vector3.Angle(checkingObject.forward + Vector3.up * heightMultiplayer, directionBetween);
                if (angle <= maxAngle)
                {
                    //     isInFOV = true; 
                    Debug.Log("Target detected");
                    return true;
                }
            }
        }
        //  Debug.Log ("target out");
        return false;

    }

    private void FixedUpdate()
    {
        isInFov = inFOV(transform, player, maxAngle, maxRadius);
    }
}
