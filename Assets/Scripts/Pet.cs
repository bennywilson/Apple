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
    protected float MaxFollowDistance = 3;

    [field: SerializeField]
    protected float SecondsBetweenBobs = 1.0f;

    [field: SerializeField]
    protected float BobAmplitude = 1.0f;

    [field: SerializeField]
    protected float BobFrequency = 1.0f;

    EPetState PetState = EPetState.Idle;

    float next_move_time = 0.0f;

    // Start is called before the first frame update
    void Start() {
        //LastPos = gameObject.transform.position;
    }

    private void FixedUpdate() {
        Apple apple = Owner as Apple;
        if (apple.BodyState == ECharacterBodyState.Sleeping) {
            RB.gravityScale = 0.0f;
            RB.velocity = new Vector3(0.0f, 0.0f, 0.0f);
			Vector3 Offset = new Vector3(0.0f, 0.0f, 0.0f);
			if (apple.BodySprite.flipX) {
                Offset = new Vector3(0.3f, 0.2f, 0.0f);
            } else {
				Offset = new Vector3(-0.3f, 0.2f, 0.0f);
			}
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, apple.transform.position + Offset, Mathf.Clamp01(Time.fixedDeltaTime * 3.0f));
            if ((gameObject.transform.position - apple.transform.position + Offset).magnitude < 0.8f) {
                if (SleepingAnimation.SpriteRenderer == null) {
					SleepingAnimation.StartAnimation(BodySprite);
                }
            }

			return;
		}

        Vector2 VecTo = ((Owner.gameObject.transform.position) - gameObject.transform.position);
        float distTo = VecTo.magnitude;
        if (VecTo.x > MaxFollowDistance && transform.position.y < -3.5f && Time.time > next_move_time) {
            next_move_time = Time.time + 0.4f;

            float y_scale = 1.0f + (Mathf.Clamp(VecTo.y / 5.0f, 0.0f, 1.0f) * 1.5f);
			if (VecTo.x > 0.0f) {
                RB.AddForce(new Vector2(300.0f, y_scale * 300.0f));
            } else {
                RB.AddForce(new Vector2(-300.0f, y_scale * 300.0f));
            }
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