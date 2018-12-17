#include "stdafx.h"
#include "SetWorkingDirectoryHandler.h"
#include "MemSharpHost.h"

using namespace System;
using namespace System::IO;

void SetWorkingDirectoryHandler::Handle(INetPacketStream ^ packet)
{
	auto workingDirectory = packet->Read<String^>();
	if (!(gcnew DirectoryInfo(workingDirectory))->Exists)
		return;

	MemSharpHost::Instance->WorkingDirectory = workingDirectory;
}
