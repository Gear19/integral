using Choe.Syntactic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choe.Integrals
{
    public abstract class SimpleFunctionalIntegral : FunctionalIntegral
    {
        public abstract BaseExpression BaseIntegral(BaseExpression arg);

        public override int GetArgumentCount()
        {
            return 1;
        }

        public override int GetParameterCount()
        {
            return 0;
        }

        public override BaseExpression Integral(FunctionExpression afunction, string vName)
        {
            BaseExpression arg = afunction.Arguments[0].Copy(simplified: false);
            return BaseIntegral(arg);
        }
    }
}
