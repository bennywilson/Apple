using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPetState {
    Idle,
    Following,
}

public class Pet : BaseCharacter {
    [field: SerializeField]
    protected BaseCharacter Owner;

	[field: SerializeField]
	protected AnimationInfo SleepAnimation;


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
    void Start() {
        //LastPos = gameObject.transform.position;
    }

    private void FixedUpdate() {
        Apple apple = Owner as Apple;

        Vector3 Offset = new Vector3(0.0f, 0.0f, 0.0f);
        if (apple.BodyState == ECharacterBodyState.Sleeping) {
            Offset = Offset + new Vector3(0.0f, 1.0f, 0.0f);
            if (apple.BodySprite.flipX) {
                Offset = Offset + new Vector3(0.5f, 0.0f, 0.0f);
            } else {
                Offset = Offset + new Vector3(-0.5f, 0.0f, 0.0f);
            }
        } else {
            Offset = Offset + new Vector3(0.0f, 0.5f, 0.0f);
        }

        Vector2 VecTo = ((Owner.gameObject.transform.position + Offset) - gameObject.transform.position);
        float distTo = VecTo.magnitude;
        if (PetState == EPetState.Idle) {
            RB.velocity = Vector2.zero;
            if (distTo > MaxFollowDistance) {
                PetState = EPetState.Following;
            }
        }
        else if (PetState == EPetState.Following) {
            RB.velocity = VecTo * WalkSpeed;
            float closeDist = (apple.BodyState != ECharacterBodyState.Sleeping) ? (MaxFollowDistance - 1.0f) : (0.5f);
            if (distTo <= closeDist - 1.0f && apple.BodyState != ECharacterBodyState.Sleeping) {
                PetState = EPetState.Idle;
                RB.velocity = Vector2.zero;
            }
        }

        if (Owner.IsOnGround()) {
            RB.gravityScale = 5.0f;
        } else {
            RB.gravityScale = 0.0f;
        }

        if (apple.BodyState != ECharacterBodyState.Sleeping) {
            float bobAmt = Mathf.Sin(Time.time * BobFrequency) * BobAmplitude;
            BodySprite.gameObject.transform.localPosition = new Vector2(0.0f, bobAmt);
        } else if (distTo < 1.2f) {
            if (SleepingAnimation.SpriteRenderer == null) {
				SleepingAnimation.StartAnimation(BodySprite);
            } else {
				SleepingAnimation.UpdateAnimation();
            }
		}
    }
}