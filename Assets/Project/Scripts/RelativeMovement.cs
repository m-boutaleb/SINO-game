﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RelativeMovement : MonoBehaviour
{
    [SerializeField] private Transform target;

    public float moveSpeed = 6.0f;
    public float speedUpMultiplier = 2.0f;

    private CharacterController _charController;
    private Animator _animator;
    private bool isDead = false;
    private float gravity = -9.81f;
    private float yVelocity = 0.0f;
    private bool endAnimation = false;

    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        bool speedUp = Input.GetKey(KeyCode.LeftShift);
        _animator.SetBool("isRunning", speedUp);

        Vector3 movement = Vector3.zero;
        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");

        bool idle = horInput == 0 && vertInput == 0;
        _animator.SetBool("idle", idle);

        if (!idle)
        {
            movement.x = horInput;
            movement.z = vertInput;

            //create movement vector from camera perspective
            Quaternion tmp = target.rotation;
            target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
            movement = target.TransformDirection(movement);
            target.rotation = tmp;

            //rotation payer
            Quaternion to = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, to, 0.1f);

            //movement palyer
            Vector3.ClampMagnitude(movement, moveSpeed * (speedUp ? speedUpMultiplier : 1.0f));
            _charController.Move(movement * Time.deltaTime * moveSpeed * (speedUp ? speedUpMultiplier : 1.0f));
        }

        //gravity for stairs
        if (!_charController.isGrounded)
        {
            movement = Vector3.zero;
            yVelocity += gravity;
            movement.y = yVelocity;
            _charController.Move(movement * Time.deltaTime);
        }
        else
        {
            yVelocity = 0.0f;
        }

    }

    public void reactToGuard(Transform guard)
    {
        //set parameters for animations
        _animator.SetBool("idle", true);
        _animator.SetBool("isRunning", false);

        // disable movement player
        isDead = true;

        //rotation in y in the direction of the guard
        Quaternion rot = Quaternion.LookRotation(guard.position - transform.position);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.01f);

        ObjectsInteraction oi = GetComponent<ObjectsInteraction>();
        oi.Running = false;
        OrbitCamera oc = target.GetComponent<OrbitCamera>();
        oc.Running = false;

        //start animation after few moments
        StartCoroutine(Die());

        if(endAnimation)
            DisplayGameOver();
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(.6f);
        _animator.SetBool("isDead", isDead);
        yield return new WaitForSeconds(1.0f);
        endAnimation = true;
    }

    private void DisplayGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}