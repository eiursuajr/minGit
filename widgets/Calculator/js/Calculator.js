var Operators = {Add : function(a, b) {return a + b;}, 
                 Subtract : function(a, b) {return a - b;}, 
                 Multiply : function(a, b) {return a * b;}, 
                 Divide : function(a, b) {return a / b;}, 
                 Evaluate : function(a, b) {return b;}, 
                 Blank : function(a, b) {return b;}};

var UnaryOperators = {Percent : function(a) {return a/100;}, 
                      Negate : function(a) {return -a;}, 
                      Reciprocal : function(a) {return 1/a;}};
                 

function Calculator(id)
{
    if(typeof window.controls == "undefined")
        window.controls = {};
    else if(typeof window.controls[id] != "undefined")
        return window.controls[id];
    
    window.controls[id] = this;
    
    var _id = id, _currentValue = 0, _previousValue, _operator = Operators.Blank;
    var _isDecimal = false, _power = 1, _decimalString = "";
   
    function _Display(value)
    {
        var divOutput = $ektron("#" + _id + " .calculator-output");
        divOutput.html(value == 0 ? "0" : value);
    };
    
    this.Clear = function()
    {
        _isDecimal = false;
        _currentValue = _previousValue = 0;
        _operator = Operators.Blank;
        _Display("0");
    };
    
    this.Operate = function(operator)
    {
        _isDecimal = false;
        _previousValue = _operator(_previousValue, _currentValue);
        _operator = operator;
        _currentValue = 0;
        _Display(_previousValue);
    };
    
    this.OperateUnary = function(unaryOperator)
    {
        _currentValue = unaryOperator(_currentValue);
        _Display(_currentValue);
    };
    
    this.Evaluate = function()
    {
        _isDecimal = false;
        _currentValue = _operator(_previousValue, _currentValue);
        _Display(_currentValue);
        _operator = Operators.Evaluate;
    };
    
    this.Decimal = function()
    {
        if(_isDecimal == false)
        {
            _power = 1;
            _isDecimal = true;
            _decimalString = "";
            
            if(_operator == Operators.Evaluate)
            {
                _operator = Operators.Blank;
                _currentValue = 0;
            }
        }
    };
    
    this.Digit = function(value)
    {
        if(_isDecimal)
        {
            _decimalString += value;
            _power /= 10;
            value *= _power;
        }
        else
        {
            _currentValue *= 10;
        }
        
        if(_currentValue < 0)
            value *= -1;
        
        if(_operator == Operators.Evaluate)
        {
            _operator = Operators.Blank;
            _currentValue = 0;
        }
        
        _currentValue += value;
        
        if(_isDecimal)  
        {
            _Display((_currentValue >= 0 ? Math.floor(_currentValue) : Math.ceil(_currentValue)) + "." + _decimalString);
        }
        else
            _Display(_currentValue);
    };
    
    this.KeyPress = function(e)
    {
        var code;
        if (!e) var e = window.event;
	    if (e.keyCode) code = e.keyCode;
	    else if (e.which) code = e.which;
	    var character = String.fromCharCode(code);
        if(character == '0'){
            this.Digit(0);
        }else if (character == '1'){
            this.Digit(1);
        }else if (character == '2'){
            this.Digit(2);
        }else if (character == '3'){
            this.Digit(3);
        }else if (character == '4'){
            this.Digit(4);
        }else if (character == '5'){
            this.Digit(5);
        }else if (character == '6'){
            this.Digit(6);
        }else if (character == '7'){
            this.Digit(7);
        }else if (character == '8'){
            this.Digit(8);
        }else if (character == '9'){
            this.Digit(9);
        }else if (character == '+'){
            this.Operate(Operators.Add);
        }else if (character == '-'){
            this.Operate(Operators.Subtract);
        }else if (character == '*'){
            this.Operate(Operators.Multiply);;
        }else if (character == '/'){
            this.Operate(Operators.Divide);;
        }else if (character == '^'){
            this.OperateUnary(UnaryOperators.Negate);
        }else if (character == '%'){
            this.OperateUnary(UnaryOperators.Percent);
        }else if (character == '\\'){
            this.OperateUnary(UnaryOperators.Reciprocal);
        }else if (code == 13){
            this.Evaluate();
        }else if (character == '.'){
            this.Decimal();
        }else if (character == 'C'){
            this.Clear();
        }
        return false;
    };
}

if( typeof Sys != "undefined" ){
   Sys.Application.notifyScriptLoaded();
}