namespace Choe.Syntactic
{
  public abstract class ParanthesisError : SyntaxError
  {
    protected SymbolPair errorpair;

    public ParanthesisError(string value, int pos1, int pos2, SymbolPair p)
      : base(value, pos1, pos2)
    {
      this.errorpair = p;
    }
  }
}
