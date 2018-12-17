#include "stdafx.h"
#include "SetWorkingDirectoryHandler.h"

using namespace System;
using namespace System::IO;

void SetWorkingDirectoryHandler::Handle(INetPacketStream ^ packet)
{
	auto workingDirectory = packet->Read<String^>();
	auto directoryInfo = gcnew DirectoryInfo(workingDirectory);

	if (!directoryInfo->Exists)
		return;

	MemSharpHost::Instance->WorkingDirectory = directoryInfo->FullName;

	if (MemSharpHost::Instance->ScriptDomain)
	{
		Console::WriteLine("ScriptDomain is currently running, restarting into new Working Directory...");
		ScriptDomain::Unload(MemSharpHost::Instance->ScriptDomain);

		MemSharpHost::Instance->ScriptDomain = ScriptDomain::Load(MemSharpHost::Instance->WorkingDirectory);
		MemSharpHost::Instance->ScriptDomain->Start();
		Console::WriteLine("ScriptDomain restart successful.");
	}
}
