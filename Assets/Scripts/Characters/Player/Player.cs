using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [SerializeField] StatsBar_HUD statsBar_HUD;
    [SerializeField] bool regenerateHealth = true;
    [SerializeField] float healthRegenerateTime;
    [SerializeField, Range(0f, 1f)] float healthRegeneratePercent;

    [Header("----INPUT----")]
    [SerializeField] PlayerInput input;

    [Header("----MOVE----")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float accelerationTime = 3f;
    [SerializeField] float decelerationTime = 3f;
    [SerializeField] float moveRotationAngle = 50f;
    [SerializeField] float paddingX = 0.2f;
    [SerializeField] float paddingY = 0.2f;


    [Header("----FIRE----")]
    [SerializeField] GameObject projectile1;
    [SerializeField] GameObject projectile2;
    [SerializeField] GameObject projectile3;

    [SerializeField] Transform muzzleMiddle;
    [SerializeField] Transform muzzleTop;
    [SerializeField] Transform muzzleBottom;

    [SerializeField] AudioData projectileLaunchSFX;

    [SerializeField, Range(0, 2)] int weaponPower = 0;

    [SerializeField] float fireInterval = 0.2f;

    [Header("----DODGE----")]
    [SerializeField] AudioData dodgeSFX;
    [SerializeField] int dodgeEnergyCost = 25;
    [SerializeField] float maxRoll = 720f;
    [SerializeField] float rollSpeed = 360f;
    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);


    [Header("----OVERDRIVE----")]
    [SerializeField] int overdriveDodgeFactor = 2;
    [SerializeField] float overdriveSpeedFactor = 1.2f;
    [SerializeField] float overdriveFireFactor = 1.2f;



    bool isDodging = false;

    bool isOverdriving = false;

    float currentRoll;

    float dodgeDuration;

    float t;
    Vector2 previousVelocity;
    Quaternion previousRotation;

    WaitForSeconds waitForFireInterval;

    WaitForSeconds waitForOverdriveFireInterval;//等待能量爆发时的开火间隔

    WaitForSeconds waitHealthRegenerateTime;

    Coroutine moveCoroutine;

    Coroutine healthRegenerateCoroutine;

    new Rigidbody2D rigidbody;

    new Collider2D collider;


    #region UNITY EVENT FUNTIONS
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        dodgeDuration = maxRoll / rollSpeed;
        rigidbody.gravityScale = 0f;

        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval /= overdriveFireFactor);//开火间隔缩短
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onOverdrive += Overdrive;

        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdriveOff;
    }

    private void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;
        input.onOverdrive -= Overdrive;

        PlayerOverdrive.on -= OverdriveOn;
        PlayerOverdrive.off -= OverdriveOff;
    }


    void Start()
    {
        statsBar_HUD.Initialize(health, maxHealth);

        input.EnableGamePlayInput();
    }
    #endregion

    #region HEALTH
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        statsBar_HUD.UpdateStats(health, maxHealth);

        if (gameObject.activeSelf)
        {
            if (regenerateHealth)
            {
                if (healthRegenerateCoroutine != null)
                {
                    StopCoroutine(healthRegenerateCoroutine);
                }
                healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }
    }

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        statsBar_HUD.UpdateStats(health, maxHealth);
    }

    public override void Die()
    {
        statsBar_HUD.UpdateStats(0f, maxHealth);
        base.Die();
    }
    #endregion

    #region MOVE
    void Move(Vector2 moveInput)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveInput.normalized * moveSpeed, Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right)));
        StartCoroutine(nameof(MovePositionLimitCoroutine));
    }

    void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, Vector2.zero, Quaternion.identity));
        StopCoroutine(nameof(MovePositionLimitCoroutine));
    }

    IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation)
    {
        t = 0f;
        previousVelocity = rigidbody.velocity;
        previousRotation = transform.rotation;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime / time;
            rigidbody.velocity = Vector2.Lerp(previousVelocity, moveVelocity, t);
            transform.rotation = Quaternion.Lerp(previousRotation, moveRotation, t);

            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator MovePositionLimitCoroutine()
    {
        while (true)
        {
            transform.position = Viewport.Instance.PlayerMoveablePostion(transform.position, paddingX, paddingY);
            yield return null;
        }
    }
    #endregion

    #region FIRE
    void Fire()
    {
        StartCoroutine(nameof(FireCoroutine));
    }

    void StopFire()
    {
        StopCoroutine(nameof(FireCoroutine));
    }

    IEnumerator FireCoroutine()
    {

        while (true)
        {
            switch (weaponPower)
            {
                case 0:
                    PoolManager.Release(projectile1, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(projectile1, muzzleTop.position);
                    PoolManager.Release(projectile1, muzzleBottom.position);

                    break;
                case 2:
                    PoolManager.Release(projectile1, muzzleMiddle.position);
                    PoolManager.Release(projectile2, muzzleTop.position);
                    PoolManager.Release(projectile3, muzzleBottom.position);
                    break;
                default:
                    break;
            }

            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);

            yield return isOverdriving ? waitForOverdriveFireInterval : waitForFireInterval;
            //if (isOverdriving)
            //{
            //    yield return waitForOverdriveFireInterval;
            //}
            //else
            //{
            //    yield return waitForFireInterval;
            //}

        }
    }

    #endregion

    #region DODGE
    void Dodge()
    {
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;

        StartCoroutine(nameof(DodgeCoroutine));

        //Change player's scale 改变玩家的缩放值
    }


    IEnumerator DodgeCoroutine()
    {
        isDodging = true;
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);

        //Cost energy 消耗能量
        PlayerEnergy.Instance.Use(dodgeEnergyCost);

        //Make player invincible 让玩家无敌
        collider.isTrigger = true;
        //Make player rotate alone X axis 让玩家沿着X轴旋转
        currentRoll = 0;


        #region 方法一
        //var scale = transform.localScale;
        //while(currentRoll < maxRoll)
        //{
        //    currentRoll += rollSpeed * Time.deltaTime;
        //    transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);

        //    if (currentRoll < maxRoll / 2f)
        //    {
        //        scale.x = Mathf.Clamp(scale.x - Time.deltaTime / dodgeDuration, dodgeScale.x, 1f);
        //        scale.y = Mathf.Clamp(scale.y - Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
        //        scale.z = Mathf.Clamp(scale.z - Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);

        //    }
        //    else
        //    {
        //        scale.x = Mathf.Clamp(scale.x + Time.deltaTime / dodgeDuration, dodgeScale.x, 1f);
        //        scale.y = Mathf.Clamp(scale.y + Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
        //        scale.z = Mathf.Clamp(scale.z + Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);
        //    }
        //    transform.localScale = scale;

        //    yield return null;
        //}
        #endregion

        #region 线性插值

        //    var t1 = 0f;
        //    var t2 = 0f;

        //    while (currentRoll < maxRoll)
        //    {
        //        currentRoll += rollSpeed * Time.deltaTime;
        //        transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);

        //        if (currentRoll < maxRoll / 2f)
        //        {
        //            t1 += Time.deltaTime / dodgeDuration;
        //            transform.localScale = Vector3.Lerp(transform.localScale, dodgeScale, t1);
        //        }
        //        else
        //        {
        //            t2 += Time.deltaTime / dodgeDuration;
        //            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, t2);
        //        }

        //        yield return null;
        //    }
        #endregion

        //方法三：二次  贝塞尔曲线
        while (currentRoll < maxRoll)
        {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);
            transform.localScale = BezierCurve.QuadraticPoint(Vector3.one, Vector3.one, dodgeScale, currentRoll / maxRoll);

            yield return null;
        }

        collider.isTrigger = false;
        isDodging = false;

    }



    #endregion

    #region OVERDRIVE
    void Overdrive()
    {
        if (!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX)) return;

        PlayerOverdrive.on.Invoke();

    }

    void OverdriveOn()
    {
        isOverdriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor;
    }

    void OverdriveOff()
    {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }

    #endregion
}
