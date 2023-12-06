using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
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
            ClearConsole();

            Directory.CreateDirectory(Path.Combine("../", logPath));
            logStartTime = DateTime.Now;
            logFilePath = Path.Combine("../", logPath + logStartTime.ToString("yyyy-dd-M--HH-mm-ss") + ".csv");

            //tello = new();
            //tello.OnConnection += Tello_OnConnection;
            //tello.OnUpdate += Tello_OnUpdate;
            //tello.Connect();

            // sleep of 2 seconds
            System.Threading.Thread.Sleep(2000);

            // load class dynamically
            // Get a path to the file(s) to compile.
            FileInfo sourceFile = new FileInfo(@"..\..\..\tello2.cs");
            Console.WriteLine("Loading file: " + sourceFile.Exists);
            // Prepary a file path for the compiled library.
            string outputName = string.Format(@"{0}\{1}.dll", Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(sourceFile.Name));
            // load file content
            var fileContent = File.ReadAllText(sourceFile.FullName);


            var telloAssemblyPath = Path.GetFullPath("TelloSharp.dll");

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

            //// compilation starts
            //var tree = SyntaxFactory.ParseSyntaxTree(fileContent);

            //// Detect the file location for the library that defines the object type
            //var systemRefLocation = typeof(object).GetTypeInfo().Assembly.Location;
            //// Create a reference to the library
            //var systemReference = MetadataReference.CreateFromFile(systemRefLocation);
            //// A single, immutable invocation to the compiler
            //// to produce a library
            //var compilation = CSharpCompilation.Create(outputName)
            //  .WithOptions(
            //    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            //  .AddReferences(systemReference)
            //  .AddReferences(MetadataReference.CreateFromFile(typeof(Tello).Assembly.Location))
            //  .AddSyntaxTrees(tree);
            //string path = Path.Combine(Directory.GetCurrentDirectory(), outputName);
            //EmitResult compilationResult = compilation.Emit(path);
            //if (compilationResult.Success)
            //{
            //    // Load the assembly
            //    Assembly asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);

            //    asm.GetType("TelloSharp.DynamicCode02.DynamicTelloActions").GetMethod("run").Invoke(null, new object[] { tello });
            //}
            //else
            //{
            //    foreach (Diagnostic codeIssue in compilationResult.Diagnostics)
            //    {
            //        string issue = $"ID: {codeIssue.Id}, Message: {codeIssue.GetMessage()}, Location: {codeIssue.Location.GetLineSpan()}, Severity: {codeIssue.Severity}";
            //        Console.WriteLine(issue);
            //    }
            //}

            //var csc = new CSharpCodeProvider();
            //var parameters = new CompilerParameters();
            //var results = csc.CompileAssemblyFromSource(parameters, fileContent);
            ////Check if compilation is successful, run the code
            //if (!results.Errors.HasErrors)
            //{
            //    var t = results.CompiledAssembly.GetType("DynamicTelloActions");
            //    dynamic o = Activator.CreateInstance(t);
            //    o.run(tello);
            //}
            //else
            //{
            //    var errors = string.Join(Environment.NewLine,
            //        results.Errors.Cast<CompilerError>().Select(x => x.ErrorText));
            //    Console.WriteLine(errors);
            //}

            Console.WriteLine("Exit...");
        }

        private static void Tello_OnConnection(object? sender, Tello.ConnectionState e)
        {
            if (e == Tello.ConnectionState.Connected)
            {
                Connected = true;
                tello.GetSSID();
                tello.GetVersion();
            }
            if (e == Tello.ConnectionState.Disconnected)
                Connected = false;
        }

        private static void Tello_OnUpdate(object? sender, TelloStateEventArgs e)
        {
            if (e.LastCmdId == Messages.MessageTypes.msgFlightStatus)
            {
                //write update to log.
                var elapsed = DateTime.Now - logStartTime;
                File.AppendAllText(logFilePath, elapsed.ToString(@"mm\:ss\:ff\,") + tello?.State.GetLogLine());
            }
        }


        static void PrintAt(int x, int y, string? str)
        {
            var saveLeft = Console.CursorLeft;
            var saveTop = Console.CursorTop;
            Console.SetCursorPosition(x, y);
            Console.WriteLine(str + "     ");
            Console.SetCursorPosition(saveLeft, saveTop);

        }
        static void ClearConsole()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 23);
            Console.WriteLine("Commands:takeoff,land,exit,cls");
        }
    }
}