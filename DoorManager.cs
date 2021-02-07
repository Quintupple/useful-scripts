using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorManager : MonoBehaviour
{

    [SerializeField] private int rayLength = 5;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private string excludeLayerName = null;

    private Door doorRaycast;
    public KeyManager keyManager;
    public TextType textType;

    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0;

    [SerializeField] private Image crosshair = null;
    private bool isCrosshairActive;
    private bool doOnce;

    private const string interactableTag = "InteractableDoor";

    public GameObject rayHit;
    void Start()
    {
        doorRaycast = GetComponent<Door>();
    }
    void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        int mask = 1 << LayerMask.NameToLayer(excludeLayerName) | interactableMask.value;

        if(Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
        {
            rayHit = hit.collider.gameObject;
            if(hit.collider.CompareTag(interactableTag))
            {
                if(!doOnce)
                {
                    doorRaycast = hit.collider.gameObject.GetComponent<Door>();

                    CrosshairChange(true);
                    Debug.Log(hit.collider.name);
                }

                isCrosshairActive = true;
                doOnce = true;

                if(Input.GetKeyDown(interactKey))
                {
                    if(doorRaycast.isLocked && !keyManager.hasKey)
                    {
                        AudioSource.PlayClipAtPoint(doorRaycast.doorLockedSound, transform.position);
                        doorRaycast.doorAnim.Play("door_locked");
                        doorRaycast.isLocked = true;
                        doorRaycast.hasUnlocked = false;
                        textType.txt.SetText("It's locked.");
                        textType.StartCoroutine("RemoveText");
                        print("You do not have the key! This door is locked.");
                    }
                    else
                    {
                        if(doorRaycast.isLocked && keyManager.hasKey)
                        {
                            doorRaycast.isLocked = false;
                            doorRaycast.hasUnlocked = true;
                            doorRaycast.PlayAnim();
                            print("You have unlocked the door!");
                            keyManager.hasKey = false;
                        }
                        else
                        {
                            if(!doorRaycast.isLocked && keyManager.hasKey)
                            {
                                doorRaycast.PlayAnim();
                                print("door was not locked");
                            }
                            else
                            {
                                if(doorRaycast.hasUnlocked == true)
                                {
                                    doorRaycast.PlayAnim();
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if(isCrosshairActive)
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
        if(on && !doOnce)
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
