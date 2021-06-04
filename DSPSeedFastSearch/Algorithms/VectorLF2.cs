using System;
using System.Collections.Generic;
using System.Text;

namespace DSPSeedFastSearch.Algorithms
{
    public struct VectorLF2
    {
        public VectorLF2(VectorLF2 vec)
        {
            this.x = vec.x;
            this.y = vec.y;
        }

        public VectorLF2(double x_, double y_)
        {
            this.x = x_;
            this.y = y_;
        }

        public VectorLF2(float x_, float y_)
        {
            this.x = (double)x_;
            this.y = (double)y_;
        }

        public static VectorLF2 zero
        {
            get
            {
                return new VectorLF2(0f, 0f);
            }
        }

        public static VectorLF2 one
        {
            get
            {
                return new VectorLF2(1f, 1f);
            }
        }

        public static VectorLF2 minusone
        {
            get
            {
                return new VectorLF2(-1f, -1f);
            }
        }

        public static VectorLF2 unit_x
        {
            get
            {
                return new VectorLF2(1f, 0f);
            }
        }

        public static VectorLF2 unit_y
        {
            get
            {
                return new VectorLF2(0f, 1f);
            }
        }

        public static bool operator ==(VectorLF2 lhs, VectorLF2 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(VectorLF2 lhs, VectorLF2 rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y;
        }

        public static VectorLF2 operator *(VectorLF2 lhs, VectorLF2 rhs)
        {
            return new VectorLF2(lhs.x * rhs.x, lhs.y * rhs.y);
        }

        public static VectorLF2 operator *(VectorLF2 lhs, double rhs)
        {
            return new VectorLF2(lhs.x * rhs, lhs.y * rhs);
        }

        public static VectorLF2 operator /(VectorLF2 lhs, double rhs)
        {
            return new VectorLF2(lhs.x / rhs, lhs.y / rhs);
        }

        public static VectorLF2 operator -(VectorLF2 vec)
        {
            return new VectorLF2(-vec.x, -vec.y);
        }

        public static VectorLF2 operator -(VectorLF2 lhs, VectorLF2 rhs)
        {
            return new VectorLF2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static VectorLF2 operator +(VectorLF2 lhs, VectorLF2 rhs)
        {
            return new VectorLF2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static implicit operator VectorLF2(Vector2 vec2)
        {
            return new VectorLF2(vec2.x, vec2.y);
        }

        public static implicit operator Vector2(VectorLF2 vec2)
        {
            return new Vector2((float)vec2.x, (float)vec2.y);
        }

        public double sqrMagnitude
        {
            get
            {
                return this.x * this.x + this.y * this.y;
            }
        }

        public double magnitude
        {
            get
            {
                return Math.Sqrt(this.x * this.x + this.y * this.y);
            }
        }

        public double Distance(VectorLF2 vec)
        {
            return Math.Sqrt((vec.x - this.x) * (vec.x - this.x) + (vec.y - this.y) * (vec.y - this.y));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is VectorLF2)
            {
                VectorLF2 vectorLF = (VectorLF2)obj;
                return this.x == vectorLF.x && this.y == vectorLF.y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", this.x, this.y);
        }

        public VectorLF2 normalized
        {
            get
            {
                double num = this.x * this.x + this.y * this.y;
                if (num < 1E-34)
                {
                    return new VectorLF2(0f, 0f);
                }
                double num2 = Math.Sqrt(num);
                return new VectorLF2(this.x / num2, this.y / num2);
            }
        }

        public double x;

        public double y;
    }
}
