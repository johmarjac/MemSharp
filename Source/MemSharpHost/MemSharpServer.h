#pragma once
#include "MemSharpUser.h"

using namespace Ether::Network::Server;

ref class MemSharpServer : public NetServer<MemSharpUser^>
{
public:
	MemSharpServer();

	virtual void Initialize() override;

	virtual void OnClientConnected(MemSharpUser ^connection) override;

	virtual void OnClientDisconnected(MemSharpUser ^connection) override;

	virtual void OnError(System::Exception ^exception) override;

};
