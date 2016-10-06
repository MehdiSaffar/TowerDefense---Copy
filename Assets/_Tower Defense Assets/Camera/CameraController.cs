using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    class FlyParams
    {
        public GameObject target;
        public float elapsedTime;
        public AnimationCurve x;
        public AnimationCurve y;
        public AnimationCurve z;
    };

    private FlyParams flyParams;

    public Vector3 currentTarget;
    public bool useKeyboard = true;

    [Header("Speeds")]
    public float orbitSpeed;
    public float panSpeedCoefficient;
    public float zoomSpeed;
    /// <summary>
    /// Maximum zoom speed
    /// </summary>
    public float panAcceleration;
    /// <summary>
    /// Maximum pan acceleration 
    /// </summary>
    public float maxVelocity;
    /// <summary>
    /// Maximum pan velocity
    /// </summary>

    [Header("Constraints")]

    public float maxArmLength;
    /// <summary>
    /// The maximum arm length
    /// </summary>
    public float minArmLength;
    /// <summary>
    /// The minimum arm length
    /// </summary>
    public float minRequiredFlyDistanceRatio;
    /// <summary>
    /// Minimum distance in pixels between the camera target position on the screen and the object we would like to fly to's screen positon
    /// It is a percentage of the width of the player screen so that it works correctly on all aspect ratios
    /// </summary>
    public float maxVerticalOrbitAngle;
    /// <summary>
    /// Maximum vertical orbit angle
    /// </summary>
    public float minVerticalOrbitAngle;
    /// <summary>
    /// Minimum vertical orbit angle
    /// </summary>

    [Header("For Debug ONLY")]
    public Rect levelLimits;
    /// <summary>
    /// Bounds of the level within which the camera is allowed to fly
    /// </summary>
    public float armLength;
    /// <summary>
    /// Current arm length
    /// </summary>

    private Vector3 arm;
    /// <summary>
    /// Current arm vector (from target to camera position)
    /// </summary>
    private Vector3 panForwardVector;
    /// <summary>
    /// Vector that is parallel to the pan plane and points forward
    /// </summary>
    private Vector3 panRightVector;
    /// <summary>
    /// Vector that is parallel to the pan pane and points to the right
    /// </summary>

    private Vector3 panVelocity = Vector3.zero;
    /// <summary>
    /// Current pan velocity
    /// </summary>

    private float prevPinchDistance = 0f;
    private float armLengthVelocity = 0f;
    /// <summary>
    /// Current velocity with which the length of the arm va
    /// </summary>

    private Vector3 lastMousePosition = Vector2.zero;
    private float lastMousePositionTimestamp = 0f;
    private float mousePositionDeltaTime = 0f;
    private Vector3 panAccel = Vector3.zero;

    void Start()
    {
        GameManager.Fsm.Changed += Fsm_Changed;

        arm = (Vector3.up + Vector3.right).normalized;
        transform.position = currentTarget + arm * maxArmLength;
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
        armLengthVelocity = Mathf.Lerp(armLengthVelocity, 0f, Time.unscaledDeltaTime * 5f);

        if (flyParams != null)
        {
            flyParams.elapsedTime += Time.unscaledDeltaTime;
            currentTarget.x = flyParams.x.Evaluate(flyParams.elapsedTime);
            currentTarget.y = flyParams.y.Evaluate(flyParams.elapsedTime);
            currentTarget.z = flyParams.z.Evaluate(flyParams.elapsedTime);

            if (flyParams.elapsedTime >= 0.7f)
            {
                flyParams = null;
            }
        }
        else
        {
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
                if (panAccel == Vector3.zero)
                {
                    panVelocity = Vector3.Lerp(panVelocity, Vector3.zero, Time.unscaledDeltaTime * 5f);
                }
                
                if (Input.GetKey(KeyCode.Q))
                {
                    armLengthVelocity += zoomSpeed;
                }
                if (Input.GetKey(KeyCode.E))
                {
                    armLengthVelocity -= zoomSpeed;
                }
                if (Input.GetMouseButton(1))
                {
                    mousePositionDeltaTime = Time.unscaledTime - lastMousePositionTimestamp;
                    Vector2 mouseVelocity = (Input.mousePosition - lastMousePosition) / mousePositionDeltaTime;
                    lastMousePosition = Input.mousePosition;
                    lastMousePositionTimestamp = Time.unscaledTime;
                    Quaternion horizontalOrbitAxis = Quaternion.AngleAxis(mouseVelocity.x * orbitSpeed / 20f * Time.unscaledDeltaTime, Vector3.up);
                    Quaternion verticalOrbitAxis = Quaternion.AngleAxis(-mouseVelocity.y * orbitSpeed / 20f * Time.unscaledDeltaTime, panRightVector);
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
        }
        //------------------- Update -------------------//
        // PAN
        panAccel = panAccel.normalized * panAcceleration * Time.unscaledDeltaTime;
        panVelocity += panAccel * Time.unscaledDeltaTime;
        panVelocity = panVelocity.normalized * Mathf.Min(panVelocity.magnitude, maxVelocity);

        currentTarget      += panRightVector   * panVelocity.x * Time.unscaledDeltaTime;
        transform.position += panRightVector   * panVelocity.x * Time.unscaledDeltaTime;
        currentTarget      += panForwardVector * panVelocity.y * Time.unscaledDeltaTime;
        transform.position += panForwardVector * panVelocity.y * Time.unscaledDeltaTime;

        armLength = Mathf.Clamp(armLength + armLengthVelocity * Time.unscaledDeltaTime, minArmLength, maxArmLength);

        currentTarget.x = Mathf.Clamp(currentTarget.x, levelLimits.x + arm.x, levelLimits.xMax);
        currentTarget.z = Mathf.Clamp(currentTarget.z, levelLimits.y + arm.z, levelLimits.yMax);

        // WHOLE THING
        transform.position = currentTarget + arm * armLength;
        transform.rotation = Quaternion.LookRotation(-arm);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, levelLimits.x, levelLimits.xMax);
        pos.z = Mathf.Clamp(pos.z, levelLimits.y, levelLimits.yMax);
        transform.position = pos;
    }
    public void FlyToObject(GameObject obj)
    {
        Vector2 objScreenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
        Vector2 targetScreenPos = Camera.main.WorldToScreenPoint(currentTarget);
        Vector2 dir = objScreenPos - targetScreenPos;
        // TODO: Cache the minRequiredFlyDistance and use sqrMagnitude instead
        if (dir.x <= minRequiredFlyDistanceRatio * Screen.width &&
            dir.y <= minRequiredFlyDistanceRatio * Screen.height) return;

        // TODO: Clean this mess up, maybe extension methods?
        flyParams = new FlyParams();
        flyParams.target = obj;
        flyParams.elapsedTime = 0f;

        Keyframe[] keyframes = new Keyframe[3 * 2];

        keyframes[0].value = currentTarget.x;
        keyframes[1].value = currentTarget.y;
        keyframes[2].value = currentTarget.z;
        keyframes[0].time = 0f;
        keyframes[1].time = 0f;
        keyframes[2].time = 0f;

        keyframes[3].value = obj.transform.position.x;
        keyframes[4].value = currentTarget.y;
        keyframes[5].value = obj.transform.position.z;
        keyframes[3].time = 0.7f;
        keyframes[4].time = 0.7f;
        keyframes[5].time = 0.7f;

        flyParams.x = new AnimationCurve();
        flyParams.y = new AnimationCurve();
        flyParams.z = new AnimationCurve();

        for (int i = 0; i < 2; i++)
        {
            flyParams.x.AddKey(keyframes[i * 3]);
        }
        for (int i = 0; i < 2; i++)
        {
            flyParams.y.AddKey(keyframes[i * 3 + 1]);
        }
        for (int i = 0; i < 2; i++)
        {
            flyParams.z.AddKey(keyframes[i * 3 + 2]);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        // TODO: Maybe extend bounds ?
        Gizmos.DrawRay(new Vector3(levelLimits.xMin, 0, levelLimits.yMin), Vector3.up * 100);
        Gizmos.DrawRay(new Vector3(levelLimits.xMin, 0, levelLimits.yMax), Vector3.up * 100);
        Gizmos.DrawRay(new Vector3(levelLimits.xMax, 0, levelLimits.yMin), Vector3.up * 100);
        Gizmos.DrawRay(new Vector3(levelLimits.xMax, 0, levelLimits.yMax), Vector3.up * 100);
        Gizmos.DrawLine(new Vector3(levelLimits.xMin, 0, levelLimits.yMin), new Vector3(levelLimits.xMin, 0, levelLimits.yMax));
        Gizmos.DrawLine(new Vector3(levelLimits.xMin, 0, levelLimits.yMin), new Vector3(levelLimits.xMax, 0, levelLimits.yMin));
        Gizmos.DrawLine(new Vector3(levelLimits.xMax, 0, levelLimits.yMax), new Vector3(levelLimits.xMax, 0, levelLimits.yMin));
        Gizmos.DrawLine(new Vector3(levelLimits.xMax, 0, levelLimits.yMax), new Vector3(levelLimits.xMin, 0, levelLimits.yMax));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, currentTarget);
        Gizmos.DrawSphere(currentTarget, 1);
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, panForwardVector);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.height / 2f, Screen.width / 2f)),
            new Vector3(Screen.height / 2f, Screen.width / 2f, 2));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.height / 2f, Screen.width / 2f)),
            new Vector3(minRequiredFlyDistanceRatio * Screen.height / 2f, minRequiredFlyDistanceRatio * Screen.width / 2f, 1));
    }
}
