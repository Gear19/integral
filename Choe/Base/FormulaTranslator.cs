using Choe.Integrals;
using Choe.Syntactic;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choe
{
    public static class FormulaTranslator
    {
        public static BaseExpression CheckExpression(BaseExpression checkExpr, string vName)//, BaseExpression formulaExpr)
        {
            BaseExpression formulaValueExpr = (BaseExpression)null;
            foreach (var valuePair in Formulas.FormulasDictionary())
            {
                Dictionary<string, string> matchingVariables = new Dictionary<string, string>();
                BaseExpression formulaKeyExpr = BaseExpression.Build(new ExpressionContext((IntegralContext)new AutoIntegralContext()), valuePair.Key);
                if (IsValidExpression(checkExpr, formulaKeyExpr, vName, matchingVariables))
                {
                    formulaValueExpr = BaseExpression.Build(new ExpressionContext((IntegralContext)new AutoIntegralContext()), valuePair.Value, matchingVariables);
                    //formulaValueExpr = BaseExpression.Build(new ExpressionContext((IntegralContext)new AutoIntegralContext()), valuePair.Value);
                    //formulaValueExpr = BaseExpression.Build(new ExpressionContext((IntegralContext)new AutoIntegralContext()), valuePair.Value);
                    break;
                }
            }
            return formulaValueExpr;
        }

        public static bool IsValidExpression(BaseExpression expr1, BaseExpression expr2, string vName, Dictionary<string, string> matchingVariables)
        {
            bool isValid = false;

            if (expr2 is VariableExpression && expr2.Value != vName)
            {
                isValid = true;
                matchingVariables.Add(expr2.Value, expr1.Value);
            }

            if (expr1.GetType() == expr2.GetType())
            {
                if(expr2 is VariableExpression && expr2.Value == vName)
                {
                    isValid = expr1.Value == vName;
                }
                else if(expr1 is LiteralExpression)
                {
                    isValid = expr1.Value == expr2.Value;
                }
                else if (expr1 is UnaryOperatorExpression)
                {
                    UnaryOperatorExpression uexpr1 = (UnaryOperatorExpression)expr1;
                    UnaryOperatorExpression uexpr2 = (UnaryOperatorExpression)expr2;
                    if (uexpr1.Sign == uexpr2.Sign
                        && uexpr1.Position == uexpr2.Position)
                    {
                        isValid = IsValidExpression(uexpr1.Operand, uexpr2.Operand, vName, matchingVariables);
                    }
                    else isValid = false;
                }
                else if (expr1 is BinaryOperationsExpression)
                {
                    BinaryOperationsExpression biexpr1 = (BinaryOperationsExpression)expr1;
                    BinaryOperationsExpression biexpr2 = (BinaryOperationsExpression)expr2;
                    if (biexpr1.OperationCount == biexpr2.OperationCount
                        && Utilities.GetOperatorType(biexpr1.Operators[0], OperatorArity.Binary) == Utilities.GetOperatorType(biexpr2.Operators[0], OperatorArity.Binary))
                    {
                        bool isValid1 = IsValidExpression(biexpr1.Operands[0], biexpr2.Operands[0], vName, matchingVariables);
                        bool isValid2 = IsValidExpression(biexpr1.Operands[1], biexpr2.Operands[1], vName, matchingVariables);
                        isValid = isValid1 && isValid2;
                    }
                    else isValid = false;
                }
                else if (expr1 is FunctionExpression)
                {
                    FunctionExpression fexpr1 = (FunctionExpression)expr1;
                    FunctionExpression fexpr2 = (FunctionExpression)expr2;
                    if (fexpr1.Name == fexpr2.Name
                        && fexpr1.ParameterCount == fexpr2.ParameterCount
                        && fexpr1.ArgumentCount == fexpr2.ArgumentCount)
                    {
                        bool isValidF = false;
                        if (fexpr1.ParameterCount != 0)
                            for (int i = 0; i < fexpr1.Parameters.Count; i++)
                            {
                                isValidF |= IsValidExpression(fexpr1.Parameters[i], fexpr2.Parameters[i], vName, matchingVariables);
                            }
                        if (fexpr1.ArgumentCount != 0)
                            for (int i = 0; i < fexpr1.Arguments.Count; i++)
                            {
                                isValidF |= IsValidExpression(fexpr1.Arguments[i], fexpr2.Arguments[i], vName, matchingVariables);
                            }
                        isValid = isValidF;
                    }
                    else isValid = false;
                }
            }
            return isValid;
        }

        public static BaseExpression BuildFormula(string formula, Dictionary<string, string> matchingVariables)
        {
            BaseExpression expr = BaseExpression.Build(new ExpressionContext((IntegralContext)new AutoIntegralContext()), formula, matchingVariables);
            return expr;
        }
    }
}
