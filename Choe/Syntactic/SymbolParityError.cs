namespace Choe.Syntactic
{
  public abstract class SymbolParityError : SyntaxError
  {
    protected SymbolPair errorpair;

    public SymbolParityError(string value, int pos1, int pos2, SymbolPair p)
      : base(value, pos1, pos2)
    {
      this.errorpair = p;
    }
  }
}
