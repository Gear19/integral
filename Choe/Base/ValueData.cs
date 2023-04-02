using System;

namespace Choe
{
  public class ValueData
  {
    public Type T;
    public object Value;

    public ValueData(Type t, object v)
    {
      this.T = t;
      this.Value = v;
    }
  }
}
