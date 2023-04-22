using Choe.Integrals;
using Choe.Syntactic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choe
{
    //public sealed class SquareRootIntegral : SimpleFunctionalIntegral
    //{
    //    public override BaseExpression BaseIntegral(BaseExpression arg)
    //    {
    //        BaseExpression expr = PowerExpression.MakeSquareRoot(arg);
    //        BaseExpression result = expr.int
    //        return result;
    //    }

    //    public override string GetFunctionName()
    //    {
    //        return Names.SquareRoot;
    //    }
    //}

    public sealed class SineIntegral : SimpleFunctionalIntegral
    {
        public override BaseExpression BaseIntegral(BaseExpression arg)
        {
            BaseExpression result = FunctionExpression.CreateSimple(Names.Cosine, arg);
            result = UnaryOperatorExpression.Negate(result);
            return result;
        }

        public override string GetFunctionName()
        {
            return Names.Sine;
        }
    }

    public sealed class CosineIntegral : SimpleFunctionalIntegral
    {
        public override BaseExpression BaseIntegral(BaseExpression arg)
        {
            BaseExpression result = FunctionExpression.CreateSimple(Names.Sine, arg);
            return result;
        }

        public override string GetFunctionName()
        {
            return Names.Cosine;
        }
    }

    public sealed class TangentIntegral : SimpleFunctionalIntegral
    {
        public override BaseExpression BaseIntegral(BaseExpression arg)
        {
            BaseExpression result = FunctionExpression.CreateSimple(Names.NaturalLogarithm, FunctionExpression.CreateSimple(Names.Modulus, FunctionExpression.CreateSimple(Names.Cosine, arg)));
            result = UnaryOperatorExpression.Negate(result);
            return result;
        }

        public override string GetFunctionName()
        {
            return Names.Tangent;
        }
    }

    public sealed class CotangentIntegral : SimpleFunctionalIntegral
    {
        public override BaseExpression BaseIntegral(BaseExpression arg)
        {
            BaseExpression result = FunctionExpression.CreateSimple(Names.NaturalLogarithm, FunctionExpression.CreateSimple(Names.Modulus, FunctionExpression.CreateSimple(Names.Sine, arg)));
            return result;
        }

        public override string GetFunctionName()
        {
            return Names.Cotangent;
        }
    }
}
