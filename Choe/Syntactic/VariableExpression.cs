namespace Choe.Syntactic
{
  public class VariableExpression : SimpleExpression
  {
    protected override int GetPrecedence() => Utilities.GetVariablePrecedence();

    public VariableExpression(string aValue)
      : base(aValue)
    {
    }

    public string Name => this.value;

    public override string Reconstruct() => this.Name;

    public override bool IsConstant() => false;

    public override bool DependsOn(string vName) => this.Name == vName;

    protected override BaseExpression SelfIntegral(string vName)
    {
      BaseExpression baseExpression = !(vName == this.value) ? ProductExpression.MakeProduct((BaseExpression) new VariableExpression(this.value), (BaseExpression) new VariableExpression(vName)) : ProductExpression.MakeProduct(ProductExpression.OneHalf, PowerExpression.MakePower((BaseExpression) new VariableExpression(vName), (BaseExpression) LiteralExpression.Make(2L)));
      baseExpression.Context = this.Context;
      return baseExpression;
    }

    public override string ToString() => "Variable: " + this.Name;

    public override string Print(int level) => new string(' ', level * 2) + this.ToString();

    public static bool IsVariable(BaseExpression expr, string vName)
    {
      BaseExpression baseExpression = expr.This;
      bool flag = baseExpression is VariableExpression;
      if (flag && vName != string.Empty)
        flag = ((VariableExpression) baseExpression).Name == vName;
      return flag;
    }
  }
}
