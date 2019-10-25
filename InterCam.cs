using UnityEngine;

public class InterCam : MonoBehaviour
{
    private Vector3 preMousePosition_;
    private Vector3 targetEuler_;
    private Vector3 targetPos_;
    private float Flashfactor = 0.5f;

    [SerializeField] float speedXZ               = 10f;
    [SerializeField] float speedUpDown           = 10f;
    [SerializeField] float Scrollspeed           = 10f;
    [SerializeField] float Sensitivity           = 30f;
    [SerializeField] float FlashfactorMultiplyer = 4f;
    [SerializeField] bool LowerNearClipping      = true;

    void Start()
    {
        preMousePosition_ = Input.mousePosition;
        targetPos_ = transform.position;
        targetEuler_ = transform.rotation.eulerAngles;
        DontDestroyOnLoad(this);
        if (LowerNearClipping)
        {
            GetComponent<Camera>().nearClipPlane = 0.01f;
        }
    }

    void Update()
    {
        UpdateXY();
        UpdateXZ();
        UpdateForward();
        UpdateRotation();
        UpdateMousePosition();
        UpdateTransform();
    }

    void UpdateXY()
    {
        if (Input.GetMouseButton(2) && !Input.GetMouseButtonDown(2)) {
            var dPos = Input.mousePosition - preMousePosition_;
            var velocity = ((-transform.up) * dPos.y + (-transform.right) * dPos.x);
            targetPos_ += velocity * Time.deltaTime ;
        }
    }

    void UpdateXZ()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Flashfactor = FlashfactorMultiplyer / 2;
            Debug.Log("SPRINTING");
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Flashfactor = 0.5f;
            Debug.Log("STOPED SPRINTING");
        }

        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");

        var velocity = Quaternion.AngleAxis(targetEuler_.y, Vector3.up) * (new Vector3(x, 0f, z)) * speedXZ;
        targetPos_ += velocity * Flashfactor * Time.deltaTime;
    }

    void UpdateForward()
    {
        var x = Input.mouseScrollDelta.x;
        var z = Input.mouseScrollDelta.y;


        var velocity = (transform.forward * z + transform.right * x) * Scrollspeed;
        targetPos_ += velocity * Time.deltaTime;
    }

    void UpdateRotation()
    {
        if ((Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)) ||
            (Input.GetMouseButton(1) && !Input.GetMouseButtonDown(1))) {
            var dPos = Input.mousePosition - preMousePosition_;
            targetEuler_ += new Vector3(-dPos.y, dPos.x, 0f) * Time.deltaTime * Sensitivity;
            targetEuler_.x = Mathf.Clamp(targetEuler_.x, -90f, 90f);
        }
    }

    void UpdateMousePosition()
    {
        preMousePosition_ = Input.mousePosition;
    }

    void UpdateTransform()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            InvokeRepeating("Up", 0f , 0.01f);
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            InvokeRepeating("Down", 0f, 0.01f);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            CancelInvoke("Up");
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            CancelInvoke("Down");
        }

        transform.position = Vector3.Lerp(transform.position, targetPos_, 0.5f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetEuler_), 0.5f);
    }

    void Up()
    {
        targetPos_.y += speedUpDown * Flashfactor * 0.5f * Time.deltaTime;
    }
    void Down()
    {
        targetPos_.y -= speedUpDown * Flashfactor * 0.5f * Time.deltaTime;
    }
}
