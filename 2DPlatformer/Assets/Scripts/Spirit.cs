using UnityEngine;
using System.Collections;
using System.Linq;

public class Spirit : Monster
{
    [SerializeField]
    private float speed = 2.0F;
    [SerializeField]
    private int lives = 3;
    [SerializeField]
    private int damage = 1;

    private Vector3 direction;


    private SpriteRenderer sprite;

    protected override void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Start()
    {
        direction = transform.right;
    }

    protected override void Update()
    {
        Move();
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();
        if (unit && unit is Character)
        {
            unit.ReceiveDamage(damage); 
        }
    }

    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.5F + transform.right * direction.x * 0.5F, 0.1F);

        if (colliders.Length > 0 && colliders.All(x => !x.GetComponent<Character>())) direction *= -1.0F;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }
}
