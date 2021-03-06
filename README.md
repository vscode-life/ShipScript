## About
ShipScript is a [ClearScript](http://clearscript.codeplex.com) based runtime inspired by Node.js. It supports dynamic module loading in the same manner as Node.js by using the require function.

While ClearScript allows adding scripting from within .NET applications, the purpose of this framework is to allow script access to existing .NET libraries by pure JavaScript without any .NET code. This means that you can require() .dll's and have access to their types in script.

ShipScript can be thought of as the synchronous version of Node.js, aimed at single user procedural applications with an easy way to bootstrap business-layer .NET components.

The second way of using ShipScript is to host it inside your own application. The embedded core supports adding your components as requirable modules, a virtual console for redirected I/O, hosting your own REPL on those I/O's, piping mechanisms, and many other planned features.

---

## Features
* High performance V8 JavaScript engine supplied by ClearScript
* Standalone .exe launcher that runs supplied script files, with REPL support
* Require .ship, .js, .json, and .dll (.NET) files, with support to add custom compilers
* _function (exports, require, module, __filename, __dirname) { code }_ wrapper for script modules
* Required assemblies can have their types imported and instantiated from script
* Piping API to connect native components with script callbacks (similar to Node events)
* TPL Task and Task(T) to JavaScript Promises conversion
* Virtual console that can be piped to any output
* Easy to embed core in any .NET application
* Easy to add or replace native modules

---

## How to build and run
1. Build > Transform All T4 Templates to make sure the latest scripts are used before building
2. Build the solution
3. Open ship.exe from /bin/[Debug|Release]
4. Optionally link .ship files to launcher (ship.exe) and/or setup a system variable for it

---

## Examples
### Example 1
hello.js launched from ship.exe
```javascript
console.log('Hello world');

// Just like a C# console app, we need to block with something to prevent closing
console.read();
```
### Example 2
main.js launched from ship.exe
```javascript
var myLib = require('./library.js');

var result = myLib.getResult();
console.log(result);

// Expose the result to global namespace
global.result = result;

// Instead of closing, we can start a REPL session on current application to inspect our application state
core.sleep(true);
// If we pass false to sleep, an infinite read loop is started without REPL
// so we can pipe the 'stdin' module to any custom handler or .stop() the loop
```
library.js in the same folder as main.js
```javascript
var data = require('./data.json');
var internalResult = data.result;

exports.getResult = function () {
    return internalResult;
}
```
data.json in the same folder as main.js
```json
{ "result": 42 }
```
### Example 3
main.ship launched from ship.exe
```javascript
var lib = require('./MyLibrary.dll');
var name = 'ShipScript';

var myClrObject = new lib.MyLibrary.MyBusinessLogic.MyClass(name);
myClrObject.SayHello();
myClrObject.Name = 'Clr Object';
myClrObject.SayHello();

var sum = myClrObject.AddNumbers(2, 3);
console.log('2 + 3 = ' + sum);

// Start REPL after module runs
core.sleep();
```
MyLibrary.dll in the same directory as main.ship containing this class:
```cs
namespace MyLibrary.MyBusinessLogic
{
    public class MyClass
    {
        public MyClass(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public void SayHello()
        {
            Console.WriteLine("Hello " + Name + "!");
        }

        public int AddNumbers(int a, int b)
        {
            return a + b;
        }
    }
}
```
