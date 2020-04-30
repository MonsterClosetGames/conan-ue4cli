/*
	DO NOT EDIT THIS FILE! THIS FILE WAS GENERATED AUTOMATICALLY BY CONAN-UE4CLI VERSION ${VERSION}.
	THIS BOILERPLATE CODE IS INTENDED FOR USE WITH UNREAL ENGINE VERSIONS 4.20, 4.21, 4.22 AND 4.23.
*/
using System;
using System.IO;
using UnrealBuildTool;
using System.Diagnostics;

//For Tools.DotNETCommon.JsonObject and Tools.DotNETCommon.FileReference
using Tools.DotNETCommon;

public class ${MODULE} : ModuleRules
{
	//Returns the identifier string for the given target, which includes its platform, architecture (if specified), and debug CRT status
	private string TargetIdentifier(ReadOnlyTargetRules target)
	{
		//Append the target's architecture to its platform name if an architecture was specified
		string id = (target.Architecture != null && target.Architecture.Length > 0) ?
			String.Format("{0}-{1}", target.Platform.ToString(), target.Architecture) :
			target.Platform.ToString();
		
		//Append a debug suffix for Windows debug targets that actually use the debug CRT
		bool isDebug = (target.Configuration == UnrealTargetConfiguration.Debug || target.Configuration == UnrealTargetConfiguration.DebugGame);
		if (isDebug && target.bDebugBuildsActuallyUseDebugCRT) {
			id += "-Debug";
		}
		
		return id;
	}
	
	//Processes the JSON data produced by Conan that describes our dependencies
	private void ProcessDependencies(string depsJson, ReadOnlyTargetRules target)
	{
		//We need to ensure libraries end with ".lib" under Windows
		string libSuffix = ((target.IsInPlatformGroup(UnrealPlatformGroup.Windows))) ? ".lib" : "");
		
		//Attempt to parse the JSON file
		JsonObject deps = JsonObject.Read(new FileReference(depsJson));
		
		//Process the list of dependencies
		foreach (JsonObject dep in deps.GetObjectArrayField("dependencies"))
		{
			//Add the header and library paths for the dependency package
			PublicIncludePaths.AddRange(dep.GetStringArrayField("include_paths"));
			PublicLibraryPaths.AddRange(dep.GetStringArrayField("lib_paths"));
			
			//Add the preprocessor definitions from the dependency package
			PublicDefinitions.AddRange(dep.GetStringArrayField("defines"));
			
			//Link against the libraries from the package
			string[] libs = dep.GetStringArrayField("libs");
			foreach (string lib in libs)
			{
				string libFull = lib + ((libSuffix.Length == 0 || lib.EndsWith(libSuffix)) ? "" : libSuffix);
				PublicAdditionalLibraries.Add(libFull);
			}
		}
	}
	
	public ${MODULE}(ReadOnlyTargetRules Target) : base(Target)
	{
		Type = ModuleType.External;
		
		//Install third-party dependencies using Conan
		Process.Start(new ProcessStartInfo
		{
			FileName = "conan",
			Arguments = "install . --profile ue4-" + this.TargetIdentifier(Target),
			WorkingDirectory = ModuleDirectory
		})
		.WaitForExit();
		
		//Link against our Conan-installed dependencies
		this.ProcessDependencies(Path.Combine(ModuleDirectory, "conanbuildinfo.json"), Target);
	}
}
