using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Physics2DModule;

public enum ECharacterBodyState
{
    Idle,
    Walking,
    PickingFruit,
    Flying
}

public enum ECharacterFaceState
{
    Idle,
    StartingAttack,
    Attacking,
    Eating,
}

public class Apple : BaseCharacter
{
    [field: SerializeField]
    protected AnimationInfo PickingFruitAnimation;

    [field: SerializeField]
    protected AnimationInfo EatAnimation;

    public Camera MainCamera;
    public BackgroundImage[] BackgroundImages;

    public Events MoveLeftBtn;
    public Events MoveRightBtn;
    public Events InteractBtn;
    public Events AttackBtn;
    public Events FlyBtn;

    protected ECharacterBodyState BodyState = ECharacterBodyState.Idle;
    protected ECharacterFaceState FaceState = ECharacterFaceState.Idle;
    protected float FaceStateChangeStartTime;
    protected float BodyStateChangeStartTime;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < BackgroundImages.Length; i++)
        {
            BackgroundImages[i].DisplayImage.material = Instantiate(BackgroundImages[i].DisplayImage.material);
        }
        LastPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveVec = new Vector2();

        RB.velocity = new Vector2(0.0f, 0.0f);

        bool bIsFlyButtonPressed = Input.GetKey(KeyCode.Space) || FlyBtn.GetButtonDown();

        // Update body
        if (BodyState != ECharacterBodyState.PickingFruit)
        {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || MoveRightBtn.GetButtonDown())
            {
                moveVec = new Vector3(1.0f, 0.0f);
                BodySprite.flipX = false;
                HeadSprite.flipX = false;
                HeadSprite.transform.localPosition = new Vector2(0.25f, 0.139f);

                AttackList[0].AttackSprite.flipX = false;
                AttackList[0].AttackSprite.transform.localPosition = new Vector2(0.553f, 0.036f);

                if (BodyState != ECharacterBodyState.Flying && BodyState != ECharacterBodyState.Walking)
                {
                    BodyState = ECharacterBodyState.Walking;
                    WalkAnimation.StartAnimation(BodySprite);
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || MoveLeftBtn.GetButtonDown())
            {
                moveVec = new Vector3(-1.0f, 0.0f, 0.0f);
                BodySprite.flipX = true;
                HeadSprite.flipX = true;
                HeadSprite.transform.localPosition = new Vector2(-0.25f, 0.139f);

                AttackList[0].AttackSprite.flipX = true;
                AttackList[0].AttackSprite.transform.localPosition = new Vector2(-0.55f, 0.036f);

                if (BodyState != ECharacterBodyState.Flying && BodyState != ECharacterBodyState.Walking)
                {
                    BodyState = ECharacterBodyState.Walking;
                    WalkAnimation.StartAnimation(BodySprite);
                }
            }
            else if (FaceState != ECharacterFaceState.StartingAttack && FaceState != ECharacterFaceState.Attacking && BodyState != ECharacterBodyState.Flying)
            {
                if (Input.GetKeyDown(KeyCode.E) || InteractBtn.GetButtonDown())
                {
                    BaseProp[] Props = GameObject.FindObjectsOfType<BaseProp>();
                    for (int i = 0; i < Props.Length; i++)
                    {
                        BaseProp CurProp = Props[i];
                        if (CurProp.InteractType == BaseProp.EInteractType.None)
                        {
                            continue;
                        }

                        float distTo = (HeadSprite.gameObject.transform.position - CurProp.transform.position).magnitude;

                        if (distTo < 1.3f)
                        {
                            BodyState = ECharacterBodyState.PickingFruit;
                            PickingFruitAnimation.StartAnimation(BodySprite);
                            HeadSprite.gameObject.SetActive(false);
                            BodyStateChangeStartTime = Time.time;
                            break;
                        }
                    }
                }
            }

            if (bIsFlyButtonPressed && BodyState != ECharacterBodyState.Flying && BodyState != ECharacterBodyState.PickingFruit)
            {
                BodyState = ECharacterBodyState.Flying;
                FlyAnimation.StartAnimation(BodySprite);
            }

            if (BodyState == ECharacterBodyState.Flying)
            {
                if (bIsFlyButtonPressed)
                {
                    RB.velocity += new Vector2(0.0f, FlyUpSpeed);
                }
            }

            if (moveVec.sqrMagnitude > 0.001f)
            {
                RB.velocity += moveVec * WalkSpeed;
                MainCamera.transform.position = new Vector3(gameObject.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z);

                BaseProp[] Props = GameObject.FindObjectsOfType<BaseProp>();
                for (int i = 0; i < Props.Length; i++)
                {
                    BaseProp CurProp = Props[i];
                    if (CurProp.IsAlive() == true)
                    {
                        continue;
                    }

                    float distTo = (gameObject.transform.position - CurProp.transform.position).magnitude;
                    if (distTo < 0.5f)
                    {
                        StopCoroutine(ApplySoot());
                        StartCoroutine(ApplySoot());
                    }
                }
            }

        }

        switch(BodyState)
        {
            case ECharacterBodyState.Walking:
                {
                    if (moveVec.sqrMagnitude > 0.001f)
                    {
                        WalkAnimation.UpdateAnimation();
                    }
                    break;
                }

            case ECharacterBodyState.Flying:
                {
                    if (gameObject.transform.position.y < -3.0f && bIsFlyButtonPressed == false)
                    {
                        WalkAnimation.StartAnimation(BodySprite);
                        BodyState = ECharacterBodyState.Walking;
                    }
                    else
                    {
                        FlyAnimation.UpdateAnimation();
                    }
                    break;
                }

            case ECharacterBodyState.PickingFruit :
            {
                PickingFruitAnimation.UpdateAnimation();
                if (PickingFruitAnimation.AnimIsFinished())
                {
                    HeadSprite.gameObject.SetActive(true);
                    BodyStateChangeStartTime = Time.time;
                    BodyState = ECharacterBodyState.Idle;
                    FaceState = ECharacterFaceState.Eating;
                    FaceStateChangeStartTime = Time.time;
                    EatAnimation.StartAnimation(HeadSprite);
                }
                break;
            }
        }

        // Update Face
        switch(FaceState)
        {
            case ECharacterFaceState.Idle :
            {
                if (BodyState != ECharacterBodyState.PickingFruit && (Input.GetMouseButton(1) || AttackBtn.GetButtonDown()))
                {
                    FaceState = ECharacterFaceState.StartingAttack;
                    FaceStateChangeStartTime = Time.time;
                    AttackAnimations[0].StartAnimation(HeadSprite);
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
            Vector3 scrollVec = gameObject.transform.position - LastPos;
            BackgroundImages[i].Scroll(scrollVec);
        }
        LastPos = gameObject.transform.position;
    }

    IEnumerator ApplySoot()
    {
        BodySprite.material.SetVector("_BodyTint_1", new Vector4(0.1f, 0.1f, 0.1f, 0.1f));
        yield return new WaitForSeconds(4);
        BodySprite.material.SetVector("_BodyTint_1", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
    }
}
