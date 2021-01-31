using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class UserMovementController : MonoBehaviour
{
    public float moveSpeed = 6;

    Rigidbody rigidbody;
    Camera viewCamera;
    Vector3 velocity;
    private Vector3 lookDirection;

    public bool movementEnabled = false;

    void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody>();
        // viewCamera = Camera.main;
        movementEnabled = true;
    }

    void OnDrawGizmos()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
        Gizmos.DrawRay(transform.position, direction);
    }


    void LookAtMouse()
    {
        Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, Input.mousePosition.y,
            viewCamera.transform.position.y));
        transform.LookAt(mousePos + Vector3.up * transform.position.y);
    }

    void SlowLookAt()
    {
        Vector3 moveDirection = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        ).normalized;
        lookDirection = moveDirection + gameObject.transform.position;


        // Determine which direction to rotate towards
        Vector3 targetDirection = lookDirection - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = 5 * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection =
            Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    void GetMovementInput()
    {
        Vector3 movementInput =
            new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))
                .normalized * moveSpeed;
        velocity = movementInput;
    }

    void Update()
    {
        if (movementEnabled)
            Move();
    }

    void Move()
    {
        SlowLookAt();
        GetMovementInput();
    }

    void FixedUpdate()
    {
        if (movementEnabled)
            rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
		
//		if (velocity.magnitude != 0)
//			rigidbody.MovePosition (rigidbody.position + velocity * Time.fixedDeltaTime);
//		else
//		{
//			rigidbody.velocity = Vector3.zero; // ensure doesnt start floating
//		}
    }
}