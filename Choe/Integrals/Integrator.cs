using Choe.Syntactic;

namespace Choe.Integrals
{
  public abstract class Integrator
  {
    public abstract BaseExpression Integral(BaseExpression expr, string vName);
  }
}
