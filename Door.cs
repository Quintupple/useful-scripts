using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Properties")]
    public bool isOpen;
    public bool isLocked;

    [Header("Hinge Properties")]
    public float totalTime = 4f;
    public Transform pivot;
    public float degrees = 90;
    public float speed = 3.5f;

    [Header("Raycast Properties")]
    public Camera player;
    public float range = 3.77f;
    
    [Header("Audio Clips")]
    public AudioSource closeDoorSource;
    public AudioSource openDoorSource;
    public AudioClip open_door;
    public AudioClip close_door;

    void Start()
    {
        closeDoorSource = GetComponent<AudioSource>();
        openDoorSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, range))
        {
            if(Input.GetKeyDown(KeyCode.E) && !isOpen)
            {
                Debug.Log("open door");

                openDoorSource.PlayOneShot(open_door);
                StartCoroutine(OpenDoor());

            }
            else
            {
                if(Input.GetKeyDown(KeyCode.E) && isOpen)
                {
                    Debug.Log("close door");

                    StartCoroutine(CloseDoor());
                    closeDoorSource.PlayOneShot(close_door);
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