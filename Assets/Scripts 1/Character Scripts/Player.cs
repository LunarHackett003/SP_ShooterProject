using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Player Controller script, written by Lunar :p
/// Handles the player's input, movement, all that jazz
/// </summary>
public class Player : Character
{
    [SerializeField] Vector2 moveInput, lookInput, lookAngle, oldLookAngle, deltaLookAngle;
    [SerializeField] Vector2 lookSpeed;
    [SerializeField] float aimPitchOffset;
    [SerializeField] Transform aimTransform;
    [SerializeField] float drag;
    [SerializeField] bool movingCamera;
    [SerializeField, Tooltip("The transform that directly holds the weapon, NOT the transform for the weapon"), Header("Weapon Sway")] Transform weaponTransform;
    [SerializeField] Vector3 weaponSwayPositionScalar, weaponSwayRotationScalar;
    [SerializeField] AnimationCurve swayPositionBounceCurve, swayRotationBounceCurve;
    [SerializeField] float swayPositionReturnSpeed, swayRotationReturnSpeed, swayPositionDamping, swayRotationDamping, aimingSwayPositionDamping, aimingSwayRotationDamping, swayPositionMultiplier, swayRotationMultiplier;
    Vector3 weaponSwayPositionTarget, weaponSwayRotationTarget, maxWeaponSwayPosition, maxWeaponSwayRotation, weaponSwayPos, weaponSwayRot;
    Vector3 swayPosDampVelocity;
    float swayPositionReturn, swayRotationReturn;
    Vector3 compositePosition, compositeRotation;
    public RecoilProfile currentRecoilProfile;
    [SerializeField] Vector3 recoilPositionScalar, recoilRotationScalar;
    [SerializeField] float recoilPosReturn, recoilRotReturn;
    Vector3 recoilPosTarget, recoilRotTarget, maxRecoilPos, maxRecoilRot, recoilPosDampVelocity, recoilRotDampVelocity, recoilPos, recoilRot;
    [SerializeField] Quaternion weaponRecoilOrientation, cameraRecoilOrientation;
    public WeaponManager weaponManager;
    [SerializeField] Vector3 temporaryAimAngleTarget, temporaryAimAngle;
    [SerializeField] float permanentAimAngle;
    [SerializeField] float permanentAimAngleMultiplier;
    float tempAimAngleLerp;
    Vector3 maxTempAimAngle;
    [SerializeField] Transform viewCamera, worldCamera;

