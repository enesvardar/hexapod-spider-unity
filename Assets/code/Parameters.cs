using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.code
{
    class Parameters
    {

        public static float coxia = 53;
        public static float tibiaX = 70.8f;
        public static float tibiaH = 25;
        public static float femuarX = 0;
        public static float femuarH = 92.00085f;

        public static Vector3 bodyLocalEulerAngles = new Vector3(0, 0, 0);
        public static Vector3 bodyLocalPosition = new Vector3(0, 0, (femuarH + tibiaH)/2);

        public static float lenght = coxia + tibiaX + femuarX;

        public static Vector3 lbEulerAngles = new Vector3(0, 0, 120);// left back
        public static Vector3 lmEulerAngles = new Vector3(0, 0, 180);// left middle
        public static Vector3 lfEulerAngles = new Vector3(0, 0, 240);// left front
        public static Vector3 rbEulerAngles = new Vector3(0, 0, 60);// right back
        public static Vector3 rmEulerAngles = new Vector3(0, 0, 360);// right middle
        public static Vector3 rfEulerAngles = new Vector3(0, 0, 300);// right front

        // ContactCenterPoint => ContCntrPnt
        
        public static Vector3 lbContCntrPnt = new Vector3(-68, 120.0f, 0);
        public static Vector3 lmContCntrPnt = new Vector3(-140, 0, 0);
        public static Vector3 lfContCntrPnt = new Vector3(-68, -120.0f, 0);
        public static Vector3 rbContCntrPnt = new Vector3(68, 120.0f, 0);
        public static Vector3 rmContCntrPnt = new Vector3(140, 0, 0);
        public static Vector3 rfContCntrPnt = new Vector3(68, -120.0f, 0);

        // For Orgin
        public static Vector3 lbLegBaseFORG = lbContCntrPnt + new Vector3(Mathf.Cos(Mathf.PI * lbEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * lbEulerAngles.z / 180) * lenght, 0);
        public static Vector3 lmLegBaseFORG = lmContCntrPnt + new Vector3(Mathf.Cos(Mathf.PI * lmEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * lmEulerAngles.z / 180) * lenght, 0);
        public static Vector3 lfLegBaseFORG = lfContCntrPnt + new Vector3(Mathf.Cos(Mathf.PI * lfEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * lfEulerAngles.z / 180) * lenght, 0);
        public static Vector3 rbLegBaseFORG = rbContCntrPnt + new Vector3(Mathf.Cos(Mathf.PI * rbEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * rbEulerAngles.z / 180) * lenght, 0);
        public static Vector3 rmLegBaseFORG = rmContCntrPnt + new Vector3(Mathf.Cos(Mathf.PI * rmEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * rmEulerAngles.z / 180) * lenght, 0);
        public static Vector3 rfLegBaseFORG = rfContCntrPnt + new Vector3(Mathf.Cos(Mathf.PI * rfEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * rfEulerAngles.z / 180) * lenght, 0);

    }
}
