using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleScriptingEngine
{
    class Program
    {
        static void Main(string[] args)
        {

            var engine = new ScriptEngine();
            engine.Code = "Result = (VehicleType == 90);";
            Console.WriteLine("evaluate: " + engine.Code);
            engine.AddProperty("VehicleType", "int");
            engine.Compile();
            engine.SetProperty("VehicleType", 90);
            Console.WriteLine("Vehicle Type is:" + engine.GetProperty("VehicleType"));

            var result = engine.Evaluate();

            Console.WriteLine("Result: " + result.ToString());
            Console.ReadLine();
        }
    }
}
