namespace Choe.Syntactic
{
  public class UnexpectedParanthesis : ParanthesisError
  {
    protected override string GetDescription() => string.Format(Messages.UnexpectedParanthesisFormat, (object) this.errorpair.Closing, (object) this.errorpair.Opening);

    public UnexpectedParanthesis(string value, int pos1, int pos2, SymbolPair p)
      : base(value, pos1, pos2, p)
    {
    }
  }
}
