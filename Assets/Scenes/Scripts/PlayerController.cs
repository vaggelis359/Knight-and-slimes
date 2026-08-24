using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static MyGame.Weapons.Weapon;


public class PlayerController : MonoBehaviour
{
    public float vertical;
    public float horizontal;
    public float runInput;

    [SerializeField] private float moveSpeed = 0;
    [SerializeField] private float rotationSpeed = 1;
    private float speedMultiplier;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffSet;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;

    float ySpeed;
    Quaternion targetRotation;

    CameraController cameraController;
    CharacterController characterController;
    Animator animator;

    [SerializeField] private float lastClickedTime = 0f;
    [SerializeField] public static int noOfClicks = 0;
    float maxComboDelay = 1f;
    private float nextFireTime = 0f;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

    }

    private void Update()
    {
        GetInput();

        { 
            playerMoving();
            // if (Input.GetButton("Run"))
            // {
            //     moveSpeed = 7f;
            // }
            // else
            // {
            //     moveSpeed = 1f;
            // }
            // Attack animation checks, resetting states if animations are finished
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName("attack1"))
            {
                animator.SetBool("attack1", false);
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName("attack2"))
            {
                animator.SetBool("attack2", false);
                noOfClicks = 0;
            }

            // Reset combo if time exceeds maxComboDelay
            if (Time.time - lastClickedTime > maxComboDelay)
            {
                noOfClicks = 0;
            }

            // Check for sword attack on Fire2 input
            if (Time.time > nextFireTime && Input.GetButtonDown("Fire2"))
            {
                swordAttack();
            }
        }

    }

    void GetInput()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        runInput = Input.GetButton("RunInput") ? 8f : 1f;
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffSet), groundCheckRadius, groundLayer);
    }

    //// WeaponHolder.isAiming = true;  NA einai true otan kanoume  attack me mouseclick
    private void swordAttack()                  // Επιθεση οταν κραταμε Sword ΕΧΕΙ ΤΑ ΑΝIMATIONS TOY SWORD
    {
        {
            lastClickedTime = Time.time;
            noOfClicks++;

            // Εκκίνηση πρώτου animation επίθεσης
            if (noOfClicks == 1)
            {
                animator.SetBool("attack1", true);
            }

            noOfClicks = Mathf.Clamp(noOfClicks, 0, 2);

            // Έλεγχος αν το πρώτο animation έχει ολοκληρωθεί (στο layer "SwordLayer")
            int swordLayerIndex = animator.GetLayerIndex("SwordLayer");  // Βρίσκουμε το index του "SwordLayer"
            if (noOfClicks >= 1 && animator.GetCurrentAnimatorStateInfo(swordLayerIndex).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(swordLayerIndex).IsName("attack1"))
            {
                animator.SetBool("attack1", false);
                animator.SetBool("attack2", true);  // Ενεργοποίηση δεύτερου animation επίθεσης
            }
        }
    }

    //  private void AnvilHammerAttack()                  // Επιθεση οταν κραταμε AnvilHammer ΕΧΕΙ ΤΑ ΑΝIMATIONS TOY AnvilHammer
    //  {
    //      {
    //          lastClickedTime = Time.time;
    //          noOfClicks++;
    //
    //          // Εκκίνηση πρώτου animation επίθεσης
    //          if (noOfClicks == 1)
    //          {
    //              animator.SetBool("attack1", true);
    //          }
    //
    //          noOfClicks = Mathf.Clamp(noOfClicks, 0, 2);
    //
    //          // Έλεγχος αν το πρώτο animation έχει ολοκληρωθεί (στο layer "SwordLayer")
    //          int AnvilHammerLayerIndex = animator.GetLayerIndex("AnvilHammerLayer");  // Βρίσκουμε το index του "SwordLayer"
    //          if (noOfClicks >= 1 && animator.GetCurrentAnimatorStateInfo(AnvilHammerLayerIndex).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(AnvilHammerLayerIndex).IsName("attack1"))           
    //          {
    //              animator.SetBool("attack1", false);
    //              animator.SetBool("attack2", true);  // Ενεργοποίηση δεύτερου animation επίθεσης
    //          }
    //      }
    //  }

    private void FindFloorPositions(Transform objectTransform, ref Vector3 targetPosition, ref Quaternion targetRotation, Vector3 direction)
    {
        if (Physics.Raycast(objectTransform.position, direction, out RaycastHit hit, Mathf.Infinity))
        {
            targetPosition = hit.point;
            targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * objectTransform.rotation;
        }
    }

    private void playerMoving()
    {
        float h = horizontal;                                             // Με το a/d μετακινειται δεξιά/αριστερά
        float v = vertical;                                                // Με το w/s μετακινειται μπροστα/πισω

        //Debug.Log($"Horizontal: {h}, Vertical: {v}");                                     // Log για να βλεπουμε τις τιμες κατα την κινηση
        if (Mathf.Approximately(h, 0f) & Mathf.Approximately(v, 0f))                        // Αν δεν υπάρχει κίνηση, μηδενίζουμε την ταχύτητα
        {
            moveSpeed = 0;
        }

        Vector2 inputVector = new Vector2(h, v);                                            // Δημιουργία διανύσματος κίνησης

        // Υπολογισμός έντασης κίνησης
        float targetMoveAmount = inputVector.magnitude;                                     // Εύρος 0-2 για κανονικοποιημένα διανύσματα

        if (targetMoveAmount > 1f)
        {
            inputVector = inputVector.normalized;
            targetMoveAmount = inputVector.magnitude;
        }
        float speedMultiplier = runInput;                           // Aυξανεται η τιμη οταν ειναι πατημένο το shift

        // Εφαρμογή Clamp για αποφυγή υπερβάσεων
        targetMoveAmount = Mathf.Clamp(targetMoveAmount * speedMultiplier, 0f, 2f);         

        // Ομαλή μετάβαση στο νέο moveAmount
        moveSpeed = Mathf.Lerp(moveSpeed, speedMultiplier, Time.deltaTime * 2.5f);         // Το 2.5f καθορίζει την ταχύτητα μετάβασης του animation

        // Υπολογισμός κίνησης
        GroundCheck();
        ySpeed = isGrounded ? -Physics.gravity.y * Time.deltaTime : ySpeed + Physics.gravity.y * Time.deltaTime;


        if (moveSpeed > 0.1f)
        {
            Vector3 moveInput = new Vector3(inputVector.x, 0, inputVector.y).normalized;
            Vector3 moveDir = cameraController.PlanarRotation * moveInput;
            Vector3 velocity = moveDir * moveSpeed;
            velocity.y = ySpeed;

            // an ginei epi8esh na apenergopoihte
            characterController.Move(velocity * Time.deltaTime);

            Vector3 targetPosition = transform.position;
            Quaternion targetRotation = Quaternion.identity;
            FindFloorPositions(transform, ref targetPosition, ref targetRotation, Vector3.down);

            targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Ενημέρωση animator
        animator.SetFloat("moveAmount", targetMoveAmount, 0.1f, Time.deltaTime);
    }

}