using Choe.Syntactic;
using System.Collections.Generic;

namespace Choe.Integrals
{
  public abstract class SubstituteIntegrator : AlgorithmIntegrator
  {
    protected static int Counter;

    protected abstract SubstituteData GetSubstitution(BaseExpression expr, string vName);

    protected abstract BaseExpression Substitute(SubstituteData data);

    protected static string GetSubstName(string vName)
    {
      int counter = SubstituteIntegrator.Counter;
      ++SubstituteIntegrator.Counter;
      return "subst___" + vName + "_" + counter.ToString();
    }

    public override BaseExpression Integral(BaseExpression expr, string vName)
    {
      BaseExpression baseExpression1 = (BaseExpression) null;
      SubstituteData substitution = this.GetSubstitution(expr, vName);
      if (substitution != null)
      {
        BaseExpression baseExpression2 = this.Substitute(substitution).Integrate(substitution.u);
        if (baseExpression2 != null)
        {
          List<BaseExpression> expressions = new List<BaseExpression>();
          expressions.Add(baseExpression2);
          StructuredExpression.ReplaceInList(expressions, substitution.u, substitution.g);
          baseExpression1 = expressions[0];
        }
      }
      return baseExpression1;
    }
  }
}
