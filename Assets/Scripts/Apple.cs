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
        if (SpriteAnimationFrameList.Length == 0)
        {
            return;
        }

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
    public bool FreezeOnLastFrame;
    public SpriteAnimationFrame[] SpriteAnimationFrameList;
};

public enum ECharacterBodyState
{
    Idle,
    Walking,
    StartAttack,
    Attack,
    PickingFruit,
}

public enum ECharacterFaceState
{
    Idle,
    StartingAttack,
    Attacking,
    Eating,
}

[System.Serializable]
public struct BaseAttack
{
    public SpriteRenderer AttackSprite;
    public AnimationInfo AttackAnimationFrameList;
    public float DamageRadius;

    public void StartAttack()
    {
        AttackAnimationFrameList.StartAnimation(AttackSprite);
        AttackSprite.gameObject.SetActive(true);
    }
    public void UpdateAttack()
    {
        AttackAnimationFrameList.UpdateAnimation();
        BaseProp[] Props = GameObject.FindObjectsOfType<BaseProp>();
        for (int i = 0; i < Props.Length; i++)
        {
            BaseProp CurProp = Props[i];
            float distTo = (AttackSprite.gameObject.transform.position - CurProp.transform.position).magnitude;

            if (distTo < DamageRadius)
            {
                CurProp.TakeDamage(99999.0f);
            }
        }
    }

    public void StopAttack()
    {
        AttackSprite.gameObject.SetActive(false);
    }
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
    protected AnimationInfo[] AttackAnimations;

    [field: SerializeField]
    protected BaseAttack[] AttackList;

    protected ECharacterBodyState BodyState = ECharacterBodyState.Idle;
    protected ECharacterFaceState FaceState = ECharacterFaceState.Idle;
    protected float FaceStateChangeStartTime;
    protected float BodyStateChangeStartTime;
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
    [field: SerializeField]
    protected AnimationInfo PickingFruitAnimation;

    [field: SerializeField]
    protected AnimationInfo EatAnimation;

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
        if (BodyState != ECharacterBodyState.PickingFruit)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveVec = new Vector3(1.0f, 0.0f, 0.0f);
                AppleBody.flipX = false;
                AppleHead.flipX = false;
                AppleHead.transform.localPosition = new Vector3(0.2f, 0.139f, 0.0f);

                AttackList[0].AttackSprite.flipX = false;
                AttackList[0].AttackSprite.transform.localPosition = new Vector3(0.481f, 0.036f, 0.0f);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveVec = new Vector3(-1.0f, 0.0f, 0.0f);
                AppleBody.flipX = true;
                AppleHead.flipX = true;
                AppleHead.transform.localPosition = new Vector3(-0.2f, 0.139f, 0.0f);

                AttackList[0].AttackSprite.flipX = true;
                AttackList[0].AttackSprite.transform.localPosition = new Vector3(-0.481f, 0.036f, 0.0f);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                BaseProp[] Props = GameObject.FindObjectsOfType<BaseProp>();
                for (int i = 0; i < Props.Length; i++)
                {
                    BaseProp CurProp = Props[i];
                    if (CurProp.InteractType == BaseProp.EInteractType.None)
                    {
                        continue;
                    }

                    float distTo = (AppleHead.gameObject.transform.position - CurProp.transform.position).magnitude;

                    if (distTo < 1.3f)
                    {
                        BodyState = ECharacterBodyState.PickingFruit;
                        PickingFruitAnimation.StartAnimation(AppleBody);
                        AppleHead.gameObject.SetActive(false);
                        BodyStateChangeStartTime = Time.time;
                        break;
                    }
                }
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
        }

        switch(BodyState)
        {
            case ECharacterBodyState.PickingFruit :
            {
                PickingFruitAnimation.UpdateAnimation();
                if (PickingFruitAnimation.AnimIsFinished())
                {
                    AppleHead.gameObject.SetActive(true);
                    BodyStateChangeStartTime = Time.time;
                    BodyState = ECharacterBodyState.Idle;
                    FaceState = ECharacterFaceState.Eating;
                    FaceStateChangeStartTime = Time.time;
                    EatAnimation.StartAnimation(AppleHead);
                }
                break;
            }
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
                    AttackAnimations[0].StartAnimation(AppleHead);
                }
                break;
            }

            case ECharacterFaceState.StartingAttack :
            {
                string[] FrameTriggers = AttackAnimations[0].UpdateAnimation();
                if (FrameTriggers != null && FrameTriggers.Length > 0)
                {
                    FaceState = ECharacterFaceState.Attacking;
                    FaceStateChangeStartTime = Time.time;
                    AttackList[0].StartAttack();
                }
                break;
            }

            case ECharacterFaceState.Attacking :
            {
                string[] FrameTriggers = AttackAnimations[0].UpdateAnimation();
                AttackList[0].UpdateAttack();
                if (AttackAnimations[0].AnimIsFinished())
                {
                    FaceState = ECharacterFaceState.Idle;
                    FaceStateChangeStartTime = Time.time;
                    AttackList[0].StopAttack();
                }
                break;
            }

            case ECharacterFaceState.Eating :
            {
                EatAnimation.UpdateAnimation();
                if (EatAnimation.AnimIsFinished())
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
