namespace Choe.Syntactic
{
  public class PairErrorData
  {
    public int Error;
    public int ErrorIndex;
    public SymbolPair ErrorPair;

    public PairErrorData(int err, int index, SymbolPair errPair)
    {
      this.Error = err;
      this.ErrorIndex = index;
      this.ErrorPair = errPair;
    }
  }
}
