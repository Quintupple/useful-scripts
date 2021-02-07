using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    public AudioSource openSource;
    public AudioSource closeSource;
    public AudioClip cabinet_openAudio;
    public AudioClip cabinet_closeAudio;
    private Animator drawerAnim;
    private bool drawerOpen = false;

    [Header("Pause Timer")]
    [SerializeField] private int waitTimer = 1;
    [SerializeField] private bool pauseInteraction = false;

    void Awake()
    {
        drawerAnim = gameObject.GetComponent<Animator>();
    }

    public void PlayAnim()
    {
        if(!drawerOpen && !pauseInteraction)
        {
            drawerAnim.Play("open_drawer1", 0);
            AudioSource.PlayClipAtPoint(cabinet_openAudio, transform.position);
            drawerOpen = true;
            StartCoroutine(PauseInteraction());
        }
        else if(drawerOpen && !pauseInteraction)
        {
            drawerAnim.Play("close_drawer1", 0);
            AudioSource.PlayClipAtPoint(cabinet_closeAudio, transform.position);
            drawerOpen = false;
            StartCoroutine(PauseInteraction());
        }
    }
    private IEnumerator PauseInteraction()
    {
        pauseInteraction = true;
        yield return new WaitForSeconds(waitTimer);
        pauseInteraction = false;

    }

}
