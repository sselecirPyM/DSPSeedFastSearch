using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPSeedFastSearch.Algorithms
{
    public struct Quaternion : IEquatable<Quaternion>
    {
        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
        {
            fromDirection = Vector3.Normalize(fromDirection);
            toDirection = Vector3.Normalize(toDirection);
            Vector3 v=Vector3.Normalize( Vector3.Cross(fromDirection, toDirection));
            return Quaternion.AngleAxis(Mathf.Acos(Vector3.Dot(fromDirection, toDirection)), v);
        }

        public static Quaternion Inverse(Quaternion rotation)
        {
            return System.Numerics.Quaternion.Inverse(rotation);
        }

        public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
        {
            return System.Numerics.Quaternion.Slerp(a, b, t);
        }

        //public static Quaternion SlerpUnclamped(Quaternion a, Quaternion b, float t)
        //{
        //    Quaternion result;
        //    Quaternion.SlerpUnclamped_Injected(ref a, ref b, t, out result);
        //    return result;
        //}

        //public static Quaternion Lerp(Quaternion a, Quaternion b, float t)
        //{
        //    Quaternion result;
        //    Quaternion.Lerp_Injected(ref a, ref b, t, out result);
        //    return result;
        //}

        //public static Quaternion LerpUnclamped(Quaternion a, Quaternion b, float t)
        //{
        //    Quaternion result;
        //    Quaternion.LerpUnclamped_Injected(ref a, ref b, t, out result);
        //    return result;
        //}

        //private static Quaternion Internal_FromEulerRad(Vector3 euler)
        //{
        //    Quaternion result;
        //    Quaternion.Internal_FromEulerRad_Injected(ref euler, out result);
        //    return result;
        //}

        //private static Vector3 Internal_ToEulerRad(Quaternion rotation)
        //{
        //    Vector3 result;
        //    Quaternion.Internal_ToEulerRad_Injected(ref rotation, out result);
        //    return result;
        //}

        //private static void Internal_ToAxisAngleRad(Quaternion q, out Vector3 axis, out float angle)
        //{
        //    Quaternion.Internal_ToAxisAngleRad_Injected(ref q, out axis, out angle);
        //}

        public static Quaternion AngleAxis(float angle, Vector3 axis)
        {
            return System.Numerics.Quaternion.CreateFromAxisAngle(new System.Numerics.Vector3(axis.x, axis.y, axis.z), angle);
        }

        //public static Quaternion LookRotation(Vector3 forward, Vector3 upwards)
        //{
        //    Quaternion result;
        //    Quaternion.LookRotation_Injected(ref forward, ref upwards, out result);
        //    return result;
        //}

        //public static Quaternion LookRotation(Vector3 forward)
        //{
        //    return Quaternion.LookRotation(forward, Vector3.up);
        //}

        public float this[int index]
        {
            get
            {
                float result;
                switch (index)
                {
                    case 0:
                        result = this.x;
                        break;
                    case 1:
                        result = this.y;
                        break;
                    case 2:
                        result = this.z;
                        break;
                    case 3:
                        result = this.w;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
                return result;
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    case 3:
                        this.w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
        }

        public void Set(float newX, float newY, float newZ, float newW)
        {
            this.x = newX;
            this.y = newY;
            this.z = newZ;
            this.w = newW;
        }

        public static Quaternion identity
        {
            get
            {
                return Quaternion.identityQuaternion;
            }
        }

        public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
        {
            return new Quaternion(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y, lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z, lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x, lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }

        public static Vector3 operator *(Quaternion rotation, Vector3 point)
        {
            float num = rotation.x * 2f;
            float num2 = rotation.y * 2f;
            float num3 = rotation.z * 2f;
            float num4 = rotation.x * num;
            float num5 = rotation.y * num2;
            float num6 = rotation.z * num3;
            float num7 = rotation.x * num2;
            float num8 = rotation.x * num3;
            float num9 = rotation.y * num3;
            float num10 = rotation.w * num;
            float num11 = rotation.w * num2;
            float num12 = rotation.w * num3;
            Vector3 result;
            result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
            result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
            result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
            return result;
        }

        private static bool IsEqualUsingDot(float dot)
        {
            return dot > 0.999999f;
        }

        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            return Quaternion.IsEqualUsingDot(Quaternion.Dot(lhs, rhs));
        }

        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            return !(lhs == rhs);
        }

        public static float Dot(Quaternion a, Quaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        //public void SetLookRotation(Vector3 view)
        //{
        //    Vector3 up = Vector3.up;
        //    this.SetLookRotation(view, up);
        //}

        //public void SetLookRotation(Vector3 view, Vector3 up)
        //{
        //    this = Quaternion.LookRotation(view, up);
        //}

        public static float Angle(Quaternion a, Quaternion b)
        {
            float num = Quaternion.Dot(a, b);
            return (!Quaternion.IsEqualUsingDot(num)) ? (Mathf.Acos(Mathf.Min(Mathf.Abs(num), 1f)) * 2f * 57.29578f) : 0f;
        }

        private static Vector3 Internal_MakePositive(Vector3 euler)
        {
            float num = -0.005729578f;
            float num2 = 360f + num;
            if (euler.x < num)
            {
                euler.x += 360f;
            }
            else if (euler.x > num2)
            {
                euler.x -= 360f;
            }
            if (euler.y < num)
            {
                euler.y += 360f;
            }
            else if (euler.y > num2)
            {
                euler.y -= 360f;
            }
            if (euler.z < num)
            {
                euler.z += 360f;
            }
            else if (euler.z > num2)
            {
                euler.z -= 360f;
            }
            return euler;
        }

        //public Vector3 eulerAngles
        //{
        //    get
        //    {
        //        return Quaternion.Internal_MakePositive(Quaternion.Internal_ToEulerRad(this) * 57.29578f);
        //    }
        //    set
        //    {
        //        this = Quaternion.Internal_FromEulerRad(value * 0.0174532924f);
        //    }
        //}

        //public static Quaternion Euler(float x, float y, float z)
        //{
        //    return Quaternion.Internal_FromEulerRad(new Vector3(x, y, z) * 0.0174532924f);
        //}

        //public static Quaternion Euler(Vector3 euler)
        //{
        //    return Quaternion.Internal_FromEulerRad(euler * 0.0174532924f);
        //}

        //public void ToAngleAxis(out float angle, out Vector3 axis)
        //{
        //    Quaternion.Internal_ToAxisAngleRad(this, out axis, out angle);
        //    angle *= 57.29578f;
        //}

        //public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection)
        //{
        //    this = Quaternion.FromToRotation(fromDirection, toDirection);
        //}

        //public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
        //{
        //    float num = Quaternion.Angle(from, to);
        //    Quaternion result;
        //    if (num == 0f)
        //    {
        //        result = to;
        //    }
        //    else
        //    {
        //        result = Quaternion.SlerpUnclamped(from, to, Mathf.Min(1f, maxDegreesDelta / num));
        //    }
        //    return result;
        //}

        public static Quaternion Normalize(Quaternion q)
        {
            float num = Mathf.Sqrt(Quaternion.Dot(q, q));
            Quaternion result;
            if (num < 1e-6)
            {
                result = Quaternion.identity;
            }
            else
            {
                result = new Quaternion(q.x / num, q.y / num, q.z / num, q.w / num);
            }
            return result;
        }

        public void Normalize()
        {
            this = Quaternion.Normalize(this);
        }

        public Quaternion normalized
        {
            get
            {
                return Quaternion.Normalize(this);
            }
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
        }

        public override bool Equals(object other)
        {
            return other is Quaternion && this.Equals((Quaternion)other);
        }

        public bool Equals(Quaternion other)
        {
            return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);
        }

        //public static Quaternion EulerRotation(float x, float y, float z)
        //{
        //    return Quaternion.Internal_FromEulerRad(new Vector3(x, y, z));
        //}

        //public static Quaternion EulerRotation(Vector3 euler)
        //{
        //    return Quaternion.Internal_FromEulerRad(euler);
        //}

        //public void SetEulerRotation(float x, float y, float z)
        //{
        //    this = Quaternion.Internal_FromEulerRad(new Vector3(x, y, z));
        //}

        //public void SetEulerRotation(Vector3 euler)
        //{
        //    this = Quaternion.Internal_FromEulerRad(euler);
        //}

        //public Vector3 ToEuler()
        //{
        //    return Quaternion.Internal_ToEulerRad(this);
        //}

        //public static Quaternion EulerAngles(float x, float y, float z)
        //{
        //    return Quaternion.Internal_FromEulerRad(new Vector3(x, y, z));
        //}

        //public static Quaternion EulerAngles(Vector3 euler)
        //{
        //    return Quaternion.Internal_FromEulerRad(euler);
        //}

        //public void ToAxisAngle(out Vector3 axis, out float angle)
        //{
        //    Quaternion.Internal_ToAxisAngleRad(this, out axis, out angle);
        //}

        //public void SetEulerAngles(float x, float y, float z)
        //{
        //    this.SetEulerRotation(new Vector3(x, y, z));
        //}

        //public void SetEulerAngles(Vector3 euler)
        //{
        //    this = Quaternion.EulerRotation(euler);
        //}

        //public static Vector3 ToEulerAngles(Quaternion rotation)
        //{
        //    return Quaternion.Internal_ToEulerRad(rotation);
        //}

        //public Vector3 ToEulerAngles()
        //{
        //    return Quaternion.Internal_ToEulerRad(this);
        //}

        public void SetAxisAngle(Vector3 axis, float angle)
        {
            this = Quaternion.AxisAngle(axis, angle);
        }

        public static Quaternion AxisAngle(Vector3 axis, float angle)
        {
            return Quaternion.AngleAxis(57.29578f * angle, axis);
        }

        public float x;

        public float y;

        public float z;

        public float w;

        private static readonly Quaternion identityQuaternion = new Quaternion(0f, 0f, 0f, 1f);

        public const float kEpsilon = 1E-06f;

        public static implicit operator System.Numerics.Quaternion(Quaternion q)
        {
            return new System.Numerics.Quaternion(q.x, q.y, q.z, q.w);
        }

        public static implicit operator Quaternion(System.Numerics.Quaternion q)
        {
            return new Quaternion(q.X, q.Y, q.Z, q.W);
        }
    }
}
