using Choe.Integrals;
using Choe.Syntactic;
using Mathematics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Choe
{
  public class Translator
  {
    protected IntegralContext integrals;
    protected ExpressionContext context;
 
    protected virtual void RecreateIntegrals()
    {
      if (this.integrals != null)
        return;
      this.integrals = (IntegralContext) new AutoIntegralContext();
    }

    protected virtual void RecreateContext()
    {
      if (this.context != null)
        return;
      this.context = new ExpressionContext(this.integrals);
    }

    protected virtual void RecreateData()
    {
      this.RecreateIntegrals();
      this.RecreateContext();
    }

    public bool CheckSyntax(string value) => Syntax.Check(value);

    public ExpressionContext Context => this.context;

    public BaseExpression Integral(BaseExpression expr, string vName)
    {
      if (expr == null)
        return (BaseExpression) null;
      BaseExpression baseExpression = expr.Integral(vName);
      baseExpression?.Simplify();
      return baseExpression;
    }

    public string Integral(string formula, string vName)
    {
      BaseExpression expr = BaseExpression.Build(this.Context, formula);
      expr?.Simplify();
      BaseExpression baseExpression = this.Integral(expr, vName);
      string str = string.Empty;
      if (baseExpression != null)
        str = baseExpression.Reconstruct();
      return str;
    }
  }
}
