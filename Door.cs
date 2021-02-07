using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public AudioSource openSource;
    public AudioSource closeSource;
    public AudioClip door_openAudio;
    public AudioClip door_closeAudio;
    public AudioClip doorLockedSound;
    public Animator doorAnim;
    public bool doorOpen = false;
    public bool isLocked = false;
    public bool hasUnlocked = false;
    public bool hasKey = false;

    [Header("Pause Timer")]
    [SerializeField] private int waitTimer = 1;
    [SerializeField] private bool pauseInteraction = false;

    void Awake()
    {
        doorAnim = gameObject.GetComponent<Animator>();
    }

    public void PlayAnim()
    {
        if(!doorOpen && !pauseInteraction)
        {
            doorAnim.Play("door_open");
            AudioSource.PlayClipAtPoint(door_openAudio, transform.position);
            doorOpen = true;
            StartCoroutine(PauseInteraction());
        }
        else if(doorOpen && !pauseInteraction)
        {
            doorAnim.Play("door_close");
            AudioSource.PlayClipAtPoint(door_closeAudio, transform.position);
            doorOpen = false;
            StartCoroutine(PauseInteraction());
        }
    }

    public IEnumerator PauseInteraction()
    {
        pauseInteraction = true;
        yield return new WaitForSeconds(waitTimer);
        pauseInteraction = false;

    }
}