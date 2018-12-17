#pragma once
#include "PacketHandlerAttribute.h"
#include "MemSharpPacketHandler.h"

ref class SetWorkingDirectoryHandler
{
public:
	[PacketHandlerAttribute(MemSharpPacketHandler::OpCode::SetWorkingDirectory)]
	static void Handle(INetPacketStream^ packet);
};
