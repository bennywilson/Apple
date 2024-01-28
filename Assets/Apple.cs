using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AnimationInfo
{
    public void StartAnimation(SpriteRenderer renderer)
    {
        CurrentFrame = 0;
        NextFrameTime = Time.time + AnimationFrameDuration;
        SpriteRenderer = renderer;
    }

    public void UpdateAnimation()
    {
        if (WalkAnimation.Length == 0)
        {
            return;
        }

        if (Time.time > NextFrameTime)
        {
            NextFrameTime = Time.time + AnimationFrameDuration;
            CurrentFrame++;
            if (CurrentFrame >= WalkAnimation.Length)
            {
                CurrentFrame = 0;
            }

            SpriteRenderer.sprite = WalkAnimation[CurrentFrame];
        }
    }

    [System.NonSerialized]
    public int CurrentFrame;

    [System.NonSerialized]
    public float NextFrameTime;

    [System.NonSerialized]
    public SpriteRenderer SpriteRenderer;

    public float AnimationFrameDuration;
    public Sprite[] WalkAnimation;
};

public enum ECharacterState
{
    Idle,
    Walking
}

public class BaseCharacter : MonoBehaviour
{
    [field: SerializeField]
    public float WalkSpeed { get; private set;} = 1.0f;

    [field: SerializeField]
    public SpriteRenderer AppleHead { get; private set;}

    [field: SerializeField]
    public SpriteRenderer AppleBody { get; private set;}

    [field: SerializeField]
    public SpriteRenderer AppleHat { get; private set;}

    [field: SerializeField]
    protected AnimationInfo WalkAnimation;

    protected ECharacterState CharacterState = ECharacterState.Idle;
}

public class Apple : BaseCharacter
{
    public Camera MainCamera;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVec = new Vector3();

        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveVec = new Vector3(1.0f, 0.0f, 0.0f);
            AppleBody.flipX = false;
            AppleHead.flipX = false;
            AppleHead.transform.localPosition = new Vector3(0.2f, 0.139f, 0.0f);

        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveVec = new Vector3(-1.0f, 0.0f, 0.0f);
            AppleBody.flipX = true;
            AppleHead.flipX = true;
            AppleHead.transform.localPosition = new Vector3(-0.2f, 0.139f, 0.0f);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {

        }

        if (moveVec.sqrMagnitude > 0.001f)
        {
            if (CharacterState != ECharacterState.Walking)
            {
                WalkAnimation.StartAnimation(AppleBody);
                CharacterState = ECharacterState.Walking;
            }
            else
            {
                WalkAnimation.UpdateAnimation();
            }
            moveVec = moveVec * WalkSpeed * Time.deltaTime;
            MainCamera.transform.position = new Vector3(gameObject.transform.position.x,  MainCamera.transform.position.y,  MainCamera.transform.position.z);
            gameObject.transform.position = gameObject.transform.position + moveVec;
        }
    }
}
