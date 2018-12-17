#include "stdafx.h"
#include "MemSharpServer.h"

using namespace System;

MemSharpServer::MemSharpServer()
{
	this->Configuration->Host = "0.0.0.0";
	this->Configuration->Port = 31010;
	this->Configuration->Blocking = true;
}

void MemSharpServer::Initialize()
{
	Console::WriteLine("MemSharpServer has been started!");
}

void MemSharpServer::OnClientConnected(MemSharpUser ^connection)
{
	Console::WriteLine("Client connection established!");
}

void MemSharpServer::OnClientDisconnected(MemSharpUser ^connection)
{
	Console::WriteLine("Client connection dropped!");
}

void MemSharpServer::OnError(System::Exception ^exception)
{
	Console::WriteLine(exception->ToString());
}