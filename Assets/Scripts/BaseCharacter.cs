using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Physics2DModule;

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
    public float FlyUpSpeed { get; private set; } = 1.0f;

    [field: SerializeField]
    public SpriteRenderer HeadSprite { get; private set;}

    [field: SerializeField]
    public SpriteRenderer BodySprite { get; private set;}

    [field: SerializeField]
    public SpriteRenderer HatSprite { get; private set;}

    [field: SerializeField]
    public Rigidbody2D RB { get; private set;}


    [field: SerializeField]
    protected AnimationInfo WalkAnimation;

    [field: SerializeField]
    protected AnimationInfo[] AttackAnimations;

    [field: SerializeField]
    protected BaseAttack[] AttackList;

    [field: SerializeField]
    protected AnimationInfo FlyAnimation;

    protected Vector3 LastPos;
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
        DisplayImage.materialForRendering.SetVector("_UVOffset", new Vector2(UVOffset.x, 0.0f));
    }
}
