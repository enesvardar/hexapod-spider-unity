using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.code
{
    class MyVector3
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;
    }

    class MyQuaternion
    {
        public float w = 0;
        public float x = 0;
        public float y = 0;
        public float z = 0;

        public void EulertoQuaternion(MyVector3 vec)
        {
            float deg2rad = 0.0174532925f;

            float X = deg2rad* vec.x; 
            float Y = deg2rad* vec.y; 
            float Z = deg2rad* vec.z;

            float cy = (float)Math.Cos(Z * 0.5f);
            float sy = (float)Math.Sin(Z * 0.5f);
            float cp = (float)Math.Cos(X * 0.5f);
            float sp = (float)Math.Sin(X * 0.5f);
            float cr = (float)Math.Cos(Y * 0.5f);
            float sr = (float)Math.Sin(Y * 0.5f);

            this.w = cr * cp * cy + sr * sp * sy;
            this.x = cr * sp * cy + sr * cp * sy;
            this.y = sr * cp * cy - cr * sp * sy;
            this.z = cr * cp * sy - sr * sp * cy;

        }
    }

    class MyRotationMatrix
    {
        public float m00 = 0;
        public float m01 = 0;
        public float m02 = 0;
        public float m03 = 0;

        public float m10 = 0;
        public float m11 = 0;
        public float m12 = 0;
        public float m13 = 0;

        public float m20 = 0;
        public float m21 = 0;
        public float m22 = 0;
        public float m23 = 0;

        public void CreateRotation(MyQuaternion vec)
        {
            this.m00 = 1 - 2 * vec.y * vec.y - 2 * vec.z * vec.z;
            this.m01 = 2 * vec.x * vec.y - 2 * vec.z * vec.w;
            this.m02 = 2 * vec.x * vec.z + 2 * vec.y * vec.w;
            this.m03 = 0;
            
            this.m10 = 2 * vec.x * vec.y + 2 * vec.z * vec.w;
            this.m11 = 1 - 2 * vec.x * vec.x - 2 * vec.z * vec.z;
            this.m12 = 2 * vec.y * vec.z - 2 * vec.x * vec.w;
            this.m13 = 0;
            
            this.m20 = 2 * vec.x * vec.z - 2 * vec.y * vec.w;
            this.m21 = 2 * vec.y * vec.z + 2 * vec.x * vec.w;
            this.m22 = 1 - 2 * vec.x * vec.x - 2 * vec.y * vec.y;
            this.m23 = 0;

        }
    }


}
