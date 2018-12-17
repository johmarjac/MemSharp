#pragma once
#include "PacketHandlerAttribute.h"
#include "MemSharpPacketHandler.h"
#include "MemSharpHost.h"

using namespace System::IO;

ref class ScriptDomainHandler
{
public:
	[PacketHandlerAttribute(OpCode::StartScriptDomain)]
	static void HandleStart(INetPacketStream^ packet);

	[PacketHandlerAttribute(OpCode::StopScriptDomain)]
	static void HandleStop(INetPacketStream^ packet);
};