    [SerializeField] float passiveHealPerSec, passiveHealDelay;
    float currentHealDelay;
    protected override void Start()
    {
    
        base.Start();
        if(!weaponManager)
            weaponManager = GetComponent<WeaponManager>();
    }
    private void Aim()
    {
        //Rotate the player based on the delta time
        //If no aim transform is specified, the player is incorrectly set up and will not rotate.
        if (!aimTransform)
            return;
        //Add the look input to the look angle
        if (!GameManager.instance.paused)
        {
            Vector2 lookModified = lookInput * lookSpeed * Time.fixedDeltaTime * Mathf.Lerp(1, weaponManager.CurrentWeapon.aimSensitivityMultiplier, weaponManager.currentAimLerp);
            lookAngle += lookModified;
            lookAngle.y = Mathf.Clamp(lookAngle.y, -85, 85);
        }
        deltaLookAngle = oldLookAngle - lookAngle;
            //modulo the look yaw by 360
        lookAngle.x %= 360;
        transform.localRotation = Quaternion.Euler(0, lookAngle.x, 0);
        oldLookAngle = lookAngle;
    }
    private void Update()
    {
        if(permanentAimAngle > 0)
            permanentAimAngle -= Time.unscaledDeltaTime * currentRecoilProfile.permAimAngleDamp;
        lookAngle.y += Mathf.Max(0, permanentAimAngle) * permanentAimAngleMultiplier;
        temporaryAimAngle = Vector3.Lerp(temporaryAimAngle, temporaryAimAngleTarget, Time.deltaTime * currentRecoilProfile.tempAimAngleDamp);
        aimTransform.localRotation = Quaternion.Euler(temporaryAimAngle + new Vector3(Mathf.Clamp(-lookAngle.y, -90, 90) + aimPitchOffset, 0, 0));

        weaponTransform.SetLocalPositionAndRotation(weaponSwayPos + (weaponRecoilOrientation * recoilPos.ScaleReturn(recoilPositionScalar) * currentRecoilProfile.recoilPosMultiplier),
   Quaternion.Euler(weaponSwayRot) * Quaternion.Euler(weaponRecoilOrientation * recoilRot.ScaleReturn(recoilRotationScalar) * currentRecoilProfile.recoilRotMultiplier));
    }
    private void LateUpdate()
    {

        WeaponSwayMaths();
        WeaponSwayVisuals();
    }
    void WeaponSwayVisuals()
    {
        weaponSwayPos = Vector3.SmoothDamp(weaponSwayPos, (weaponSwayPositionTarget * swayPositionMultiplier),
            ref swayPosDampVelocity, swayPositionDamping);
        weaponSwayRot = Vector3.LerpUnclamped(weaponSwayRot, weaponSwayRotationTarget * swayRotationMultiplier, Time.smoothDeltaTime * swayRotationDamping);




        viewCamera.SetLocalPositionAndRotation(cameraRecoilOrientation * (recoilPos * currentRecoilProfile.viewmodelCameraInfluence).ScaleReturn(currentRecoilProfile.viewPositionScalar),
    Quaternion.Euler((recoilRot * currentRecoilProfile.viewmodelCameraInfluence).ScaleReturn(currentRecoilProfile.viewRotationScalar)));
        worldCamera.SetLocalPositionAndRotation(cameraRecoilOrientation * (recoilPos * currentRecoilProfile.worldCameraInfluence).ScaleReturn(currentRecoilProfile.worldPositionScalar),
            Quaternion.Euler((recoilRot * currentRecoilProfile.worldCameraInfluence).ScaleReturn(currentRecoilProfile.worldRotationScalar)));
    }
    void WeaponSwayMaths()
    {
        if (movingCamera)
        {
            weaponSwayPositionTarget += Time.deltaTime* (new Vector3(deltaLookAngle.x, 0, deltaLookAngle.y).ScaleReturn(weaponSwayPositionScalar));
            weaponSwayRotationTarget += Time.deltaTime * (new Vector3(deltaLookAngle.y, deltaLookAngle.x, deltaLookAngle.x).ScaleReturn(weaponSwayRotationScalar));
            maxWeaponSwayPosition = weaponSwayPositionTarget;
            maxWeaponSwayRotation = weaponSwayRotationTarget;
            swayPositionReturn = 0;
            swayRotationReturn = 0;
            weaponSwayPositionTarget -= aimingSwayPositionDamping * Time.deltaTime * weaponSwayPositionTarget;
            weaponSwayRotationTarget -= aimingSwayRotationDamping * Time.deltaTime * weaponSwayRotationTarget;
        }
        else
        {
            if (swayPositionReturn < 1)
            {
                swayPositionReturn += Time.deltaTime * swayPositionReturnSpeed;
                weaponSwayPositionTarget = Vector3.LerpUnclamped(maxWeaponSwayPosition, Vector3.zero, swayPositionBounceCurve.Evaluate(swayPositionReturn));
            }
            if (swayRotationReturn < 1)
            {
                swayRotationReturn += Time.deltaTime * swayRotationReturnSpeed;
                weaponSwayRotationTarget = Vector3.LerpUnclamped(maxWeaponSwayRotation, Vector3.zero, swayRotationBounceCurve.Evaluate(swayRotationReturn));
            }
        }
    }
    bool firing;
    void RecoilMaths()
    {
        if (firing)
        {
            recoilPosTarget -= currentRecoilProfile.firingRecoilPosDamping * Time.fixedDeltaTime * recoilPosTarget;
            recoilRotTarget -= currentRecoilProfile.firingRecoilRotDamping * Time.fixedDeltaTime * recoilRotTarget;

            maxRecoilPos = recoilPosTarget;
            maxRecoilRot = recoilRotTarget;

            tempAimAngleLerp = 0;

            temporaryAimAngleTarget = Vector3.Lerp(temporaryAimAngleTarget, Vector3.zero, currentRecoilProfile.tempAimAngleDecay * Time.fixedDeltaTime);
            maxTempAimAngle = temporaryAimAngleTarget;
        }
        else
        {

            if (recoilPosReturn < 1)
            {
                recoilPosReturn += Time.fixedDeltaTime * currentRecoilProfile.recoilPosReturnSpeed;
            }
            if (recoilRotReturn < 1)
            {
                recoilRotReturn += Time.fixedDeltaTime * currentRecoilProfile.recoilRotReturnSpeed;
            }
                recoilPosTarget = Vector3.LerpUnclamped(maxRecoilPos, Vector3.zero, currentRecoilProfile.recoilPosBounceCurve.Evaluate(recoilPosReturn));
                recoilRotTarget = Vector3.LerpUnclamped(maxRecoilRot, Vector3.zero, currentRecoilProfile.recoilRotBounceCurve.Evaluate(recoilRotReturn));

            if(tempAimAngleLerp < 1)
            {
                tempAimAngleLerp += Time.fixedDeltaTime * currentRecoilProfile.tempAimAngleReturnSpeed;
            }

            temporaryAimAngleTarget = Vector3.LerpUnclamped(maxTempAimAngle, Vector3.zero, currentRecoilProfile.temporaryAimBounceCurve.Evaluate(tempAimAngleLerp));
        }
        firing = false;

        recoilPos = Vector3.SmoothDamp(recoilPos, recoilPosTarget, ref recoilPosDampVelocity, currentRecoilProfile.recoilPosDamping);
        recoilRot = Vector3.SmoothDamp(recoilRot, recoilRotTarget, ref recoilRotDampVelocity, currentRecoilProfile.recoilRotDamping);


    }
    private void FixedUpdate()
    {
        if (!IsAlive)
            return;
        rb.drag = drag;
        Move();
        RecoilMaths();

        InteractCheck();


        if (currentHealDelay < passiveHealDelay)
        {
            currentHealDelay += Time.fixedDeltaTime;
        }
        else
        {
            UpdateHealth(passiveHealPerSec * Time.fixedDeltaTime, transform.position);
        }
    }
    public override void Move()
    {
        //We want to move the player in the direction they're looking
        Vector3 movevec = transform.rotation * new Vector3(moveInput.x, 0, moveInput.y) * MoveSpeed;
        rb.AddForce(movevec);
    }
    public void InteractCheck()
    {
        //Currently not implemented;
    }

