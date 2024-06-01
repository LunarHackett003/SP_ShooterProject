using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class RecoilProfile : ScriptableObject
{
    public float recoilPosReturnSpeed, recoilRotReturnSpeed,firingRecoilPosDamping, firingRecoilRotDamping, recoilPosMultiplier, recoilRotMultiplier;
    public float recoilForce;
    public Vector3 minHipRecoilPos, maxHipRecoilPos, minAimRecoilPos, maxAimRecoilPos;
    public Vector3 minHipRecoilRot, maxHipRecoilRot, minAimRecoilRot, maxAimRecoilRot;
    public AnimationCurve recoilPosBounceCurve, recoilRotBounceCurve;
    public float recoilPosDamping, recoilRotDamping;
    public Vector3 temporaryAimAnglePerShot;
    public float permanentAimAnglePerShot, tempAimAngleDecay, tempAimAngleDamp, permAimAngleDamp, tempAimAngleReturnSpeed;
    public AnimationCurve temporaryAimBounceCurve;
    public float viewmodelCameraInfluence;
    public float worldCameraInfluence;
    public Vector3 worldPositionScalar, worldRotationScalar, viewPositionScalar, viewRotationScalar;
}
