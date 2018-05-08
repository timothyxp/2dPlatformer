using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Character : Unit
{
    private SecureInt lives = new SecureInt();

    private bool istabpressed = false;
    public bool double_jump = false;
    private float deltatime = 0F;
    private float reload = 2.0F;
    private float deltatime_damage = 0F;
    private float shield = 0.2F;
    private bool isOpenMenu = false;
    private float speed = 3.0F;
    [SerializeField]
    float[] speeds;
    [SerializeField]
    float[] jumpForces;

    private float jumpForce = 15.0F;
    private bool isandriod;

    private bool isGrounded = false;

    private Bullet bullet;
    new private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer sprite;
    [SerializeField]
    public Menu_table menu;
    LoadMenuControl load;

    public int Lives
    {
        get { return lives.GetValue(); }
        set
        {
           if (value <= 3)
            {
                lives = new SecureInt();
                lives.SetValue(value);
            }
            livesBar.Refresh();
        }
    }
    private LivesBar livesBar;

    public override void Die()
    {
        menu.Die();
        load.SetLevel(0);
        load.loadlevel();
    }

    private CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    private void Awake()
    {
        lives.SetValue(3);
        load = FindObjectOfType<LoadMenuControl>();
        livesBar = FindObjectOfType<LivesBar>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        bullet = Resources.Load<Bullet>("Bullet");
        menu =  FindObjectOfType<Menu_table>();
        isandriod = menu.isAndroid;
        speed = speeds[menu.GetDifficult()];
        jumpForce = jumpForces[menu.GetDifficult()];
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    public int GetDifficult()
    {
        if (!menu)
        {
            menu = FindObjectOfType<Menu_table>();
            isandriod = menu.isAndroid;
        }
        return menu.GetDifficult();
    }

    private void Update()
    {
        if (isGrounded)  State = CharState.Idle;
        deltatime += Time.deltaTime;
        deltatime_damage += Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && !isandriod) Shoot();
        int move = menu.getMove();
        if (Input.GetButton("Horizontal") || move != 0) Run(move);
        if ((Input.GetButton("Menu") || menu.getMenu()) && !istabpressed) OpenMenu();
        if (!Input.GetButton("Menu") && !menu.getMenu()) istabpressed = false;
        if (isGrounded) double_jump = false;
        if ((isGrounded || double_jump) && (Input.GetButtonDown("Jump") || menu.getJump())) Jump();
    }

    private void OpenMenu()
    {
        istabpressed = true;
        isOpenMenu = !isOpenMenu;
        menu.SetAct(isOpenMenu);
    }

    private void Run(int dir)
    {
        Vector3 direction = transform.right * (Input.GetAxis("Horizontal") + dir);
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        sprite.flipX = direction.x < 0.0F;
        if (isGrounded)  State = CharState.Run;
    }

    private void Jump()
    {
        if (double_jump)
        {
            rigidbody.velocity = Vector3.zero;
            double_jump = false;
            rigidbody.AddForce(transform.up * (1.0F * jumpForce), ForceMode2D.Impulse);
        }
        else
        {
            rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void Shoot()
    {
        if (deltatime < reload)
            return;
        deltatime = 0;
        Vector3 position = transform.position; position.y += 0.8F;
        Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation) as Bullet;
        newBullet.Parent = gameObject;
        newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1.0F : 1.0F);
    }

    public override void ReceiveDamage(int damage)
    {
        if (deltatime_damage > shield)  {
            Lives= Lives - damage;
            deltatime_damage = 0;
        }
        if (Lives <= 0)  {
            Die();
        }
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3F);
        int count = 0;
        for(int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].isTrigger)
            {
                count++;
            }
        }

        isGrounded = count > 1;
          
        if (!isGrounded) State = CharState.Jump;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Bullet bullet = collider.gameObject.GetComponent<Bullet>();
        if (bullet && bullet.Parent != gameObject)
        {
            ReceiveDamage(1);
        }
    }
}


public enum CharState
{
    Idle,
    Run,
    Jump
}

public struct SecureInt
{
    private int key;
    private int val;
    public void Start()
    {
        key = Random.Range(-1000, 1000);
        val = 0;
    }
    public int GetValue()
    {
        return val - key;
    }
    public override string ToString()
    {
        return GetValue().ToString();
    }
    public void SetValue(int value)
    {
        val = value + key;
    }
    public static SecureInt operator +(SecureInt d1,SecureInt d2)
    {
        SecureInt res = new SecureInt();
        res.SetValue(d1.GetValue() + d2.GetValue());
        return res;
    }
    public static SecureInt operator -(SecureInt d1,SecureInt d2)
    {
        SecureInt res = new SecureInt();
        res.SetValue(d1.GetValue() - d2.GetValue());
        return res;
    }
}