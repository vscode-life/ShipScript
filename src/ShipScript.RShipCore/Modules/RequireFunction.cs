﻿using ShipScript.Common;

namespace ShipScript.RShipCore
{
    [NativeObject("Function")]
    public class RequireFunction : IScriptNativeObject
    {
        private readonly IScriptEvaluator scriptEvaluator;

        private object function;
        private bool loaded;

        public RequireFunction(IScriptEvaluator scriptEvaluator, IModuleLoader loader, Module module)
        {
            this.scriptEvaluator = scriptEvaluator;
            Loader = loader;
            Module = module;
        }

        public IModuleLoader Loader { get; }

        public Module Module { get; }

        [ScriptMember("main")]
        public Module Main => Loader.MainModule;

        [ScriptMember("invoke")]
        public Module Invoke(string id)
        {
            return Module.Require(id);
        }

        [ScriptMember("resolve")]
        public string Resolve()
        {
            return Module.FileName;
        }

        public object GetScriptObject()
        {
            if (loaded)
            {
                return function;
            }

            function = ((dynamic)scriptEvaluator.Evaluate("native", @"
                (function (nativeRequire) { 
                    function require(id) {
                        var module = nativeRequire.invoke(id);
                        return module.exports;
                    }

                    function resolve() { return nativeRequire.resolve(); }
                    Object.defineProperty(require, 'resolve', { value: resolve, enumerable: true  });
                    Object.defineProperty(require, 'main', { get: () => nativeRequire.main, enumerable: true });

                    var outer = toString;
                    function toString() { return 'function toString() { [native code] }' }
                    Object.defineProperty(toString, 'toString', { value: outer });

                    function requireToString() { return 'function require() { [native code] }'; }
                    Object.defineProperty(require, 'toString', { value: requireToString });
                    Object.defineProperty(requireToString, 'toString', { value: toString });
                    function resolveToString() { return 'function resolve() { [native code] }'; }
                    Object.defineProperty(resolve, 'toString', { value: resolveToString });
                    Object.defineProperty(resolveToString, 'toString', { value: toString });

                    return require;
                }).valueOf()"))(this);
            loaded = true;
            return function;
        }
    }
}