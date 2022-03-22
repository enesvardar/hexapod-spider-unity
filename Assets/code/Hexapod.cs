using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Assets.code
{

    //   (leftFront) (rightFront)
    //v2          v1
    //           \   head  /
    //            *---*---*
    //           /    |    \
    // (left    /     |     \
    // Middle) /      |      \
    //   v3 --*------cog------*-- v0(rightMiddle)
    //         \      |      /
    //          \     |     /
    //           \    |    /
    //            *---*---*
    //           /         \
    //         v4 v5
    //      (leftBack)   (rightBack)

    //   leftBack,rightMiddle,leftFront == > grup1
    //   rightBack,leftMiddle,rightFront == > grup2

    class Hexapod
    {
        public const int group1 = 1;
        public const int group2 = 2;
        
        public GameObject hexapod;
        private readonly List<Leg> joints;

        public WalkingStep stepWalk = WalkingStep.sleepy;
        public RotatingStep stepRotate = RotatingStep.sleepy;
        public DancingStep stepDancing = DancingStep.sleepy;

        public int changePosY = 0;
        public float changeRotateZ = 0;

        public float A = 0;

        public float danceTheta = 0;
        public float danceX = 0;
        public float danceY = 0;
        public float bufDanceX = 0;
        public float bufDanceY = 0;
        
        public UnityEngine.Vector3 bufBodyLocalPosition;
        public UnityEngine.Vector3 bufBodyLocalEulerAngles;

        public Hexapod()
        {
            hexapod = GameObject.Find("hexapod");

            hexapod.transform.localPosition = new UnityEngine.Vector3(0, 0, 30);
            hexapod.transform.localRotation = UnityEngine.Quaternion.Lerp(hexapod.transform.localRotation, UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0, 0, 0)), 1);

            joints = new List<Leg>
            {
                new Leg(GameObject.Find("leftBack"), -10),
                new Leg(GameObject.Find("leftMiddle"), -10),
                new Leg(GameObject.Find("leftFront"), -10),
                new Leg(GameObject.Find("rightBack"), -10),
                new Leg(GameObject.Find("rightMiddle"), -10),
                new Leg(GameObject.Find("rightFront"), -10)
            };
        }

        public void MoveHexapodBodyDir(float ofset, Direction dir)
        {
            float x = 0;
            float y = 0;

            if (dir == Direction.back || dir == Direction.forward)
            {
                if (dir == Direction.back)
                {
                    ofset = ofset * -1;
                }

                x = -Mathf.Sin(Mathf.PI * hexapod.transform.localEulerAngles.z / 180) * ofset;

                y = Mathf.Cos(Mathf.PI * hexapod.transform.localEulerAngles.z / 180) * ofset;
            }
            else if (dir == Direction.right || dir == Direction.left)
            {
                if (dir == Direction.left)
                {
                    ofset = ofset * -1;
                }

                x = Mathf.Cos(Mathf.PI * hexapod.transform.localEulerAngles.z / 180) * ofset;

                y = Mathf.Sin(Mathf.PI * hexapod.transform.localEulerAngles.z / 180) * ofset;
            }

            hexapod.transform.localPosition = new UnityEngine.Vector3(hexapod.transform.localPosition.x + x, hexapod.transform.localPosition.y + y, hexapod.transform.localPosition.z);

            Parameters.bodyLocalPosition = new UnityEngine.Vector3(Parameters.bodyLocalPosition.x + x, Parameters.bodyLocalPosition.y + y, Parameters.bodyLocalPosition.z);
        }

        public void MoveHexapodBodyXYZ(float x, float y, float z)
        {
            hexapod.transform.localPosition = new UnityEngine.Vector3(hexapod.transform.localPosition.x + x, hexapod.transform.localPosition.y + y, hexapod.transform.localPosition.z + z);

            Parameters.bodyLocalPosition = new UnityEngine.Vector3(Parameters.bodyLocalPosition.x + x, Parameters.bodyLocalPosition.y + y, Parameters.bodyLocalPosition.z + z);
        }

        public void RotateHexapodBodyXYZ(float x, float y, float z)
        {
            hexapod.transform.localRotation = UnityEngine.Quaternion.Euler(hexapod.transform.localEulerAngles.x + x, hexapod.transform.localEulerAngles.y + y, hexapod.transform.localEulerAngles.z + z);

            Parameters.bodyLocalEulerAngles = new UnityEngine.Vector3(Parameters.bodyLocalEulerAngles.x + x, Parameters.bodyLocalEulerAngles.y + y, Parameters.bodyLocalEulerAngles.z + z);
        }


        public void SetLocalPositionHexapodBody(UnityEngine.Vector3 value)
        {
            hexapod.transform.localPosition = value;

            Parameters.bodyLocalPosition = value;
        }

        public void SetLocalEulerAnglesHexapodBody(UnityEngine.Vector3 value)
        {
            hexapod.transform.localRotation = UnityEngine.Quaternion.Euler(value.x, value.y, value.z);

            Parameters.bodyLocalEulerAngles = value;
        }

        public void MoveLegGroup(int group, int step, Direction dir)
        {
            for (int i = 0; i < step; i++)
            {
                switch (group)
                {
                    case group1:
                        joints[(int)(Legs.leftBack)].MoveLeg(dir);
                        joints[(int)(Legs.rightMiddle)].MoveLeg(dir);
                        joints[(int)(Legs.leftFront)].MoveLeg(dir);
                        break;

                    case group2:
                        joints[(int)(Legs.rightBack)].MoveLeg(dir);
                        joints[(int)(Legs.leftMiddle)].MoveLeg(dir);
                        joints[(int)(Legs.rightFront)].MoveLeg(dir);
                        break;

                    default:
                        break;
                }
            }
        }

        public bool WalkingSpecialStep(Direction dir, int group, int conditionY)
        {
            bool done = false;

            if (changePosY < (conditionY / 2))
            {
                MoveLegGroup(group, 1, Direction.up);
            }
            else
            {
                MoveLegGroup(group, 1, Direction.down);
            }

            MoveHexapodBodyDir(1, dir);
            MoveLegGroup(group,2, dir);

            changePosY = changePosY + 1;

            if (changePosY == conditionY)
            {
                changePosY = 0;
                done = true;
            }

            return done;
        }

        public void Walking(Direction dir, bool contFlag)
        {
            bool done = false;

            switch (stepWalk)
            {
                case WalkingStep.sleepy:
                    stepWalk = contFlag == true ? WalkingStep.start : stepWalk;
                    break;
                case WalkingStep.start:
                    done = WalkingSpecialStep(dir, group1, 50);
                    stepWalk = done == true ? WalkingStep.walking2 : stepWalk;
                    break;
                case WalkingStep.walking1:
                    done = WalkingSpecialStep(dir, group1, 100);
                    stepWalk = done == true ? WalkingStep.walking2 : stepWalk;
                    if (done == true)
                        stepWalk = contFlag == false ? WalkingStep.stop : stepWalk;
                    break;
                case WalkingStep.walking2:
                    done = WalkingSpecialStep(dir, group2, 100);
                    stepWalk = done == true ? WalkingStep.walking1 : stepWalk;
                    break;
                case WalkingStep.stop:
                    done = WalkingSpecialStep(dir, group2, 50);
                    stepWalk = done == true ? WalkingStep.sleepy : stepWalk;
                    break;
                default:
                    break;
            }
        }

        public void RotateLegGroup(int group,float rotateZ)
        {
            switch (group)
            {
                case group1:
                    joints[(int)(Legs.leftBack)].UpdateLegBaseFORG(rotateZ);
                    joints[(int)(Legs.rightMiddle)].UpdateLegBaseFORG(rotateZ);
                    joints[(int)(Legs.leftFront)].UpdateLegBaseFORG(rotateZ);
                    break;

                case group2:
                    joints[(int)(Legs.rightBack)].UpdateLegBaseFORG(rotateZ);
                    joints[(int)(Legs.leftMiddle)].UpdateLegBaseFORG(rotateZ);
                    joints[(int)(Legs.rightFront)].UpdateLegBaseFORG(rotateZ);
                    break;

                default:
                    break;
            }
        }

        public bool RotatingSpecialStep(int group, float stepZ, float rotateZ, int condZ)
        {
            bool done = false;

            RotateHexapodBodyXYZ(0, 0, stepZ);

            RotateLegGroup(group, rotateZ);

            if (changeRotateZ <= condZ)
            {
                MoveLegGroup(group, 4, Direction.up);
            }
            else
            {
                MoveLegGroup(group, 4, Direction.down);
            }

            if (changeRotateZ ==  (2 * condZ))
            {
                done = true;
                changeRotateZ = 0;

                if (group == group1)
                    A = (joints[(int)(Legs.rightFront)].alphaAngleRad * Mathf.Rad2Deg % 360);
                else
                    A = (joints[(int)(Legs.leftFront)].alphaAngleRad * Mathf.Rad2Deg % 360);
            }

            return done;
        }

        public void Rotating(bool contFlag)
        {
            bool done = false;

            switch (stepRotate)
            {
                case RotatingStep.sleepy:
                    stepRotate = contFlag == true ? RotatingStep.start : stepRotate;
                    break;
                case RotatingStep.start:
                    changeRotateZ = changeRotateZ + 1;
                    done = RotatingSpecialStep(group1, 1, changeRotateZ, 10);
                    stepRotate = done == true ? RotatingStep.rotating1 : stepRotate;
                    break;
                case RotatingStep.rotating1:
                    changeRotateZ = changeRotateZ + 0.5f;
                    done = RotatingSpecialStep(group2, 0.5f, A + (20 - A) * changeRotateZ / 20, 10);
                    stepRotate = done == true ? RotatingStep.rotating2 : stepRotate;
                    break;
                case RotatingStep.rotating2:
                    changeRotateZ = changeRotateZ + 0.5f;
                    done = RotatingSpecialStep(group1, 0.5f, A + (20 - A) * changeRotateZ / 20, 10);
                    stepRotate = done == true ? RotatingStep.rotating1 : stepRotate;
                    if (done == true)
                        stepRotate = contFlag == false ? RotatingStep.stop : stepRotate;
                    break;
                case RotatingStep.stop:
                    changeRotateZ = changeRotateZ + 0.5f;
                    done = RotatingSpecialStep(group2, 0.5f, A + (-A) * changeRotateZ / 10, 5);
                    stepRotate = done == true ? RotatingStep.sleepy : stepRotate;
                    break;
                default:
                    break;
            }

            Thread.Sleep(2);
        }

        public void Dancing(bool contFlag)
        {
            switch (stepDancing)
            {
                case DancingStep.sleepy:

                    bufBodyLocalPosition = Parameters.bodyLocalPosition;
                    bufBodyLocalEulerAngles = Parameters.bodyLocalEulerAngles;
                    stepDancing = contFlag == true ? DancingStep.dancing : stepDancing;

                    break;
                case DancingStep.dancing:

                    danceTheta++;
                    danceX = (float)Math.Cos(Mathf.Deg2Rad * danceTheta);
                    danceY = (float)Math.Sin(Mathf.Deg2Rad * danceTheta);
                    MoveHexapodBodyXYZ((danceX - bufDanceX) * 30, (danceY - bufDanceY) * 30, 0);
                    RotateHexapodBodyXYZ((danceX - bufDanceX) * 5, (danceY - bufDanceY) * 5, 0);
                    bufDanceX = danceX;
                    bufDanceY = danceY;
                    stepDancing = contFlag == false ? DancingStep.stop : stepDancing;

                    break;
                case DancingStep.stop:

                    UnityEngine.Vector3 difPos = Parameters.bodyLocalPosition - bufBodyLocalPosition;
                    UnityEngine.Vector3 difAngle = Parameters.bodyLocalEulerAngles - bufBodyLocalEulerAngles;

                    if (Math.Abs(difPos.x) < 1 && Math.Abs(difAngle.x) < 1)
                    {
                        danceTheta = 0;
                        stepDancing = DancingStep.sleepy;

                        SetLocalPositionHexapodBody(bufBodyLocalPosition);
                        SetLocalEulerAnglesHexapodBody(bufBodyLocalEulerAngles);

                        bufDanceX = 0;
                        bufDanceY = 0;
                    }
                    else
                    {
                        MoveHexapodBodyXYZ(-difPos.x / 100, -difPos.y / 100, 0);
                        RotateHexapodBodyXYZ(-difAngle.x / 100, -difAngle.y / 100, 0);
                    }

                    break;
                default:
                    break;
            }
        }
        public void Update()
        {
            foreach (var joint in joints)
            {
                joint.Update();
            }
        }
    }
}

