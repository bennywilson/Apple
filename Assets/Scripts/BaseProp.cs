using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProp : MonoBehaviour
{
    public SpriteRenderer PropSprite;
    public float Health;

    public AnimationInfo IdleAnim;
    public AnimationInfo DeathAnim;
    public ParticleSystem DeathParticleSystem;

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
        Block,
        Bomb,
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
	IEnumerator Explode(Apple apple)
	{
		yield return new WaitForSeconds(3);

		if (DeathParticleSystem != null)
        {
            DeathParticleSystem.gameObject.SetActive(true);

		}
        PropSprite.enabled = false;
		yield return new WaitForSeconds(5);
		Object.Destroy(transform.parent.gameObject);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
        if (PropState != EPropState.Idle)
        {
            return;
        }

        Apple apple = other.gameObject.GetComponent<Apple>();
        if (apple == null && other.transform.parent != null)
		{
            apple = other.transform.parent.GetComponent<Apple>();
		}

		if (InteractType == EInteractType.Bomb && apple)
        {
			PropState = EPropState.Dying;
			DeathAnim.StartAnimation(PropSprite);
            StartCoroutine(Explode(apple));
		}
	}
}
