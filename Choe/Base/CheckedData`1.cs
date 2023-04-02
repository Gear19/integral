namespace Choe
{
  public class CheckedData<T>
  {
    public bool Is;
    public T Value;

    public CheckedData(bool _is, T v)
    {
      this.Is = _is;
      this.Value = v;
    }
  }
}
