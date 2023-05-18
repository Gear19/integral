using Choe.Syntactic;
using System.Numerics;

namespace Choe
{
  public static class Converters
  {
    public static string ConvertComplex(object value) => Parser.ComplexToString((Complex) value);
  }
}
