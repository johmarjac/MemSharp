#pragma once
#include "PacketHandlerAttribute.h"
#include "MemSharpPacketHandler.h"

ref class ShutdownHandler
{
public:
	[PacketHandlerAttribute(MemSharpPacketHandler::OpCode::Shutdown)]
	static void Handle(INetPacketStream^ packet);
};
