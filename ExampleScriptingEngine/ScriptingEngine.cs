using System;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

namespace ExampleScriptingEngine
{

    public class ScriptEngine
    {
        private AppDomain domain = null;
        private dynamic evaluator = null;
        private Type evaluatorType = null;

        private string source { get; set; }
        public string Source { get { return source; } }

        private StringBuilder variables { get; set; }
        public string Code { get; set; }
        public string[] Messages = null;

        public ScriptEngine()
        {
            domain = AppDomain.CreateDomain("ScriptEngine");
            variables = new StringBuilder();
        }

        public void Unload()
        {
            if (domain != null)
            {
                AppDomain.Unload(domain);
                domain = null;
            }
        }

        public void AddProperty(string PropertyName, string PropertyType)
        {
            //refactor to be true property
            const string propertyFormat = "public {0} {1} {{get; set;}}";
            variables.AppendLine(string.Format(propertyFormat, PropertyType, PropertyName));

        }

        public void SetProperty(string PropertyName, object Value)
        {
            object o = evaluatorType.InvokeMember(
                        PropertyName,
                        BindingFlags.SetProperty,
                        null,
                        evaluator,
                        new object[] { Value }
                     );
        }

        public object GetProperty(string PropertyName)
        {
            object o = evaluatorType.InvokeMember(
                        PropertyName,
                        BindingFlags.GetProperty,
                        null,
                        evaluator,
                        new object[] { }
                     );

            return o;
        }

        public bool Compile()
        {
            Messages = null;

            var compiler = new CSharpCodeProvider();

            //Concat source code
            source = GenSource();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            CompilerResults results = compiler.CompileAssemblyFromSource(parameters, source);

            // Check for compile errors / warnings
            if (results.Errors.HasErrors || results.Errors.HasWarnings)
            {
                Messages = new string[results.Errors.Count];
                for (int i = 0; i < results.Errors.Count; i++)
                    Messages[i] = results.Errors[i].ToString();
                return false;
            }
            else
            {
                Assembly assembly = results.CompiledAssembly;
                Type[] tt = assembly.GetTypes();
                evaluatorType = assembly.GetType("UserScript.RunScript");
                evaluator = Activator.CreateInstance(evaluatorType);

                return true;
            }
        }

        private string GenSource()
        {
            const string sourceFormat = @"namespace UserScript
                       {{
                          using System;
                          using System.Text;
                          public class RunScript
                          {{
                              //Properties
                              {0}
                              public bool? Eval()
                              {{
                                 bool? Result = null;
                                 //Code 
                                 {1}
                                return Result;
                               }}
                          }}
                       }}";

            return string.Format(sourceFormat, variables.ToString(), Code);
        }

        /// <summary>
        /// Execute the 'Eval' method and return results
        /// </summary>
        /// <returns></returns>
        public bool? Evaluate()
        {
            return (evaluator != null) ? evaluator.Eval() : null; //dynamic object invoke
        }

    }

}