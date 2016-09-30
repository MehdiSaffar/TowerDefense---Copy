using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Vector3 target;
    public bool useKeyboard = true;

    [Header("Speeds")]
    public float orbitSpeed = 2f;
    public float panSpeedCoefficient = 2f;
    public float zoomSpeed = 2f;
    public float panAcceleration;
    public float maxVelocity;

    [Header("Constraints")]
    public float maxArmLength = 50f;
    public float minArmLength = 10f;
    [HideInInspector] public float armLength;
    public Rect limits;

    public float maxVerticalOrbitAngle = 70f;
    public float minVerticalOrbitAngle = 15f;

    private Vector3 arm;
    private Vector3 panForwardVector;
    private Vector3 panRightVector;

    private Vector3 panVelocity = Vector3.zero;

    private float prevPinchDistance = 0f;
    private float armLengthVelocity = 0f;

    private Vector3 lastMousePosition = Vector2.zero;
    private float lastMousePositionTimestamp = 0f;
    private float mousePositionDeltaTime = 0f;
    private Vector3 panAccel = Vector3.zero;

    void Start()
    {
        GameManager.Fsm.Changed += Fsm_Changed;

        arm = (Vector3.up + Vector3.right).normalized;
        transform.position = target + arm * maxArmLength;
        transform.rotation = Quaternion.LookRotation(-arm);
    }
    private void Fsm_Changed(GameManager.States state)
    {
        enabled = state == GameManager.States.Edit || state == GameManager.States.Play;
    }
    void OnDestroy()
    {
        GameManager.Fsm.Changed -= Fsm_Changed;
    }

    void LateUpdate()
    {
        panForwardVector = transform.forward;
        panForwardVector.y = 0f;
        panForwardVector.Normalize();

        panRightVector = Vector3.Cross(Vector3.up, panForwardVector).normalized;

        //------------------- Init -------------------//
        armLengthVelocity = Mathf.Lerp(armLengthVelocity, 0f, Time.deltaTime * 5f);

        //------------------- Input -------------------//
        // PAN
        if (useKeyboard)
        {
            panAccel = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                panAccel += Vector3.up;
            }
            if (Input.GetKey(KeyCode.S))
            {
                panAccel += Vector3.down;
            }
            if (Input.GetKey(KeyCode.D))
            {
                panAccel += Vector3.right;
            }
            if (Input.GetKey(KeyCode.A))
            {
                panAccel += Vector3.left;
            }
            if(panAccel == Vector3.zero)
            {
                panVelocity = Vector3.Lerp(panVelocity, Vector3.zero, Time.deltaTime * 5f);
            }
            panAccel = panAccel.normalized * panAcceleration * Time.deltaTime;
            panVelocity += panAccel * Time.deltaTime;
            panVelocity = panVelocity.normalized * Mathf.Min(panVelocity.magnitude, maxVelocity);

            if(Input.GetKey(KeyCode.Q))
            {
                armLengthVelocity += zoomSpeed;
            }
            if(Input.GetKey(KeyCode.E))
            {
                armLengthVelocity -= zoomSpeed;
            }
            /*float mouseScrollDelta = Input.mouseScrollDelta.y;
            if (mouseScrollDelta != 0f) armLengthVelocity =  -mouseScrollDelta * zoomSpeed * 4f;*/
            if(Input.GetMouseButton(1))
            {
                mousePositionDeltaTime = Time.time - lastMousePositionTimestamp;
                Vector2 mouseVelocity = (Input.mousePosition - lastMousePosition) / mousePositionDeltaTime;
                lastMousePosition = Input.mousePosition;
                lastMousePositionTimestamp = Time.time;
                Quaternion horizontalOrbitAxis = Quaternion.AngleAxis(mouseVelocity.x * orbitSpeed / 20f * Time.deltaTime, Vector3.up);
                Quaternion verticalOrbitAxis  = Quaternion.AngleAxis(-mouseVelocity.y * orbitSpeed / 20f * Time.deltaTime, panRightVector);
                arm = horizontalOrbitAxis * verticalOrbitAxis * arm;
            }
        }
        else
        {
            if (Input.touchCount == 1)
            {
                panVelocity = -Input.touches[0].deltaPosition / Input.touches[0].deltaTime;
            }
            // ZOOM
            else if (Input.touchCount == 2)
            {
                Touch[] touch = Input.touches;
                if (touch[0].phase == TouchPhase.Began || touch[1].phase == TouchPhase.Began ||
                    touch[0].phase == TouchPhase.Stationary || touch[1].phase == TouchPhase.Stationary)
                {
                    prevPinchDistance = (touch[0].position - touch[1].position).magnitude;
                }
                else if (touch[0].phase == TouchPhase.Moved || touch[1].phase == TouchPhase.Moved)
                {
                    float pinchDistance = (touch[0].position - touch[1].position).magnitude;
                    float deltaPinchDistance = pinchDistance - prevPinchDistance;
                    armLengthVelocity = -deltaPinchDistance * zoomSpeed;
                }
            }
        }

        //------------------- Update -------------------//
        // PAN
        target             += panRightVector   * panVelocity.x * Time.deltaTime;
        transform.position += panRightVector   * panVelocity.x * Time.deltaTime;
        target             += panForwardVector * panVelocity.y * Time.deltaTime;
        transform.position += panForwardVector * panVelocity.y * Time.deltaTime;

        armLength = Mathf.Clamp(armLength + armLengthVelocity * Time.deltaTime, minArmLength, maxArmLength);


        target.x = Mathf.Clamp(target.x, limits.x + arm.x, limits.xMax);
        target.z = Mathf.Clamp(target.z, limits.y + arm.z, limits.yMax);

        // WHOLE THING
        transform.position = target + arm * armLength;
        transform.rotation = Quaternion.LookRotation(-arm);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, limits.x, limits.xMax);
        pos.z = Mathf.Clamp(pos.z, limits.y, limits.yMax);
        transform.position = pos;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Vector3(limits.xMin, 0, limits.yMin), Vector3.up * 100);
        Gizmos.DrawRay(new Vector3(limits.xMin, 0, limits.yMax), Vector3.up * 100);
        Gizmos.DrawRay(new Vector3(limits.xMax, 0, limits.yMin), Vector3.up * 100);
        Gizmos.DrawRay(new Vector3(limits.xMax, 0, limits.yMax), Vector3.up * 100);
        Gizmos.DrawLine(new Vector3(limits.xMin, 0, limits.yMin), new Vector3(limits.xMin, 0, limits.yMax));
        Gizmos.DrawLine(new Vector3(limits.xMin, 0, limits.yMin), new Vector3(limits.xMax, 0, limits.yMin));
        Gizmos.DrawLine(new Vector3(limits.xMax, 0, limits.yMax), new Vector3(limits.xMax, 0, limits.yMin));
        Gizmos.DrawLine(new Vector3(limits.xMax, 0, limits.yMax), new Vector3(limits.xMin, 0, limits.yMax));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target);
        Gizmos.DrawSphere(target, 1);
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, panForwardVector);

    }
}
