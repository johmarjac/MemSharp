#include "stdafx.h"

#include "MemSharpHost.h"
#include "MemSharpServer.h"

using namespace MemSharpHost;

BOOL WINAPI DllMain(HMODULE hModule, DWORD dwReason, LPVOID pReserved)
{
	switch (dwReason)
	{
	case DLL_PROCESS_ATTACH:
		DisableThreadLibraryCalls(hModule);
		CreateThread(NULL, NULL, reinterpret_cast<LPTHREAD_START_ROUTINE>(OnAttach), hModule, NULL, NULL);
		break;
	}
	return TRUE;
}

void OnAttach(HMODULE hModule)
{
	{
		auto server = gcnew MemSharpServer();
		server->Start();
	}
}