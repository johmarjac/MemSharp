#pragma once
#include "PacketHandlerAttribute.h"
#include "MemSharpPacketHandler.h"
#include "MemSharpHost.h"

ref class SetWorkingDirectoryHandler
{
public:
	[PacketHandlerAttribute(MemSharpPacketHandler::OpCode::SetWorkingDirectory)]
	static void Handle(INetPacketStream^ packet);
};
