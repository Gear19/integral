using Choe.Syntactic;

namespace Choe.Integrals
{

  public abstract class FunctionalIntegral
  {
    public string FunctionName => this.GetFunctionName();

    public int ParameterCount => this.GetParameterCount();

    public int ArgumentCount => this.GetArgumentCount();

    public abstract string GetFunctionName();

    public abstract int GetParameterCount();

    public abstract int GetArgumentCount();

    public abstract BaseExpression Integral(FunctionExpression afunction, string vName);

    public virtual bool Overrides() => false;
  }
}
