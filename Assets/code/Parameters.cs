using UnityEngine;

namespace Assets.code
{
    class Parameters
    {

        public static float coxia = 53;
        public static float tibiaX = 70.8f;
        public static float tibiaH = 25;
        public static float femuarX = 0;
        public static float femuarH = 92.00085f;

        public static float endOfset = -10;

        public static MyVector3 bodyLocalEulerAngles = new MyVector3(0, 0, 0);
        public static MyVector3 bodyLocalPosition = new MyVector3(0, 0, (femuarH + tibiaH)/2);

        public static float lenght = coxia + tibiaX + femuarX;

        public static MyVector3 lbEulerAngles = new MyVector3(0, 0, 120);// left back
        public static MyVector3 lmEulerAngles = new MyVector3(0, 0, 180);// left middle
        public static MyVector3 lfEulerAngles = new MyVector3(0, 0, 240);// left front
        public static MyVector3 rbEulerAngles = new MyVector3(0, 0, 60); // right back
        public static MyVector3 rmEulerAngles = new MyVector3(0, 0, 360);// right middle
        public static MyVector3 rfEulerAngles = new MyVector3(0, 0, 300);// right front

        // ContactCenterPoint => ContCntrPnt
        
        public static MyVector3 lbContCntrPnt = new MyVector3(-68, 120.0f, 0);
        public static MyVector3 lmContCntrPnt = new MyVector3(-140, 0, 0);
        public static MyVector3 lfContCntrPnt = new MyVector3(-68, -120.0f, 0);
        public static MyVector3 rbContCntrPnt = new MyVector3(68, 120.0f, 0);
        public static MyVector3 rmContCntrPnt = new MyVector3(140, 0, 0);
        public static MyVector3 rfContCntrPnt = new MyVector3(68, -120.0f, 0);

        // For Orgin
        public static MyVector3 lbLegBaseFORG = lbContCntrPnt + new MyVector3(Mathf.Cos(Mathf.PI * lbEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * lbEulerAngles.z / 180) * lenght, 0);
        public static MyVector3 lmLegBaseFORG = lmContCntrPnt + new MyVector3(Mathf.Cos(Mathf.PI * lmEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * lmEulerAngles.z / 180) * lenght, 0);
        public static MyVector3 lfLegBaseFORG = lfContCntrPnt + new MyVector3(Mathf.Cos(Mathf.PI * lfEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * lfEulerAngles.z / 180) * lenght, 0);
        public static MyVector3 rbLegBaseFORG = rbContCntrPnt + new MyVector3(Mathf.Cos(Mathf.PI * rbEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * rbEulerAngles.z / 180) * lenght, 0);
        public static MyVector3 rmLegBaseFORG = rmContCntrPnt + new MyVector3(Mathf.Cos(Mathf.PI * rmEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * rmEulerAngles.z / 180) * lenght, 0);
        public static MyVector3 rfLegBaseFORG = rfContCntrPnt + new MyVector3(Mathf.Cos(Mathf.PI * rfEulerAngles.z / 180) * lenght, Mathf.Sin(Mathf.PI * rfEulerAngles.z / 180) * lenght, 0);

    }
}
