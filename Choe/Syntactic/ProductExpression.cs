using Mathematics.Fractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Choe.Syntactic
{
    public class ProductExpression : BinaryOperationsExpression
    {
        protected override int GetSelfPrecedence() => Utilities.GetOperatorPrecedence(OperatorType.Multiply);

        protected override BinaryOperationType GetBinaryType() => BinaryOperationType.Product;

        protected void Decompose(
          out List<BaseExpression> numerator,
          out List<BaseExpression> denominator,
          out List<string> noperators,
          out List<string> doperators)
        {
            numerator = new List<BaseExpression>();
            denominator = new List<BaseExpression>();
            noperators = new List<string>();
            doperators = new List<string>();
            int operationCount = this.OperationCount;
            if (operationCount < 0)
                return;
            numerator.Add(this.operands[0]);
            for (int index = 1; index <= operationCount; ++index)
            {
                string sign = this.operators[index - 1];
                OperatorType operatorType = Utilities.GetOperatorType(sign, OperatorArity.Binary);
                switch (operatorType)
                {
                    case OperatorType.Multiply:
                    case OperatorType.Dot:
                    case OperatorType.Cross:
                        noperators.Add(sign);
                        numerator.Add(this.operands[index]);
                        break;
                    case OperatorType.Divide:
                        if (denominator.Count > 0)
                        {
                            string operatorSign = Utilities.GetOperatorSign(OperatorType.Multiply);
                            doperators.Add(operatorSign);
                        }
                        denominator.Add(this.operands[index]);
                        break;
                    default:
                        return;
                }
            }
        }

        private int Collect_AddOperation(
          int i,
          List<BaseExpression> opnds,
          List<string> ops,
          bool extractminus)
        {
            int num = 1;
            bool flag1 = false;
            if (i > 0)
            {
                string op = this.operators[i - 1];
                bool flag2 = ProductExpression.IsInverseOperator(op);
                if (opnds.Count > 0)
                {
                    if (LiteralExpression.IsUnit(opnds[0].This) && !flag2)
                        flag1 = true;
                    else
                        ops.Add(op);
                }
                else if (flag2)
                {
                    opnds.Add((BaseExpression)LiteralExpression.Unit);
                    ops.Add(op);
                }
            }
            BaseExpression operand = this.operands[i];
            if (extractminus && UnaryOperatorExpression.IsUnary(operand.This, OperatorType.Minus))
            {
                operand = ((UnaryOperatorExpression)operand.This).Operand;
                num = -1;
            }
            BaseExpression baseExpression = operand.Copy(false);
            if (flag1)
                opnds[0] = baseExpression;
            else
                opnds.Add(baseExpression);
            return num;
        }

        private ProductExpression Collect_CreateMultiplier(
          List<BaseExpression> opnds,
          List<string> ops,
          int s)
        {
            if (s != 1)
            {
                if (opnds.Count > 0)
                    ops.Add(Symbols.MultiplyOperator.ToString());
                opnds.Add((BaseExpression)LiteralExpression.Make((long)s));
            }
            ProductExpression multiplier;
            if (opnds.Count == 0)
            {
                multiplier = (ProductExpression)null;
            }
            else
            {
                multiplier = new ProductExpression(Symbols.NotDefinedSign, ops, opnds);
                multiplier.Context = this.Context;
            }
            return multiplier;
        }

        protected void Collect(string vName, out ProductExpression cmult, out ProductExpression dmult)
        {
            int operationCount = this.OperationCount;
            List<BaseExpression> opnds1 = new List<BaseExpression>();
            List<BaseExpression> opnds2 = new List<BaseExpression>();
            List<string> ops1 = new List<string>();
            List<string> ops2 = new List<string>();
            int s = 1;
            for (int index = 0; index <= operationCount; ++index)
            {
                if (this.operands[index].DependsOn(vName))
                {
                    int num = this.Collect_AddOperation(index, opnds2, ops2, true);
                    s *= num;
                }
                else
                    this.Collect_AddOperation(index, opnds1, ops1, false);
            }
            cmult = this.Collect_CreateMultiplier(opnds1, ops1, s);
            dmult = this.Collect_CreateMultiplier(opnds2, ops2, 1);
        }

        public override bool CanExpand()
        {
            bool flag = base.CanExpand();
            if (flag)
                return flag;
            int num = Utilities.SafeCount((IList)this.operands);
            for (int index = 0; index < num; ++index)
            {
                if (this.operands[index].This is SumExpression && ((BinaryOperationsExpression)this.operands[index].This).OperationCount >= 1)
                {
                    OperatorType type = OperatorType.Multiply;
                    if (index > 0)
                        type = Utilities.GetOperatorType(this.operators[index - 1], OperatorArity.Binary);
                    switch (type)
                    {
                        case OperatorType.Multiply:
                        case OperatorType.Dot:
                        case OperatorType.Cross:
                            flag = true;
                            continue;
                        case OperatorType.Divide:
                            continue;
                        default:
                            return false;
                    }
                }
            }
            return flag;
        }

        public override BaseExpression Expand()
        {
            BaseExpression baseExpression1 = base.Expand();
            if (baseExpression1 == null)
            {
                int num = Utilities.SafeCount((IList)this.operands);
                if (num == 0)
                    return baseExpression1;
                List<BaseExpression> expressions = new List<BaseExpression>();
                for (int index1 = 0; index1 < num; ++index1)
                {
                    OperatorType type = OperatorType.Multiply;
                    if (index1 > 0)
                        type = Utilities.GetOperatorType(this.operators[index1 - 1], OperatorArity.Binary);
                    string operatorSign = Utilities.GetOperatorSign(type);
                    switch (type)
                    {
                        case OperatorType.Multiply:
                        case OperatorType.Divide:
                        case OperatorType.Dot:
                        case OperatorType.Cross:
                            BaseExpression baseExpression2 = this.operands[index1].This;
                            int count1 = expressions.Count;
                            if (!ProductExpression.IsInverseOperator(operatorSign) && baseExpression2 is SumExpression && ((BinaryOperationsExpression)baseExpression2).OperationCount > 0)
                            {
                                SumExpression sumExpression = (SumExpression)baseExpression2;
                                int count2 = sumExpression.Operands.Count;
                                bool[] flagArray = new bool[count2];
                                for (int index2 = 0; index2 < count2; ++index2)
                                    flagArray[index2] = index2 != 0 && SumExpression.IsInverseOperator(sumExpression.Operators[index2 - 1]);
                                if (count1 == 0)
                                {
                                    for (int index3 = 0; index3 < count2; ++index3)
                                    {
                                        BaseExpression expr = sumExpression.Operands[index3].Copy(false);
                                        if (flagArray[index3])
                                            expr = UnaryOperatorExpression.Negate(expr);
                                        expr.Simplify();
                                        expressions.Add(expr);
                                    }
                                    continue;
                                }
                                List<BaseExpression> baseExpressionList = new List<BaseExpression>();
                                for (int index4 = 0; index4 < count1; ++index4)
                                {
                                    for (int index5 = 0; index5 < count2; ++index5)
                                    {
                                        BaseExpression operand1 = expressions[index4].Copy(false);
                                        BaseExpression expr = sumExpression.Operands[index5].Copy(false);
                                        if (flagArray[index5])
                                            expr = UnaryOperatorExpression.Negate(expr);
                                        BaseExpression operand2 = expr;
                                        string sign = operatorSign;
                                        BaseExpression baseExpression3 = ProductExpression.MakeOperation(operand1, operand2, sign);
                                        baseExpression3.Simplify();
                                        baseExpressionList.Add(baseExpression3);
                                    }
                                }
                                expressions = baseExpressionList;
                                continue;
                            }
                            if (count1 == 0)
                            {
                                BaseExpression baseExpression4 = baseExpression2.Copy(true);
                                expressions.Add(baseExpression4);
                                continue;
                            }
                            for (int index6 = 0; index6 < count1; ++index6)
                            {
                                BaseExpression operand2 = baseExpression2.Copy(true);
                                expressions[index6] = ProductExpression.MakeOperation(expressions[index6], operand2, operatorSign);
                                expressions[index6].Simplify();
                            }
                            continue;
                        default:
                            return null;
                    }
                }
                baseExpression1 = expressions.Count != 1 ? BinaryOperationsExpression.MakeBinary(expressions, Symbols.AddOperator.ToString()) : expressions[0];
            }
            if (baseExpression1 != null && baseExpression1.This is BinaryOperationsExpression)
            {
                BinaryOperationsExpression operationsExpression = (BinaryOperationsExpression)baseExpression1.This;
                if (operationsExpression.CanExpand())
                    baseExpression1 = operationsExpression.Expand();
            }
            return baseExpression1;
        }

        public ProductExpression(
          string aValue,
          List<string> theOperators,
          List<BaseExpression> theOperands)
          : base(aValue, theOperators, theOperands)
        {
        }

        public bool IsInvertible()
        {
            if (this.IsDegenerated)
                return true;
            int operationCount = this.OperationCount;
            for (int index = 0; index < operationCount; ++index)
            {
                if (!ProductExpression.IsInvertible(this.operators[index]))
                    return false;
            }
            return true;
        }

        public BaseExpression ToFraction()
        {
            List<BaseExpression> numerator;
            List<BaseExpression> denominator;
            List<string> noperators;
            List<string> doperators;
            this.Decompose(out numerator, out denominator, out noperators, out doperators);
            return denominator == null || denominator.Count == 0 ? BinaryOperationsExpression.MakeBinary(BinaryOperationType.Product, numerator, noperators) : ProductExpression.MakeDivision(BinaryOperationsExpression.MakeBinary(BinaryOperationType.Product, numerator, noperators), BinaryOperationsExpression.MakeBinary(BinaryOperationType.Product, denominator, doperators));
        }

        protected override BaseExpression SelfIntegral(string vName)
        {
            BaseExpression x2 = (BaseExpression)null;

            if (this.OperationCount == 1 && Utilities.GetOperatorType(this.operators[0], OperatorArity.Binary) == OperatorType.Divide && LiteralExpression.IsUnit(this.operands[0]))
            {
                BaseExpression baseExpression1;
                if (VariableExpression.IsVariable(this.operands[1], vName))
                {
                    VariableExpression variableExpression = new VariableExpression(vName);
                    variableExpression.Context = this.Context;
                    baseExpression1 = (BaseExpression)FunctionExpression.CreateSimple(Names.NaturalLogarithm, (BaseExpression)variableExpression);
                    baseExpression1.Context = this.Context;
                }
                else if (this.operands[1] is PowerExpression) //(FunctionExpression.IsFunction(this.operands[1], Names.Sine, vName))
                {
                    VariableExpression variableExpression = new VariableExpression(vName);
                    variableExpression.Context = this.Context;
                    baseExpression1 = (BaseExpression)FunctionExpression.CreateSimple(Names.Cotangent, (BaseExpression)variableExpression);
                    baseExpression1 = UnaryOperatorExpression.Negate(baseExpression1);
                    baseExpression1.Context = this.Context;
                }
                else
                {
                    LinearExpression linearExpression = LinearExpression.Extract(this.operands[1], vName);
                    if (linearExpression != null)
                    {
                        BaseExpression baseExpression2;
                        if (linearExpression.B != null)
                        {
                            baseExpression2 = this.operands[1].Copy(false);
                        }
                        else
                        {
                            baseExpression2 = (BaseExpression)new VariableExpression(vName);
                            baseExpression2.Context = this.Context;
                        }
                        BaseExpression simple = (BaseExpression)FunctionExpression.CreateSimple(Names.NaturalLogarithm, baseExpression2);
                        simple.Context = this.Context;
                        baseExpression1 = ProductExpression.MakeDivision(simple, linearExpression.A.Copy(false));
                    }
                    else
                    {
                        BaseExpression baseExpression3 = PowerExpression.MakePower(this.operands[1].Copy(false), (BaseExpression)LiteralExpression.Negation);
                        baseExpression3.Context = this.Context;
                        baseExpression3.Simplify();
                        baseExpression1 = baseExpression3.Integrate(vName);
                    }
                }
                return baseExpression1;
            }
            int operationCount = this.OperationCount;
            bool flag = true;
            if (operationCount > 0)
            {
                for (int index = 0; index <= operationCount; ++index)
                {
                    if (!this.operands[index].DependsOn(vName) && !LiteralExpression.IsUnit(this.operands[index].This))
                    {
                        flag = false;
                        break;
                    }
                }
            }
            ProductExpression cmult;
            ProductExpression dmult;
            this.Collect(vName, out cmult, out dmult);
            if (cmult != null)
            {
                cmult.Simplify();
                if (LiteralExpression.IsUnit(cmult.This))
                    cmult = (ProductExpression)null;
            }
            if (flag && cmult == null)
                return x2;
            dmult.Simplify();
            if (cmult != null)
                cmult.Context = this.Context;
            if (dmult != null)
                dmult.Context = this.Context;
            BaseExpression baseExpression = dmult.Integrate(vName);
            if (baseExpression != null)
            {
                x2 = baseExpression;
                BaseExpression x1 = (BaseExpression)null;
                if (cmult != null)
                    x1 = !cmult.IsDegenerated ? (BaseExpression)cmult : cmult.Degenerated;
                if (x1 != null)
                    x2 = ProductExpression.MakeProduct(x1, x2);
            }
            return x2;
        }

        public static bool IsInverseOperator(string op)
        {
            OperatorType operatorType = Utilities.GetOperatorType(op, OperatorArity.Binary);
            switch (operatorType)
            {
                case OperatorType.Multiply:
                case OperatorType.Dot:
                case OperatorType.Cross:
                    return false;
                case OperatorType.Divide:
                    return true;
                default:
                    return false;
            }
        }

        protected static void InverseOperation(ref string op)
        {
            OperatorType operatorType = Utilities.GetOperatorType(op, OperatorArity.Binary);
            switch (operatorType)
            {
                case OperatorType.Multiply:
                    op = Utilities.GetOperatorSign(OperatorType.Divide);
                    break;
                case OperatorType.Divide:
                    op = Utilities.GetOperatorSign(OperatorType.Multiply);
                    break;
                default:
                    return;
            }
        }

        protected static bool IsInvertible(string op)
        {
            OperatorType operatorType = Utilities.GetOperatorType(op, OperatorArity.Binary);
            switch (operatorType)
            {
                case OperatorType.Multiply:
                case OperatorType.Divide:
                    return true;
                case OperatorType.Dot:
                case OperatorType.Cross:
                    return false;
                default:
                    return false;
            }
        }

        protected override void MergeOperands()
        {
            if (this.IsDegenerated)
                return;
            bool flag1 = false;
            bool flag2 = false;
            for (int index1 = Utilities.SafeCount((IList)this.operands) - 1; index1 >= 0; --index1)
            {
                BaseExpression operand = this.operands[index1];
                if (operand is ProductExpression)
                {
                    ProductExpression productExpression = (ProductExpression)operand;
                    if (!productExpression.IsDegenerated)
                    {
                        bool flag3 = false;
                        if (index1 > 0)
                            flag3 = ProductExpression.IsInverseOperator(this.operators[index1 - 1]);
                        bool flag4 = true;
                        if (flag3)
                            flag4 = productExpression.IsInvertible();
                        if (flag4 && index1 > 0)
                        {
                            OperatorType operatorType = Utilities.GetOperatorType(this.operators[index1 - 1], OperatorArity.Binary);
                            flag4 = operatorType == OperatorType.Multiply || operatorType == OperatorType.Divide;
                        }
                        if (flag4)
                        {
                            int num = Utilities.SafeCount((IList)productExpression.Operands);
                            string[] array1 = productExpression.Operators.ToArray();
                            BaseExpression[] array2 = productExpression.Operands.ToArray();
                            flag1 = true;
                            if (index1 > 0)
                            {
                                string str = this.operators[index1 - 1];
                                this.RemoveOperationRight(index1 - 1);
                                for (int index2 = num - 1; index2 >= 0; --index2)
                                {
                                    string op;
                                    if (index2 == 0)
                                    {
                                        op = str;
                                    }
                                    else
                                    {
                                        op = array1[index2 - 1];
                                        if (flag3)
                                            ProductExpression.InverseOperation(ref op);
                                    }
                                    this.InsertOperationRight(index1 - 1, op, array2[index2]);
                                }
                            }
                            else
                            {
                                this.operands[0] = array2[0];
                                for (int index3 = 1; index3 < num; ++index3)
                                {
                                    string op = array1[index3 - 1];
                                    if (flag3)
                                        ProductExpression.InverseOperation(ref op);
                                    this.InsertOperationRight(index3 - 1, op, array2[index3]);
                                }
                            }
                        }
                    }
                }
            }
            int num1 = Utilities.SafeCount((IList)this.operands);
            int index4 = -1;
            int index5 = -1;
            double num2 = double.NaN;
            for (int index6 = num1 - 1; index6 >= 0; --index6)
            {
                BaseExpression operand1 = this.operands[index6];
                switch (operand1)
                {
                    case UnaryOperatorExpression _:
                        UnaryOperatorExpression operatorExpression = (UnaryOperatorExpression)operand1;
                        if (!operatorExpression.IsDegenerated && operatorExpression.Operator == OperatorType.Minus)
                        {
                            if (index4 < 0 && index5 < 0)
                            {
                                index4 = index6;
                                break;
                            }
                            if (index4 >= 0)
                            {
                                BaseExpression operand2 = ((UnaryOperatorExpression)this.operands[index4]).Operand;
                                BaseExpression operand3 = operatorExpression.Operand;
                                this.operands[index4] = operand2;
                                this.operands[index6] = operand3;
                                index4 = -1;
                                flag1 = true;
                                break;
                            }
                            if (index5 >= 0)
                            {
                                LiteralExpression operand4 = (LiteralExpression)this.operands[index5];
                                BaseExpression baseExpression = (BaseExpression)LiteralExpression.Make(-num2);
                                BaseExpression operand5 = operatorExpression.Operand;
                                this.operands[index5] = baseExpression;
                                this.operands[index6] = operand5;
                                index5 = -1;
                                flag1 = true;
                                break;
                            }
                            break;
                        }
                        break;
                    case LiteralExpression _:
                        LiteralExpression expr = (LiteralExpression)operand1;
                        double num3 = 0.0;
                        ref double local = ref num3;
                        if (LiteralExpression.IsRealValue((BaseExpression)expr, ref local) && num3 < 0.0)
                        {
                            if (index4 < 0)
                            {
                                index5 = index6;
                                num2 = num3;
                                break;
                            }
                            BaseExpression operand6 = ((UnaryOperatorExpression)this.operands[index4]).Operand;
                            BaseExpression baseExpression = (BaseExpression)LiteralExpression.Make(-num3);
                            this.operands[index4] = operand6;
                            this.operands[index6] = baseExpression;
                            index4 = -1;
                            index5 = -1;
                            flag1 = true;
                            break;
                        }
                        break;
                }
            }
            int num4 = Utilities.SafeCount((IList)this.operands);
            if (num4 > 1 && !ProductExpression.IsInverseOperator(this.operators[0]) && (LiteralExpression.IsNegation(this.operands[0]) || UnaryOperatorExpression.IsNegation(this.operands[0])))
            {
                this.RemoveOperationLeft(0);
                this.Negate();
                flag1 = true;
                if (this.OperationCount == 0)
                {
                    this.operands[0].Simplify();
                    this.Degenerate();
                    return;
                }
            }
            else
            {
                for (int index7 = num4 - 1; index7 > 0; --index7)
                {
                    BaseExpression operand = this.operands[index7];
                    BaseExpression baseExpression = (BaseExpression)null;
                    if (operand is UnaryOperatorExpression)
                    {
                        UnaryOperatorExpression operatorExpression = operand as UnaryOperatorExpression;
                        if (!operatorExpression.IsDegenerated && operatorExpression.Operator == OperatorType.Minus)
                            baseExpression = operand;
                    }
                    if (operand is LiteralExpression)
                    {
                        LiteralExpression expr = operand as LiteralExpression;
                        double num5 = 0.0;
                        ref double local = ref num5;
                        if (LiteralExpression.IsRealValue((BaseExpression)expr, ref local) && num5 < 0.0)
                            baseExpression = operand;
                    }
                    if (baseExpression != null)
                    {
                        this.operands[index7] = UnaryOperatorExpression.Negate(this.operands[index7]);
                        this.operands[0] = UnaryOperatorExpression.Negate(this.operands[0]);
                        flag1 = true;
                        break;
                    }
                }
            }
            int count1 = this.operands.Count;
            if (count1 >= 2)
            {
                bool flag5 = false;
                for (int index8 = 0; index8 < count1 - 1; ++index8)
                {
                    BaseExpression x1 = (BaseExpression)null;
                    int num6 = 0;
                    string str1 = "";
                    bool[] flagArray = new bool[count1];
                    BaseExpression[] baseExpressionArray = new BaseExpression[count1];
                    int[] numArray1 = new int[count1];
                    int[] numArray2 = new int[count1];
                    for (int index9 = index8; index9 < count1; ++index9)
                    {
                        BaseExpression operand = this.operands[index9];
                        double x2 = 0.0;
                        if (!LiteralExpression.IsRealValue(operand, ref x2))
                        {
                            if (UnaryOperatorExpression.IsUnary(operand, OperatorType.Minus))
                            {
                                operand = (operand as UnaryOperatorExpression).Operand;
                                numArray2[index9] = 1;
                            }
                            string str2 = Symbols.MultiplyOperator.ToString();
                            OperatorType operatorType = OperatorType.Multiply;
                            if (index9 > 0)
                            {
                                str2 = this.operators[index9 - 1];
                                operatorType = Utilities.GetOperatorType(str2, OperatorArity.Binary);
                            }
                            int num7;
                            switch (operatorType)
                            {
                                case OperatorType.Multiply:
                                    num7 = 1;
                                    break;
                                case OperatorType.Divide:
                                    num7 = -1;
                                    break;
                                case OperatorType.Dot:
                                case OperatorType.Cross:
                                    continue;
                                default:
                                    return;
                            }
                            BaseExpression baseExpression1;
                            BaseExpression baseExpression2;
                            if (operand is PowerExpression)
                            {
                                PowerExpression powerExpression = operand as PowerExpression;
                                if (powerExpression.OperationCount == 1)
                                {
                                    baseExpression1 = powerExpression.Operands[0];
                                    baseExpression2 = powerExpression.Operands[1];
                                }
                                else
                                {
                                    baseExpression1 = (BaseExpression)powerExpression;
                                    baseExpression2 = (BaseExpression)LiteralExpression.Unit;
                                }
                            }
                            else
                            {
                                baseExpression1 = operand;
                                baseExpression2 = (BaseExpression)LiteralExpression.Unit;
                            }
                            numArray1[index9] = num7;
                            if (x1 != null)
                            {
                                if (baseExpression1.Reconstruct() == str1)
                                {
                                    baseExpressionArray[index9] = baseExpression2;
                                    flagArray[index9] = true;
                                    ++num6;
                                }
                            }
                            else
                            {
                                x1 = baseExpression1;
                                str1 = x1.Reconstruct();
                                baseExpressionArray[index9] = baseExpression2;
                                flagArray[index9] = true;
                                ++num6;
                            }
                        }
                    }
                    if (num6 > 1)
                    {
                        BaseExpression baseExpression3 = (BaseExpression)null;
                        for (int index10 = count1 - 1; index10 >= 0; --index10)
                        {
                            if (flagArray[index10])
                            {
                                if (index10 > 0)
                                    this.RemoveOperationRight(index10 - 1);
                                else if (this.operands.Count > 1)
                                {
                                    if (ProductExpression.IsInverseOperator(this.operators[0]))
                                    {
                                        baseExpression3 = this.operands[0];
                                        this.operands[0] = (BaseExpression)LiteralExpression.Unit;
                                    }
                                    else
                                        this.RemoveOperationLeft(0);
                                }
                                else
                                {
                                    baseExpression3 = this.operands[0];
                                    this.operands.Clear();
                                }
                            }
                        }
                        BaseExpression baseExpression4 = (BaseExpression)null;
                        bool flag6 = false;
                        int num8 = 0;
                        for (int index11 = 0; index11 < count1; ++index11)
                        {
                            if (flagArray[index11])
                            {
                                if (numArray1[index11] != 1)
                                {
                                    LiteralExpression x1_1 = LiteralExpression.Make((long)numArray1[index11]);
                                    baseExpressionArray[index11] = ProductExpression.MakeProduct((BaseExpression)x1_1, baseExpressionArray[index11]);
                                }
                                if (flag6)
                                {
                                    baseExpression4 = SumExpression.MakeSum(baseExpression4, baseExpressionArray[index11]);
                                }
                                else
                                {
                                    baseExpression4 = baseExpressionArray[index11];
                                    flag6 = true;
                                }
                                num8 += numArray2[index11];
                            }
                        }
                        baseExpression4.Simplify();
                        if (baseExpression4 is SumExpression)
                        {
                            SumExpression sumExpression = baseExpression4 as SumExpression;
                            if (sumExpression.IsDegenerated)
                                baseExpression4 = sumExpression.Degenerated;
                        }
                        BaseExpression baseExpression5 = PowerExpression.MakePower(x1, baseExpression4);
                        if (num8 % 2 == 1)
                        {
                            baseExpression5 = UnaryOperatorExpression.Negate(baseExpression5);
                            flag2 = true;
                        }
                        count1 = this.operands.Count;
                        if (count1 == 0)
                            this.operands.Add(baseExpression5);
                        else
                            this.InsertOperationRight(count1 - 1, Symbols.MultiplyOperator.ToString(), baseExpression5);
                        flag1 = true;
                        flag5 = true;
                    }
                    if (flag5)
                        break;
                }
            }
            int count2 = this.operands.Count;
            if (count2 > 0)
            {
                for (int index12 = 0; index12 < count2; ++index12)
                {
                    BaseExpression operand7 = this.operands[index12];
                    if (operand7 is PowerExpression)
                    {
                        PowerExpression opnd = (PowerExpression)operand7;
                        if (opnd.OperationCount == 1)
                        {
                            BaseExpression operand8 = opnd.Operands[1];
                            double x = 0.0;
                            if (UnaryOperatorExpression.IsUnary(operand8, OperatorType.Minus) || LiteralExpression.IsRealValue(operand8, ref x) && x < 0.0)
                            {
                                string op = Symbols.MultiplyOperator.ToString();
                                OperatorType operatorType = OperatorType.Multiply;
                                if (index12 > 0)
                                {
                                    op = this.operators[index12 - 1];
                                    operatorType = Utilities.GetOperatorType(op, OperatorArity.Binary);
                                }
                                switch (operatorType)
                                {
                                    case OperatorType.Multiply:
                                        if (UnaryOperatorExpression.IsNegation(operand8) || LiteralExpression.IsNegation(operand8))
                                            goto case OperatorType.Divide;
                                        else
                                            continue;
                                    case OperatorType.Divide:
                                        ProductExpression.InverseOperation(ref op);
                                        opnd.Operands[1] = UnaryOperatorExpression.Negate(opnd.Operands[1]);
                                        opnd.Operands[1].Simplify();
                                        opnd.Simplify();
                                        if (index12 > 0)
                                        {
                                            this.operands[index12] = (BaseExpression)null;
                                            this.operands[index12] = (BaseExpression)opnd;
                                            this.operators[index12 - 1] = op;
                                        }
                                        else
                                        {
                                            int operationCount = this.OperationCount;
                                            if (operationCount > 0)
                                            {
                                                if (ProductExpression.IsInverseOperator(this.operators[0]))
                                                {
                                                    this.operands[index12] = (BaseExpression)LiteralExpression.Unit;
                                                    this.InsertOperationRight(operationCount, op, (BaseExpression)opnd);
                                                }
                                                else
                                                {
                                                    this.operands[index12] = (BaseExpression)null;
                                                    this.RemoveOperationLeft(0);
                                                    this.InsertOperationRight(operationCount - 1, op, (BaseExpression)opnd);
                                                }
                                            }
                                            else
                                            {
                                                this.operands.Insert(0, (BaseExpression)LiteralExpression.Unit);
                                                if (this.operators == null)
                                                    this.operators = new List<string>();
                                                this.operators.Add(op);
                                            }
                                        }
                                        flag1 = true;
                                        goto label_136;
                                    case OperatorType.Dot:
                                    case OperatorType.Cross:
                                        continue;
                                    default:
                                        return;
                                }
                            }
                        }
                    }
                }
            }
        label_136:
            if (flag1)
                this.MergeOperands();
            if (!flag2)
                return;
            this.MergeConstants();
        }

        protected override void SimplifyBase()
        {
            if (this.OperationCount <= 0)
                return;
            for (int operation = this.operators.Count - 1; operation >= 0; --operation)
            {
                if (LiteralExpression.IsUnit(this.operands[operation + 1]))
                    this.RemoveOperationRight(operation);
            }
            if (this.OperationCount > 0)
            {
                List<BaseExpression> numerator;
                List<BaseExpression> denominator;
                List<string> noperators;
                List<string> doperators;
                try
                {
                    this.Decompose(out numerator, out denominator, out noperators, out doperators);
                }
                catch
                {
                    return;
                }
                if (BaseExpression.FindCount(numerator, (BaseExpression)LiteralExpression.Zero) > BaseExpression.FindCount(denominator, (BaseExpression)LiteralExpression.Zero))
                {
                    this.ClearOperations();
                    this.operands.Add((BaseExpression)LiteralExpression.Zero);
                    return;
                }
                if (BaseExpression.RemoveSame(numerator, denominator, noperators, doperators) > 0)
                {
                    int count1 = numerator.Count;
                    int count2 = denominator.Count;
                    if (count1 == 0 && count2 == 0)
                    {
                        this.ClearOperations();
                        this.operands.Add((BaseExpression)LiteralExpression.Unit);
                        return;
                    }
                    if (count2 == 0)
                    {
                        this.ClearOperations();
                        for (int index = 0; index < count1; ++index)
                        {
                            this.operands.Add(numerator[index]);
                            if (index != count1 - 1)
                                this.operators.Add(noperators[index]);
                        }
                    }
                    else if (count1 == 0)
                    {
                        this.ClearOperations();
                        this.operands.Add((BaseExpression)LiteralExpression.Unit);
                        string operatorSign = Utilities.GetOperatorSign(OperatorType.Divide);
                        for (int index = 0; index < count2; ++index)
                        {
                            this.operators.Add(operatorSign);
                            this.operands.Add(denominator[index]);
                        }
                    }
                    else
                    {
                        this.ClearOperations();
                        for (int index = 0; index < count1; ++index)
                        {
                            this.operands.Add(numerator[index]);
                            if (index != count1 - 1)
                                this.operators.Add(noperators[index]);
                        }
                        string operatorSign = Utilities.GetOperatorSign(OperatorType.Divide);
                        for (int index = 0; index < count2; ++index)
                        {
                            this.operators.Add(operatorSign);
                            this.operands.Add(denominator[index]);
                        }
                    }
                }
            }
            if (this.OperationCount <= 0 || Utilities.GetOperatorType(this.operators[0], OperatorArity.Binary) != OperatorType.Multiply || !LiteralExpression.IsUnit(this.operands[0]))
                return;
            this.RemoveOperationLeft(0);
        }

        private bool MustMergeSpecial(BaseExpression expr)
        {
            switch (expr)
            {
                case SumExpression _:
                case VariableExpression _:
                    return true;
                default:
                    return expr is FunctionExpression;
            }
        }

        protected bool MergeSpecialConst()
        {
            bool flag = false;
            if (this.OperationCount == 1)
            {
                BaseExpression operand1 = this.operands[0];
                BaseExpression operand2 = this.operands[1];
                BaseExpression expr = (BaseExpression)null;
                if (UnaryOperatorExpression.IsNegation(operand2) || LiteralExpression.IsNegation(operand2))
                {
                    if (this.MustMergeSpecial(operand1))
                        expr = operand1;
                }
                else if ((UnaryOperatorExpression.IsNegation(operand1) || LiteralExpression.IsNegation(operand1)) && this.MustMergeSpecial(operand2) && Utilities.GetOperatorType(this.operators[0], OperatorArity.Binary) != OperatorType.Divide)
                    expr = operand2;
                if (expr != null)
                {
                    BaseExpression baseExpression;
                    if (expr is SumExpression)
                    {
                        SumExpression sumExpression = expr as SumExpression;
                        sumExpression.Negate();
                        baseExpression = (BaseExpression)sumExpression;
                    }
                    else
                        baseExpression = UnaryOperatorExpression.Negate(expr);
                    this.ClearOperations();
                    this.operands.Add(baseExpression);
                    flag = true;
                }
            }
            return flag;
        }

        protected override void MergeConstants()
        {
            if (this.MergeSpecialConst())
                return;
            bool[] iv = (bool[])null;
            double[] rv = (double[])null;
            if (this.GetRealValues(ref iv, ref rv) < 2)
                return;
            int num1 = Utilities.SafeCount((IList)this.operands);
            double num2 = 1.0;
            double num3 = 1.0;
            double num4 = 1.0;
            double num5 = 1.0;
            long ivalue = 0;
            for (int index = num1 - 1; index >= 0; --index)
            {
                if (iv[index])
                {
                    OperatorType t = index != 0 ? Utilities.GetOperatorType(this.operators[index - 1], OperatorArity.Binary) : OperatorType.Multiply;
                    bool flag = Utilities.IsInteger(rv[index], out ivalue);
                    switch (t)
                    {
                        case OperatorType.Multiply:
                            if (flag)
                                num2 *= (double)ivalue;
                            else
                                num4 *= rv[index];
                            if (index > 0)
                            {
                                this.RemoveOperationRight(index - 1);
                                continue;
                            }
                            continue;
                        case OperatorType.Divide:
                            if (flag)
                                num3 *= (double)ivalue;
                            else
                                num5 *= rv[index];
                            if (index > 0)
                            {
                                this.RemoveOperationRight(index - 1);
                                continue;
                            }
                            continue;
                        case OperatorType.Dot:
                        case OperatorType.Cross:
                            if (index < this.OperationCount && Utilities.GetOperatorType(this.operators[index], OperatorArity.Binary) == OperatorType.Multiply)
                            {
                                if (flag)
                                    num2 *= (double)ivalue;
                                else
                                    num4 *= rv[index];
                                this.RemoveOperationLeft(index);
                                continue;
                            }
                            continue;
                        default:
                            return;
                    }
                }
            }
            bool flag1 = false;
            double num6 = num2 * num4 / (num3 * num5);
            bool flag2;
            bool flag3;
            BaseExpression opnd;
            if (Utilities.IsInteger(num6, out ivalue))
            {
                double num7 = (double)ivalue;
                flag2 = ivalue == 1L;
                flag3 = ivalue == -1L;
                opnd = !(flag2 | flag3) ? (BaseExpression)LiteralExpression.Make(num7) : (BaseExpression)LiteralExpression.Unit;
            }
            else
            {
                flag2 = false;
                flag3 = false;
                long num8;
                long den;
                if (Utilities.IsFraction(num6, out num8, out den))
                {
                    opnd = ProductExpression.MakeDivision((BaseExpression)LiteralExpression.Make(num8), (BaseExpression)LiteralExpression.Make(den));
                    flag1 = true;
                }
                else
                    opnd = (BaseExpression)LiteralExpression.Make(num6);
            }
            int operationCount = this.OperationCount;
            if (flag3)
                flag2 = true;
            if (iv[0])
            {
                if (flag2 && operationCount > 0)
                {
                    OperatorType operatorType = Utilities.GetOperatorType(this.operators[0], OperatorArity.Binary);
                    switch (operatorType)
                    {
                        case OperatorType.Multiply:
                            this.RemoveOperationLeft(0);
                            break;
                        case OperatorType.Divide:
                            this.operands[0] = opnd;
                            break;
                        default:
                            return;
                    }
                }
                else
                    this.operands[0] = opnd;
            }
            else if (!flag2)
                this.InsertOperationLeft(0, Utilities.GetOperatorSign(OperatorType.Multiply), opnd);
            if (flag3)
                this.Negate();
            if (flag1)
                this.MergeOperands();
            this.MergeSpecialConst();
        }

        public static BaseExpression MakeOperation(
          BaseExpression operand1,
          BaseExpression operand2,
          string sign)
        {
            List<string> theOperators = new List<string>((IEnumerable<string>)new string[1]
            {
        sign
            });
            List<BaseExpression> theOperands = new List<BaseExpression>((IEnumerable<BaseExpression>)new BaseExpression[2]
            {
        operand1,
        operand2
            });
            return (BaseExpression)new ProductExpression(Symbols.NotDefinedSign, theOperators, theOperands);
        }

        public void Negate() => this.operands[0] = UnaryOperatorExpression.Negate(this.operands[0]);

        public static BaseExpression MakeProduct(BaseExpression x1, BaseExpression x2)
        {
            if (LiteralExpression.IsNaN(x1) || LiteralExpression.IsNaN(x2))
                return (BaseExpression)LiteralExpression.NaN;
            bool flag1 = LiteralExpression.IsZero(x1);
            bool flag2 = LiteralExpression.IsZero(x2);
            bool flag3 = LiteralExpression.IsInfinity(x1);
            bool flag4 = LiteralExpression.IsInfinity(x2);
            if (flag1 & flag2)
                return (BaseExpression)LiteralExpression.Zero;
            if (flag1 && !flag4)
                return (BaseExpression)LiteralExpression.Zero;
            if (flag2 && !flag3)
                return (BaseExpression)LiteralExpression.Zero;
            if (LiteralExpression.IsUnit(x1))
                return x2;
            if (LiteralExpression.IsUnit(x2))
                return x1;
            if (LiteralExpression.IsNegation(x1) || UnaryOperatorExpression.IsNegation(x1))
                return UnaryOperatorExpression.Negate(x2);
            return LiteralExpression.IsNegation(x2) || UnaryOperatorExpression.IsNegation(x2) ? UnaryOperatorExpression.Negate(x1) : ProductExpression.MakeOperation(x1, x2, Symbols.MultiplyOperator.ToString());
        }

        /// <summary>
        /// Makes division of two operands x1/x2.
        /// Simplifies result using equalities:
        /// NaN/x2 or x1/NaN = NaN
        /// x1/1 = x1
        /// 0/x2 = 0, x2 is not zero
        /// x1/(-1) = -x1
        /// </summary>
        public static BaseExpression MakeDivision(BaseExpression x1, BaseExpression x2)
        {
            if (LiteralExpression.IsNaN(x1) || LiteralExpression.IsNaN(x2))
                return (BaseExpression)LiteralExpression.NaN;
            if (LiteralExpression.IsUnit(x2))
                return x1;
            int num = LiteralExpression.IsZero(x1) ? 1 : 0;
            bool flag = LiteralExpression.IsZero(x2);
            if (num != 0 && !flag)
                return (BaseExpression)LiteralExpression.Zero;
            return LiteralExpression.IsNegation(x2) || UnaryOperatorExpression.IsNegation(x2) ? UnaryOperatorExpression.Negate(x1) : ProductExpression.MakeOperation(x1, x2, Symbols.DivideOperator.ToString());
        }

        public static BaseExpression SimplifyProduct(List<BaseExpression> multipliers)
        {
            if (multipliers == null || multipliers.Count == 0)
                return (BaseExpression)null;
            int count = multipliers.Count;
            BaseExpression x1 = multipliers[0];
            for (int index = 1; index < count; ++index)
                x1 = ProductExpression.MakeProduct(x1, multipliers[index]);
            return x1;
        }

        public static bool IsRational(BaseExpression expr, out Fraction r)
        {
            r = new Fraction(1L);
            BaseExpression baseExpression = expr.This;
            bool flag1 = baseExpression is ProductExpression;
            if (flag1)
            {
                ProductExpression productExpression = baseExpression as ProductExpression;
                int operationCount = productExpression.OperationCount;
                string op = Symbols.MultiplyOperator.ToString();
                long numerator = 1;
                long denominator = 1;
                for (int index = 0; index <= operationCount; ++index)
                {
                    double x = 0.0;
                    bool flag2 = LiteralExpression.IsRealValue(productExpression.Operands[index], ref x);
                    if (!flag2)
                        return flag2;
                    long ivalue;
                    flag1 = Utilities.IsInteger(x, out ivalue);
                    if (!flag1)
                        return flag1;
                    if (index > 0)
                        op = productExpression.Operators[index - 1];
                    if (ProductExpression.IsInverseOperator(op))
                        denominator *= ivalue;
                    else
                        numerator *= ivalue;
                }
                r = new Fraction(numerator, denominator);
            }
            return flag1;
        }

        public static BaseExpression OneHalf => (BaseExpression)new LiteralExpression(Symbols.UnitString) / 2L;

        public static BaseExpression OneThird => (BaseExpression)new LiteralExpression(Symbols.UnitString) / 3L;
    }
}
