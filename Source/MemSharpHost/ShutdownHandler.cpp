#include "stdafx.h"
#include "ShutdownHandler.h"
#include "MemSharpHost.h"

void ShutdownHandler::Handle(INetPacketStream ^ packet)
{
	MemSharpHost::Instance->Server->Stop();
}
