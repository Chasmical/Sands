using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace SandsProjectGenerator
{
#pragma warning disable CA2211
    public static class Program
    {
        public static void Write(string text, ConsoleColor fore = ConsoleColor.Gray, ConsoleColor back = ConsoleColor.Black)
        {
            if (fore != ConsoleColor.Gray) Console.ForegroundColor = fore;
            if (back != ConsoleColor.Black) Console.BackgroundColor = back;
            Console.Write(text);
            if (fore != ConsoleColor.Gray || back != ConsoleColor.Black) Console.ResetColor();
        }
        public static void WriteLine(string? text = null, ConsoleColor fore = ConsoleColor.Gray, ConsoleColor back = ConsoleColor.Black)
        {
            if (fore != ConsoleColor.Gray) Console.ForegroundColor = fore;
            if (back != ConsoleColor.Black) Console.BackgroundColor = back;
            if (text is not null) Console.Write(text);
            if (fore != ConsoleColor.Gray || back != ConsoleColor.Black) Console.ResetColor();
            Console.WriteLine();
        }
        public static string ReadLine()
        {
            string? line = Console.ReadLine();
            if (line is null) throw new InvalidOperationException();
            return line;
        }

        public static string? ProjectPrefix;
        public static string ProjectName = null!;
        public static string PluginClassName = null!;
        public static string PluginTitle = null!;
        public static string Author = null!;
        public static string Version = null!;
        public static bool UsingRogueLibs;
        public static bool UsingResources;
        public static bool UsingMicroTemplate;

        private static readonly Stopwatch sw = new Stopwatch();

        public static bool ParseBooleanResponse(string response) => response.ToUpperInvariant() switch
        {
            "Y" or "YES" or "T" or "TRUE" => true,
            "N" or "NO" or "F" or "FALSE" => false,
            _ => throw new InvalidOperationException(),
        };
        public static string Prompt(string? title, string? prompt, string? examples = null, string? defaultValue = null)
        {
            WriteLine();
            if (title is not null) WriteLine(title, ConsoleColor.DarkCyan);
            if (prompt is not null) WriteLine(prompt, ConsoleColor.White);
            if (examples is not null) WriteLine(examples, ConsoleColor.DarkGray);
            WriteLine();
            if (defaultValue is not null) WriteLine($"(default: {defaultValue})", ConsoleColor.DarkGray);
            Write("> ", ConsoleColor.Yellow);
            string line = ReadLine().Trim();
            if (line.Length is 0) return defaultValue ?? throw new InvalidOperationException();
            return line;
        }

        public static string SolutionFile = null!;
        public static string SolutionDir = null!;

        private static string? FindSolutionFile()
        {
            string? dir = Environment.CurrentDirectory;
            while (dir is not null)
            {
                string? solutionFile = Directory.EnumerateFiles(dir, "*.sln").FirstOrDefault();
                if (solutionFile is not null) return solutionFile;
                dir = Path.GetDirectoryName(dir);
            }
            return null;
        }
        public static void Operation(string text, [InstantHandle] Action action)
        {
            Write($"[{sw.Elapsed:mm\\:ss\\.ffffff}] {text}...");
            try
            {
                action();
                WriteLine(" Success.", ConsoleColor.DarkGreen);
            }
            catch
            {
                WriteLine(" Error.", ConsoleColor.Red);
                throw;
            }
        }

        public static void Main()
        {
            try
            {
                Console.InputEncoding = Encoding.UTF8;
                Console.OutputEncoding = Encoding.UTF8;
                EntryPoint();
            }
            catch (InvalidOperationException) { }
            catch (Exception e)
            {
                WriteLine();
                WriteLine(e.ToString(), ConsoleColor.Red);
                ReadLine();
            }
        }
        public static void EntryPoint()
        {
            SolutionFile = FindSolutionFile() ?? throw new NotImplementedException("Could not find a solution in the working directory!");
            SolutionDir = Path.GetDirectoryName(SolutionFile)!;

            do
            {
                #region Header
                static void Title(string line)
                {
                    int pad = Console.BufferWidth - 1 - line.Length;
                    if (pad > 0)
                    {
                        int right = pad / 2;
                        int left = pad - right;
                        line = new string(' ', left) + line + new string(' ', right);
                    }
                    WriteLine(line, ConsoleColor.Yellow);
                }

                WriteLine($"Solution: {SolutionFile}");
                WriteLine();
                Title(@"╔════════════════════════════════════════════════════════════╗");
                Title(@"║   ________           ________             ________         ║");
                Title(@"║  |\   ____\         |\   __  \           |\   ____\        ║");
                Title(@"║  \ \  \___|_        \ \  \|\  \  /\      \ \  \___|_       ║");
                Title(@"║   \ \_____  \        \ \__     \/  \      \ \_____  \      ║");
                Title(@"║    \|____|\  \        \|_/  __     /|      \|____|\  \     ║");
                Title(@"║      ____\_\  \         /  /_|\   / /        ____\_\  \    ║");
                Title(@"║     |\_________\       /_______   \/        |\_________\   ║");
                Title(@"║     \|_________|       |_______|\__\        \|_________|   ║");
                Title(@"║                                \|__|                       ║");
                Title(@"║                                                            ║");
                Title(@"╠════════════════════════════════════════════════════════════╣");
                Title(@"║                 S-and-S Project Generator                  ║");
                Title(@"╚════════════════════════════════════════════════════════════╝");
                WriteLine();
                #endregion

                #region Prompts
                ProjectName = Prompt("# Project Name",
                                     "What should the project be called?",
                                     "Ex: BlinkingMod, DemolishThatFreakingWall");

                ProjectPrefix = null;
                if (ProjectName.StartsWith('['))
                {
                    int closingBracketIndex = ProjectName.IndexOf(']');
                    if (closingBracketIndex is not -1)
                    {
                        ProjectPrefix = ProjectName[..(closingBracketIndex + 1)] + " ";
                        ProjectName = ProjectName[(closingBracketIndex + 1)..].TrimStart();
                    }
                }

                string defaultPluginClass = ProjectName.EndsWith("Mod", StringComparison.Ordinal)
                    ? ProjectName[..^"Mod".Length] + "Plugin"
                    : ProjectName + "Plugin";

                PluginClassName = Prompt("# Plugin Class Name",
                                         "What should the plugin class be called?",
                                         "Ex: BlinkingPlugin, DemolishThatFreakingWallPlugin",
                                         defaultPluginClass);

                string defaultPluginTitle = Regex.Replace(ProjectName, "([a-z])([A-Z])", static m => $"{m.Groups[1]} {m.Groups[2]}");
                PluginTitle = Prompt("# Plugin Title",
                                     "What should the mod be called?",
                                     "Ex: Blinking Mod, Demolish That Freaking Wall",
                                     defaultPluginTitle);

                Author = Prompt("# Project Author",
                                "Who are you?");

                Version = Prompt("# Project Version",
                                 "What version do you want to start from?",
                                 "Ex: 1.0.0, 2.3.0-rc.5",
                                 "1.0.0")
                    .TrimStart('v');

                string rogueLibsStr = Prompt("# RogueLibs Dependency",
                                             "Are you planning on using RogueLibs? (y/n)",
                                             defaultValue: "yes");

                UsingRogueLibs = ParseBooleanResponse(rogueLibsStr);

                string resourcesStr = Prompt("# Resources File",
                                             "Are you planning on using resources? (y/n)",
                                             defaultValue: "no");

                UsingResources = ParseBooleanResponse(resourcesStr);

                string useMicroStr = Prompt("# Micro Template",
                                            "Do you want to use a micro plugin template? (y/n)",
                                            defaultValue: "no");

                UsingMicroTemplate = ParseBooleanResponse(useMicroStr);
                #endregion

                #region Confirm configuration
                static void WriteField(string name, string value, string? prefix = null, string? suffix = null)
                {
                    Write($"# {name}: ", ConsoleColor.DarkCyan);
                    if (prefix is not null) Write(prefix, ConsoleColor.DarkGray);
                    Write(value);
                    if (suffix is not null) Write(suffix, ConsoleColor.DarkGray);
                    WriteLine();
                }
                static void WriteBoolField(string name, bool value)
                {
                    Write($"# {name}: ", ConsoleColor.DarkCyan);
                    WriteLine(value ? "yes" : "no", value ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                }

                WriteLine();
                WriteLine("# Entered configuration", ConsoleColor.DarkCyan);
                WriteLine();
                WriteField("Project Name    ", ProjectName);
                WriteField("Plugin Class    ", PluginClassName, suffix: ".cs");
                WriteField("Author          ", Author);
                WriteField("Version         ", Version, "v");
                WriteBoolField("Using RogueLibs ", UsingRogueLibs);
                WriteBoolField("Using Resources ", UsingResources);
                WriteBoolField("Micro-template  ", UsingMicroTemplate);

                string doGenerateStr = Prompt("Do you want to generate a project with this configuration?",
                                              "Enter 'no' to enter the configuration again.");
                WriteLine();
                bool doGenerate = ParseBooleanResponse(doGenerateStr);
                #endregion

                if (doGenerate) break;
                Console.Clear();
            }
            while (true);

            sw.Start();

            string projectDir = Path.Combine(SolutionDir, ProjectName);
            Operation($"Creating './{ProjectName}/' directory",
                      () => Directory.CreateDirectory(projectDir));

            string projectFile = Path.Combine(projectDir, ProjectName + ".csproj");

            StreamWriter writer = null!;
            Operation($"Creating './{ProjectName}/{ProjectName}.csproj' file",
                      () => writer = File.CreateText(projectFile));

            #region Writing to {ProjectName}.csproj
            Operation($"Writing to './{ProjectName}/{ProjectName}.csproj' file", () =>
            {
                writer.WriteLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
                // General
                {
                    writer.WriteLine();
                    writer.WriteLine("  <PropertyGroup>");
                    writer.WriteLine();
                    writer.WriteLine("    <!-- Project Properties -->");
                    writer.WriteLine();
                    writer.WriteLine("    <!-- Header -->");
                    writer.WriteLine($"    <AssemblyName>{SecurityElement.Escape(ProjectPrefix)}{ProjectName}</AssemblyName>");
                    writer.WriteLine($"    <RootNamespace>{(ProjectPrefix is not null ? ProjectName : "$(AssemblyName)")}</RootNamespace>");
                    writer.WriteLine($"    <PackageId>{(ProjectPrefix is not null ? ProjectName : "$(AssemblyName)")}</PackageId>");
                    writer.WriteLine($"    <Version>{Version}</Version>");
                    writer.WriteLine($"    <Authors>{SecurityElement.Escape(Author)}</Authors>");
                    writer.WriteLine("    <Company>$(Authors)</Company>");
                    writer.WriteLine("    <Copyright></Copyright>");
                    writer.WriteLine();
                    writer.WriteLine("    <!-- Title, Description, Tags -->");
                    writer.WriteLine($"    <Title>{SecurityElement.Escape(ProjectPrefix)}{SecurityElement.Escape(PluginTitle)}</Title>");
                    writer.WriteLine("    <Description>");
                    writer.WriteLine();
                    writer.WriteLine("    </Description>");
                    writer.WriteLine("    <PackageTags></PackageTags>");
                    writer.WriteLine();
                    writer.WriteLine("  </PropertyGroup>");
                }
                // Post-Build Events
                {
                    writer.WriteLine();
                    writer.WriteLine("  <Target Name=\"PostBuild\" AfterTargets=\"PostBuildEvent\">");
                    writer.WriteLine("    <Exec Command=\"&quot;$(SolutionDir)\\..\\.events\\PluginBuildEvents.exe&quot; &quot;$(TargetPath)&quot; &quot;Streets of Rogue&quot;\" />");
                    writer.WriteLine("  </Target>");
                }
                // References
                {
                    writer.WriteLine();
                    void WriteReference(string name, bool isDynamic = false)
                    {
                        writer.WriteLine($"    <Reference Include=\"{name}\">");
                        writer.WriteLine($"      <HintPath>..\\..\\.ref{(isDynamic ? null : "\\static")}\\{name}.dll</HintPath>");
                        writer.WriteLine("    </Reference>");
                    }

                    writer.WriteLine("  <ItemGroup>");

                    WriteReference("0Harmony");
                    WriteReference("Assembly-CSharp", true);
                    WriteReference("BepInEx");
                    WriteReference(@"com.unity.multiplayer-hlapi.Runtime");
                    WriteReference("netstandard");
                    if (UsingRogueLibs) WriteReference("RogueLibsCore", true);
                    WriteReference("UnityEngine");
                    WriteReference("UnityEngine.AnimationModule");
                    WriteReference("UnityEngine.AudioModule");
                    WriteReference("UnityEngine.CoreModule");
                    WriteReference("UnityEngine.InputLegacyModule");
                    WriteReference("UnityEngine.Physics2DModule");
                    WriteReference("UnityEngine.UI");
                    WriteReference("UnityEngine.UIModule");

                    writer.WriteLine("  </ItemGroup>");
                }
                if (UsingResources)
                {
                    writer.WriteLine();
                    writer.WriteLine("  <ItemGroup>");
                    writer.WriteLine("    <Compile Update=\"Properties\\Resources.Designer.cs\">");
                    writer.WriteLine("      <DesignTime>True</DesignTime>");
                    writer.WriteLine("      <AutoGen>True</AutoGen>");
                    writer.WriteLine("      <DependentUpon>Resources.resx</DependentUpon>");
                    writer.WriteLine("    </Compile>");
                    writer.WriteLine("    <EmbeddedResource Update=\"Properties\\Resources.resx\">");
                    writer.WriteLine("      <Generator>ResXFileCodeGenerator</Generator>");
                    writer.WriteLine("      <LastGenOutput>Resources.Designer.cs</LastGenOutput>");
                    writer.WriteLine("    </EmbeddedResource>");
                    writer.WriteLine("  </ItemGroup>");
                }
                writer.WriteLine();
                writer.WriteLine("</Project>");
            });
            #endregion

            writer.Close();

            #region Resources
            if (UsingResources)
            {
                string propertiesDir = Path.Combine(projectDir, "Properties");
                Operation($"Creating './{ProjectName}/Properties' directory",
                          () => Directory.CreateDirectory(propertiesDir));

                string resourcesDir = Path.Combine(projectDir, "Resources");
                Operation($"Creating './{ProjectName}/Resources' directory",
                          () => Directory.CreateDirectory(resourcesDir));

                string resourcesFile = Path.Combine(propertiesDir, "Resources.resx");
                Operation($"Creating './{ProjectName}/Properties/Resources.resx' file",
                          () => File.WriteAllBytes(resourcesFile, Properties.Resources.TemplateResources));

                string templateSpriteFile = Path.Combine(resourcesDir, "template.png");
                Operation($"Creating './{ProjectName}/Resources/template.png' file",
                          () => File.WriteAllBytes(templateSpriteFile, Properties.Resources.TemplateSprite));

            }
            #endregion

            string pluginClassFile = Path.Combine(projectDir, PluginClassName + ".cs");
            Operation($"Creating './{ProjectName}/{PluginClassName}.cs' file",
                      () => writer = File.CreateText(pluginClassFile));

            #region Writing to {PluginClassName}.cs
            Operation($"Writing to './{ProjectName}/{PluginClassName}.cs' file", () =>
            {
                string guid = @$"{Author.ToLowerInvariant()}.streetsofrogue.{ProjectName.ToLowerInvariant()}";
                if (UsingMicroTemplate)
                {
                    if (UsingRogueLibs)
                    {
                        writer.WriteLine("using RogueLibsCore;");
                        writer.WriteLine();
                    }
                    writer.WriteLine($"namespace {ProjectName};");
                    writer.WriteLine($"[BepInEx.BepInPlugin(@\"{guid}\", \"{ProjectPrefix}{PluginTitle}\", \"{Version}\")]");
                    if (UsingRogueLibs) writer.WriteLine("[BepInEx.BepInDependency(RogueLibs.GUID, RogueLibs.CompiledVersion)]");
                    writer.WriteLine($"public class {PluginClassName} : BepInEx.BaseUnityPlugin");
                    writer.WriteLine("{");
                    writer.WriteLine("    public void Awake() { }");
                    writer.WriteLine("}");
                    return;
                }

                writer.WriteLine("using BepInEx;");
                if (UsingRogueLibs) writer.WriteLine("using RogueLibsCore;");
                writer.WriteLine();
                writer.WriteLine($"namespace {ProjectName}");
                writer.WriteLine("{");
                {
                    writer.WriteLine("    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]");
                    if (UsingRogueLibs) writer.WriteLine("    [BepInDependency(RogueLibs.GUID, RogueLibs.CompiledVersion)]");
                    writer.WriteLine($"    public sealed class {PluginClassName} : BaseUnityPlugin");
                    writer.WriteLine("    {");
                    {
                        writer.WriteLine($"        public const string PluginGuid = @\"{guid}\";");
                        writer.WriteLine($"        public const string PluginName = \"{ProjectPrefix}{PluginTitle}\";");
                        writer.WriteLine($"        public const string PluginVersion = \"{Version}\";");
                        writer.WriteLine();
                        writer.WriteLine("        public new static BepInEx.Logging.ManualLogSource Logger = null!;");
                        writer.WriteLine();
                        writer.WriteLine("        public void Awake()");
                        writer.WriteLine("        {");
                        {
                            writer.WriteLine("            Logger = base.Logger;");
                            if (UsingRogueLibs)
                            {
                                writer.WriteLine("            RogueLibs.LoadFromAssembly();");
                                writer.WriteLine("            RoguePatcher patcher = new RoguePatcher(this);");
                            }
                            writer.WriteLine();
                        }
                        writer.WriteLine("        }");
                        writer.WriteLine();
                    }
                    writer.WriteLine("}");
                }
                writer.WriteLine("}");
            });
            #endregion

            writer.Close();

            #region Adding project to the solution
            string addProjectStr = Prompt($"# Adding {ProjectName} to the solution",
                                          $"Would you like to add {ProjectName}.csproj to {Path.GetFileName(SolutionFile)}?",
                                          defaultValue: "yes");
            WriteLine();
            bool addProject = ParseBooleanResponse(addProjectStr);

            if (addProject)
            {
                Operation($"Adding the project to {Path.GetFileName(SolutionFile)}", static () =>
                {
                    string hexGuid = Guid.NewGuid().ToString("B").ToUpperInvariant();

                    List<string> lines = new List<string>(File.ReadAllLines(SolutionFile));
                    int globalIndex = lines.FindIndex(static l => l.StartsWith("Global", StringComparison.OrdinalIgnoreCase));
                    if (globalIndex is -1) throw new Exception("'Global' was not found in the solution file!");

                    lines.InsertRange(globalIndex, new string[]
                    {
                        $"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{ProjectName}\", \"{ProjectName}\\{ProjectName}.csproj\", \"{hexGuid}\"",
                        "EndProject",
                    });

                    int configsIndex = lines.FindIndex(static l => l.TrimStart().StartsWith("GlobalSection(ProjectConfigurationPlatforms)",
                                                                                            StringComparison.OrdinalIgnoreCase));
                    if (configsIndex is not -1)
                    {
                        int configsEndIndex = lines.FindIndex(configsIndex, static l => l.TrimStart().StartsWith("EndGlobalSection",
                                                                  StringComparison.OrdinalIgnoreCase));
                        if (configsEndIndex is -1) throw new Exception("'EndGlobalSection' was not found in the solution file!");

                        lines.InsertRange(configsEndIndex, new string[]
                        {
                            $"\t\t{hexGuid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU",
                            $"\t\t{hexGuid}.Debug|Any CPU.Build.0 = Debug|Any CPU",
                            $"\t\t{hexGuid}.Release|Any CPU.ActiveCfg = Release|Any CPU",
                            $"\t\t{hexGuid}.Release|Any CPU.Build.0 = Release|Any CPU",
                        });
                    }

                    File.WriteAllLines(SolutionFile, lines.ToArray());
                });
            }
            #endregion

            sw.Stop();

            WriteLine();
            WriteLine("╔═════════════════════════════════════════════════════╗", ConsoleColor.DarkCyan);
            WriteLine("║                                                     ║", ConsoleColor.DarkCyan);
            WriteLine("║           Project successfully generated!           ║", ConsoleColor.DarkCyan);
            WriteLine("║          ‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾          ║", ConsoleColor.DarkCyan);
            WriteLine("║   Thanks for using the S-and-S project generator!   ║", ConsoleColor.DarkCyan);
            WriteLine("║                                                     ║", ConsoleColor.DarkCyan);
            WriteLine("╚═════════════════════════════════════════════════════╝", ConsoleColor.DarkCyan);
            WriteLine();
            Write("Press any key to exit... ", ConsoleColor.DarkGray);
            Console.ReadKey();

        }
    }
}