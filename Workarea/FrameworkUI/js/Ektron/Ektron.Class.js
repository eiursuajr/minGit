// This function implements multiple inheritance and is compatible with prototype inheritance.
Ektron.Class =
    {
        functionName: function(fn) {
            if (typeof fn != "function") throw new TypeError("fn must be of type Function");
            var a = fn.toString().match(/function (\w+)\(/);
            return (a != null ? a[1] : "anonymous");
        },
        nonEnumerables: ["toLocaleString", "toString", "valueOf"],
        inherits: function(thisObject, objBase, baseClassName)
        /*
        USAGE:
        Example 1
        function MyClass(arg1, arg2, ...)
        {
        Ektron.Class.inherits(this, new MyBaseClass(arg1, arg2, ...));
        this.myMethod = function(arg1, arg2, ...)
        {
        this.MyBaseClass_myMethod(arg1, arg2, ...); // call base class method
        };
        }
        var objMy = new MyClass(arg1, arg2, arg3);
        Example 2
        function MyClass(base, arg1, arg2, ...)
        {
        Ektron.Class.inherits(this, base, "base");
        this.myMethod = function(arg1, arg2, ...)
        {
        this.base_myMethod(arg1, arg2, ...); // call base class method
        };
        }
        var objMyBase = new MyBaseClass(arg1, arg2, ...);
        var objMy = new MyClass(objMyBase, arg1, arg2, arg3);
        Note: multiple inheritance is supported
        */
        {
            if (typeof thisObject != "object") throw new TypeError("thisObject must be of type Object");
            if (null === thisObject) throw new RangeError("thisObject is null");
            if (typeof objBase != "object") throw new TypeError("objBase must be of type Object");
            if (null === objBase) throw new RangeError("objBase is null");
            if (typeof objBase.constructor != "function") throw new TypeError("objBase.constructor must be of type Function");
            if (typeof baseClassName != "string" && typeof baseClassName != "undefined") throw new TypeError("baseClassName must be of type String or undefined");
            if ("undefined" == typeof baseClassName) {
                for (var p in objBase.constructor.prototype) {
                    throw new TypeError("baseClassName must be specified when objBase is derived using prototype");
                }
            }
            if (typeof baseClassName != "string") {
                baseClassName = Ektron.Class.functionName(objBase.constructor);
            }
            if ("anonymous" == baseClassName) throw new TypeError("baseClassName must be specified when objBase constructor is anonymous");
            // Copy properties from base object to thisObject
            for (var p in objBase) {
                if (p != "constructor") {
                    thisObject[p] = objBase[p];
                    if ("function" == typeof objBase[p]) {
                        thisObject[baseClassName + "_" + p] = objBase[p];
                    }
                }
            }
            for (var i in Ektron.Class.nonEnumerables) {
                thisObject[baseClassName + "_" + Ektron.Class.nonEnumerables[i]] = objBase[Ektron.Class.nonEnumerables[i]];
            }
            return thisObject;
        },
        overrides: function(baseClassName, methods)
        /*
        baseClassName: (optional) the (arbitrary) name of the base class, used to access base methods
        methods: (optional) array of method names (as strings) to override.
        If undefined, all methods are overridable.
        */
        {
            if (typeof baseClassName != "string" && typeof baseClassName != "undefined") throw new TypeError("baseClassName must be of type String or undefined");
            if (typeof baseClassName != "string") baseClassName = Ektron.Class.functionName(objBase.constructor);
            if (typeof methods != "undefined" && methods != null && methods.constructor != Array) throw new TypeError("methods must be of type Array or undefined");
            return function(objBase, args)
            /*
            USAGE:
            function MyClass(arg_1, arg_2, ...)
            {
            this.myMethod = function(arg_m1, arg_m2, ...)
            {
            this.base_myMethod(arg_m1, arg_m2, ...); // call base class method
            };
            }
            MyClass.overrides = Ektron.Class.overrides("base", [ "myMethod" ]);
            var objMyBase = new MyBaseClass(arg_b1, arg_b2, ...);
            MyClass.overrides(objMyBase, [arg_1, arg_2, ...]);
            */
            {
                if (typeof objBase != "object") throw new TypeError("objBase must be of type Object");
                if (null === objBase) throw new RangeError("objBase is null");
                if (typeof objBase.constructor != "function") throw new TypeError("objBase.constructor must be of type Function");
                if (typeof args != "object" && typeof args != "undefined") throw new TypeError("args must be of type Array or undefined");
                if ("undefined" == typeof args) args = [];
                objBase.constructor = this;
                var name = "";
                // Copy base methods in case they are overridden
                if ("object" == typeof methods && methods.constructor == Array) {
                    for (var i = 0; i < methods.length; i++) {
                        name = baseClassName + "_" + methods[i];
                        if ("undefined" == typeof objBase[name]) {
                            objBase[name] = objBase[methods[i]];
                        }
                    }
                }
                else {
                    var aryBaseMethods = [];
                    for (var p in objBase) {
                        if (("function" == typeof objBase[p]) && (p != "constructor")) {
                            aryBaseMethods[p] = objBase[p];
                        }
                    }
                    for (var p in aryBaseMethods) {
                        name = baseClassName + "_" + p;
                        if ("undefined" == typeof objBase[name]) {
                            objBase[name] = aryBaseMethods[p];
                        }
                    }
                    for (var i in Ektron.Class.nonEnumerables) {
                        name = baseClassName + "_" + Ektron.Class.nonEnumerables[i];
                        if ("undefined" == typeof objBase[name]) {
                            objBase[name] = objBase[Ektron.Class.nonEnumerables[i]];
                        }
                    }
                }
                // 'this' is the derived class constructor function
                this.apply(objBase, args);
                return objBase;
            };
        }
    };