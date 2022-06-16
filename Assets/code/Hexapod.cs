using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.code
{

    //        (leftFront)       (rightFront)
    //               
    //                \   head  /
    //                 *---*---*
    //                /    |    \
    //               /     |     \
    //              /      |      \
    // leftMiddle --*------cog------*-- (rightMiddle)
    //              \      |      /
    //               \     |     /
    //                \    |    /
    //                 *---*---*
    //                /         \
    //              
    //           (leftBack)   (rightBack)

    //   leftBack,rightMiddle,leftFront == > LEG_GROUP.firstly
    //   rightBack,leftMiddle,rightFront == > LEG_GROUP.secondly

    class Hexapod
    {
        public GameObject hexapod; // robot objesi

        public MyVector3 bufBodyLocalPosition; // gövdesinin origine göre pozisyonunu tutan buffer
        public MyVector3 bufBodyLocalEulerAngles; // gövdesinin origine göre açısal farkını tutan buffer

        private readonly List<Leg> legs; // robota bağlı 6 bacağı tutan liste

        // yürüme, dönme ve dans adımlarını tutan değişkenler
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
        
        public Hexapod()
        {
            hexapod = GameObject.Find("hexapod");

            hexapod.transform.localPosition = new UnityEngine.Vector3(0, 0, Parameters.bodyLocalPosition.z);
            hexapod.transform.localRotation = UnityEngine.Quaternion.Lerp(hexapod.transform.localRotation, UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(0, 0, 0)), 1);

            legs = new List<Leg>
            {
                new Leg(GameObject.Find("leftBack"), Parameters.endOfset),
                new Leg(GameObject.Find("leftMiddle"), Parameters.endOfset),
                new Leg(GameObject.Find("leftFront"), Parameters.endOfset),
                new Leg(GameObject.Find("rightBack"), Parameters.endOfset),
                new Leg(GameObject.Find("rightMiddle"), Parameters.endOfset),
                new Leg(GameObject.Find("rightFront"), Parameters.endOfset)
            };
        }

        // Bu fonkisyon robotun gövdesini hareket ettirmek için kullanılır.
        public void MoveHexapodBodyDir(float step, Direction dir)
        {
            float stepX = 0;
            float stepY = 0;

            // gövedenin bulunduğu rotation bilgisini ve hareket ettirmilken isteninlen yöne göre x ve y eksenlerinde gidilecek step değerleri hesaplanıyor 
            if (dir == Direction.back || dir == Direction.forward)
            {
                if (dir == Direction.back)
                {
                    step *= -1;
                }

                stepX = -Mathf.Sin(Mathf.PI * hexapod.transform.localEulerAngles.z / 180) * step;

                stepY = Mathf.Cos(Mathf.PI * hexapod.transform.localEulerAngles.z / 180) * step;
            }
            else if (dir == Direction.right || dir == Direction.left)
            {
                if (dir == Direction.left)
                {
                    step *= -1;
                }

                stepX = Mathf.Cos(Mathf.PI * hexapod.transform.localEulerAngles.z / 180) * step;

                stepY = Mathf.Sin(Mathf.PI * hexapod.transform.localEulerAngles.z / 180) * step;
            }

            hexapod.transform.localPosition = new UnityEngine.Vector3(hexapod.transform.localPosition.x + stepX, hexapod.transform.localPosition.y + stepY, hexapod.transform.localPosition.z);
            
            Parameters.bodyLocalPosition = new MyVector3(Parameters.bodyLocalPosition.x + stepX, Parameters.bodyLocalPosition.y + stepY, Parameters.bodyLocalPosition.z);
        }

        // Bu fonksiyon robot gövdesini x y z ekseninde hareket ettirmek için kullanılır
        public void MoveHexapodBodyXYZ(float stepX, float stepY, float stepZ)
        {
            hexapod.transform.localPosition = new UnityEngine.Vector3(hexapod.transform.localPosition.x + stepX, hexapod.transform.localPosition.y + stepY, hexapod.transform.localPosition.z + stepZ);

            Parameters.bodyLocalPosition = new MyVector3(Parameters.bodyLocalPosition.x + stepX, Parameters.bodyLocalPosition.y + stepY, Parameters.bodyLocalPosition.z + stepZ);
        }

        // Bu fonksiyon robot gövdesini x y z ekseninde döndürmek ettirmek için kullanılır
        public void RotateHexapodBodyXYZ(float stepX, float stepY, float stepZ)
        {
            hexapod.transform.localRotation = UnityEngine.Quaternion.Euler(hexapod.transform.localEulerAngles.x + stepX, hexapod.transform.localEulerAngles.y + stepY, hexapod.transform.localEulerAngles.z + stepZ);

            Parameters.bodyLocalEulerAngles = new MyVector3(Parameters.bodyLocalEulerAngles.x + stepX, Parameters.bodyLocalEulerAngles.y + stepY, Parameters.bodyLocalEulerAngles.z + stepZ);
        }

        // BU fonksiyon ile gövde pozisyonu istenilen bir konuma set edilir
        public void SetLocalPositionHexapodBody(MyVector3 value)
        {
            hexapod.transform.localPosition = new UnityEngine.Vector3(value.x, value.y, value.z); 

            Parameters.bodyLocalPosition = value;
        }

        // BU fonksiyon ile gövde rotatini istenilen bir değere set edilir
        public void SetLocalEulerAnglesHexapodBody(MyVector3 value)
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
                    case (int)LEG_GROUP.firstly:
                        legs[(int)(LEG_NAME.leftBack)].MoveDirLegBasePoint(dir);
                        legs[(int)(LEG_NAME.rightMiddle)].MoveDirLegBasePoint(dir);
                        legs[(int)(LEG_NAME.leftFront)].MoveDirLegBasePoint(dir);
                        break;

                    case (int)LEG_GROUP.secondly:
                        legs[(int)(LEG_NAME.rightBack)].MoveDirLegBasePoint(dir);
                        legs[(int)(LEG_NAME.leftMiddle)].MoveDirLegBasePoint(dir);
                        legs[(int)(LEG_NAME.rightFront)].MoveDirLegBasePoint(dir);
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

            changePosY++;

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
                    done = WalkingSpecialStep(dir, (int)LEG_GROUP.firstly, 50);
                    stepWalk = done == true ? WalkingStep.walking2 : stepWalk;
                    break;
                case WalkingStep.walking1:
                    done = WalkingSpecialStep(dir, (int)LEG_GROUP.firstly, 100);
                    stepWalk = done == true ? WalkingStep.walking2 : stepWalk;
                    if (done == true)
                        stepWalk = contFlag == false ? WalkingStep.stop : stepWalk;
                    break;
                case WalkingStep.walking2:
                    done = WalkingSpecialStep(dir, (int)LEG_GROUP.secondly, 100);
                    stepWalk = done == true ? WalkingStep.walking1 : stepWalk;
                    break;
                case WalkingStep.stop:
                    done = WalkingSpecialStep(dir, (int)LEG_GROUP.secondly, 50);
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
                case (int)LEG_GROUP.firstly:
                    legs[(int)(LEG_NAME.leftBack)].UpdateLegBaseFORG(rotateZ);
                    legs[(int)(LEG_NAME.rightMiddle)].UpdateLegBaseFORG(rotateZ);
                    legs[(int)(LEG_NAME.leftFront)].UpdateLegBaseFORG(rotateZ);
                    break;

                case (int)LEG_GROUP.secondly:
                    legs[(int)(LEG_NAME.rightBack)].UpdateLegBaseFORG(rotateZ);
                    legs[(int)(LEG_NAME.leftMiddle)].UpdateLegBaseFORG(rotateZ);
                    legs[(int)(LEG_NAME.rightFront)].UpdateLegBaseFORG(rotateZ);
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

                if (group == (int)LEG_GROUP.firstly)
                    A = (legs[(int)(LEG_NAME.rightFront)].alphaAngleRad * Mathf.Rad2Deg % 360);
                else
                    A = (legs[(int)(LEG_NAME.leftFront)].alphaAngleRad * Mathf.Rad2Deg % 360);
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
                    done = RotatingSpecialStep((int)LEG_GROUP.firstly, 1, changeRotateZ, 10);
                    stepRotate = done == true ? RotatingStep.rotating1 : stepRotate;
                    break;
                case RotatingStep.rotating1:
                    changeRotateZ = changeRotateZ + 0.5f;
                    done = RotatingSpecialStep((int)LEG_GROUP.secondly, 0.5f, A + (20 - A) * changeRotateZ / 20, 10);
                    stepRotate = done == true ? RotatingStep.rotating2 : stepRotate;
                    break;
                case RotatingStep.rotating2:
                    changeRotateZ = changeRotateZ + 0.5f;
                    done = RotatingSpecialStep((int)LEG_GROUP.firstly, 0.5f, A + (20 - A) * changeRotateZ / 20, 10);
                    stepRotate = done == true ? RotatingStep.rotating1 : stepRotate;
                    if (done == true)
                        stepRotate = contFlag == false ? RotatingStep.stop : stepRotate;
                    break;
                case RotatingStep.stop:
                    changeRotateZ = changeRotateZ + 0.5f;
                    done = RotatingSpecialStep((int)LEG_GROUP.secondly, 0.5f, A + (-A) * changeRotateZ / 10, 5);
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

                    MyVector3 difPos = Parameters.bodyLocalPosition - bufBodyLocalPosition;
                    MyVector3 difAngle = Parameters.bodyLocalEulerAngles - bufBodyLocalEulerAngles;

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
            foreach (var joint in legs)
            {
                joint.Update();
            }
        }
    }
}

