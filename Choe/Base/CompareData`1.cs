namespace Choe
{
  public class CompareData<T>
  {
    public T Left;
    public T Right;

    public CompareData(T l, T r)
    {
      this.Left = l;
      this.Right = r;
    }
  }
}
