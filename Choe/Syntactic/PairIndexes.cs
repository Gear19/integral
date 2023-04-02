namespace Choe.Syntactic
{
  public class PairIndexes
  {
    protected int index1;
    protected int index2;

    public PairIndexes(int i1, int i2)
    {
      this.index1 = i1;
      this.index2 = i2;
    }

    public int Index1
    {
      get => this.index1;
      set => this.index1 = value;
    }

    public int Index2
    {
      get => this.index2;
      set => this.index2 = value;
    }

    public int Length => this.index2 - this.index1 + 1;

    public int LengthInside => this.index2 - this.index1 - 1;

    public override string ToString() => string.Format("PairIndexes ({0},{1}):{2}", (object) this.index1, (object) this.index2, (object) this.Length);
  }
}
