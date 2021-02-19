using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Character Controller")]
    public CharacterController controller;
    public float controllerHeight = 1.79f;
    public float crouchHeight = 0.8f;
    public float speed = 1.5f;
    public float sprintSpeed = 2.5f;
    public float gravity = -9.81f;
    Vector3 currentVelocity;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;
    public float jumpHeight = 3f;
    bool isWalking = false;

    [Header("Footsteps")]
    [SerializeField] Footsteps footsteps;
    [SerializeField] float timer = 0.6f;

    private void Start()
    {
        footsteps = GetComponent<Footsteps>();
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && currentVelocity.y < 0)
        {
            currentVelocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        Vector3 playerVelocity = new Vector3(x, 0, z);
        playerVelocity *= speed * Time.deltaTime;
        playerVelocity = transform.TransformDirection(playerVelocity);

        if(playerVelocity.x < 0 || playerVelocity.x > 0 || playerVelocity.y < 0 || playerVelocity.y > 0 || playerVelocity.z < 0 || playerVelocity.z > 0)
        {
            if (!isWalking)
            {
                PlayFootSounds();
            }
        }

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            currentVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        currentVelocity.y += gravity * Time.deltaTime;

        controller.Move(currentVelocity * Time.deltaTime);

        if(Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
            timer = 0.45f;
            footsteps.audioSource.volume = 0.300f;
        }
        else
        {
            speed = 1.5f;
            timer = 0.6f;
        }

        // Crouch
        if(Input.GetKey(KeyCode.LeftControl))
        {
            controller.height = controllerHeight / 3;
            footsteps.audioSource.volume = 0.03f;
        }
        else
        {
            controller.height = controllerHeight;
        }
    }

    void PlayFootSounds()
    {
        StartCoroutine("PlayFootsteps", timer);
    }

    IEnumerator PlayFootsteps(float timer)
    {
        var randomIndex = Random.Range(0, 8);
        var randomVolume = Random.Range(0.1f, 0.2f);
        footsteps.audioSource.clip = footsteps.footStepSounds[randomIndex];
        footsteps.audioSource.volume = randomVolume;
        isWalking = true;

        footsteps.audioSource.Play();

        yield return new WaitForSeconds(timer);

        isWalking = false;
    }
}
