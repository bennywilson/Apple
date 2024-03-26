using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPetState
{
    Idle,
    Following,
}

public class Pet : BaseCharacter
{
    [field: SerializeField]
    protected BaseCharacter Owner;

    [field: SerializeField]
    protected float MaxFollowDistance = 3;

    EPetState PetState = EPetState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        //LastPos = gameObject.transform.position;
    }

    private void Update()
    {
        Vector2 VecTo = (Owner.gameObject.transform.position - gameObject.transform.position);
        float distTo = VecTo.magnitude;
        if (PetState == EPetState.Idle)
        {
            RB.velocity = Vector2.zero;
            if (distTo > MaxFollowDistance)
            {
                PetState = EPetState.Following;
            }
        }
        else if (PetState == EPetState.Following)
        {
            RB.velocity = VecTo * WalkSpeed;
            if (distTo <= MaxFollowDistance - 1.0f)
            {
                PetState = EPetState.Idle;
                RB.velocity = Vector2.zero;
            }
        }
    }
}
