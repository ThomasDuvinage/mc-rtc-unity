#include <fstream>
#include <functional>
#include <string>
#include <type_traits>

#include "types.h"

template<typename T>
std::string TypeToStr()
{
  static_assert(!std::is_same_v<T, T>, "TypeToStr not implement for this type");
}

#define TYPE_TO_STR(T, STR)\
template<>\
std::string TypeToStr<T>()\
{                           \
  return STR;\
}

#define T_TO_STR(T) TYPE_TO_STR(T, #T)

T_TO_STR(void)
T_TO_STR(bool)
T_TO_STR(float)
TYPE_TO_STR(const char *, "string")
TYPE_TO_STR(float *, "IntPtr")
TYPE_TO_STR(McRtc::PTransform, "PTransform")
TYPE_TO_STR(size_t, "nuint")

template<typename T>
struct PointerToDelegate
{
  static_assert(!std::is_same_v<T, T>, "SPECIALIZE ME");
};

template<typename RetT, typename ... Args>
struct PointerToDelegate < RetT (*)(Args...)>
{
    template<typename Arg, typename... Args>
    void print_args(std::ostream & os, const std::vector<std::string> & names, size_t i =0)
    {
      if(i != 0)
      {
        os << ", ";
      }
      os << TypeToStr<Arg>() << " " << names[i];
      if constexpr(sizeof...(Args))
      {
        print_args<Args...>(os, names, i+1);
      }
    }

    std::string operator()(const char * name, const std::vector<std::string> & args)
    {
      std::stringstream ss;
      ss << "protected delegate " << TypeToStr<RetT>() << " " << name << "Callback(";
      if constexpr(sizeof...(Args))
      {
        print_args<Args...>(ss, args);
      }
      ss << ");";
      return ss.str();
    }
};

void generate_client(std::ostream & os)
{

    os << R"(using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;
using UnityEngine;

namespace McRtc
{
  public class ClientBase : MonoBehaviour
  {
)";

#define DEFINE_CALLBACK(VAR, FUNCTION, TYPE, ...) \
    {                                                \
        std::vector<std::string> args = {__VA_ARGS__}; \
        os << "    " << PointerToDelegate<TYPE>{}(#FUNCTION, args) << "\n"; \
        os << "    [DllImport(\"McRtcPlugin\", CallingConvention = CallingConvention.Cdecl)]\n"; \
        os << "    protected static extern void " << #FUNCTION << "(" << #FUNCTION << "Callback cb);\n\n"; \
    }

#include "callbacks.h"

  os << "  }\n}\n";

}

int main(int argc, char* argv[])
{
  std::string unity_dir = argc < 2 ? "." : argv[1];
  std::ofstream out(unity_dir + "/ClientBase.cs");
  generate_client(out);
  return 0;
}