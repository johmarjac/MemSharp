#pragma once

using namespace Ether::Network::Common;
using namespace Ether::Network::Packets;

public ref class MemSharpUser : public NetUser
{
public:
	virtual void HandleMessage(INetPacketStream^ packet) override;
};
