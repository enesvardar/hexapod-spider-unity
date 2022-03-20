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
        List<Leg> joints;

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

            joints = new List<Leg>();

            joints.Add(new Leg(GameObject.Find("leftBack"), -10));
            joints.Add(new Leg(GameObject.Find("leftMiddle"), -10));
            joints.Add(new Leg(GameObject.Find("leftFront"), -10));
            joints.Add(new Leg(GameObject.Find("rightBack"), -10));
            joints.Add(new Leg(GameObject.Find("rightMiddle"), -10));
            joints.Add(new Leg(GameObject.Find("rightFront"), -10));
        }

        public void moveHexapodBodyDir(float ofset, Direction dir)
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

        public void moveHexapodBodyXYZ(float x, float y, float z)
        {
            hexapod.transform.localPosition = new UnityEngine.Vector3(hexapod.transform.localPosition.x + x, hexapod.transform.localPosition.y + y, hexapod.transform.localPosition.z + z);

            Parameters.bodyLocalPosition = new UnityEngine.Vector3(Parameters.bodyLocalPosition.x + x, Parameters.bodyLocalPosition.y + y, Parameters.bodyLocalPosition.z + z);
        }

        public void rotateHexapodBodyXYZ(float x, float y, float z)
        {
            hexapod.transform.localRotation = UnityEngine.Quaternion.Euler(hexapod.transform.localEulerAngles.x + x, hexapod.transform.localEulerAngles.y + y, hexapod.transform.localEulerAngles.z + z);

            Parameters.bodyLocalEulerAngles = new UnityEngine.Vector3(Parameters.bodyLocalEulerAngles.x + x, Parameters.bodyLocalEulerAngles.y + y, Parameters.bodyLocalEulerAngles.z + z);
        }


        public void setLocalPositionHexapodBody(UnityEngine.Vector3 value)
        {
            hexapod.transform.localPosition = value;

            Parameters.bodyLocalPosition = value;
        }

        public void setLocalEulerAnglesHexapodBody(UnityEngine.Vector3 value)
        {
            hexapod.transform.localRotation = UnityEngine.Quaternion.Euler(value.x, value.y, value.z);

            Parameters.bodyLocalEulerAngles = value;
        }

        public void moveLegGroup(int group, int step, Direction dir)
        {
            for (int i = 0; i < step; i++)
            {
                switch (group)
                {
                    case group1:
                        joints[(int)(legs.leftBack)].moveLeg(dir);
                        joints[(int)(legs.rightMiddle)].moveLeg(dir);
                        joints[(int)(legs.leftFront)].moveLeg(dir);
                        break;

                    case group2:
                        joints[(int)(legs.rightBack)].moveLeg(dir);
                        joints[(int)(legs.leftMiddle)].moveLeg(dir);
                        joints[(int)(legs.rightFront)].moveLeg(dir);
                        break;

                    default:
                        break;
                }
            }
        }

        public bool walkingSpecialStep(Direction dir, int group, int conditionY)
        {
            bool done = false;

            if (changePosY < (conditionY / 2))
            {
                moveLegGroup(group, 1, Direction.up);
            }
            else
            {
                moveLegGroup(group, 1, Direction.down);
            }

            moveHexapodBodyDir(1, dir);
            moveLegGroup(group,2, dir);

            changePosY = changePosY + 1;

            if (changePosY == conditionY)
            {
                changePosY = 0;
                done = true;
            }

            return done;
        }

        public void walking(Direction dir, bool contFlag)
        {
            bool done = false;

            switch (stepWalk)
            {
                case WalkingStep.sleepy:
                    stepWalk = contFlag == true ? WalkingStep.start : stepWalk;
                    break;
                case WalkingStep.start:
                    done = walkingSpecialStep(dir, group1, 50);
                    stepWalk = done == true ? WalkingStep.walking2 : stepWalk;
                    break;
                case WalkingStep.walking1:
                    done = walkingSpecialStep(dir, group1, 100);
                    stepWalk = done == true ? WalkingStep.walking2 : stepWalk;
                    if (done == true)
                        stepWalk = contFlag == false ? WalkingStep.stop : stepWalk;
                    break;
                case WalkingStep.walking2:
                    done = walkingSpecialStep(dir, group2, 100);
                    stepWalk = done == true ? WalkingStep.walking1 : stepWalk;
                    break;
                case WalkingStep.stop:
                    done = walkingSpecialStep(dir, group2, 50);
                    stepWalk = done == true ? WalkingStep.sleepy : stepWalk;
                    break;
                default:
                    break;
            }
        }

        public void rotateLegGroup(int group,float rotateZ)
        {
            switch (group)
            {
                case group1:
                    joints[(int)(legs.leftBack)].updateLegBaseFORG(rotateZ);
                    joints[(int)(legs.rightMiddle)].updateLegBaseFORG(rotateZ);
                    joints[(int)(legs.leftFront)].updateLegBaseFORG(rotateZ);
                    break;

                case group2:
                    joints[(int)(legs.rightBack)].updateLegBaseFORG(rotateZ);
                    joints[(int)(legs.leftMiddle)].updateLegBaseFORG(rotateZ);
                    joints[(int)(legs.rightFront)].updateLegBaseFORG(rotateZ);
                    break;

                default:
                    break;
            }
        }

        public bool rotatingSpecialStep(int group, float stepZ, float rotateZ, int condZ)
        {
            bool done = false;

            rotateHexapodBodyXYZ(0, 0, stepZ);

            rotateLegGroup(group, rotateZ);

            if (changeRotateZ <= condZ)
            {
                moveLegGroup(group, 4, Direction.up);
            }
            else
            {
                moveLegGroup(group, 4, Direction.down);
            }

            if (changeRotateZ ==  (2 * condZ))
            {
                done = true;
                changeRotateZ = 0;

                if (group == group1)
                    A = (joints[(int)(legs.rightFront)].alphaAngleRad * Mathf.Rad2Deg % 360);
                else
                    A = (joints[(int)(legs.leftFront)].alphaAngleRad * Mathf.Rad2Deg % 360);
            }

            return done;
        }

        public void rotating(bool contFlag)
        {
            bool done = false;

            switch (stepRotate)
            {
                case RotatingStep.sleepy:
                    stepRotate = contFlag == true ? RotatingStep.start : stepRotate;
                    break;
                case RotatingStep.start:
                    changeRotateZ = changeRotateZ + 1;
                    done = rotatingSpecialStep(group1, 1, changeRotateZ, 10);
                    stepRotate = done == true ? RotatingStep.rotating1 : stepRotate;
                    break;
                case RotatingStep.rotating1:
                    changeRotateZ = changeRotateZ + 0.5f;
                    done = rotatingSpecialStep(group2, 0.5f, A + (20 - A) * changeRotateZ / 20, 10);
                    stepRotate = done == true ? RotatingStep.rotating2 : stepRotate;
                    break;
                case RotatingStep.rotating2:
                    changeRotateZ = changeRotateZ + 0.5f;
                    done = rotatingSpecialStep(group1, 0.5f, A + (20 - A) * changeRotateZ / 20, 10);
                    stepRotate = done == true ? RotatingStep.rotating1 : stepRotate;
                    if (done == true)
                        stepRotate = contFlag == false ? RotatingStep.stop : stepRotate;
                    break;
                case RotatingStep.stop:
                    changeRotateZ = changeRotateZ + 0.5f;
                    done = rotatingSpecialStep(group2, 0.5f, A + (-A) * changeRotateZ / 10, 5);
                    stepRotate = done == true ? RotatingStep.sleepy : stepRotate;
                    break;
                default:
                    break;
            }

            Thread.Sleep(2);
        }

        public void dancing(bool contFlag)
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
                    moveHexapodBodyXYZ((danceX - bufDanceX) * 30, (danceY - bufDanceY) * 30, 0);
                    rotateHexapodBodyXYZ((danceX - bufDanceX) * 5, (danceY - bufDanceY) * 5, 0);
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

                        setLocalPositionHexapodBody(bufBodyLocalPosition);
                        setLocalEulerAnglesHexapodBody(bufBodyLocalEulerAngles);

                        bufDanceX = 0;
                        bufDanceY = 0;
                    }
                    else
                    {
                        moveHexapodBodyXYZ(-difPos.x / 100, -difPos.y / 100, 0);
                        rotateHexapodBodyXYZ(-difAngle.x / 100, -difAngle.y / 100, 0);
                    }

                    break;
                default:
                    break;
            }
        }
        public void update()
        {
            foreach (var joint in joints)
            {
                joint.update();
            }
        }
    }
}

