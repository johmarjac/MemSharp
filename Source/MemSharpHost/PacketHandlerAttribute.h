#pragma once
#include "MemSharpPacketHandler.h"

using namespace System;
using namespace MemSharpCommon;

[AttributeUsageAttribute(AttributeTargets::Method, AllowMultiple = false, Inherited = false)]
ref class PacketHandlerAttribute : public Attribute
{
public:
	PacketHandlerAttribute(MemSharpCommon::OpCode opCode);

public:
	MemSharpCommon::OpCode OpCode;
};
