using Choe.Integrals;
using Choe.Syntactic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choe
{
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
