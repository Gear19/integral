using Choe.Integrals;

namespace Choe.Syntactic
{
  public class ExpressionContext
  {
    protected IntegralContext _integrals;

    public ExpressionContext(IntegralContext i)
    {
      this._integrals = i;
    }

    public IntegralContext Integrals => this._integrals;
  }
}
