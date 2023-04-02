using System;
using System.Collections.Generic;

namespace Choe.Syntactic
{
  public abstract class ArrayExpression : StructuredExpression
  {
    protected BaseExpression[] data;
    protected int[] size;

    protected override ExpressionContext GetInternalContext()
    {
      ExpressionContext internalContext = (ExpressionContext) null;
      if (this.data != null)
        internalContext = BaseExpression.GetDataContext(this.data);
      return internalContext;
    }

    protected override void SetInternalContext(ExpressionContext ctxt)
    {
      if (this.data == null)
        return;
      BaseExpression.SetDataContext(this.data, ctxt);
    }

    protected abstract ArrayExpression CreateSame(BaseExpression[] items);

    protected abstract string GetArrayType();

    protected virtual int GetDimension() => this.size.Length;

    protected int GetLength() => this.data.Length;

    protected virtual void SimplifyData()
    {
      int length = this.Length;
      for (int index = 0; index < length; ++index)
        this.data[index].Simplify();
    }

    protected virtual string[] GetReconstructedItems()
    {
      int length = this.Length;
      string[] reconstructedItems = new string[length];
      for (int index = 0; index < length; ++index)
        reconstructedItems[index] = this.data[index].Reconstruct();
      return reconstructedItems;
    }

    public virtual string DimToString()
    {
      string empty = string.Empty;
      int dimension = this.Dimension;
      for (int dim = 1; dim <= dimension; ++dim)
      {
        empty += this.Size(dim).ToString();
        if (dim < dimension)
          empty += "x";
      }
      return empty;
    }

    protected override int ReplaceInside(string vName, BaseExpression expr)
    {
      List<BaseExpression> expressions = new List<BaseExpression>((IEnumerable<BaseExpression>) this.data);
      int num = StructuredExpression.ReplaceInList(expressions, vName, expr);
      if (num <= 0)
        return num;
      this.data = expressions.ToArray();
      return num;
    }

    protected override int GetSelfPrecedence() => Utilities.GetArrayPrecedence();

    protected override BaseExpression GetExplicit(bool simplified)
    {
      BaseExpression[] items = (BaseExpression[]) null;
      if (this.data != null)
      {
        int length = this.Length;
        items = new BaseExpression[length];
        for (int index = 0; index < length; ++index)
          items[index] = BaseExpression.ExplicitExpression(this.data[index], simplified);
      }
      return (BaseExpression) this.CreateSame(items);
    }

    public override bool IsConstant()
    {
      int length = this.Length;
      for (int index = 0; index < length; ++index)
      {
        if (!this.Data[index].IsConstant())
          return false;
      }
      return true;
    }

    public override bool DependsOn(string vName)
    {
      int length = this.Length;
      for (int index = 0; index < length; ++index)
      {
        if (this.Data[index].DependsOn(vName))
          return true;
      }
      return false;
    }

    public int Dimension => this.GetDimension();

    public int Length => this.GetLength();

    public int Size(int dim) => this.size[dim - 1];


    public BaseExpression[] Data => this.data;

    public ArrayExpression(string aValue, BaseExpression[] items, int[] sizes)
      : base(aValue)
    {
      if (items == null || items.Length < 1)
        throw new ArgumentException("Array expression items");
      int num = sizes != null && sizes.Length >= 1 ? items.Length : throw new ArgumentException("Array expression sizes");
      int length = sizes.Length;
      int siz = sizes[0];
      for (int index = 1; index < length; ++index)
        siz *= sizes[index];
      if (siz != num)
        throw new ArgumentException("Array expression items count");
      this.data = items;
      this.size = sizes;
      this._context = this.GetInternalContext();
    }

    public override void Simplify()
    {
      base.Simplify();
      this.SimplifyData();
    }

    public override string ToString() => this.GetArrayType() + " expression: " + this.DimToString();

    public override string Print(int level)
    {
      string str1 = new string(' ', level * 2);
      string str2 = str1 + this.GetArrayType() + " expression: " + this.DimToString();
      int length = this.Length;
      for (int index = 0; index < length; ++index)
        str2 = str2 + "\r" + str1 + "Item " + index.ToString() + ":\r" + this.Data[index].Print(level + 1);
      return str2;
    }
  }
}
