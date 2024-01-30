using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct SpriteAnimationFrame
{
    public Sprite AnimSprite;
    public float FrameDuration;
    public string[] FrameTriggers;
}

[System.Serializable]
public struct AnimationInfo
{
    public void StartAnimation(SpriteRenderer renderer)
    {
        CurrentFrame = 0;
        NextFrameTime = Time.time + SpriteAnimationFrameList[0].FrameDuration;
        SpriteRenderer = renderer;
        SpriteRenderer.sprite = SpriteAnimationFrameList[CurrentFrame].AnimSprite;
    }

    public string[] UpdateAnimation()
    {
        if (SpriteAnimationFrameList.Length == 0)
        {
            return null;
        }

        if (Time.time > NextFrameTime)
        {
            CurrentFrame++;
            if (CurrentFrame >= SpriteAnimationFrameList.Length)
            {
                if (LoopingAnimation)
                {
                    CurrentFrame = 0;
                }
                else
                {
                    CurrentFrame = SpriteAnimationFrameList.Length - 1;
                }
            }

            NextFrameTime = Time.time + SpriteAnimationFrameList[CurrentFrame].FrameDuration;
            SpriteRenderer.sprite = SpriteAnimationFrameList[CurrentFrame].AnimSprite;
        }

        return SpriteAnimationFrameList[CurrentFrame].FrameTriggers;
    }

    public bool AnimIsFinished()
    {
        if (CurrentFrame >= SpriteAnimationFrameList.Length)
            return true;

        if ((CurrentFrame == SpriteAnimationFrameList.Length - 1) && Time.time >= SpriteAnimationFrameList[SpriteAnimationFrameList.Length - 1].FrameDuration)
            return true;

        return false;
    }

    [System.NonSerialized]
    public int CurrentFrame;

    [System.NonSerialized]
    public float NextFrameTime;

    [System.NonSerialized]
    public SpriteRenderer SpriteRenderer;

    public bool LoopingAnimation;
    public SpriteAnimationFrame[] SpriteAnimationFrameList;
};

public enum ECharacterBodyState
{
    Idle,
    Walking,
    StartAttack,
    Attack,
}

public enum ECharacterFaceState
{
    Idle,
    StartingAttack,
    Attacking
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

    [field: SerializeField]
    protected AnimationInfo[] AttackAnimationList;

    protected ECharacterBodyState BodyState = ECharacterBodyState.Idle;
    protected ECharacterFaceState FaceState = ECharacterFaceState.Idle;
    protected float FaceStateChangeStartTime;
}

[System.Serializable]
public struct BackgroundImage
{
    public UnityEngine.UI.Image DisplayImage;
    public float ScrollRate;
    public Vector2 UVOffset;

    public void Scroll(Vector2 Offset)
    {
        UVOffset = UVOffset + Offset * ScrollRate;
        DisplayImage.materialForRendering.SetVector("_UVOffset", UVOffset);
    }
}

public class Apple : BaseCharacter
{
    public Camera MainCamera;
    public BackgroundImage[] BackgroundImages;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < BackgroundImages.Length; i++)
        {
            BackgroundImages[i].DisplayImage.material = Instantiate(BackgroundImages[i].DisplayImage.material);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVec = new Vector3();

        // Update body
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
            if (BodyState != ECharacterBodyState.Walking)
            {
                WalkAnimation.StartAnimation(AppleBody);
                BodyState = ECharacterBodyState.Walking;
            }
            else
            {
                WalkAnimation.UpdateAnimation();
            }
            moveVec = moveVec * WalkSpeed * Time.deltaTime;
            MainCamera.transform.position = new Vector3(gameObject.transform.position.x,  MainCamera.transform.position.y,  MainCamera.transform.position.z);
            gameObject.transform.position = gameObject.transform.position + moveVec;
        }

        // Update Face
        switch(FaceState)
        {
            case ECharacterFaceState.Idle :
            {
                if (Input.GetMouseButton(0))
                {
                    FaceState = ECharacterFaceState.StartingAttack;
                    FaceStateChangeStartTime = Time.time;
                    AttackAnimationList[0].StartAnimation(AppleHead);
                }
                break;
            }

            case ECharacterFaceState.StartingAttack :
            {
                string[] FrameTriggers = AttackAnimationList[0].UpdateAnimation();
                if (FrameTriggers != null && FrameTriggers.Length > 0)
                {
                    FaceState = ECharacterFaceState.Attacking;
                    FaceStateChangeStartTime = Time.time;
                }
                break;
            }

            case ECharacterFaceState.Attacking :
            {
                string[] FrameTriggers = AttackAnimationList[0].UpdateAnimation();
                if (AttackAnimationList[0].AnimIsFinished())
                {
                    FaceState = ECharacterFaceState.Idle;
                    FaceStateChangeStartTime = Time.time;
                }
                break;
            }
        }


        // Update backgrounds
        for (int i = 0; i < BackgroundImages.Length; i++)
        {
            BackgroundImages[i].Scroll(moveVec);
        }
    }
}
