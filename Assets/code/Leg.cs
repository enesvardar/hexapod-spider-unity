using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.code
{
    enum Legs : int
    {
       leftBack = 0,
       leftMiddle = 1,
       leftFront = 2,
       rightBack = 3,
       rightMiddle = 4,
       rightFront = 5,
    }

    class Leg
    {
        public string name;
        public Transform alpha;
        public Transform beta;
        public Transform gama;

        public Vector3 legBaseFORG;
        public Vector3 legBaseFCCP;
        public Vector3 legCCP;
        public Vector3 legLocalEulerAngles;

        public float alphaAngleRad = 0;
        public float betaAngleRad = 0;
        public float gamaAngleRad = 0;

        public Leg(GameObject leg, float _endOfset)
        {
            
            Transform[] allChildren = leg.GetComponentsInChildren<Transform>();

            foreach (Transform child in allChildren)
            {
                switch (child.name)
                {
                    case "alpha":
                        this.alpha = child;
                        break;
                    case "beta":
                        this.beta = child;
                        break;
                    case "gama":
                        this.gama = child;
                        break;

                    default:
                        break;
                }
            }

            name = leg.name;

            switch (name)
            {
                
                case "leftBack":
                    legLocalEulerAngles = Parameters.lbEulerAngles;
                    legBaseFORG = Parameters.lbLegBaseFORG;
                    legCCP = Parameters.lbContCntrPnt;
                    break;
                case "leftMiddle":
                    legLocalEulerAngles = Parameters.lmEulerAngles;
                    legBaseFORG = Parameters.lmLegBaseFORG;
                    legCCP = Parameters.lmContCntrPnt;
                    break;
                case "leftFront":
                    legLocalEulerAngles = Parameters.lfEulerAngles;
                    legBaseFORG = Parameters.lfLegBaseFORG;
                    legCCP = Parameters.lfContCntrPnt;
                    break;
                case "rightBack":
                    legLocalEulerAngles = Parameters.rbEulerAngles;
                    legBaseFORG = Parameters.rbLegBaseFORG;
                    legCCP = Parameters.rbContCntrPnt;
                    break;
                case "rightMiddle":
                    legLocalEulerAngles = Parameters.rmEulerAngles;
                    legBaseFORG = Parameters.rmLegBaseFORG;
                    legCCP = Parameters.rmContCntrPnt;
                    break;
                case "rightFront":
                    legLocalEulerAngles = Parameters.rfEulerAngles;
                    legBaseFORG = Parameters.rfLegBaseFORG;
                    legCCP = Parameters.rfContCntrPnt;
                    break;

                default:
                    break;
            }

            float _endOfsetX = 0;
            float _endOfsetY = 0;

            if (Convert.ToInt32(legBaseFORG.x) > 0)
            {
                _endOfsetX = _endOfset * -1;
            }
            else
            {
                _endOfsetX = _endOfset;
            }

            if(Convert.ToInt32(legBaseFORG.y) < 0)
            {
                _endOfsetX = _endOfsetX/2;
                _endOfsetY = _endOfset/2;
            }
            else if(Convert.ToInt32(legBaseFORG.y) > 0)
            {
                _endOfsetX = _endOfsetX / 2;
                _endOfsetY = (_endOfset / 2) * -1;
            }
            
            legBaseFORG = new Vector3(legBaseFORG.x + _endOfsetX, legBaseFORG.y +_endOfsetY, legBaseFORG.z);

            İnverseKinematicsForEndJoint();

            UpdateLegBaseFCCP();
        }

        public void MoveLegBasePoint(float _endOfsetX, float _endOfsetY, float _endOfsetZ)
        {
            float endOfsetX = 0;
            float endOfsetY = 0;

            if (_endOfsetY != 0)
            {
                endOfsetX = -Mathf.Sin(Mathf.PI * Parameters.bodyLocalEulerAngles.z / 180) * _endOfsetY;

                endOfsetY = Mathf.Cos(Mathf.PI * Parameters.bodyLocalEulerAngles.z / 180) * _endOfsetY;
            }
            else if (_endOfsetX != 0)
            {

                endOfsetX = Mathf.Cos(Mathf.PI * Parameters.bodyLocalEulerAngles.z / 180) * _endOfsetX;

                endOfsetY = Mathf.Sin(Mathf.PI * Parameters.bodyLocalEulerAngles.z / 180) * _endOfsetX;
            }


            legBaseFORG = new Vector3(legBaseFORG.x + endOfsetX, legBaseFORG.y + endOfsetY, legBaseFORG.z + _endOfsetZ);
        }

        public void MoveLeg(Direction dir)
        {
            switch (dir)
            {
                case Direction.forward:
                    MoveLegBasePoint(0, 1, 0);
                    break;
                case Direction.back:
                    MoveLegBasePoint(0, -1, 0);
                    break;
                case Direction.up:
                    MoveLegBasePoint(0, 0, 0.2f);
                    break;
                case Direction.down:
                    MoveLegBasePoint(0, 0, -0.2f);
                    break;
                case Direction.left:
                    MoveLegBasePoint(-1, 0, 0);
                    break;
                case Direction.right:
                    MoveLegBasePoint(+1, 0, 0);
                    break;
                case Direction.none:
                    break;
                default:
                    break;
            }
        }

        public void UpdateLegBaseFORG(float ofsetZ)
        {
            Quaternion rotation = Quaternion.Euler(legLocalEulerAngles.x + Parameters.bodyLocalEulerAngles.x,
                legLocalEulerAngles.y + Parameters.bodyLocalEulerAngles.y, legLocalEulerAngles.z + Parameters.bodyLocalEulerAngles.z + ofsetZ);

            Matrix4x4 T = Matrix4x4.Rotate(rotation);

            Vector3 alphaPosForOrigin = GetAlphaPosForOrigin();
        
            T.m03 = alphaPosForOrigin.x;
            T.m13 = alphaPosForOrigin.y;
            T.m23 = alphaPosForOrigin.z;

            Vector4 temp = new Vector4();

            temp = T * new Vector4(legBaseFCCP.x, legBaseFCCP.y, legBaseFCCP.z, 1.0f);

            legBaseFORG = new Vector3(temp.x, temp.y, legBaseFORG.z);
        }

        public void UpdateLegBaseFCCP()
        {
            float Q1 = alphaAngleRad;
            float Q2 = betaAngleRad;
            float Q3 = gamaAngleRad;

            float px = (float)(Mathf.Cos(Q1) * (Parameters.coxia + Parameters.femuarX * Mathf.Cos(Q2 + Q3) - 1.0 * Parameters.femuarH * Mathf.Sin(Q2 + Q3) + Parameters.tibiaX * Mathf.Cos(Q2) - Parameters.tibiaH * Mathf.Sin(Q2)));
            float py = (float)(Mathf.Sin(Q1) * (Parameters.coxia + Parameters.femuarX * Mathf.Cos(Q2 + Q3) - 1.0 * Parameters.femuarH * Mathf.Sin(Q2 + Q3) + Parameters.tibiaX * Mathf.Cos(Q2) - Parameters.tibiaH * Mathf.Sin(Q2)));
            float pz = -Parameters.femuarH * Mathf.Cos(Q2 + Q3) - Parameters.femuarX * Mathf.Sin(Q2 + Q3) - Parameters.tibiaH * Mathf.Cos(Q2) - Parameters.tibiaX * Mathf.Sin(Q2);

            legBaseFCCP = new Vector3(px, py, pz);
        }

        private Vector3 GetAlphaPosForOrigin()
        {

            Vector3 rot = new Vector3(Parameters.bodyLocalEulerAngles.x, Parameters.bodyLocalEulerAngles.y, Parameters.bodyLocalEulerAngles.z);

            Quaternion rotation = Quaternion.Euler(rot);

            Matrix4x4 T = Matrix4x4.Rotate(rotation);

            T.m03 = Parameters.bodyLocalPosition.x;
            T.m13 = Parameters.bodyLocalPosition.y;
            T.m23 = Parameters.bodyLocalPosition.z;

            return T * new Vector4(legCCP.x, legCCP.y, legCCP.z, 1.0f);
        }

        private void İnverseKinematicsForEndJoint()
        {
            Quaternion rotation = Quaternion.Euler(legLocalEulerAngles.x + Parameters.bodyLocalEulerAngles.x,
                legLocalEulerAngles.y + Parameters.bodyLocalEulerAngles.y, legLocalEulerAngles.z + Parameters.bodyLocalEulerAngles.z);


            Matrix4x4 T = Matrix4x4.Rotate(rotation);

            Vector3 alphaPosForOrigin = GetAlphaPosForOrigin();

            T.m03 = alphaPosForOrigin.x;
            T.m13 = alphaPosForOrigin.y;
            T.m23 = alphaPosForOrigin.z;

            T = T.inverse;

            Vector4 P = T * new Vector4(legBaseFORG.x, legBaseFORG.y, legBaseFORG.z, 1.0f);

            alphaAngleRad = Mathf.Atan2(P.y, P.x);

            float K = P.x * Mathf.Cos(alphaAngleRad) - Parameters.coxia + P.y * Mathf.Sin(alphaAngleRad);

            float a = (2 * K * Parameters.tibiaH + 2 * P.z * Parameters.tibiaX);
            float b = (-2 * K * Parameters.tibiaX + 2 * P.z * Parameters.tibiaH); ;
            float c = Parameters.femuarH * Parameters.femuarH + Parameters.femuarX * Parameters.femuarX - K * K - P.z * P.z - Parameters.tibiaH * Parameters.tibiaH - Parameters.tibiaX * Parameters.tibiaX;

            betaAngleRad = Mathf.Atan2(a, b) + Mathf.Atan2(Mathf.Sqrt(a * a + b * b - c * c), c);

            a = Parameters.femuarX;
            b = Parameters.femuarH;
            c = Parameters.coxia * Mathf.Sin(betaAngleRad) - P.z * Mathf.Cos(betaAngleRad) - P.x * Mathf.Cos(alphaAngleRad) * Mathf.Sin(betaAngleRad) - P.y * Mathf.Sin(alphaAngleRad) * Mathf.Sin(betaAngleRad) - Parameters.tibiaH;

            float[] gamaAngleX = { 0, 0 };

            gamaAngleX[0] = Mathf.Atan2(a, b) + Mathf.Atan2(Mathf.Sqrt(a * a + b * b - c * c), c);
            gamaAngleX[1] = Mathf.Atan2(a, b) - Mathf.Atan2(Mathf.Sqrt(a * a + b * b - c * c), c);

            float[] dif = { 0, 0 };

            for (int i = 0; i < 2; i++)
            {

                float px = Mathf.Cos(alphaAngleRad) * (Parameters.coxia + Parameters.femuarX * Mathf.Cos(betaAngleRad + gamaAngleX[i]) - Parameters.femuarH * Mathf.Sin(betaAngleRad + gamaAngleX[i]) + Parameters.tibiaX * Mathf.Cos(betaAngleRad) - Parameters.tibiaH * Mathf.Sin(betaAngleRad));
                float py = Mathf.Sin(alphaAngleRad) * (Parameters.coxia + Parameters.femuarX * Mathf.Cos(betaAngleRad + gamaAngleX[i]) - Parameters.femuarH * Mathf.Sin(betaAngleRad + gamaAngleX[i]) + Parameters.tibiaX * Mathf.Cos(betaAngleRad) - Parameters.tibiaH * Mathf.Sin(betaAngleRad));
                float pz = -Parameters.femuarH * Mathf.Cos(betaAngleRad + gamaAngleX[i]) - Parameters.femuarX * Mathf.Sin(betaAngleRad + gamaAngleX[i]) - Parameters.tibiaH * Mathf.Cos(betaAngleRad) - Parameters.tibiaX * Mathf.Sin(betaAngleRad);

                dif[i] = Mathf.Sqrt((px - P.x) * (px - P.x) + (py - P.y) * (py - P.y) + (pz - P.z) * (pz - P.z));
            }

            if (dif[0] <= dif[1])
            {
                gamaAngleRad = gamaAngleX[0];
            }
            else
            {
                gamaAngleRad = gamaAngleX[1];
            }
        }

        public void Update()
        {
            İnverseKinematicsForEndJoint();

            float alphaAngleDeg = (alphaAngleRad * Mathf.Rad2Deg) % 360;
            float betaAngleDeg = -(betaAngleRad * Mathf.Rad2Deg) % 360;
            float gamaAngleDeg = -(gamaAngleRad * Mathf.Rad2Deg) % 360;

            if (alphaAngleDeg >= -360 && 360 > alphaAngleDeg &&
                betaAngleDeg >= -360 && 360 > betaAngleDeg &&
                gamaAngleDeg >= -360 && 360 > gamaAngleDeg)
            {
                // Dampen towards the target rotation
                alpha.localRotation = Quaternion.Lerp(alpha.localRotation, Quaternion.Euler(0, 0, alphaAngleDeg), Time.deltaTime * 100);
                beta.localRotation = Quaternion.Lerp(beta.localRotation, Quaternion.Euler(new Vector3(90, 0, betaAngleDeg)), Time.deltaTime * 100);
                gama.localRotation = Quaternion.Lerp(gama.localRotation, Quaternion.Euler(new Vector3(0, 0, gamaAngleDeg)), Time.deltaTime * 100);
            }
        }
    }
}
