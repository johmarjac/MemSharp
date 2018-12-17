#include "stdafx.h"
#include "ScriptDomainHandler.h"


void ScriptDomainHandler::HandleStart(INetPacketStream ^ packet)
{
	if (!MemSharpHost::Instance->ScriptDomain)
	{
		auto directoryInfo = gcnew DirectoryInfo(MemSharpHost::Instance->WorkingDirectory);

		if (String::IsNullOrEmpty(MemSharpHost::Instance->WorkingDirectory) || !directoryInfo->Exists)
			Console::WriteLine("The Working Directory has not been set or is invalid.");
		else
		{
			MemSharpHost::Instance->ScriptDomain = ScriptDomain::Load(directoryInfo->FullName);
			MemSharpHost::Instance->ScriptDomain->Start();
			Console::WriteLine("ScriptDomain has been started.");
		}
	}
	else
		Console::WriteLine("ScriptDomain is already started.");
}

void ScriptDomainHandler::HandleStop(INetPacketStream ^ packet)
{
	if (!MemSharpHost::Instance->ScriptDomain)
		Console::WriteLine("ScriptDomain is not yet started.");
	else
	{
		ScriptDomain::Unload(MemSharpHost::Instance->ScriptDomain);
		Console::WriteLine("ScriptDomain has been stopped.");
	}
}
