<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Calculator.ascx.cs" Inherits="widgets_Calculator" %>
<link rel="Stylesheet" type="text/css" href="<%= Page.ResolveClientUrl(AppRelativeTemplateSourceDirectory) %>Calculator/css/Calculator.css" />
<div style="width: 100%">
    <center>
        <div id="<%= ClientID %>" class="calculator" style="max-width: 300px;">
            <table>
                <tbody>
                    <tr>
                        <td colspan="4">
                            <div class="calculator-output">
                                0</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <input type="button" value="C" on onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Clear();" /></td>
                        <td>
                            <input type="button" value="%" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].OperateUnary(UnaryOperators.Percent);" /></td>
                        <td>
                            <input type="button" value="1/x" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].OperateUnary(UnaryOperators.Reciprocal);" /></td>
                        <td>
                            <input type="button" value="+" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Operate(Operators.Add);" /></td>
                    </tr>
                    <tr>
                        <td>
                            <input type="button" value="7" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Digit(7);" /></td>
                        <td>
                            <input type="button" value="8" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Digit(8);" /></td>
                        <td>
                            <input type="button" value="9" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Digit(9);" /></td>
                        <td>
                            <input type="button" value="-" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Operate(Operators.Subtract);" /></td>
                    </tr>
                    <tr>
                        <td>
                            <input type="button" value="4" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Digit(4);" /></td>
                        <td>
                            <input type="button" value="5" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Digit(5);" /></td>
                        <td>
                            <input type="button" value="6" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Digit(6);" /></td>
                        <td>
                            <input type="button" value="*" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Operate(Operators.Multiply);" /></td>
                    </tr>
                    <tr>
                        <td>
                            <input type="button" value="1" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Digit(1);" /></td>
                        <td>
                            <input type="button" value="2" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick=" window.controls['<%= ClientID %>'].Digit(2);" /></td>
                        <td>
                            <input type="button" value="3" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Digit(3);" /></td>
                        <td>
                            <input type="button" value="/" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Operate(Operators.Divide);" /></td>
                    </tr>
                    <tr>
                        <td>
                            <input type="button" value="0" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Digit(0);" /></td>
                        <td>
                            <input type="button" value="+/-" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].OperateUnary(UnaryOperators.Negate);" /></td>
                        <td>
                            <input type="button" value="." onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Decimal();" /></td>
                        <td>
                            <input type="button" value="=" onkeypress="return window.controls['<%= ClientID %>'].KeyPress(event);" onclick="window.controls['<%= ClientID %>'].Evaluate();" /></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </center>
</div>
