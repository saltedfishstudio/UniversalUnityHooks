﻿using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;

namespace HooksInjector
{
	public class ScriptsCompiler
	{
		private string pluginsDir;
		private string managedFolder;
		public ScriptsCompiler(string plugins, string managed)
		{
			pluginsDir = plugins;
			managedFolder = managed;
			if (!Directory.Exists(pluginsDir))
			{
				Console.WriteLine("HooksInjector: ERROR: Plugins Directory does not exist! Check you have permission to create directories here.");
				Console.Read();
				return;

			}
		}

		public string CompileScript(string scriptFile)
		{
			var options = new Options();
			var main = new Program();
			CSharpCodeProvider provider = new CSharpCodeProvider();
			string output = "Plugins/" + new FileInfo(scriptFile).Name.Replace("cs", ".dll");
			CompilerParameters cp = new CompilerParameters();
			cp.GenerateExecutable = false;
			cp.OutputAssembly = output;
			cp.WarningLevel = 1;

			if (CommandLine.Parser.Default.ParseArguments(main.gArgs, options))
			{
				foreach (var refs in options.refs)
				{
					cp.ReferencedAssemblies.Add(refs);
				}
				if (!options.optimize)
				{
					cp.CompilerOptions = "/optimize";
				}
			}

			foreach (var file in Directory.GetFiles(managedFolder))
			{
				if (file.EndsWith("dll", StringComparison.CurrentCulture) && !file.Contains("msc") && !file.Contains("sys"))
				{
					cp.ReferencedAssemblies.Add(file);
				}
			}

			var results = provider.CompileAssemblyFromSource(cp, File.ReadAllText(scriptFile));
			foreach (var error in results.Errors)
			{
				Console.WriteLine(error);
			}
			Console.WriteLine("Compiled script:" + scriptFile + "Sucessfully");
			return cp.OutputAssembly;

		}
	}
}
