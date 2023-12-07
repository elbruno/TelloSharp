using System.Text;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace TelloSharp.DynamicCode02
{
    public static class Program
    {
        private static bool Connected;
        static string logPath = "logs/";
        static string logFilePath = string.Empty;
        static DateTime logStartTime = DateTime.Now;
        static Tello? tello = null;

        public static void Main()
        {
            FileInfo sourceFile = new FileInfo(@"..\..\..\tello2.cs");
            Console.WriteLine("Loading file: " + sourceFile.Exists);
            var fileContent = File.ReadAllText(sourceFile.FullName);

            var options =ScriptOptions.Default
                .AddReferences(typeof(object).Assembly)
                .AddReferences(typeof(TelloSharp.Tello).Assembly)
                .AddImports("System", "System.IO", "System.Text", "System.Text.RegularExpressions");

            try
            {
                CSharpScript.RunAsync(fileContent, options);                
            }
            catch (CompilationErrorException ex)
            {
                Console.WriteLine(fileContent);

                var sb = new StringBuilder();
                foreach (var err in ex.Diagnostics)
                    sb.AppendLine(err.ToString());

                Console.WriteLine(sb.ToString());
            }
            // Runtime Errors
            catch (Exception ex)
            {
                Console.WriteLine(fileContent);
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Exit...");
        }
    }
}