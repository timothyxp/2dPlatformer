using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceControl : MonoBehaviour {
    [SerializeField]
    Butoon Jump;
    [SerializeField]
    Butoon Left;
    [SerializeField]
    Butoon Right;

    public void SetAct(bool q)
    {
        Jump.SetAct(q);
        Left.SetAct(q);
        Right.SetAct(q);
    }
    public void Activate(bool q)
    {
        gameObject.SetActive(q);
    }
}
