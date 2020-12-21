﻿using DG.Tweening;
using TomWill;
using UnityEngine;

public class CharaBehaviourTraining : MonoBehaviour
{
    [SerializeField] protected TrainingManager trainingManager;
    [SerializeField] protected CharaData data;
    [SerializeField] protected float startDashTime;
    protected float dashTime;

    protected Vector2 direction, lastDirection;
    protected bool isDashed, immune, canDash, insight = false;
    private bool dead;
    protected float dashDelay;
    [SerializeField] protected Rigidbody2D rb;

    protected SpriteRenderer sprite;
    protected Material defaultMaterial;
    [SerializeField] protected Material whiteflash;
    [SerializeField] protected float flashDelay;
    [SerializeField] protected bool isAccelerating;
    [SerializeField] protected float timeToStop;

    protected float timeMoveElapsed;
    public GameObject kiriatas, kananbawah;
    float minX, maxX, minY, maxY;
    float posisilamaX, posisilamaY;
    float posisibaruX, posisibaruY;

    [SerializeField] protected Animator anim;
    [SerializeField] protected ParticleSystem walkDustParticle, dashDustParticle;

    [SerializeField] private CharaInteract interact;


    public void Init()
    {
        Time.timeScale = 1f;
        dashDelay = data.BaseDashDelay;
        GameVariables.STILL_ALIVE = true;
        canDash = true;
        dead = false;
        data.Hp = 3;
        GameTime.PlayerTimeScale = 1f;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        defaultMaterial = sprite.material;
        anim = GetComponent<Animator>();
        minX = kiriatas.transform.position.x;
        maxX = kananbawah.transform.position.x;

        maxY = kiriatas.transform.position.y;
        minY = kananbawah.transform.position.y;

        if (GameData.ActiveItem && GameData.ActiveItem.ActivateOnStart)
        {
            UseItem();
        }

        TrainingUI.instance.UpdateLive();
        TrainingUI.instance.UpdateShield();
    }

    protected void Clamp()
    {
        posisilamaX = this.transform.position.x;

        posisibaruX = Mathf.Clamp(posisilamaX, minX, maxX);

        posisilamaY = this.transform.position.y;

        posisibaruY = Mathf.Clamp(posisilamaY, minY, maxY);
        transform.position = new Vector3(posisibaruX, posisibaruY, 0f);
    }

    protected void MoveAccelerate()
    {
        if (isDashed) return;

        if (isAccelerating)
        {
            if (!walkDustParticle.isPlaying) walkDustParticle.Play();
            Movement(1);
        }
        else
        {
            Stop();
            Movement(timeMoveElapsed / timeToStop);
        }
    }

    protected void Movement(float accelerate)
    {
        if (direction == Vector2.zero) walkDustParticle.Stop();
        float newSpeed = data.Speed * (GameVariables.SPEED_BUFF > 0 ? GameVariables.SPEED_BUFF : 1);
        transform.Translate(direction * newSpeed * GameTime.PlayerTime * accelerate);
    }


    private void Stop()
    {
        timeMoveElapsed -= GameTime.PlayerTime;

        if (timeMoveElapsed <= 0) timeMoveElapsed = 0;
    }

    protected void Dash()
    {
        if (lastDirection != Vector2.zero && isDashed)
        {
            if (dashTime <= 0)
            {
                rb.velocity = Vector2.zero;
                isDashed = false;
                data.IsDashing = false;
                anim.SetBool("dash", false);
                dashDustParticle.Stop();
                immune = false;
                DOVirtual.DelayedCall(dashDelay, () => canDash = true).timeScale = GameTime.PlayerTimeScale;
            }
            else
            {
                anim.SetBool("dash", true);
                if (walkDustParticle.isPlaying) walkDustParticle.Stop();
                dashDustParticle.Play();
                timeMoveElapsed = 0f;
                dashTime -= GameTime.PlayerTime;
                rb.velocity = lastDirection * data.DashSpeed;

                interact.DashingButtonInteract();
            }
        }
    }

    public bool PlayerDashing()
    {
        return data.IsDashing;
    }


    public void TakeDamage()
    {
        if (!immune)
        {
            TWAudioController.PlaySFX("SFX_PLAYER", "player_damaged");
            sprite.material = whiteflash;
            DOVirtual.DelayedCall(flashDelay, () => { sprite.material = defaultMaterial; }).timeScale = GameTime.PlayerTimeScale;

            if (data.Hp >= 1)
            {
                immune = true;
                DOTween.Kill("ImmuneDamage");
                DOVirtual.DelayedCall(2f, () => { immune = false; }).SetId("ImmuneDamage").timeScale = GameTime.PlayerTimeScale;

                if (data.Shield > 0)
                {
                    data.Shield -= 1;
                    TrainingUI.instance.UpdateShield();
                }
                else data.Hp -= 1;

                TrainingUI.instance.UpdateLive();

                if (data.Hp < 1)
                {
                    dead = true;
                    anim.SetBool("dead", true);
                    GameVariables.STILL_ALIVE = false;
                    GameVariables.GAME_OVER = true;
                    TWTransition.ScreenFlash(1, 0.1f);
                    DOTween.Sequence()
                        .AppendInterval(1f)
                        .AppendCallback(() =>
                        { TrainingUI.instance.GameOver(); });
                }
            }
        }
    }

    public void UpdateAnimationWalk(float x, float y, float speed)
    {
        anim.SetFloat("x", x);
        anim.SetFloat("y", y);
        anim.SetFloat("speed", speed);
    }

    protected void UseItem()
    {
        if (!GameData.ActiveItem.wasUsed)
        {
            trainingManager.CompleteActiveTLE("item_used");
            GameData.ActiveItem.TakeEffect();
            TrainingUI.instance.UpdateItemImage();

            GameData.ItemHolds.Remove(GameData.ActiveItem);
            GameData.ItemUsed.Add(GameData.ActiveItem);
        }
    }

    public void SetDashDelay(float time)
    {
        dashDelay = time;
    }

    public float GetDashDelay()
    {
        return dashDelay;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("damage area") && !immune)
        {
            TakeDamage();
            collision.GetComponent<CannonGK>()?.Explode();
        }

        if (collision.CompareTag("button_interact"))
        {
            interact.buttonInteract = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (interact.buttonInteract && collision.CompareTag("button_interact"))
        {
            interact.buttonInteract = null;
        }
    }
}