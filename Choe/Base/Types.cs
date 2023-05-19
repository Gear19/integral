using System;
using System.Numerics;

namespace Choe
{
  public static class Types
  {
    public static readonly Type BooleanType = typeof (bool);
    public static readonly Type BooleanArrayType = typeof (bool[]);
    public static readonly Type BooleanMatrixType = typeof (bool[,]);
    public static readonly Type BooleanBlockType = typeof (bool[,,]);
    public static readonly Type RealType = typeof (double);
    public static readonly Type RealArrayType = typeof (double[]);
    public static readonly Type RealMatrixType = typeof (double[,]);
    public static readonly Type RealBlockType = typeof (double[,,]);
    public static readonly Type ComplexType = typeof (Complex);
    public static readonly Type ComplexArrayType = typeof (Complex[]);
    public static readonly Type ComplexMatrixType = typeof (Complex[,]);
    public static readonly Type ComplexBlockType = typeof (Complex[,,]);

    static Types()
    {
      Converter converter;
      converter.TargetType = Types.ComplexType;
      converter.Convert = new ValueToString(Converters.ConvertComplex);
      Utilities.RegisterConverter(converter);
    }
  }
}
