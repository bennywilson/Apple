using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProp : MonoBehaviour
{
    public SpriteRenderer PropSprite;
    public float Health;

    public AnimationInfo IdleAnim;
    public AnimationInfo DeathAnim;

    enum EPropState
    {
        Idle,
        Dying,
        Dead
    }
    EPropState PropState;

    [System.Serializable]
    public enum EInteractType
    {
        None,
        PickFruit,
        Block
    }
    public EInteractType InteractType;
    
    // Start is called before the first frame update
    void Start()
    {
        PropState = EPropState.Idle;
        IdleAnim.StartAnimation(PropSprite);
    }

    // Update is called once per frame
    void Update()
    {
        switch (PropState)
        {
            case EPropState.Idle :
            {
                IdleAnim.UpdateAnimation();
                break;
            }

            case EPropState.Dying :
            {
                DeathAnim.UpdateAnimation();
              /*  if (DeathAnim.AnimIsFinished())
                {
                    Object.Destroy(gameObject);
                }*/
                break;
            }
        }
    }

    public void TakeDamage(float DamageAmount)
    {
        if (PropState != EPropState.Idle)
        {
            return;
        }

        Health -= DamageAmount;
        if (Health <= 0.0f)
        {
            PropState = EPropState.Dying;
            DeathAnim.StartAnimation(PropSprite);
        }
    }

    public bool IsAlive()
    {
        return Health > 0;
    }
}
