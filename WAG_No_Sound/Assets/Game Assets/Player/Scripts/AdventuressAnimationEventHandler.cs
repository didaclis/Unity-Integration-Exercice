////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
////////////////////////////////////////////////////////////////////////

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdventuressAnimationEventHandler : MonoBehaviour
{
    [Header("Wwise")]
    public AK.Wwise.Event Swing = new AK.Wwise.Event();
    public AK.Wwise.Event GetItem = new AK.Wwise.Event();
    public AK.Wwise.Trigger GetItemStinger = new AK.Wwise.Trigger();

    [Header("Object Links")]
    [SerializeField]
    private Animator playerAnimator;

    [SerializeField]
    private GameObject runParticles;

    private PlayerFoot foot_L;
    private PlayerFoot foot_R;
    public List<AudioClip> footsteps_run_dirt;
    public List<AudioClip> footsteps_walk_dirt;

    public List<AudioClip> footsteps_run_grass;
    public List<AudioClip> footsteps_walk_grass;

    public List<AudioClip> footsteps_run_rubble;
    public List<AudioClip> footsteps_walk_rubble;

    public AudioClip swing_audio_clip;
    public AudioClip get_item_audio_clip;
    AudioSource audioSource;

    #region private variables
    private bool hasPausedMovement;
    private readonly int canShootMagicHash = Animator.StringToHash("CanShootMagic");
    private readonly int isAttackingHash = Animator.StringToHash("IsAttacking");
    #endregion

    private void Awake()
    {
        GameObject L = GameObject.Find("toe_left");
        GameObject R = GameObject.Find("toe_right");
        audioSource = GetComponent<AudioSource>();

        if (L != null)
        {
            foot_L = L.GetComponent<PlayerFoot>();
        }
        else {
            print("Left foot missing");
        }
        if (R != null)
        {
            foot_R = R.GetComponent<PlayerFoot>();
        }
        else
        {
            print("Right foot missing");
        }
    }


    void enableWeaponCollider()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.equippedWeaponInfo != null)
        {
            PlayerManager.Instance.equippedWeaponInfo.EnableHitbox();
        }
    }

    void disableWeaponCollider()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.equippedWeaponInfo != null)
        {
            PlayerManager.Instance.equippedWeaponInfo.DisableHitbox();
        }

    }

    void ScreenShake()
    {
        PlayerManager.Instance.cameraScript.CamShake(new PlayerCamera.CameraShake(0.4f, 0.7f));
    }

    bool onCooldown = false;
    public enum FootSide { left, right };
    public void TakeFootstep(FootSide side)
    {
        if (foot_L != null && foot_R != null) {
            if (!PlayerManager.Instance.inAir && !onCooldown)
            {
                Vector3 particlePosition;

                if (side == FootSide.left )
                {
                    if (foot_L.FootstepSound.Validate())
                    { 
                        foot_L.PlayFootstepSound();
                        audioSource.clip = ChangeClipMaterial(foot_L);
                        audioSource.Play();
                        particlePosition = foot_L.transform.position;
                        FootstepParticles(particlePosition);
                    }
                }
                else
                {
                    if (foot_R.FootstepSound.Validate())
                    {
                        foot_R.PlayFootstepSound();
                        audioSource.clip = ChangeClipMaterial(foot_R);
                        audioSource.Play();
                        particlePosition = foot_R.transform.position;
                        FootstepParticles(particlePosition);
                    }
                }
            }
        }
    }

    private AudioClip ChangeClipMaterial(PlayerFoot foot)
    {
        if(foot.materialChecker.GetMaterial().ToString() == "Dirt")
        {
            if(PlayerManager.Instance.isSprinting)
            {
                return footsteps_run_dirt[Random.Range(0, footsteps_run_dirt.Count)];
            }
            else
            {
                return footsteps_walk_dirt[Random.Range(0, footsteps_walk_dirt.Count)];
            }
        }
        else if (foot.materialChecker.GetMaterial().ToString() == "Grass")
        {
            if (PlayerManager.Instance.isSprinting)
            {
                return footsteps_run_grass[Random.Range(0, footsteps_run_grass.Count)];
            }
            else
            {
                return footsteps_walk_grass[Random.Range(0, footsteps_walk_grass.Count)];
            }
        }
        else if (foot.materialChecker.GetMaterial().ToString() == "Rubble")
        {
            if (PlayerManager.Instance.isSprinting)
            {
                return footsteps_run_rubble[Random.Range(0, footsteps_run_rubble.Count)];
            }
            else
            {
                return footsteps_walk_rubble[Random.Range(0, footsteps_walk_rubble.Count)];
            }
        }
        return footsteps_run_dirt[0];
    }

    void FootstepParticles(Vector3 particlePosition) {
        GameObject p = Instantiate(runParticles, particlePosition + Vector3.up * 0.1f, Quaternion.identity) as GameObject;
        p.transform.parent = SceneStructure.Instance.TemporaryInstantiations.transform;
        Destroy(p, 5f);
        StartCoroutine(FootstepCooldown());
    }

    IEnumerator FootstepCooldown()
    {
        onCooldown = true;
        yield return new WaitForSecondsRealtime(0.2f);
        onCooldown = false;
    }

    void ReadyToShootMagic()
    {
        PlayerManager.Instance.playerAnimator.SetBool(canShootMagicHash, true);
    }

    public enum AttackState { NotAttacking, Attacking };
    void SetIsAttacking(AttackState state)
    {
        if (state == AttackState.NotAttacking)
        {
            playerAnimator.SetBool(isAttackingHash, false);
        }
        else
        {
            playerAnimator.SetBool(isAttackingHash, true);
        }
    }

    public void Weapon_SwingEvent()
    {
        // PLAY SOUND
        Weapon W = PlayerManager.Instance.equippedWeaponInfo;
        W.WeaponTypeSwitch.SetValue(PlayerManager.Instance.weaponSlot);
        Swing.Post(PlayerManager.Instance.weaponSlot);
        audioSource.clip = swing_audio_clip;
        audioSource.Play();
    }

    public void PauseMovement()
    {
        if (!hasPausedMovement)
        {
            hasPausedMovement = true;
            PlayerManager.Instance.motor.SlowMovement();
        }
    }

    public void ResumeMovement()
    {
        if (hasPausedMovement)
        {
            hasPausedMovement = false;
            PlayerManager.Instance.motor.UnslowMovement();
        }
    }

    public void FreezeMotor()
    {
        StartCoroutine(PickupEvent());
    }

    private IEnumerator PickupEvent()
    {
        PlayerManager.Instance.PauseMovement(gameObject);
        yield return new WaitForSeconds(2f);
        PlayerManager.Instance.ResumeMovement(gameObject);
    }

    public void PickUpItem()
    {
        PlayerManager.Instance.PickUpEvent();
        GetItem.Post(this.gameObject);
        audioSource.clip = get_item_audio_clip;
        audioSource.Play();
        GetItemStinger.Post(GameManager.Instance.MusicGameObject);
    }

    public void WeaponSound()
    {
        Weapon EquippedWeapon = PlayerManager.Instance.equippedWeaponInfo;
        EquippedWeapon.WeaponImpact.Post(EquippedWeapon.transform.parent.gameObject);
    }
}
