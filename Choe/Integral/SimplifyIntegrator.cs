using Choe.Syntactic;

namespace Choe.Integrals
{
  public abstract class SimplifyIntegrator : Integrator
  {
    protected abstract BaseExpression Simplify(BaseExpression expr, string vName);

    public override BaseExpression Integral(BaseExpression expr, string vName)
    {
      BaseExpression baseExpression1 = (BaseExpression) null;
      BaseExpression baseExpression2 = this.Simplify(expr, vName);
      if (baseExpression2 != null)
      {
        if (baseExpression2.Context == null && expr.Context != null)
          baseExpression2.Context = expr.Context;
        baseExpression1 = baseExpression2.Integrate(vName);
      }
      return baseExpression1;
    }
  }
}
