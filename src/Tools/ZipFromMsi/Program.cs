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

                // Create and add the shortcut to the zip
                string tempLnkPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SharpDevelop.lnk");
                try
                {
                    string rootBinPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\..\\bin"));
                    string targetPath = Path.Combine(rootBinPath, "SharpDevelop.exe");
                    string workingDirectory = rootBinPath;
                    string iconLocation = targetPath + ",0";

                    CreateShortcut(tempLnkPath, targetPath, workingDirectory, iconLocation);

                    using (ZipArchive theZip = ZipFile.Open(zipFileName, ZipArchiveMode.Update))
                    {
                        theZip.CreateEntryFromFile(tempLnkPath, ZipRootDirectory + "/SharpDevelop.lnk", CompressionLevel.Optimal);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning: Failed to create or add shortcut to zip: " + ex.Message);
                }
                finally
                {
                    if (File.Exists(tempLnkPath))
                    {
                        File.Delete(tempLnkPath);
                    }
                }
            }
        }

        static void CreateShortcut(string shortcutPath, string targetPath, string workingDirectory, string iconLocation)
        {
            Type shellType = Type.GetTypeFromProgID("WScript.Shell");
            object shell = Activator.CreateInstance(shellType);
            object shortcut = shellType.InvokeMember("CreateShortcut", 
                System.Reflection.BindingFlags.InvokeMethod, 
                null, shell, new object[] { shortcutPath });
            
            Type shortcutType = shortcut.GetType();
            shortcutType.InvokeMember("TargetPath", 
                System.Reflection.BindingFlags.SetProperty, 
                null, shortcut, new object[] { targetPath });
            
            shortcutType.InvokeMember("WorkingDirectory", 
                System.Reflection.BindingFlags.SetProperty, 
                null, shortcut, new object[] { workingDirectory });
            
            shortcutType.InvokeMember("IconLocation", 
                System.Reflection.BindingFlags.SetProperty, 
                null, shortcut, new object[] { iconLocation });
            
            shortcutType.InvokeMember("Save", 
                System.Reflection.BindingFlags.InvokeMethod, 
                null, shortcut, null);
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

                    theZip.CreateEntryFromFile(RelativePathToolToSetupFolder + source, 
                        AppendRelativePath(currentRelativePath, name),
                        CompressionLevel.Optimal);
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
