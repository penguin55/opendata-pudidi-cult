﻿using DG.Tweening;
using UnityEngine;

public class MissileBM : DamageArea
{
    [SerializeField] private float projectileTimeToMove;
    private Collider2D collider;
    private SpriteRenderer alertProjectileSprite;
    private GameObject projectile;

    private bool activeMissile;

    public void Launch(GameObject projectile, float timeToLaunch, bool activeMissile = true)
    {
        collider = GetComponent<Collider2D>();
        alertProjectileSprite = GetComponent<SpriteRenderer>();

        collider.enabled = false;
        alertProjectileSprite.enabled = true;

        this.projectile = projectile;
        this.activeMissile = activeMissile;
        if (activeMissile) projectile.GetComponent<Collider2D>().enabled = false;
        else projectile.GetComponent<Collider2D>().enabled = true;

        alertProjectileSprite.DOFade(0.2f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetId("Alert"+transform.GetInstanceID());

        DOVirtual.DelayedCall(timeToLaunch, OnEnter_State);
    }

    protected override void OnEnter_State()
    {
        base.OnEnter_State();
        DOVirtual.DelayedCall(projectileTimeToMove/2f, () => collider.enabled = activeMissile);
        projectile.transform.DOMove(transform.position, projectileTimeToMove).SetEase(Ease.InSine).OnComplete( () =>
        {
            DOTween.Kill("Alert" + transform.GetInstanceID());
            alertProjectileSprite.DOFade(1f, 0f);
            if (!activeMissile)
            {
                projectile.GetComponent<Animator>().SetTrigger("Jammed");
                projectile.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            OnExit_State();
        });
    }

    protected override void OnExit_State()
    {
        base.OnExit_State();

        DOVirtual.DelayedCall(0.2f, () => {
            collider.enabled = false;
            alertProjectileSprite.enabled = false;
        });
        if (activeMissile)
        {
            projectile.GetComponent<SpriteRenderer>().enabled = false;
            ParticleSystem particle = projectile.transform.GetChild(0).GetComponent<ParticleSystem>();
            particle.Play();
            DOVirtual.DelayedCall(particle.main.startLifetimeMultiplier, () => {
                Destroy(projectile);
            });
        }
    }

    public void Explode()
    {
        projectile.GetComponent<SpriteRenderer>().enabled = false;
        ParticleSystem particle = projectile.transform.GetChild(1).GetComponent<ParticleSystem>();
        particle.Play();
        DOVirtual.DelayedCall(particle.main.startLifetimeMultiplier, () => {
            Destroy(projectile);
        });
    }
}
