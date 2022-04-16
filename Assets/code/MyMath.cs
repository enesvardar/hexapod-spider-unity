using System;

namespace Assets.code
{
    class MyVector3
    {
        public float x;
        public float y;
        public float z;

        public MyVector3()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public MyVector3(float _x, float _y, float _z)
        {
            this.x = _x;
            this.y = _y;
            this.z = _z;
        }

        public static MyVector3 operator +(MyVector3 vec1, MyVector3 vec2)
        {
            MyVector3 result = new MyVector3();

            result.x = vec1.x + vec2.x;
            result.y = vec1.y + vec2.y;
            result.z = vec1.z + vec2.z;

            return result;
        }

        public static MyVector3 operator -(MyVector3 vec1, MyVector3 vec2)
        {
            MyVector3 result = new MyVector3();

            result.x = vec1.x - vec2.x;
            result.y = vec1.y - vec2.y;
            result.z = vec1.z - vec2.z;

            return result;
        }
    }
    class MyVector4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public MyVector4()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.w = 0;
        }

        public MyVector4(float _x, float _y, float _z, float _w)
        {
            this.x = _x;
            this.y = _y;
            this.z = _z;
            this.w = _w;
        }
    }
    class MyQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public MyQuaternion()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.w = 0;
        }

        public void EulertoQuaternion(MyVector3 vec)
        {
            float deg2rad = 0.0174532925f;

            float X = deg2rad * vec.x;
            float Y = deg2rad * vec.y;
            float Z = deg2rad * vec.z;

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

    class MyMatrix4x4
    {
        public float m00;
        public float m01;
        public float m02;
        public float m03;

        public float m10;
        public float m11;
        public float m12;
        public float m13;
               
        public float m20;
        public float m21;
        public float m22;
        public float m23;
               
        public float m30;
        public float m31;
        public float m32;
        public float m33;

        public MyMatrix4x4()
        {
            this.m00 = 0;
            this.m01 = 0;
            this.m02 = 0;
            this.m03 = 0;
                 
            this.m10 = 0;
            this.m11 = 0;
            this.m12 = 0;
            this.m13 = 0;
                   
            this.m20 = 0;
            this.m21 = 0;
            this.m22 = 0;
            this.m23 = 0;

            this.m30 = 0;
            this.m31 = 0;
            this.m32 = 0;
            this.m33 = 1;
        }

        public void Rotate(MyQuaternion vec)
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

        public void Inverse()
        {

            float det =
                this.m03 * this.m12 * this.m21 * this.m30 - this.m02 * this.m13 * this.m21 * this.m30 - this.m03 * this.m11 * this.m22 * this.m30 + this.m01 * this.m13 * this.m22 * this.m30 +
                this.m02 * this.m11 * this.m23 * this.m30 - this.m01 * this.m12 * this.m23 * this.m30 - this.m03 * this.m12 * this.m20 * this.m31 + this.m02 * this.m13 * this.m20 * this.m31 +
                this.m03 * this.m10 * this.m22 * this.m31 - this.m00 * this.m13 * this.m22 * this.m31 - this.m02 * this.m10 * this.m23 * this.m31 + this.m00 * this.m12 * this.m23 * this.m31 +
                this.m03 * this.m11 * this.m20 * this.m32 - this.m01 * this.m13 * this.m20 * this.m32 - this.m03 * this.m10 * this.m21 * this.m32 + this.m00 * this.m13 * this.m21 * this.m32 +
                this.m01 * this.m10 * this.m23 * this.m32 - this.m00 * this.m11 * this.m23 * this.m32 - this.m02 * this.m11 * this.m20 * this.m33 + this.m01 * this.m12 * this.m20 * this.m33 +
                this.m02 * this.m10 * this.m21 * this.m33 - this.m00 * this.m12 * this.m21 * this.m33 - this.m01 * this.m10 * this.m22 * this.m33 + this.m00 * this.m11 * this.m22 * this.m33;

            det = 1 / det;
                  
            float _m00 = det*(this.m12 * this.m23 * this.m31 - this.m13 * this.m22 * this.m31 + this.m13 * this.m21 * this.m32 - this.m11 * this.m23 * this.m32 - this.m12 * this.m21 * this.m33 + this.m11 * this.m22 * this.m33);
            float _m01 = det*(this.m03 * this.m22 * this.m31 - this.m02 * this.m23 * this.m31 - this.m03 * this.m21 * this.m32 + this.m01 * this.m23 * this.m32 + this.m02 * this.m21 * this.m33 - this.m01 * this.m22 * this.m33);
            float _m02 = det*(this.m02 * this.m13 * this.m31 - this.m03 * this.m12 * this.m31 + this.m03 * this.m11 * this.m32 - this.m01 * this.m13 * this.m32 - this.m02 * this.m11 * this.m33 + this.m01 * this.m12 * this.m33);
            float _m03 = det*(this.m03 * this.m12 * this.m21 - this.m02 * this.m13 * this.m21 - this.m03 * this.m11 * this.m22 + this.m01 * this.m13 * this.m22 + this.m02 * this.m11 * this.m23 - this.m01 * this.m12 * this.m23);
            float _m10 = det*(this.m13 * this.m22 * this.m30 - this.m12 * this.m23 * this.m30 - this.m13 * this.m20 * this.m32 + this.m10 * this.m23 * this.m32 + this.m12 * this.m20 * this.m33 - this.m10 * this.m22 * this.m33);
            float _m11 = det*(this.m02 * this.m23 * this.m30 - this.m03 * this.m22 * this.m30 + this.m03 * this.m20 * this.m32 - this.m00 * this.m23 * this.m32 - this.m02 * this.m20 * this.m33 + this.m00 * this.m22 * this.m33);
            float _m12 = det*(this.m03 * this.m12 * this.m30 - this.m02 * this.m13 * this.m30 - this.m03 * this.m10 * this.m32 + this.m00 * this.m13 * this.m32 + this.m02 * this.m10 * this.m33 - this.m00 * this.m12 * this.m33);
            float _m13 = det*(this.m02 * this.m13 * this.m20 - this.m03 * this.m12 * this.m20 + this.m03 * this.m10 * this.m22 - this.m00 * this.m13 * this.m22 - this.m02 * this.m10 * this.m23 + this.m00 * this.m12 * this.m23);
            float _m20 = det*(this.m11 * this.m23 * this.m30 - this.m13 * this.m21 * this.m30 + this.m13 * this.m20 * this.m31 - this.m10 * this.m23 * this.m31 - this.m11 * this.m20 * this.m33 + this.m10 * this.m21 * this.m33);
            float _m21 = det*(this.m03 * this.m21 * this.m30 - this.m01 * this.m23 * this.m30 - this.m03 * this.m20 * this.m31 + this.m00 * this.m23 * this.m31 + this.m01 * this.m20 * this.m33 - this.m00 * this.m21 * this.m33);
            float _m22 = det*(this.m01 * this.m13 * this.m30 - this.m03 * this.m11 * this.m30 + this.m03 * this.m10 * this.m31 - this.m00 * this.m13 * this.m31 - this.m01 * this.m10 * this.m33 + this.m00 * this.m11 * this.m33);
            float _m23 = det*(this.m03 * this.m11 * this.m20 - this.m01 * this.m13 * this.m20 - this.m03 * this.m10 * this.m21 + this.m00 * this.m13 * this.m21 + this.m01 * this.m10 * this.m23 - this.m00 * this.m11 * this.m23);
            float _m30 = det*(this.m12 * this.m21 * this.m30 - this.m11 * this.m22 * this.m30 - this.m12 * this.m20 * this.m31 + this.m10 * this.m22 * this.m31 + this.m11 * this.m20 * this.m32 - this.m10 * this.m21 * this.m32);
            float _m31 = det*(this.m01 * this.m22 * this.m30 - this.m02 * this.m21 * this.m30 + this.m02 * this.m20 * this.m31 - this.m00 * this.m22 * this.m31 - this.m01 * this.m20 * this.m32 + this.m00 * this.m21 * this.m32);
            float _m32 = det*(this.m02 * this.m11 * this.m30 - this.m01 * this.m12 * this.m30 - this.m02 * this.m10 * this.m31 + this.m00 * this.m12 * this.m31 + this.m01 * this.m10 * this.m32 - this.m00 * this.m11 * this.m32);
            float _m33 = det*(this.m01 * this.m12 * this.m20 - this.m02 * this.m11 * this.m20 + this.m02 * this.m10 * this.m21 - this.m00 * this.m12 * this.m21 - this.m01 * this.m10 * this.m22 + this.m00 * this.m11 * this.m22);
                     
            this.m00 = _m00 ;
            this.m01 = _m01 ;
            this.m02 = _m02 ;
            this.m03 = _m03 ;
            this.m10 = _m10 ;
            this.m11 = _m11 ;
            this.m12 = _m12 ;
            this.m13 = _m13 ;
            this.m20 = _m20 ;
            this.m21 = _m21 ;
            this.m22 = _m22 ;
            this.m23 = _m23 ;
            this.m30 = _m30 ;
            this.m31 = _m31 ;
            this.m32 = _m32 ;
            this.m33 = _m33;
        }

        public static MyVector4 operator *(MyMatrix4x4 matrix4x4, MyVector4 vec)
        {
            MyVector4 result = new MyVector4(0, 0, 0, 0);

            result.x = matrix4x4.m00 * vec.x + matrix4x4.m01 * vec.y + matrix4x4.m02 * vec.z + matrix4x4.m03 * vec.w;
            result.y = matrix4x4.m10 * vec.x + matrix4x4.m11 * vec.y + matrix4x4.m12 * vec.z + matrix4x4.m13 * vec.w;
            result.z = matrix4x4.m20 * vec.x + matrix4x4.m21 * vec.y + matrix4x4.m22 * vec.z + matrix4x4.m23 * vec.w;
            result.w = matrix4x4.m30 * vec.x + matrix4x4.m31 * vec.y + matrix4x4.m32 * vec.z + matrix4x4.m33 * vec.w;

            return result;
        }
    }
}
