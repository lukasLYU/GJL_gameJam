//See this image for set up information: http://i.imgur.com/pzfuzLn.png
//Don't forget to freeze the rotation on your rigidbody!
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class playerController : MonoBehaviour
{
    Camera cam;
    [HideInInspector]public Rigidbody body;
    public AudioSource footSteps;
    private float[] handOffsetsPos = {
        0.3f,
        0.25f,
        4f
    };
    private float[] handOffsetsRot = {
        180f,
        16f,
        -1f
    };
    private Quaternion desiredRot = Quaternion.Euler(0f, 90f, 0f);
    public bool useHeadBob = true;
    bool jumpPressed;
    //Players transofrm
    Transform player;
    public die die;
    //Hand transform
    public Transform hand;
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
    [SerializeField] public float walkBobSpeed = 11f;
    [SerializeField] public float walkBobAmmount = .02f;
    private float defaultYPos = 0;
    private float timer;
    private bool isWalking = true;

    //Initialize variables
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        body = GetComponent<Rigidbody>();
        player = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        defaultYPos = cam.transform.localPosition.y;
    }

    //Movement Handling
    void FixedUpdate()
    {
        if (!isAi)
        {
            //Record the world-space walking movement
            Vector3 movement = transform.rotation * new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
            if (movement == Vector3.zero) isWalking = false;
            else isWalking = true;
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
                        XYVel = Mathf.Clamp(XYVel.magnitude, 0f, 5f) * XYVel.normalized;
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
        if (isWalking) {
            footSteps.Play();
            footSteps.loop = true;
            footSteps.mute = false;
        }
        else
        {
            footSteps.Stop();
            footSteps.loop = false;
            footSteps.mute = true;
        }
        if (!isAi)
        {
            transform.Rotate(0, (Input.GetAxis("Mouse X")) * 2f, 0);
            RaycastHit hit;
            cam.transform.parent.localRotation *= Quaternion.Euler(-Input.GetAxis("Mouse Y") * 2f, 0, 0);
            Quaternion localCamParentRot = cam.transform.parent.localRotation;
            Quaternion camParentRot = cam.transform.parent.rotation;
            localCamParentRot.x = Mathf.Clamp(localCamParentRot.x, -0.6f, 0.6f);
            cam.transform.parent.localRotation = localCamParentRot;
            if (Input.GetMouseButtonDown(0) && !variableManager.switched)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit,200000000f))
                {
                    Debug.Log("shootin");
                    Debug.Log(hit.transform.name);
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
                        camera.clearFlags = CameraClearFlags.SolidColor;
                        transforms[0].gameObject.GetComponent<playerController>().cam = camera;
                        variableManager.switched = true;
                        Destroy(transform.GetComponent<Camera>());
                        die.currentPlayel = hit.transform;
                    }
                }
            }
            if (useHeadBob) {
                handleHeadBob(cam.transform);
                if (hand != null)
                    handleHandShake();
            }
        }
    }

    private void handleHeadBob(Transform objectToBob) {
        if (isWalking) timer += Time.deltaTime * walkBobSpeed;
        else timer = 0;
        objectToBob.localPosition = new Vector3(cam.transform.localPosition.x, defaultYPos+Mathf.Sin(timer) * walkBobAmmount, cam.transform.localPosition.z);
    }
    private void handleHandShake() {
        Quaternion reeeLocalRot = hand.transform.localRotation;
        Vector3 reeeLocalPos = hand.transform.localPosition;
        reeeLocalRot = Quaternion.Slerp(new Quaternion(reeeLocalRot.x+handOffsetsRot[0], reeeLocalRot.y + handOffsetsRot[1], reeeLocalRot.z + handOffsetsRot[2], 0), desiredRot, Time.deltaTime * 1f);
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
