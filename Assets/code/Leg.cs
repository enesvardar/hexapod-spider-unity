using System;
using UnityEngine;

namespace Assets.code
{
    enum LEG_NAME : int
    {
       leftBack = 0,
       leftMiddle = 1,
       leftFront = 2,
       rightBack = 3,
       rightMiddle = 4,
       rightFront = 5,
    }

    enum LEG_GROUP : int
    {
        firstly = 1,
        secondly = 2,
    }

    class Leg
    {
        public Transform alpha; // alpha açısının değiştirildiği eklem
        public Transform beta; // beta açısının değiştirildiği eklem
        public Transform gama; // gama açısının değiştirildiği eklem

        public MyVector3 legBaseFORG; // bacak tabanın orgindeki kordinat sistemine göre pozisyonu
        public MyVector3 legBaseFCCP; // bacak tabanın bacağın gövdeye bağlandığı yerdeki kordinat sistemine göre pozisyonu
        public MyVector3 legCCP; // bacağın gövdeye bağlandığı noktadaki kordinat sistemininin, gövdedeki kordinat sitemine göre pozisyonu
        public MyVector3 legLocalEulerAngles; // bacağın gövdeye bağlandığı noktadaki kordinat sistemininin, gövdedeki kordinat sitemine göre açısal konumu

        public float alphaAngleRad = 0; // alpha açısının radyan değeri
        public float betaAngleRad = 0; // beta açısının radyan değeri
        public float gamaAngleRad = 0; // gama açısının radyan değeri

        public string name; // her bir bacağın isminin tutulduğu değişken (leftBack,leftMiddle....rightFront)

        public Leg(GameObject leg, float _endOfset)
        {
            
            Transform[] allChildren = leg.GetComponentsInChildren<Transform>();

            // bacağın sahip olduğu objelerden açısal kontrollerin yapıldığı objeler bulunuyor
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

            // bacağın ismi okunuyor
            name = leg.name;

            // parametreden her bacağın parametreleri okuyor
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

            // her bir bacağın ayak tabanı _endOfset değerine göre bir ofset kadar kaydırlıyor

            float _endOfsetX = 0;
            float _endOfsetY = 0;

            if(Convert.ToInt32(legBaseFORG.x) > 0)
            {
                _endOfsetX = _endOfset * -1;
            }
            else
            {
                _endOfsetX = _endOfset;
            }

            if(Convert.ToInt32(legBaseFORG.y) < 0)
            {
                _endOfsetX /= 2;
                _endOfsetY = _endOfset/2;
            }
            else if(Convert.ToInt32(legBaseFORG.y) > 0)
            {
                _endOfsetX /= 2;
                _endOfsetY = (_endOfset / 2) * -1;
            }
            
            legBaseFORG = new MyVector3(legBaseFORG.x + _endOfsetX, legBaseFORG.y +_endOfsetY, legBaseFORG.z);

            İnverseKinematicsForLegBase();

            ForwardKinematicsForLegBase();
        }
        
        // Bu fonksiyon ile her bir bacak tabanın pozisyon bilgisi bacağın ana kordinat sistemine göre Q1,Q2 ve Q3 açıları ile güncellenmesini sağlar.
        public void ForwardKinematicsForLegBase()
        {
            float Q1 = alphaAngleRad;
            float Q2 = betaAngleRad;
            float Q3 = gamaAngleRad;

            float px = (float)(Mathf.Cos(Q1) * (Parameters.coxia + Parameters.femuarX * Mathf.Cos(Q2 + Q3) - 1.0 * Parameters.femuarH * Mathf.Sin(Q2 + Q3) + Parameters.tibiaX * Mathf.Cos(Q2) - Parameters.tibiaH * Mathf.Sin(Q2)));
            float py = (float)(Mathf.Sin(Q1) * (Parameters.coxia + Parameters.femuarX * Mathf.Cos(Q2 + Q3) - 1.0 * Parameters.femuarH * Mathf.Sin(Q2 + Q3) + Parameters.tibiaX * Mathf.Cos(Q2) - Parameters.tibiaH * Mathf.Sin(Q2)));
            float pz = -Parameters.femuarH * Mathf.Cos(Q2 + Q3) - Parameters.femuarX * Mathf.Sin(Q2 + Q3) - Parameters.tibiaH * Mathf.Cos(Q2) - Parameters.tibiaX * Mathf.Sin(Q2);

            legBaseFCCP = new MyVector3(px, py, pz);
        }

