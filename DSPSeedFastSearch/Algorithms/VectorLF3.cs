using System;
using System.Collections.Generic;
using System.Text;

namespace DSPSeedFastSearch.Algorithms
{
    public struct VectorLF3
    {
        public VectorLF3(VectorLF3 vec)
        {
            this.x = vec.x;
            this.y = vec.y;
            this.z = vec.z;
        }

        public VectorLF3(double x_, double y_, double z_)
        {
            this.x = x_;
            this.y = y_;
            this.z = z_;
        }

        public VectorLF3(float x_, float y_, float z_)
        {
            this.x = (double)x_;
            this.y = (double)y_;
            this.z = (double)z_;
        }

        public static VectorLF3 zero
        {
            get
            {
                return new VectorLF3(0f, 0f, 0f);
            }
        }

        public static VectorLF3 one
        {
            get
            {
                return new VectorLF3(1f, 1f, 1f);
            }
        }

        public static VectorLF3 minusone
        {
            get
            {
                return new VectorLF3(-1f, -1f, -1f);
            }
        }

        public static VectorLF3 unit_x
        {
            get
            {
                return new VectorLF3(1f, 0f, 0f);
            }
        }

        public static VectorLF3 unit_y
        {
            get
            {
                return new VectorLF3(0f, 1f, 0f);
            }
        }

        public static VectorLF3 unit_z
        {
            get
            {
                return new VectorLF3(0f, 0f, 1f);
            }
        }

        public static bool operator ==(VectorLF3 lhs, VectorLF3 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public static bool operator !=(VectorLF3 lhs, VectorLF3 rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
        }

        public static VectorLF3 operator *(VectorLF3 lhs, VectorLF3 rhs)
        {
            return new VectorLF3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        }

        public static VectorLF3 operator *(VectorLF3 lhs, double rhs)
        {
            return new VectorLF3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        }

        public static VectorLF3 operator /(VectorLF3 lhs, double rhs)
        {
            return new VectorLF3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
        }

        public static VectorLF3 operator -(VectorLF3 vec)
        {
            return new VectorLF3(-vec.x, -vec.y, -vec.z);
        }

        public static VectorLF3 operator -(VectorLF3 lhs, VectorLF3 rhs)
        {
            return new VectorLF3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static VectorLF3 operator +(VectorLF3 lhs, VectorLF3 rhs)
        {
            return new VectorLF3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }

        public static implicit operator VectorLF3(Vector3 vec3)
        {
            return new VectorLF3(vec3.x, vec3.y, vec3.z);
        }

        public static implicit operator Vector3(VectorLF3 vec3)
        {
            return new Vector3((float)vec3.x, (float)vec3.y, (float)vec3.z);
        }

        public double sqrMagnitude
        {
            get
            {
                return this.x * this.x + this.y * this.y + this.z * this.z;
            }
        }

        public double magnitude
        {
            get
            {
                return Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
            }
        }

        public double Distance(VectorLF3 vec)
        {
            return Math.Sqrt((vec.x - this.x) * (vec.x - this.x) + (vec.y - this.y) * (vec.y - this.y) + (vec.z - this.z) * (vec.z - this.z));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is VectorLF3)
            {
                VectorLF3 vectorLF = (VectorLF3)obj;
                return this.x == vectorLF.x && this.y == vectorLF.y && this.z == vectorLF.z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2}]", this.x, this.y, this.z);
        }

        public static double Dot(VectorLF3 a, VectorLF3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static VectorLF3 Cross(VectorLF3 a, VectorLF3 b)
        {
            return new VectorLF3(a.y * b.z - b.y * a.z, a.z * b.x - b.z * a.x, a.x * b.y - b.x * a.y);
        }

        public static double AngleRAD(VectorLF3 a, VectorLF3 b)
        {
            VectorLF3 normalized = a.normalized;
            VectorLF3 normalized2 = b.normalized;
            double num = normalized.x * normalized2.x + normalized.y * normalized2.y + normalized.z * normalized2.z;
            if (num > 1.0)
            {
                num = 1.0;
            }
            else if (num < -1.0)
            {
                num = -1.0;
            }
            return Math.Acos(num);
        }

        public static double AngleDEG(VectorLF3 a, VectorLF3 b)
        {
            VectorLF3 normalized = a.normalized;
            VectorLF3 normalized2 = b.normalized;
            double num = normalized.x * normalized2.x + normalized.y * normalized2.y + normalized.z * normalized2.z;
            if (num > 1.0)
            {
                num = 1.0;
            }
            else if (num < -1.0)
            {
                num = -1.0;
            }
            return Math.Acos(num) / 3.1415926535897931 * 180.0;
        }

        public VectorLF3 normalized
        {
            get
            {
                double num = this.x * this.x + this.y * this.y + this.z * this.z;
                if (num < 1E-34)
                {
                    return new VectorLF3(0f, 0f, 0f);
                }
                double num2 = Math.Sqrt(num);
                return new VectorLF3(this.x / num2, this.y / num2, this.z / num2);
            }
        }

        public double x;

        public double y;

        public double z;
    }
}
