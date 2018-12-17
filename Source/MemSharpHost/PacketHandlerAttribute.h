#pragma once
#include "MemSharpPacketHandler.h"

using namespace System;

[AttributeUsageAttribute(AttributeTargets::Method, AllowMultiple = false, Inherited = false)]
ref class PacketHandlerAttribute : public Attribute
{
public:
	PacketHandlerAttribute(MemSharpPacketHandler::OpCode opCode);

public:
	MemSharpPacketHandler::OpCode OpCode;
};
