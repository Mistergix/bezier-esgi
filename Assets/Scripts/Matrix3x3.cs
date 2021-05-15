using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Esgi.Bezier
{
    [Serializable]
    public class Matrix3x3
    {
        private Vector3 col1, col2, col3;

        [ShowInInspector]
        public Vector3 Row1 => new Vector3(col1.x, col2.x, col3.x);
        [ShowInInspector]
        public Vector3 Row2 => new Vector3(col1.y, col2.y, col3.y);
        [ShowInInspector]
        public Vector3 Row3 => new Vector3(col1.z, col2.z, col3.z);

        public Matrix3x3()
        {
            col1 = Vector3.zero;
            col2 = Vector3.zero;
            col3 = Vector3.zero;
        }

        public Matrix3x3(Vector3 vec1, Vector3 vec2, Vector3 vec3, bool isCol = true)
        {
            if (isCol)
            {
                SetCols(vec1, vec2, vec3);
            }
            else
            {
                SetRows(vec1, vec2, vec3);
            }
        }

        public void SetCols(Vector3 col1, Vector3 col2, Vector3 col3)
        {
            this.col1 = col1;
            this.col2 = col2;
            this.col3 = col3;
        }
        
        public void SetRows(Vector3 row1, Vector3 row2, Vector3 row3)
        {
            Vector3 col1 = new Vector3(row1.x, row2.x, row3.x);
            Vector3 col2 = new Vector3(row1.y, row2.y, row3.y);
            Vector3 col3 = new Vector3(row1.z, row2.z, row3.z);
            
            SetCols(col1, col2, col3);
        }

        public static Matrix3x3 TranslateMatrix(float dx, float dy)
        {
            return new Matrix3x3(new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(dx, dy, 1));
        }
        
        public static Matrix3x3 ScaleMatrix(float sx, float sy)
        {
            return new Matrix3x3(new Vector3(sx, 0, 0), new Vector3(0, sy, 0), new Vector3(0, 0, 1));
        }
        
        public static Matrix3x3 Identity()
        {
            return new Matrix3x3(new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1));
        }
        
        public static Matrix3x3 RotationZMatrix(float angleDegrees)
        {
            float rads = angleDegrees * Mathf.Deg2Rad;
            
            return new Matrix3x3(new Vector3(Mathf.Cos(rads), Mathf.Sin(rads), 0), new Vector3(-Mathf.Sin(rads), Mathf.Cos(rads), 0), new Vector3(0, 0, 1));
        }
        
        public static Matrix3x3 operator+ (Matrix3x3 a, Matrix3x3 b)
        {
            Vector3 col1 = new Vector3(a.col1.x + b.col1.x, a.col1.y + b.col1.y, a.col1.z + b.col1.z);
            Vector3 col2 = new Vector3(a.col2.x + b.col2.x, a.col2.y + b.col2.y, a.col2.z + a.col2.z);
            Vector3 col3 = new Vector3(a.col3.x + b.col3.x, a.col3.y + b.col3.y, a.col3.z + a.col3.z);

            return new Matrix3x3(col1, col2, col3);
        }
        
        public static Matrix3x3 operator* (Matrix3x3 a, Matrix3x3 b)
        {
            Vector3 row1 = new Vector3(a.col1.x * b.col1.x + a.col2.x * b.col1.y + a.col3.x * b.col1.z, a.col1.x * b.col2.x + a.col2.x * b.col2.y + a.col3.x * b.col2.z, a.col1.x * b.col3.x + a.col2.x * b.col3.y + a.col3.x * b.col3.z);
            Vector3 row2 = new Vector3(a.col1.y * b.col1.x + a.col2.y * b.col1.y + a.col3.y * b.col1.z, a.col1.y * b.col2.x + a.col2.y * b.col2.y + a.col3.y * b.col2.z, a.col1.y * b.col3.x + a.col2.y * b.col3.y + a.col3.y * b.col3.z);
            Vector3 row3 = new Vector3(a.col1.z * b.col1.x + a.col2.z * b.col1.y + a.col3.z * b.col1.z, a.col1.z * b.col2.x + a.col2.z * b.col2.y + a.col3.z * b.col2.z, a.col1.z * b.col3.x + a.col2.z * b.col3.y + a.col3.z * b.col3.z);

            return new Matrix3x3(row1, row2, row3, false);
        }
        
        public static Matrix3x3 operator* (Matrix3x3 a, float b)
        {
            Vector3 col1 = new Vector3(a.col1.x * b, a.col1.y * b, a.col1.z * b);
            Vector3 col2 = new Vector3(a.col2.x * b, a.col2.y * b, a.col2.z * b);
            Vector3 col3 = new Vector3(a.col3.x * b, a.col3.y * b, a.col3.z * b);

            return new Matrix3x3(col1, col2, col3);
        }

        public static Vector3 operator*(Matrix3x3 mat, Vector3 vec)
        {
            return new Vector3(mat.col1.x * vec.x + mat.col2.x * vec.y + mat.col3.x * vec.z, mat.col1.y * vec.x + mat.col2.y * vec.y + mat.col3.y * vec.z, mat.col1.z * vec.x + mat.col2.z * vec.y + mat.col3.z * vec.z);
        }
        
        public static Vector2 operator*(Matrix3x3 mat, Vector2 vec)
        {
            return mat * new Vector3(vec.x, vec.y, 1);
        }

        
    }
}
