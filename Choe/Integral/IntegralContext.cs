using System;
using System.Collections;
using System.Collections.Generic;

namespace Choe.Integrals
{
  public class IntegralContext
  {
    protected List<FunctionalIntegral> _functions;
    protected List<Integrator> _integrators;

    public List<FunctionalIntegral> FunctionIntegrals => this.GetFunctionIntegrals();

    public List<Integrator> ExpressionIntegrators => this.GetExpressionIntegrators();

    public FunctionalIntegral this[string func, int pcount, int acount] => this.GetValues(func, pcount, acount);

    public Integrator this[int index] => this.GetIntegrators(index);

    public int FunctionCount => this.GetFunctionCount();

    public int IntegratorCount => this.GetIntegratorCount();

    protected int GetFunctionCount() => Utilities.SafeCount((IList) this._functions);

    protected int GetIntegratorCount() => Utilities.SafeCount((IList) this._integrators);

    protected Integrator GetIntegrators(int index)
    {
      int integratorCount = this.IntegratorCount;
      return index >= 0 && index < integratorCount ? this._integrators[index] : (Integrator) null;
    }

    protected List<FunctionalIntegral> GetFunctionIntegrals()
    {
      if (this._functions == null)
        this._functions = new List<FunctionalIntegral>();
      return this._functions;
    }

    protected List<Integrator> GetExpressionIntegrators()
    {
      if (this._integrators == null)
        this._integrators = new List<Integrator>();
      return this._integrators;
    }

    protected int GetIndex(string func, int pcount, int acount)
    {
      int count = this.FunctionIntegrals.Count;
      for (int index = 0; index < count; ++index)
      {
        FunctionalIntegral functionIntegral = this.FunctionIntegrals[index];
        if (functionIntegral != null && functionIntegral.FunctionName == func && functionIntegral.ParameterCount == pcount && functionIntegral.ArgumentCount == acount)
          return index;
      }
      return -1;
    }

    protected int GetIndex(Type ic)
    {
      int count = this.ExpressionIntegrators.Count;
      for (int index = 0; index < count; ++index)
      {
        Integrator expressionIntegrator = this.ExpressionIntegrators[index];
        if (expressionIntegrator != null && expressionIntegrator.GetType() == ic)
          return index;
      }
      return -1;
    }

    protected FunctionalIntegral GetValues(string func, int pcount, int acount)
    {
      int index = this.GetIndex(func, pcount, acount);
      return index >= 0 ? this._functions[index] : (FunctionalIntegral) null;
    }

    public virtual bool Add(FunctionalIntegral i)
    {
      if (this.GetIndex(i.FunctionName, i.ParameterCount, i.ArgumentCount) >= 0)
        return false;
      this.FunctionIntegrals.Add(i);
      return true;
    }

    public virtual bool Add(Integrator i)
    {
      if (this.GetIndex(i.GetType()) >= 0)
        return false;
      this.ExpressionIntegrators.Add(i);
      return true;
    }

    public virtual void Clear()
    {
      this.FunctionIntegrals.Clear();
      this.ExpressionIntegrators.Clear();
    }

    public virtual bool Remove(string func, int pcount, int acount)
    {
      int index = this.GetIndex(func, pcount, acount);
      return index >= 0 && this.Remove(index);
    }

    public virtual bool Remove(int index)
    {
      if (index < 0 || index >= this.FunctionCount)
        return false;
      this.FunctionIntegrals.RemoveAt(index);
      return true;
    }

    public virtual void SetIntegral(FunctionalIntegral i)
    {
      int index = this.GetIndex(i.FunctionName, i.ParameterCount, i.ArgumentCount);
      if (index >= 0)
        this.FunctionIntegrals[index] = i;
      else
        this.FunctionIntegrals.Add(i);
    }

    public virtual bool Remove(Type ic)
    {
      int index = this.GetIndex(ic);
      if (index < 0)
        return false;
      this.ExpressionIntegrators.RemoveAt(index);
      return true;
    }

    public virtual void UpdateDefault() => this.Clear();
  }
}