        // Bu fonksiyon ile bacağın ana koordinat sistemine göre bacak tabanın pozisyonu için gerekli  Q1,Q2 ve Q3 deperleri bulunur
        private void İnverseKinematicsForLegBase()
        {

            // ilk olarak dönme matrisi oluşturuluyor. Bacağın ana kordinat sisteminin orijine göre x , y ve z eksenlerinde dönme açıları hesaplanıyor
            MyQuaternion rotation = new MyQuaternion();
            rotation.EulertoQuaternion(new MyVector3(legLocalEulerAngles.x + Parameters.bodyLocalEulerAngles.x,
                    legLocalEulerAngles.y + Parameters.bodyLocalEulerAngles.y, legLocalEulerAngles.z + Parameters.bodyLocalEulerAngles.z));

            // birim dönüşüm matrisi oluşturuluyor
            MyMatrix4x4 T = new MyMatrix4x4();

            // dönüşüm matirisi oluşturulan dönme matrisi ile döndürülüyor
            T.Rotate(rotation);

            // bacağın ana kordinat sisteminin pozisyon değeri orijine göre okunuyor
            MyVector4 alphaPosForOrigin = GetAlphaPosForOrigin();

            // dönüşüm matrisine pozisyon değerleride girilerek, dönüşüm matrisi tamamlanıyor
            T.m03 = alphaPosForOrigin.x;
            T.m13 = alphaPosForOrigin.y;
            T.m23 = alphaPosForOrigin.z;

            /*
             *  bacağın ana eksenine M, origine O diyelim.   
             *  bacak tabanının origine göre pozisyonu Op, bacağın ana eksenine göre pozisyonuna Mp diyelim  
             *                                                                      O                                    
             *   Dolayısıyla T dönüşüm matrisi O nun M ye göre durumunu tutuar       T   dir   
             *                                                                      M  
             *           O 
             *   Op =     T * Mp dir.
             *           M 
             *           
             *   Biz Mp yi bulmak istediğimiz için dönüşüm matrisinin tersini alıyoruz.
             *   
             *   
             *           M 
             *   Mp =     T * Op dir.
             *           O 
             *   
             */

            T.Inverse();

            // bacağın ana eksenine göre pozisyonu aşağıdaki çarpım işlemi ile bulunuyor
            MyVector4 P = T * new MyVector4(legBaseFORG.x, legBaseFORG.y, legBaseFORG.z, 1.0f);

            // ters kinematik denklemleri kullanılarak alpha beta gama açı değerleri hesaplanıyor.
            // kinematik denklemlerin çıkartılma şekli dokumantosyan adlı dosyadaki ppt açıklandı. Hesaplamalarda D-H tablosu kullanılıyor
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

        // Bu fonkisyon ile bacağın ana ekseninin ( bacağın gövdeye bağlandığı yerdeki eksen) origine göre pozisyon bilgsi okunuyor
        private MyVector4 GetAlphaPosForOrigin()
        {
            // gövdenin origine göre dönüş matrisi oluşturuluyor
            MyQuaternion rotation = new MyQuaternion();
            rotation.EulertoQuaternion(new MyVector3(Parameters.bodyLocalEulerAngles.x, Parameters.bodyLocalEulerAngles.y, Parameters.bodyLocalEulerAngles.z));

            // birim dönüşüm matrisi oluşturuluyor
            MyMatrix4x4 T = new MyMatrix4x4();

            // döünüşüm matirisi dönme matrisi ile döndürülüyor
            T.Rotate(rotation);

            // dönüşüm matrisine robot gövdesinin origine göre konum değerleri giriliyor
            T.m03 = Parameters.bodyLocalPosition.x;
            T.m13 = Parameters.bodyLocalPosition.y;
            T.m23 = Parameters.bodyLocalPosition.z;

            // bacağın ana ekseninin pozisyon değeri gövde deki kordinat sistemi için hesaplanıyor     

            return T * new MyVector4(legCCP.x, legCCP.y, legCCP.z, 1.0f);
        }

        public void UpdateLegBaseFORG(float ofsetZ)
        {
            MyQuaternion rotation = new MyQuaternion();
            rotation.EulertoQuaternion(new MyVector3(legLocalEulerAngles.x + Parameters.bodyLocalEulerAngles.x, legLocalEulerAngles.y + Parameters.bodyLocalEulerAngles.y, legLocalEulerAngles.z + Parameters.bodyLocalEulerAngles.z + ofsetZ));

            MyMatrix4x4 T = new MyMatrix4x4();

            T.Rotate(rotation);

            MyVector4 alphaPosForOrigin = GetAlphaPosForOrigin();

            T.m03 = alphaPosForOrigin.x;
            T.m13 = alphaPosForOrigin.y;
            T.m23 = alphaPosForOrigin.z;

            MyVector4 trans = T * new MyVector4(legBaseFCCP.x, legBaseFCCP.y, legBaseFCCP.z, 1.0f);

            legBaseFORG = new MyVector3(trans.x, trans.y, legBaseFORG.z);
        }

        // Bu fonkisyon ile bacak tabanı pozisyonu ofset değerleri kadar hareket ettirilir.
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

            legBaseFORG = new MyVector3(legBaseFORG.x + endOfsetX, legBaseFORG.y + endOfsetY, legBaseFORG.z + _endOfsetZ);
        }

        // Bu fonksiyon ile bacak tabanı noktası istenilen yönlerde sahip olduğu birim hareket kabiliyeti kadar hareket eder.
        public void MoveDirLegBasePoint(Direction dir)
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

        // Bu fonksiyon ile alpha beta ve gama açıları ile ilgili objeler hareket ettirliyor
        public void Update()
        {
            // açı değerleri ters kinematik ile hesaplanıyor
            İnverseKinematicsForLegBase();

            float alphaAngleDeg = (alphaAngleRad * Mathf.Rad2Deg) % 360;
            float betaAngleDeg = -(betaAngleRad * Mathf.Rad2Deg) % 360;
            float gamaAngleDeg = -(gamaAngleRad * Mathf.Rad2Deg) % 360;

            if (alphaAngleDeg < -180)
            {
                alphaAngleDeg += 360;
            }
            else if (alphaAngleDeg > +180)
            {
                alphaAngleDeg -= 360;
            }

            if (betaAngleDeg < -180)
            {
                betaAngleDeg += 360;
            }
            else if (betaAngleDeg > +180)
            {
                betaAngleDeg -= 360;
            }

            if (betaAngleDeg > 80)
            {
                betaAngleDeg = 80;
            }

            if (gamaAngleDeg < -180)
            {
                gamaAngleDeg += 360;
            }
            else if (gamaAngleDeg > +180)
            {
                gamaAngleDeg -= 360;
            }

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
