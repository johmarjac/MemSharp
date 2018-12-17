#include "stdafx.h"
#include "MemSharpHost.h"

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
	if (AllocConsole())
		freopen("CONOUT$", "w", stdout);

	{
		MemSharpHost::Instance
			->Server
			->Start();
	}

	printf("MemSharpServer has been stopped!\n");
	FreeConsole();
}
