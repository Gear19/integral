using Choe.Integrals;
using System;
using System.Collections.Generic;

namespace Choe.Syntactic
{
  public class FunctionExpression : StructuredExpression
  {
    protected string name;
    protected List<BaseExpression> parameters;
    protected List<BaseExpression> arguments;

    protected override ExpressionContext GetInternalContext()
    {
      ExpressionContext internalContext = (ExpressionContext) null;
      if (this.arguments != null)
        internalContext = BaseExpression.GetDataContext(this.arguments.ToArray());
      if (internalContext == null && this.parameters != null)
        internalContext = BaseExpression.GetDataContext(this.parameters.ToArray());
      return internalContext;
    }

    protected override void SetInternalContext(ExpressionContext ctxt)
    {
      if (this.arguments != null)
        BaseExpression.SetDataContext(this.arguments.ToArray(), ctxt);
      if (this.parameters == null)
        return;
      BaseExpression.SetDataContext(this.parameters.ToArray(), ctxt);
    }

    protected override int GetSelfPrecedence() => Utilities.GetFunctionPrecedence();

    protected override int ReplaceInside(string vName, BaseExpression expr) => StructuredExpression.ReplaceInList(this.parameters, vName, expr) + StructuredExpression.ReplaceInList(this.arguments, vName, expr);

    protected override BaseExpression GetExplicit(bool simplified)
    {
      List<BaseExpression> theParameters = (List<BaseExpression>) null;
      List<BaseExpression> theArguments = (List<BaseExpression>) null;
      if (this.parameters != null)
      {
        int parameterCount = this.ParameterCount;
        theParameters = new List<BaseExpression>();
        for (int index = 0; index < parameterCount; ++index)
          theParameters.Add(BaseExpression.ExplicitExpression(this.parameters[index], simplified));
      }
      if (this.arguments != null)
      {
        int argumentCount = this.ArgumentCount;
        theArguments = new List<BaseExpression>();
        for (int index = 0; index < argumentCount; ++index)
          theArguments.Add(BaseExpression.ExplicitExpression(this.arguments[index], simplified));
      }
      return (BaseExpression) new FunctionExpression(Symbols.NotDefinedSign, this.Name, theParameters, theArguments);
    }

    public FunctionExpression(
      string aValue,
      string aName,
      List<BaseExpression> theParameters,
      List<BaseExpression> theArguments)
      : base(aValue)
    {
      this.name = aName;
      this.parameters = theParameters;
      this.arguments = theArguments;
      this._context = this.GetInternalContext();
    }

    public string Name => this.name;

    public int ParameterCount => this.parameters == null ? 0 : this.parameters.Count;

    public int ArgumentCount => this.arguments == null ? 0 : this.arguments.Count;

    public List<BaseExpression> Parameters => this.parameters;

    public List<BaseExpression> Arguments => this.arguments;

    public override string Reconstruct() => Builder.BuildFunction(this.Name, BaseExpression.GetReconstructed(this.parameters), BaseExpression.GetReconstructed(this.arguments));

    public override bool IsConstant() => BaseExpression.AreConstant(this.arguments) && BaseExpression.AreConstant(this.parameters);

    public override bool DependsOn(string vName) => BaseExpression.IsDependant(this.arguments, vName) || BaseExpression.IsDependant(this.parameters, vName);

    public override void Simplify()
    {
      BaseExpression.SimplifyList(this.parameters, true);
      BaseExpression.SimplifyList(this.arguments, true);
    }

    protected override BaseExpression SelfIntegral(string vName)
    {
      BaseExpression baseExpression = (BaseExpression) null;
      FunctionalIntegral functionalIntegral = (FunctionalIntegral) null;
      int num = this._context == null ? 1 : 0;
      if (num == 0)
        functionalIntegral = this.Context.Integrals[this.Name, this.ParameterCount, this.ArgumentCount];
      if ((num != 0 ? 1 : (functionalIntegral == null ? 1 : 0)) != 0)
        return null;
      if (this.ArgumentCount == 1 && !functionalIntegral.Overrides())
      {
        BaseExpression expr = this.Arguments[0];
        if (VariableExpression.IsVariable(expr, vName))
        {
          baseExpression = functionalIntegral.Integral(this, vName);
        }
        else
        {
          LinearExpression linearExpression = LinearExpression.Extract(expr, vName);
          if (linearExpression != null)
          {
            BaseExpression x2 = linearExpression.A.Copy(false);
            baseExpression = ProductExpression.MakeDivision(functionalIntegral.Integral(this, vName), x2);
          }
        }
      }
      else
        baseExpression = functionalIntegral.Integral(this, vName);
      return baseExpression;
    }