    #region InputCallbacks
    public void GetMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void GetLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        movingCamera = lookInput != Vector2.zero;
        if (IsAlive && !GameManager.instance.paused)
            Aim();
    }
    public void GetPauseInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            GameManager.instance.PauseGame(!GameManager.instance.paused);
    }
    #endregion

    public override void UpdateHealth(float healthChange, Vector3 damagePosition)
    {
        base.UpdateHealth(healthChange, damagePosition);
        GameManager.instance.damageVolume.weight = Mathf.InverseLerp(maxHealth, 0, health);
        if(GameManager.instance.healthbar)
            GameManager.instance.healthbar.value = health;
        if (healthChange < 0)
        {
            DamageRingManager.Instance.AddRing(damagePosition);
            currentHealDelay = 0;
        }
    }
    public override void Die()
    {

    }
    
    public void ReceiveRecoilImpulse(Vector3 pos, Vector3 rot)
    {
        recoilPosTarget += pos;
        recoilRotTarget += rot;

        recoilPosReturn = 0;
        recoilRotReturn = 0;
        firing = true;

        permanentAimAngle = currentRecoilProfile.permanentAimAnglePerShot;

        Vector2 randomCircleVal = Random.insideUnitCircle;
        temporaryAimAngleTarget += new Vector3(currentRecoilProfile.temporaryAimAnglePerShot.x, randomCircleVal.x * currentRecoilProfile.temporaryAimAnglePerShot.y, randomCircleVal.y * currentRecoilProfile.temporaryAimAnglePerShot.z);
    }
}
