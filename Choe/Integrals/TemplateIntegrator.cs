using Choe.Syntactic;

namespace Choe.Integrals
{
  public abstract class TemplateIntegrator : Integrator
  {
    protected abstract MatchData Match(BaseExpression expr, string vName);

    protected abstract BaseExpression BuildIntegral(
      BaseExpression expr,
      MatchData data,
      string vName);

    public override BaseExpression Integral(BaseExpression expr, string vName)
    {
      BaseExpression baseExpression = (BaseExpression) null;
      MatchData data = this.Match(expr, vName);
      if (data != null)
        baseExpression = this.BuildIntegral(expr, data, vName);
      return baseExpression;
    }
  }
}
