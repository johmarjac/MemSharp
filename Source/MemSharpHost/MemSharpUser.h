#pragma once

using namespace Ether::Network::Common;
using namespace Ether::Network::Packets;

public ref class MemSharpUser : public NetUser
{
public:
	MemSharpUser();

public:
	virtual void HandleMessage(INetPacketStream^ packet) override;
};
