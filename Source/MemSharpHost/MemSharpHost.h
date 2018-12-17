#pragma once
#include "MemSharpServer.h"

using namespace System;

void OnAttach(HMODULE hModule);

ref class MemSharpHost
{
public:
	MemSharpHost() 
	{
		this->Server = gcnew MemSharpServer();
	}

	static property MemSharpHost^ Instance
	{
		MemSharpHost^ get()
		{
			if (instance == nullptr)
				instance = gcnew MemSharpHost();
			return instance;
		}
	};

	~MemSharpHost() {}

public:
	MemSharpServer^ Server;
	property String^ WorkingDirectory;

private:
	static MemSharpHost^ instance;

};