    public override string ToString() => "Function: " + this.Name + " Parameters: " + this.ParameterCount.ToString() + " " + Builder.BuildFunctionParameters(BaseExpression.GetReconstructed(this.parameters)) + " Arguments: " + this.ArgumentCount.ToString() + " " + Builder.BuildFunctionArguments(BaseExpression.GetReconstructed(this.arguments));

    public override string Print(int level)
    {
      string str1 = new string(' ', level * 2);
      string str2 = str1 + "Function: " + this.Name + Environment.NewLine + str1 + "Parameter Count: " + this.ParameterCount.ToString() + Environment.NewLine + str1 + "Parameters: ";
      for (int index = 0; index < this.ParameterCount; ++index)
        str2 = str2 + Environment.NewLine + this.parameters[index].Print(level + 1);
      string str3 = str2 + Environment.NewLine + str1 + "Argument count: " + this.ArgumentCount.ToString() + Environment.NewLine + str1 + "Arguments: ";
      for (int index = 0; index < this.ArgumentCount; ++index)
        str3 = str3 + Environment.NewLine + this.arguments[index].Print(level + 1);
      return str3;
    }

    public static void CheckParametersDependency(FunctionExpression function, string vName, int op)
    {
      if (function == null || function.ParameterCount == 0)
        return;
      int parameterCount = function.ParameterCount;
      for (int index = 0; index < parameterCount; ++index)
      {
        if (function.Parameters[index].DependsOn(vName))
        {
          if (op == 2)
            return;
          return;
        }
      }
    }

    public static FunctionExpression CreateSimple(string func, BaseExpression argument) => new FunctionExpression(Symbols.NotDefinedSign, func, (List<BaseExpression>) null, new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[1]
    {
      argument
    }));

    public static FunctionExpression CreateBinary(
      string func,
      BaseExpression argument1,
      BaseExpression argument2)
    {
      return new FunctionExpression(Symbols.NotDefinedSign, func, (List<BaseExpression>) null, new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[2]
      {
        argument1,
        argument2
      }));
    }

    public static FunctionExpression CreateParametric(
      string func,
      BaseExpression parameter,
      BaseExpression argument)
    {
      return new FunctionExpression(Symbols.NotDefinedSign, func, new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[1]
      {
        parameter
      }), new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[1]
      {
        argument
      }));
    }

    public static FunctionExpression CreateBinaryParametric(
      string func,
      BaseExpression parameter,
      BaseExpression argument1,
      BaseExpression argument2)
    {
      return new FunctionExpression(Symbols.NotDefinedSign, func, new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[1]
      {
        parameter
      }), new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[2]
      {
        argument1,
        argument2
      }));
    }

    public static FunctionExpression CreateBiparametric(
      string func,
      BaseExpression parameter1,
      BaseExpression parameter2,
      BaseExpression argument)
    {
      return new FunctionExpression(Symbols.NotDefinedSign, func, new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[2]
      {
        parameter1,
        parameter2
      }), new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[1]
      {
        argument
      }));
    }

    public static FunctionExpression CreateBinaryBiparametric(
      string func,
      BaseExpression parameter1,
      BaseExpression parameter2,
      BaseExpression argument1,
      BaseExpression argument2)
    {
      return new FunctionExpression(Symbols.NotDefinedSign, func, new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[2]
      {
        parameter1,
        parameter2
      }), new List<BaseExpression>((IEnumerable<BaseExpression>) new BaseExpression[2]
      {
        argument1,
        argument2
      }));
    }

    public static bool IsFunction(BaseExpression expr, string fName, string vName, int pcount)
    {
      BaseExpression baseExpression = expr.This;
      bool flag = expr is FunctionExpression;
      if (flag)
      {
        FunctionExpression functionExpression = baseExpression as FunctionExpression;
        flag = (fName == string.Empty || functionExpression.Name == fName) && functionExpression.ParameterCount == pcount && functionExpression.ArgumentCount == 1 && (vName == string.Empty || VariableExpression.IsVariable(functionExpression.Arguments[0], vName));
      }
      return flag;
    }

    public static bool IsFunction(
      BaseExpression expr,
      string fName,
      string vName1,
      string vName2,
      int pcount)
    {
      BaseExpression baseExpression = expr.This;
      bool flag = expr is FunctionExpression;
      if (flag)
      {
        FunctionExpression functionExpression = baseExpression as FunctionExpression;
        flag = (fName == string.Empty || functionExpression.Name == fName) && functionExpression.ParameterCount == pcount && functionExpression.ArgumentCount == 2 && (vName1 == string.Empty || VariableExpression.IsVariable(functionExpression.Arguments[0], vName1)) && (vName2 == string.Empty || VariableExpression.IsVariable(functionExpression.Arguments[1], vName2));
      }
      return flag;
    }

    public static bool IsFunction(BaseExpression expr, string fName, string vName) => FunctionExpression.IsFunction(expr, fName, vName, 0);
  }
}
