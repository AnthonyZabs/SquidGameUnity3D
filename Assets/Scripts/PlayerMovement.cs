using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;
    private float xRot;

    [SerializeField]
    private Animator anim;

    private bool isJumping;
    private bool isWalking;
    private bool isMoving;
    private bool isDying;
    private bool isInDeathZone;

    [SerializeField]
    private LayerMask FloorMask;

    [SerializeField]
    private Transform FeetTransform;

    [SerializeField]
    private Transform PlayerCamera;

    [SerializeField]
    private Rigidbody PlayerBody;

    [Space]

    [SerializeField]
    private float Speed;

    [SerializeField]
    private float Sensitivity;

    [SerializeField]
    private float Jumpforce;

    [SerializeField]
    AudioSource feetSteps;
    [SerializeField]
    AudioSource shoot;

    [SerializeField]
    private Transform deathZone;

    // Update is called once per frame
    void Update()
    {
        if (isDying)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        PlayerMovementInput = new Vector3(h, 0f, v);
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        CheckJumping();
        CheckMoving();
        MovePlayer();
        MovePlayerCamera();

        isWalking = (h != 0 || v != 0);
        anim.SetBool("isWalking", isWalking);

        if (isWalking && !isJumping && !feetSteps.isPlaying)
        {
            feetSteps.loop = true;
            feetSteps.Play(0);
        }
        else
            feetSteps.loop = false;

        CheckDeathTime();
    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput) * Speed;
        PlayerBody.velocity = new Vector3(MoveVector.x, PlayerBody.velocity.y, MoveVector.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isJumping)
                PlayerBody.AddForce(Vector3.up * Jumpforce, ForceMode.Impulse);
        }
    }

    private void MovePlayerCamera()
    {
        xRot -= PlayerMouseInput.y * Sensitivity;

        transform.Rotate(0f, PlayerMouseInput.x * Sensitivity, 0f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }

    private void CheckJumping()
    {
        isJumping = !Physics.CheckSphere(FeetTransform.position, 0.1f, FloorMask);
        anim.SetBool("isJumping", isJumping);
    }

    private void CheckMoving()
    {
        isMoving = isJumping || isWalking;
    }

    private void CheckDeathTime()
    {
        if (GameManager.headTime && isMoving || GameManager.headTimeFinish)
        {
            if (!isInDeathZone)
                return;

            isDying = true;
            anim.SetBool("isDying", true);
            feetSteps.Stop();
            shoot.Play(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isInDeathZone = other.transform == deathZone;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == deathZone)
            isInDeathZone = false;
    }
}
