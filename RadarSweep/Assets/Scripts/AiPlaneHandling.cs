using UnityEngine;

public class AiPlaneHandling : MonoBehaviour
{
    //Flight characteristics
    [SerializeField] float rotSpeedValue = 400;
    [SerializeField] float planeSpeed = 500;
    [SerializeField] float planeSpeedboostValue = 2;
    [SerializeField] float planeBrakeValue = 2;
    [SerializeField] SpriteRenderer targetingRangeSpriteRenderer;
    [SerializeField] SpriteRenderer planeSpriteRenderer;
    [SerializeField] Transform targetingRangeTrans;
    [SerializeField] GameObject targetingRangeObject;
    Rigidbody2D planePhys;
    Vector2 spawnPos;
    Vector2 directionToTarget = new Vector2 (0,0);
    float rotSpeed = 0;
    float rotSpeedSlow;
    float rotSpeedValueStore;
    float planeSpeedboost = 1;

    public Vector2 loiterPos = new Vector2(0,0);
    public bool useLoiterPos = false;

    //AI parameters
    /*
     * 0 = Move to Loiter
     * 1 = Loiter
     * 2 = Seeking
     * 3 = RTB
     */
    public int aiStateIndex = 0;

    private void Start()
    {
        rotSpeedSlow = rotSpeedValue / 5;
        rotSpeedValueStore = rotSpeedValue;

        planePhys = GetComponent<Rigidbody2D>();

        CullNoTeam();

        spawnPos = planePhys.position;
    }

    private void CullNoTeam()
    {
        if (CompareTag("BlueAi"))
        {
            planeSpriteRenderer.color = Color.blue;
            targetingRangeSpriteRenderer.color = Color.blue;
        }

        else if (CompareTag("BanditAi"))
        {
            planeSpriteRenderer.color = Color.red;
            targetingRangeSpriteRenderer.color = Color.red;
        }

        else
        {
            Debug.Log(gameObject.name + " Does not have a team");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        TargetingRangeHandler TargetingRangeHandlerScript = targetingRangeObject.GetComponent<TargetingRangeHandler>();
        targetingRangeTrans.rotation = Quaternion.Euler(0f, 0f, 0f);

        SpeedCtrl();
        AddSpeed();

        State0To1();

        if (aiStateIndex == 0 | aiStateIndex == 1)
        {
            UseSpawnOrLoiter();
        }

        if (aiStateIndex == 2)
        {
            if (TargetingRangeHandlerScript.targetToGoTo != null)
            directionToTarget = TargetingRangeHandlerScript.targetToGoTo.position - planePhys.position;
            else aiStateIndex = 0;
        }

        TurnToTarget();
    }

    private void State0To1()
    {
            if (Vector2.Distance(planePhys.position, loiterPos) < 50)
            {
                aiStateIndex = 1;
                Invoke("State2", .1f);
            }

            if (Vector2.Distance(planePhys.position, spawnPos) < 50)
            {
                aiStateIndex = 1;
                Invoke("State2", .1f);
            }
    }

    private void State2()
    {
        TargetingRangeHandler TargetingRangeHandlerScript = targetingRangeObject.GetComponent<TargetingRangeHandler>();
        if (TargetingRangeHandlerScript.isHere)
        {
            aiStateIndex = 2;
        }
    }

    private void TurnToTarget()
    {
        float targetAngle = Vector2.SignedAngle(planePhys.transform.up, directionToTarget);
        RotCtrl(targetAngle);
    }

    private void AddSpeed()
    {
        Vector2 tmp = new Vector2(planeSpeed * Time.deltaTime, planeSpeed * Time.deltaTime);
        planePhys.AddForce(tmp * transform.up * planeSpeedboost);
    }

    private void UseSpawnOrLoiter()
    {
        if (useLoiterPos)
        {
            directionToTarget = loiterPos - planePhys.position;
        }

        else
        {
            directionToTarget = spawnPos - planePhys.position;
        }
    }

    private void SpeedCtrl()
    {
        planeSpeedboost = 1;

        if (aiStateIndex == 1)
        {
            planeSpeedboost = planeBrakeValue;
        }

        if (aiStateIndex == 0 || aiStateIndex == 3)
        {
            planeSpeedboost = planeSpeedboostValue;
        }
    }

    private void RotCtrl(float targetAngle)
    {
        rotSpeed = 0;
        rotSpeedValue = rotSpeedValueStore;

        if (aiStateIndex == 1)
        {
            rotSpeedValue = rotSpeedSlow;
        }

        if (targetAngle > 1)
        {
            rotSpeed = rotSpeedValue;
        }

        if (targetAngle < 1)
        {
            rotSpeed = -rotSpeedValue;
        }

        planePhys.AddTorque(rotSpeed * Time.deltaTime);
    }
}