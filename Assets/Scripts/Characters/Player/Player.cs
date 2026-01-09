using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    #region FIELDS
    [SerializeField] StatsBar_HUD statsBar_HUD;
    [SerializeField] bool regenerateHealth = true;
    [SerializeField] float healthRegenerateTime;
    [SerializeField, Range(0f, 1f)] float healthRegeneratePercent;
    [SerializeField] float InvincibleTime = 1f;

    [Header("----INPUT----")]
    [SerializeField] PlayerInput input;

    [Header("----MOVE----")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float accelerationTime = 3f;
    [SerializeField] float decelerationTime = 3f;
    [SerializeField] float moveRotationAngle = 50f;

    [Header("----FIRE----")]
    [SerializeField] GameObject projectile1;
    [SerializeField] GameObject projectile2;
    [SerializeField] GameObject projectile3;
    [SerializeField] GameObject projectileOverdrive;

    [SerializeField] ParticleSystem muzzleVFX;
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

    [Header("----SLOWMOTION----")]
    [SerializeField] float overdriveSlowMotionDuration = 0.3f;//超载模式慢动作持续时间(实际为0.6秒，淡入0.3，淡出0.3)
    [SerializeField] float dodgeSlowMotionDuration = 0.2f;//闪避慢动作持续时间(实际为0.4秒，淡入0.2，淡出0.2)
    [SerializeField] float takeDamageSlowMotionDuration = 0.1f;//受伤慢动作持续时间(实际为0.1秒)


    bool isDodging = false;

    bool isOverdriving = false;

    float paddingX;//边距
    float paddingY;

    float currentRoll;

    float dodgeDuration;

    float t;

    Vector2 moveDirection;
    Vector2 previousVelocity;
    Quaternion previousRotation;

    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitForOverdriveFireInterval;//等待能量爆发时的开火间隔
    WaitForSeconds waitHealthRegenerateTime;
    WaitForSeconds waitDecelerationTime;
    WaitForSeconds waitInvincibleTime;

    Coroutine moveCoroutine;

    Coroutine healthRegenerateCoroutine;

    new Rigidbody2D rigidbody;

    new Collider2D collider;

    MissileSystem missile;
    #endregion

    #region PROPERTIES
    public bool IsFullHealth => health == maxHealth;
    public bool ISFullPower => weaponPower == 2;

    #endregion


    #region UNITY EVENT FUNTIONS
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystem>();

        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;

        dodgeDuration = maxRoll / rollSpeed;
        rigidbody.gravityScale = 0f;

        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval /= overdriveFireFactor);//开火间隔缩短
        waitDecelerationTime = new WaitForSeconds(decelerationTime);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitInvincibleTime = new WaitForSeconds(InvincibleTime);
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
        input.onLaunchMissile += LaunchMissile;

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
        input.onLaunchMissile -= LaunchMissile;

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
        //PowerDown(); //todo：暂时取消受伤降低武器等级效果，不然游戏难度过高(后续考虑游戏多命，死亡后重置武器等级）
        statsBar_HUD.UpdateStats(health, maxHealth);
        TimeController.Instance.BulletTime(takeDamageSlowMotionDuration);

        if (gameObject.activeSelf)
        {
            Move(moveDirection);
            StartCoroutine(InvincibleCoroutine());

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
        GameManager.onGameOver?.Invoke();
        GameManager.GameState = GameState.GameOver;
        statsBar_HUD.UpdateStats(0f, maxHealth);
        base.Die();
    }

    /// <summary>
    /// 无敌状态
    /// </summary>
    /// <returns></returns>
    IEnumerator InvincibleCoroutine()
    {
        collider.isTrigger = true;
        yield return waitInvincibleTime;
        collider.isTrigger = false;
    }

    #endregion

    #region MOVE
    void Move(Vector2 moveInput)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveDirection = moveInput.normalized;
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveDirection * moveSpeed, Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right)));
        //StopCoroutine(nameof(DecelerationCoroutine));
    }

    void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveDirection = Vector2.zero;
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, Vector2.zero, Quaternion.identity));
        //StartCoroutine(nameof(DecelerationCoroutine));
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

    //IEnumerator DecelerationCoroutine()
    //{
    //    yield return waitDecelerationTime;
    //}

    private void Update()
    {
        transform.position = Viewport.Instance.PlayerMoveablePostion(transform.position, paddingX, paddingY);//限制玩家移动范围不出屏幕
    }

    #endregion

    #region FIRE
    void Fire()
    {
        muzzleVFX.Play();
        StartCoroutine(nameof(FireCoroutine));
    }

    void StopFire()
    {
        muzzleVFX.Stop();
        StopCoroutine(nameof(FireCoroutine));
    }

    IEnumerator FireCoroutine()
    {

        while (true)
        {
            switch (weaponPower)
            {
                case 0:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleBottom.position);

                    break;
                case 2:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile2, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile3, muzzleBottom.position);
                    break;
                default:
                    break;
            }

            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);

            yield return isOverdriving ? waitForOverdriveFireInterval : waitForFireInterval;
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
        currentRoll = 0f;
        TimeController.Instance.BulletTime(dodgeSlowMotionDuration, dodgeSlowMotionDuration);

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

        #region 方法二 线性插值

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

        //方法三：二次贝塞尔曲线 平滑缩放
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
        TimeController.Instance.BulletTime(overdriveSlowMotionDuration, overdriveSlowMotionDuration);
    }

    void OverdriveOff()
    {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }

    #endregion

    #region MISSILE

    void LaunchMissile()
    {
        missile.Launch(muzzleMiddle);
    }

    public void PickUpMissile()
    {
        missile.PickUp();
    }

    #endregion

    #region WEAPON POWER
    public void PowerUp()
    {
        //写法1
        //weaponPower++;
        //weaponPower = Mathf.Clamp(weaponPower, 0, 2);
        //写法2
        //weaponPower = Mathf.Min(weaponPower + 1, 2);
        //写法三
        weaponPower = Mathf.Min(++weaponPower, 2);
    }

    void PowerDown()
    {
        //写法1
        //weaponPower--;
        //weaponPower = Mathf.Clamp(weaponPower, 0, 2);
        //写法2
        //weaponPower = Mathf.Max(weaponPower - 1, 0);
        //写法3
        //weaponPower = Mathf.Clamp(weaponPower, --weaponPower, 0);
        //写法4
        weaponPower = Mathf.Max(--weaponPower, 0);
    }

    #endregion

}
