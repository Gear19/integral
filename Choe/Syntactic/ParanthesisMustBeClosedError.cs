namespace Choe.Syntactic
{
  public class ParanthesisMustBeClosedError : SymbolParityError
  {
    protected override string GetDescription() => string.Format(Messages.ParanthesisMustBeClosedFormat, (object) this.errorpair.Opening, (object) this.errorpair.Closing);

    public ParanthesisMustBeClosedError(string value, int pos1, int pos2, SymbolPair p)
      : base(value, pos1, pos2, p)
    {
    }
  }
}
