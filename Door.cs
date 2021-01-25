using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen;
    public bool isLocked;
    public float degrees = 90;
    public float speed = 10f;
    public float range = 5;
    public Camera player;
    public float totalTime = 6f;
    public Transform pivot;

    void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, range))
        {
            if(Input.GetKeyDown(KeyCode.E) && !isOpen)
            {
                Debug.Log("open door");
                
                StartCoroutine(OpenDoor());

            }
            else
            {
                if(Input.GetKeyDown(KeyCode.E) && isOpen)
                {
                    Debug.Log("close door");

                    StartCoroutine(CloseDoor());
                }
            }
        }
    }
    
    IEnumerator OpenDoor()
    {
        float elapsedTime = 3;
        while (elapsedTime < totalTime)
        {
            Vector3 openDoor = new Vector3(0, degrees, 0);

            pivot.transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, openDoor, Time.deltaTime * speed);
            elapsedTime += Time.deltaTime;

            isOpen = true;
            yield return null;
        }
    }

    IEnumerator CloseDoor()
    {
        float elapsedTime = 3;
        while (elapsedTime < totalTime)
        {
            Vector3 closeDoor = new Vector3(0, 0, 0);

            pivot.transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, closeDoor, Time.deltaTime * speed);
            elapsedTime += Time.deltaTime;

            isOpen = false;
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.transform.position, range);
    }
}