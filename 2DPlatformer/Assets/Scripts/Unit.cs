using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public virtual void ReceiveDamage(int damage)
    {
        Die();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
