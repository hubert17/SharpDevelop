using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ZipFromMsi
{
    class Program
    {
        // Xml Constants
        private const string ElementNameFragment = "Fragment";
        private const string ElementNameDirectoryRef = "DirectoryRef";
        private const string ElementNameDirectory = "Directory";
        private const string ElementNameComponent = "Component";
        private const string ElementNameFile = "File";

        // NOTE: App has to be started in bin\Debug working directory, otherwise those paths don't work
        private const string WxsFilename = "../../../../Setup/Files.wxs";
        public static string RelativePathToolToSetupFolder = "..\\..\\..\\";

        // App Constants
        private static readonly XNamespace Namespace = "http://schemas.microsoft.com/wix/2006/wi";
        private const string ZipRootDirectory = "SharpDevelop";

        static void Main(string[] args)
        {
            CreateZip("SharpDevelopStandalone.zip", LoadWxsFile(WxsFilename));
        }

        static void CreateZip(string zipFileName, XDocument doc)
        {
            XElement root = doc.Root;

            var fragment = root.Element(Namespace + ElementNameFragment);
            var directoryRef = fragment.Element(Namespace + ElementNameDirectoryRef);

            var programDirectory = directoryRef.Elements().First();

            if ("ProgramFilesFolder" == (string)programDirectory.Attribute("Id"))
            {
                var sdfolder = programDirectory.Elements().First();
                var installdir = sdfolder.Elements().First();

                using (ZipArchive theZip = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
                {
                    foreach (var folder in installdir.Elements(Namespace + ElementNameDirectory))
                    {
                        ProcessFolder(folder, theZip, ZipRootDirectory);
                    }
                }

                // Compile and add the launcher to the zip
                string tempExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SharpDevelopLauncher.exe");
                try
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string iconPath = Path.GetFullPath(Path.Combine(baseDir, "..\\..\\..\\..\\Main\\SharpDevelop\\Resources\\SharpDevelop.ico"));

                    CompileLauncher(tempExePath, iconPath);

                    using (ZipArchive theZip = ZipFile.Open(zipFileName, ZipArchiveMode.Update))
                    {
                        theZip.CreateEntryFromFile(tempExePath, ZipRootDirectory + "/SharpDevelop.exe", CompressionLevel.Optimal);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning: Failed to compile or add launcher to zip: " + ex.Message);
                }
                finally
                {
                    if (File.Exists(tempExePath))
                    {
                        File.Delete(tempExePath);
                    }
                }
            }
        }

        static void CompileLauncher(string outputPath, string iconPath)
        {
            var codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
            var parameters = new System.CodeDom.Compiler.CompilerParameters();
            
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = outputPath;
            parameters.CompilerOptions = "/target:winexe";
            if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
            {
                parameters.CompilerOptions += " /win32icon:\"" + iconPath + "\"";
            }
            
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            
            string sourceCode = @"
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

class Program
{
    static void Main()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string binPath = Path.Combine(baseDir, ""bin"");
        string exePath = Path.Combine(binPath, ""SharpDevelop.exe"");
        
        if (!File.Exists(exePath))
        {
            MessageBox.Show(""Could not find bin\\SharpDevelop.exe in the current directory."", ""SharpDevelop Portable"", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = exePath;
        startInfo.WorkingDirectory = binPath;
        
        try
        {
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show(""Failed to start SharpDevelop: "" + ex.Message, ""SharpDevelop Portable"", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
";
            
            var results = codeProvider.CompileAssemblyFromSource(parameters, sourceCode);
            if (results.Errors.Count > 0)
            {
                var sb = new System.Text.StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
                {
                    sb.AppendLine(error.ErrorText);
                }
                throw new Exception("Compiler errors:\n" + sb.ToString());
            }
        }

        static void ProcessFolder(XElement folder, ZipArchive theZip, string folderRelativePath)
        {
            string targetDirectory = (string)folder.Attribute("Name");
            string currentRelativePath = AppendRelativePath(folderRelativePath, targetDirectory);

            Console.WriteLine("Processing folder " + currentRelativePath);

            foreach (var component in folder.Elements(Namespace + ElementNameComponent))
            {
                foreach (var file in component.Elements(Namespace + ElementNameFile))
                {
                    string source = (string)file.Attribute("Source");
                    string name = (string)file.Attribute("Name");

                    string sourcePath = RelativePathToolToSetupFolder + source;
                    if (File.Exists(sourcePath)) {
                        theZip.CreateEntryFromFile(sourcePath, 
                            AppendRelativePath(currentRelativePath, name),
                            CompressionLevel.Optimal);
                    } else {
                        Console.WriteLine("Warning: File not found, skipping: " + sourcePath);
                    }
                }
            }

            foreach (var secondaryFolder in folder.Elements(Namespace + ElementNameDirectory))
            {
                ProcessFolder(secondaryFolder, theZip, currentRelativePath);
            }
        }

        static string AppendRelativePath(string firstPart, string newPart)
        {
            return firstPart + "/" + newPart;
        }

        static XDocument LoadWxsFile(string filename)
        {
            return XDocument.Load(filename);
        }
    }
}
