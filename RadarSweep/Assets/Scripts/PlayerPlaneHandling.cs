using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPlaneHandling : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera mainCam;
    [SerializeField] GameObject playerRadar;
    [SerializeField] float rotSpeedValue = 400;
    [SerializeField] float planeSpeed = 500;
    [SerializeField] float planeSpeedboostValue = 2;
    [SerializeField] float planeBrakeValue = 2;
    Rigidbody2D planePhys;
    Vector2 moveInput;
    float rotSpeed = 0;
    float planeSpeedboost = 1;

    public float radarRangeScale = 1;

    private void Start()
    {
        playerRadar.transform.localScale = new Vector3(radarRangeScale, radarRangeScale, radarRangeScale);
        planePhys = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        planePhys.AddTorque(rotSpeed * Time.deltaTime);

        Vector2 tmp = new Vector2(planeSpeed * Time.deltaTime, planeSpeed * Time.deltaTime);
        planePhys.AddForce(tmp * transform.up * planeSpeedboost);
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        RotControl();
        SpeedControl();
    }

    private void SpeedControl()
    {
        planeSpeedboost = 1;

        if (moveInput.y > float.Epsilon)
        {
            planeSpeedboost = planeSpeedboostValue;
        }

        if (moveInput.y < -float.Epsilon)
        {
            planeSpeedboost = planeBrakeValue;
        }
    }

    private void RotControl()
    {
        rotSpeed = 0;

        if (moveInput.x > float.Epsilon)
        {
            rotSpeed = -rotSpeedValue;
        }

        if (moveInput.x < -float.Epsilon)
        {
            rotSpeed = rotSpeedValue;
        }
    }

    private void OnCamControlAdd()
    {
        if (mainCam.m_Lens.OrthographicSize < 2000)
        {
            Debug.Log("Cam zoom out");
            mainCam.m_Lens.OrthographicSize += 500;
        }
    }
    private void OnCamControlSubtract()
    {
        if (mainCam.m_Lens.OrthographicSize > 500)
        {
            Debug.Log("Cam zoom in");
            mainCam.m_Lens.OrthographicSize -= 500;
        }
    }
}