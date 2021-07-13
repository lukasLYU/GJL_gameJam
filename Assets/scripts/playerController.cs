//See this image for set up information: http://i.imgur.com/pzfuzLn.png
//Don't forget to freeze the rotation on your rigidbody!
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    Camera cam;
    Rigidbody body;

    bool jumpPressed;
    //Players transofrm
    Transform player;
    //Raycast layermask
    public LayerMask aiLayerMask;
    // Ai stuff
    public bool isAi;
    public patrol patrol;
    //For switching places
    Transform[] transforms;
    //Player variable manager
    public playerVariableManager variableManager;
    //Collision State
    List<Collider> colliding = new List<Collider>();
    Collider groundCollider = new Collider();
    Rigidbody groundRigidbody = new Rigidbody();
    Vector3 groundNormal = Vector3.down;
    Vector3 groundContactPoint = Vector3.zero;
    Vector3 groundVelocity = Vector3.zero;

    //Initialize variables
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        body = GetComponent<Rigidbody>();
        player = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    //Movement Handling
    void FixedUpdate()
    {
        if (!isAi)
        {
            //Record the world-space walking movement
            Vector3 movement = transform.rotation * new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

            //If we're currently contacting a wall/ground like object
            if (groundCollider != null && Vector3.Dot(Vector3.up, groundNormal) > -0.3f)
            {
                //Subtract the ground's velocity
                if (groundRigidbody != null && groundRigidbody.isKinematic)
                {
                    body.velocity -= groundVelocity;
                }

                //Walking along the ground movement
                if (Vector3.Dot(Vector3.up, groundNormal) > 0.5f)
                {
                    if (movement != Vector3.zero)
                    {
                        Vector2 XYVel = new Vector2(body.velocity.x, body.velocity.z);
                        XYVel = Mathf.Clamp(XYVel.magnitude, 0f, 3f) * XYVel.normalized;
                        body.velocity = new Vector3(XYVel.x, body.velocity.y, XYVel.y);
                    }
                    else
                    {
                        body.velocity = new Vector3(body.velocity.x * 0.8f, body.velocity.y, body.velocity.z * 0.8f);
                    }
                    body.velocity += movement;
                }

                //Handle jumping
                if (jumpPressed && body.velocity.y <= 0.1f) { body.velocity += Vector3.Slerp(Vector3.up, groundNormal, 0.2f) * 6f; }

                //Draw some debug info
                Debug.DrawLine(groundContactPoint, groundContactPoint + groundNormal, Color.blue, 2f);

                //Add back the ground's velocity
                if (groundRigidbody != null && groundRigidbody.isKinematic)
                {
                    groundVelocity = groundRigidbody.GetPointVelocity(groundContactPoint);
                    body.velocity += groundVelocity;
                }
            }
            else
            {
                body.velocity += movement * 0.1f;
                groundVelocity = Vector3.zero;
            }
            groundNormal = Vector3.down;
            groundCollider = null;
            groundRigidbody = null;
            groundContactPoint = (transform.position - Vector3.down * -0.5f);
            jumpPressed = false;
        }
    }


    //Per-Frame Updates
    void Update()
    {
        if (!isAi)
        {
            //Record whether the jump key was hit this frame
            //NOTE: Must be done in Update, not FixedUpdate
            jumpPressed = jumpPressed ? jumpPressed : Input.GetButtonDown("Jump");
            //Rotate the player
            transform.Rotate(0, (Input.GetAxis("Mouse X")) * 2f, 0);

            //Rotate the camera rig and prevent it from penetrating the environment
            cam.transform.parent.localRotation *= Quaternion.Euler(-Input.GetAxis("Mouse Y") * 2f, 0, 0);
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 200000000f))
                {
                    Debug.Log("shootin");
                    if (hit.transform.name == "fakePlayer")
                    {
                        Debug.Log("Did Hit");
                        isAi = true;
                        hit.transform.gameObject.GetComponent<playerController>().isAi = false;
                        hit.transform.gameObject.GetComponent<patrol>().shouldMove = false;
                        transforms = hit.transform.gameObject.GetComponentsInChildren<Transform>();
                        Camera camera = transforms[2].gameObject.AddComponent<Camera>();
                        camera.transform.position = transforms[2].position;
                        camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y + 0.485f, camera.transform.position.z);
                        transforms[0].gameObject.GetComponent<playerController>().cam = camera;
                        variableManager.switched = true;
                    }
                }
            }
        }
    }


    //Ground Collision Handling
    void OnCollisionEnter(Collision collision)
    {
        colliding.Add(collision.collider);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.impulse.magnitude > float.Epsilon)
        {
            if (!colliding.Contains(collision.collider))
            {
                colliding.Add(collision.collider);
            }

            //Record ground telemetry
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (Vector3.Dot(Vector3.up, collision.contacts[i].normal) > Vector3.Dot(Vector3.up, groundNormal))
                {
                    groundNormal = collision.contacts[i].normal;
                    groundCollider = collision.collider;
                    groundContactPoint = collision.contacts[i].point;
                    groundRigidbody = collision.rigidbody;
                    if (groundRigidbody != null && groundVelocity == Vector3.zero)
                    {
                        groundVelocity = groundRigidbody.GetPointVelocity(groundContactPoint);
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        colliding.Remove(collision.collider);
    }
}
