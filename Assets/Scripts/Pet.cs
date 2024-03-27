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

    [field: SerializeField]
    protected float SecondsBetweenBobs = 1.0f;

    [field: SerializeField]
    protected float BobAmplitude = 1.0f;

    [field: SerializeField]
    protected float BobFrequency = 1.0f;

    EPetState PetState = EPetState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        //LastPos = gameObject.transform.position;
    }

    private void FixedUpdate()
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

        if (Owner.IsOnGround())
        {
            RB.gravityScale = 5.0f;
        }
        else
        {
            RB.gravityScale = 0.0f;
        }

        float bobAmt = Mathf.Sin(Time.time * BobFrequency) * BobAmplitude;
        BodySprite.gameObject.transform.localPosition = new Vector2(0.0f, bobAmt);
      /*  if (Time.time > NextBobTime)
        {
            NextBobTime = Time.time + SecondsBetweenBobs;
            RB.AddForce(new Vector2(0.0f, BobForce));
        }*/
    }
}
