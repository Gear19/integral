using Choe.Syntactic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choe.Integrals
{
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

    public sealed class ExpandIntegrator : SimplifyIntegrator
    {
        protected override BaseExpression Simplify(BaseExpression expr, string vName)
        {
            if (expr is BinaryOperationsExpression)
            {
                BinaryOperationsExpression be = (BinaryOperationsExpression)expr;
                if (be.OperationCount > 0)
                {
                    if (be.CanExpand())
                    {
                        BaseExpression ee = be.Expand();
                        if (ee != null)
                        {
                            ee.Simplify();
                            return ee;
                        }
                    }
                }
            }

            return null;
        }
    }
}
