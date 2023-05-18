namespace Choe.Syntactic
{
  public class ClosingSymbolParityError : SymbolParityError
  {
    protected override string GetDescription() => string.Format(Messages.ClosingSymbolParityErrorFormat, (object) this.errorpair.Opening);

    public ClosingSymbolParityError(string value, int pos1, int pos2, SymbolPair p)
      : base(value, pos1, pos2, p)
    {
    }
  }
}
