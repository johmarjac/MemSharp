#include "stdafx.h"
#include "MemSharpUser.h"
#include "MemSharpPacketHandler.h"

MemSharpUser::MemSharpUser()
{
}

void MemSharpUser::HandleMessage(INetPacketStream ^ packet)
{
	OpCode opCode = (OpCode)packet->Read<UInt16>();
	
	auto handler = MemSharpPacketHandler::GetHandler(opCode);
	if (!handler)
	{
		Console::WriteLine("Handler not found!");
		return;
	}

	Console::WriteLine("Executing '{0}' Handler!", Enum::GetName(OpCode::typeid, opCode));
	handler->Invoke(packet);
}
