#include "stdafx.h"
#include "MemSharpPacketHandler.h"
#include "PacketHandlerAttribute.h"

using namespace System;
using namespace System::Reflection;

static MemSharpPacketHandler::MemSharpPacketHandler()
{
	auto types = Assembly::GetExecutingAssembly()->GetTypes();
	for each(auto type in types)
	{
		auto methodInfos = type->GetMethods(BindingFlags::Static | BindingFlags::Public);
		for each(auto methodInfo in methodInfos)
		{
			auto attributes = (array<PacketHandlerAttribute^>^)methodInfo->GetCustomAttributes(PacketHandlerAttribute::typeid, false);
			for each(auto attribute in attributes)
			{
				if (!attribute)
					continue;

				auto action = (Action<INetPacketStream^>^)methodInfo->CreateDelegate(Action<INetPacketStream^>::typeid);

				MemSharpPacketHandler::PacketHandlers->Add(attribute->OpCode, action);
			}
		}
	}
}

Action<INetPacketStream^>^ MemSharpPacketHandler::GetHandler(OpCode opCode)
{
	Action<INetPacketStream^>^ handler;
	if (MemSharpPacketHandler::PacketHandlers->TryGetValue(opCode, handler))
		return handler;
	
	return nullptr;
}
