/** This file was generated from @PROJECT_SOURCE_DIR@/src/types.in.h
 *
 * Do not edit this file
 */

// clang-format off

@PREPROCESSOR@

namespace McRtc
{

@SEQUENTIAL_STRUCT@
@PUBLIC_ATTRIBUTE@ struct FloatArray
{
    @PUBLIC_ATTRIBUTE@ @FLOAT_ARRAY_POINTER@ data;
    @PUBLIC_ATTRIBUTE@ @SIZE_T@ ndata;

#if MC_RTC_CSHARP
    public FloatArray(float[] floats)
    {
        ndata = (nuint)floats.Length;
        data = Marshal.AllocHGlobal(floats.Length * sizeof(float));
        Marshal.Copy(floats, 0, data, floats.Length);
    }

    public float[] ToArray()
    {
        float[] floats = new float[ndata];
        Marshal.Copy(data, floats, 0, (int)ndata);
        return floats;
    }
#endif
}@END_STRUCT@

#if MC_RTC_CPP
FloatArray ToUnity(const Eigen::VectorXd & v, std::vector<float> & buffer)
{
    buffer.resize(v.size());
    for(size_t i = 0; i < buffer.size(); ++i)
    {
        buffer[i] = static_cast<float>(v(static_cast<Eigen::DenseIndex>(i)));
    }
    return {buffer.data(), buffer.size()};
}

Eigen::VectorXd FromUnity(FloatArray fa)
{
    return Eigen::Map<Eigen::VectorXf>(fa.data, fa.ndata).cast<double>();
}
#endif

@SEQUENTIAL_STRUCT@
@PUBLIC_ATTRIBUTE@ struct StringArray
{
    @PUBLIC_ATTRIBUTE@ @STRING_ARRAY_POINTER@ data;
    @PUBLIC_ATTRIBUTE@ @SIZE_T@ ndata;
#if MC_RTC_CSHARP
    public string[] ToArray()
    {
        IntPtr[] ptrs = new IntPtr[ndata];
        Marshal.Copy(data, ptrs, 0, (int)ndata);
        string[] strings = new string[ndata];
        for(int i = 0; i < strings.Length; i++)
        {
            strings[i] = Marshal.PtrToStringAnsi(ptrs[i]);
        }
        return strings;
    }
#endif
}@END_STRUCT@

#if MC_RTC_CPP
StringArray ToUnity(const std::vector<std::string>& v, std::vector<const char*>& buffer)
{
    buffer.resize(v.size());
    for (size_t i = 0; i < v.size(); ++i)
    {
        buffer[i] = v[i].c_str();
    }
    return {buffer.data(), buffer.size()};
}
#endif

@SEQUENTIAL_STRUCT@
@PUBLIC_ATTRIBUTE@ struct Vec3
  {
    @PUBLIC_ATTRIBUTE@ float x;
    @PUBLIC_ATTRIBUTE@ float y;
    @PUBLIC_ATTRIBUTE@ float z;
#if MC_RTC_CSHARP
    public Vec3(Vector3 v)
    {
      this.x = v.x;
      this.y = v.y;
      this.z = v.z;
    }

  public
    Vector3 ToVector3()
    {
      return new Vector3(x, y, z);
    }

#endif
}@END_STRUCT@

#if MC_RTC_CPP
Eigen::Vector3f ToEigen(const Vec3 & v)
{
  return Eigen::Vector3f(v.x, v.y, v.z);
}

Vec3 FromEigen(const Eigen::Vector3f & v)
{
  return {v.x(), v.y(), v.z()};
}
#endif

@SEQUENTIAL_STRUCT@
@PUBLIC_ATTRIBUTE@ struct Quat
{
  @PUBLIC_ATTRIBUTE@ float x;
  @PUBLIC_ATTRIBUTE@ float y;
  @PUBLIC_ATTRIBUTE@ float z;
  @PUBLIC_ATTRIBUTE@ float w;
#if MC_RTC_CSHARP
  public Quat(Quaternion q)
  {
    this.x = q.x;
    this.y = q.y;
    this.z = q.z;
    this.w = q.w;
  }

  public Quaternion ToQuaternion()
  {
    return new Quaternion(x, y, z, w);
  }
#endif
}@END_STRUCT@

#if MC_RTC_CPP
Eigen::Quaternionf ToEigen(const Quat & q)
{
  return Eigen::Quaternionf(q.w, q.x, q.y, q.z);
}
Quat FromEigen(const Eigen::Quaternionf& q)
{
  return { q.x(), q.y(), q.z(), q.w()};
}
#endif

@SEQUENTIAL_STRUCT@
@PUBLIC_ATTRIBUTE@ struct PTransform
{
  @PUBLIC_ATTRIBUTE@ Quat rotation;
  @PUBLIC_ATTRIBUTE@ Vec3 translation;
#if MC_RTC_CSHARP
  public PTransform(Quaternion rotation, Vector3 translation)
  {
    this.rotation = new Quat(rotation);
    this.translation = new Vec3(translation);
  }

  public void SetLocalTransform(GameObject obj)
  {
    obj.transform.localPosition = translation.ToVector3();
    obj.transform.localRotation = rotation.ToQuaternion();
  }

  public void SetTransform(GameObject obj)
  {
    obj.transform.position = translation.ToVector3();
    obj.transform.rotation = rotation.ToQuaternion();
  }

  static public PTransform FromLocalTransform(GameObject obj)
  {
    return new PTransform(obj.transform.localRotation, obj.transform.localPosition);
  }

  static public PTransform FromTransform(GameObject obj)
  {
    return new PTransform(obj.transform.rotation, obj.transform.position);
  }
#endif
}@END_STRUCT@

#if MC_RTC_CPP
PTransform ToUnity(const sva::PTransformd & pt)
{
  static auto to_unity = []() -> Eigen::Matrix4f {
    Eigen::Matrix4f out = Eigen::Matrix4f::Zero();
    out(0, 0) = 1.0f;
    out(1, 2) = 1.0f;
    out(2, 1) = 1.0f;
    out(3, 3) = 1.0f;
    return out;
  }();
  auto homo =
      to_unity * sva::conversions::toHomogeneous(pt.cast<float>(), sva::conversions::RightHanded) * to_unity;
  Eigen::Quaternionf q(homo.block<3, 3>(0, 0));
  Eigen::Vector3f t = homo.block<3, 1>(0, 3);
  PTransform ptOut;
  ptOut.translation = FromEigen(t);
  ptOut.rotation = FromEigen(q);
  return ptOut;
}

sva::PTransformd FromUnity(const PTransform & pt)
{
  Eigen::Matrix4f homo = Eigen::Matrix4f::Identity();
  homo.block<3, 3>(0, 0) = ToEigen(pt.rotation).toRotationMatrix();
  homo.block<3, 1>(0, 3) = ToEigen(pt.translation);
  static auto from_unity = []() -> Eigen::Matrix4f
  {
    Eigen::Matrix4f out = Eigen::Matrix4f::Zero();
    out(0, 0) = 1.0f;
    out(1, 2) = 1.0f;
    out(2, 1) = 1.0f;
    out(3, 3) = 1.0f;
    return out;
  }();
  return sva::conversions::fromHomogeneous((from_unity * homo * from_unity).cast<double>());
}

bool FromUnity(bool b)
{
  return b;
}
#endif

} // namespace McRtc

// clang-format on
