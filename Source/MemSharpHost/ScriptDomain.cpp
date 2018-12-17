#include "stdafx.h"
#include "ScriptDomain.h"


ScriptDomain::ScriptDomain()
{
	this->Domain = AppDomain::CurrentDomain;
	//this->Domain->AssemblyResolve += gcnew System::ResolveEventHandler(&ScriptDomain::HandleResolve);
	this->scriptTypes = gcnew List<Tuple<String^, Type^>^>();
}

ScriptDomain ^ ScriptDomain::Load(String ^ path)
{
	auto setup = gcnew AppDomainSetup();

	setup->ApplicationBase = path;
	setup->ShadowCopyFiles = "true";
	setup->ShadowCopyDirectories = path;

	auto appDomain = AppDomain::CreateDomain("ScriptDomain_" + (path->GetHashCode() * Environment::TickCount).ToString("X"), nullptr, setup, gcnew System::Security::PermissionSet(System::Security::Permissions::PermissionState::Unrestricted));
	appDomain->InitializeLifetimeService();

	ScriptDomain^ scriptDomain = nullptr;

	try
	{
		Console::WriteLine("Loading Script Domain...");
		scriptDomain = static_cast<ScriptDomain^>(appDomain->CreateInstanceFromAndUnwrap(ScriptDomain::typeid->Assembly->Location, ScriptDomain::typeid->FullName));
	}
	catch (Exception^ ex)
	{
		Console::WriteLine(ex->ToString());
		AppDomain::Unload(appDomain);
		return nullptr;
	}

	auto scriptFiles = gcnew List<String^>();
	auto assemblyFiles = gcnew List<String^>();

	scriptFiles->AddRange(Directory::GetFiles(path, "*.cs", SearchOption::AllDirectories));
	assemblyFiles->AddRange(Directory::GetFiles(path, "*.dll", SearchOption::AllDirectories));

	for each(auto scriptFile in scriptFiles)
		scriptDomain->LoadScript(scriptFile);

	for each(auto assemblyFile in assemblyFiles)
		scriptDomain->LoadAssembly(assemblyFile);

	return scriptDomain;
}

void ScriptDomain::Unload(ScriptDomain ^% domain)
{
	Console::WriteLine("Unloading Script Domain...");
	
	try
	{
		AppDomain::Unload(domain->Domain);
	}
	catch (Exception^ ex)
	{
		Console::WriteLine(ex->ToString());
	}

	domain = nullptr;
	GC::Collect();
}

void ScriptDomain::Start()
{
	for each(auto scriptType in scriptTypes)
	{
		auto script = CreateScriptInstance(scriptType->Item2);
		
		if (script)
			script->Initialize();
		else
		{
			Console::WriteLine("Script could not be instanciated.");
		}
	}
}

bool ScriptDomain::LoadScript(String^ filename)
{
	throw gcnew NotImplementedException();
}

bool ScriptDomain::LoadAssembly(String^ filename)
{
	Assembly^ assembly = nullptr;

	try
	{
		assembly = Assembly::Load(File::ReadAllBytes(filename));
	}
	catch (Exception^ ex)
	{
		Console::WriteLine(ex->ToString());
		return false;
	}

	return this->InitializeAssembly(assembly);
}

bool ScriptDomain::InitializeAssembly(Assembly ^ assembly)
{
	try
	{
		for each(auto type in assembly->GetTypes())
		{
			if (!type->IsSubclassOf(Script::typeid) || type->IsAbstract)
				continue;

			this->scriptTypes->Add(gcnew Tuple<String^, Type^>(assembly->GetName()->FullName, type));
		}
	}
	catch (Exception^ ex)
	{
		Console::WriteLine(ex->ToString());
		return false;
	}

	return true;
}

Script ^ ScriptDomain::CreateScriptInstance(Type ^ scriptType)
{
	if (!scriptType->IsSubclassOf(Script::typeid))
		return nullptr;

	Console::WriteLine("Instanciating {0} ...", scriptType->FullName);

	try
	{
		return static_cast<Script^>(Activator::CreateInstance(scriptType));
	}
	catch (Exception^ ex)
	{
		Console::WriteLine(ex->ToString());
		return nullptr;
	}
}
