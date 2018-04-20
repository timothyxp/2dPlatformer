using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Character : Unit
{
    [SerializeField]
    private int lives = 3;

    private bool istabpressed = false;
    public bool double_jump = false;
    private float deltatime = 0F;
    private float reload = 2.0F;
    private float deltatime_damage = 0F;
    private float shield = 0.2F;
    private bool isOpenMenu = false;

    public int Lives
    {
        get { return lives; }
        set
        {
           if (value <= 3) lives = value;
            livesBar.Refresh();
        }
    }
    private LivesBar livesBar;

    public override void Die()
    {
        menu.Die();
        SceneManager.LoadScene(0);
    }

    [SerializeField]
    private float speed = 3.0F;
    [SerializeField]
    private float jumpForce = 15.0F;

    private bool isGrounded = false;

    private Bullet bullet;

    private CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    new private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer sprite;
    [SerializeField]
    public Menu_table menu;

    private void Awake()
    {
        livesBar = FindObjectOfType<LivesBar>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        bullet = Resources.Load<Bullet>("Bullet");
        menu =  FindObjectOfType<Menu_table>();
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
        }
        return menu.GetDifficult();
    }

    private void Update()
    {
        if (isGrounded && State != CharState.Die)  State = CharState.Idle;
        deltatime += Time.deltaTime;
        deltatime_damage += Time.deltaTime;
        if (Input.GetButtonDown("Fire1")) Shoot();
        if (Input.GetButton("Horizontal")) Run();
        if (Input.GetButton("Menu") && !istabpressed) OpenMenu();
        if (!Input.GetButton("Menu")) istabpressed = false;
        if (isGrounded) double_jump = false;
        if ((isGrounded || double_jump) && Input.GetButtonDown("Jump")) Jump();
    }

    private void OpenMenu()
    {
        istabpressed = true;
        isOpenMenu = !isOpenMenu;
        menu.SetAct(isOpenMenu);
    }

    private void Run()
    {
        Vector3 direction = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        sprite.flipX = direction.x < 0.0F;
        if (isGrounded && State != CharState.Die)  State = CharState.Run;
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
            Lives-=damage; 
            deltatime_damage = 0;
        }
        if (Lives <= 0)  {
            Die();
        }
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3F);

        isGrounded = colliders.Length > 1;
          
        if (!isGrounded && State != CharState.Die) State = CharState.Jump;
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
    Jump,
    Die
}