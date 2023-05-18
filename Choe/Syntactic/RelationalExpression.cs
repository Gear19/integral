using System.Collections.Generic;

namespace Choe.Syntactic
{
  public class RelationalExpression : BinaryOperationsExpression
  {
    protected override BinaryOperationType GetBinaryType() => BinaryOperationType.Relational;

    public RelationalExpression(
      string aValue,
      List<string> theOperators,
      List<BaseExpression> theOperands)
      : base(aValue, theOperators, theOperands)
    {
    }

    protected override BaseExpression SelfIntegral(string vName)
    {
         this.Reconstruct();
         return null;
    }

    public override void Simplify() => this.SimplifyOperands();
  }
}
