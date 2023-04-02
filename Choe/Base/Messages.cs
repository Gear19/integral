using System;

namespace Choe
{
  public static class Messages
  {
    public static string NullArgumentNotAllowedError = "NULL argument not allowed.";
    public static string MethodArgumentFormat = "Method - '{0}', argument - '{1}'.";
    public static string InvalidNameFormat = "Invalid name - '{0}'.";
    public static string ExpectedGotFormat = "Expected '{0}' got '{1}'.";
    public static string OperationNotApplicableFormat = "Operation '{0}' not appicable for '{1}'.";
    public static string VariableValueTypeError = "Error of variable value type.";
    public static string IndexCountError = "Error of index count.";
    public static string IndexTypeError = "Error of index type.";
    public static string IndexValueError = "Index value is not Integer.";
    public static string ValueIntegerErrorFormat = "Argument value must be integer for '{0}'.";
    public static string ValueNonnegativeErrorFormat = "Argument value must be nonnegative for '{0}'.";
    public static string ParameterCountError = "Error of parameter count.";
    public static string ParameterTypeError = "Error of parameter type.";
    public static string ArgumentCountError = "Error of argument count.";
    public static string ArgumentTypeError = "Error of argument type.";
    public static string OperandCountError = "Error of operand count.";
    public static string EmptyFormulaError = "Empty formula.";
    public static string UnknownLiteralError = "Unknown literal";
    public static string UnknownExpressionFormat = "Unknown expression '{0}'.";
    public static string UnsupportedExpressionTypeError = "Unsupported expression type";
    public static string VariableNotFoundError = "Variable NOT found";
    public static string UnknownOperatorError = "Unknown operator";
    public static string UnaryOperatorNotFoundFormat = "Unary operator '{0}' not found for operand type '{1}'.";
    public static string BinaryOperatorNotFoundFormat = "Binary operator '{0}' not found for operand types '{1}' and '{2}'.";
    public static string FunctionNotFoundError = "Function not found";
    public static string SlicingNotImplementedFormat = "Slicing not implemented for '{0}'.";
    public static string OperatorDerivativeNotDefinedFormat = "Derivative not defined for '{0}' operator." + Environment.NewLine + "Derivative can be calculated for the operand(s) not depending on the variable.";
    public static string OperationsDerivativeNotDefinedFormat = "Derivative not defined for '{0}' operations." + Environment.NewLine + "Derivative can be calculated for the operations not depending on the variable.";
    public static string FunctionDerivativeNotDefinedFormat = "Derivative not defined for function '{0}'." + Environment.NewLine + "Derivative can be calculated for the argument(s) not depending on the variable.";
    public static string FunctionDerivativeParameterNoDependencyFormat = "Function '{0}' parameter(s) must not depend on variable for derivation.";
    public static string OperationForbidden1Format = "Operation {0} FORBIDDEN for {1} type.";
    public static string OperationForbidden2Format = "Operation {0} FORBIDDEN for {1} and {2} types.";
    public static string IntegralNotEvaluatedFormat = "Integral cannot be evaluated for '{0}' expression.";
    public static string IntegralNotDefinedFormat = "Integral not defined for %s expression type ({0}).";
    public static string FunctionIntegralNotDefinedFormat = "Integral not defined for function {0}." + Environment.NewLine + "Integral can be calculated for the argument(s) not depending on the variable.";
    public static string FunctionIntegralParameterNoDependencyFormat = "Function {0} parameter(s) must not depend on variable for integration.";
    public static string FunctionIntegralArgumentCountFormat = "Function {0} not supported for integration." + Environment.NewLine + "Only 1-argument functions allowed";
    public static string SyntaxErrorMessage = "Syntax error.";
    public static string UnexpectedParanthesisFormat = "Unexpected paranthesis '{0}'. It must be preceeded by '{1}'.";
    public static string ParanthesisExpectedButFoundFormat = "Paranthesis '{0}' expected but '{1}' found.";
    public static string ParanthesisExpectedFormat = "Paranthesis '{0}' expected.";
    public static string ParanthesisMustBeClosedFormat = "Paranthesis '{0}' must be closed before '{1}' symbol.";
    public static string ClosingSymbolParityErrorFormat = "Error of symbol '{0}' parity.";
    public static string InvalidConstructionFormat = "Invalid construction '{0}'.";
    public static string InvalidSymbolFormat = "Symbol '{0}' not allowed in an expression.";
    public static string InvalidBeginSymbolFormat = "Symbol '{0}' cannot be the first symbol in an expression.";
    public static string InvalidEndSymbolFormat = "Symbol '{0}' cannot be the last symbol in an expression.";
  }
}
