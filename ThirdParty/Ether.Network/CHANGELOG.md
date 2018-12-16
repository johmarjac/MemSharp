# Changelog

## [Unreleased]

## [3.0.0] - 16/03/2018

** Breaking Changes **

### Added

- New architecture
- Add `INetPacketStream` interface and `NetPacketStream` implementation for read/write packets.
- Add `NetUser` interface. Inherits from `NetConnection`.
- Add `OnSocketError()` method to `NetClient`. (PR [#42](https://github.com/Eastrall/Ether.Network/pull/42))
- Add all interfaces support to `NetServer`. (PR [#42](https://github.com/Eastrall/Ether.Network/pull/42))
- Add support for reading byte arrays with `INetPacketStream`. (PR [#48](https://github.com/Eastrall/Ether.Network/pull/48))
- Add broadcast system for `NetServer`. (PR [#52](https://github.com/Eastrall/Ether.Network/pull/52))
- Add `NetClientConfiguration` (PR [#78](https://github.com/Eastrall/Ether.Network/pull/78))
- Add `GetUser(Guid)` to `INetServer` (PR [#85](https://github.com/Eastrall/Ether.Network/pull/85))

### Changed

- `NetConnection` is lighter.
- `NetClient` inherits from `NetUser`
- `NetClient` sending process is using a queue.
- `NetClient` message handler process is using a queue.
- `NetServer` inherits from `NetConnection`.
- Receive algorithm review.

### Fix

- Improve receive process on `NetClient` and `NetServer`.
- Fix disconnect process on `NetClient`.

### Removed

- `NetPacketBase` has been removed and replaced with `NetPacketStream`.

## [Released]

## [2.1.0-pre] - 2017-10-08

### Added

- Add support for `.NET Standard 2.0`
- Add `OnError` method to `NetServer`
- Add array reader on `NetPacketBase` (`T[] Read<T>(amount)`)

### Changed

- Change sending process on `NetServer`, now uses a sending queue

### Fix

- Call the `OnClientDisconnected` when the `DisconnectClient(Guid id)` method is called and the connection is disposed.

### Removed

- Buffer Manager

## [2.0.1] - 2017-08-06

### Added

- Add the `DisconnectClient(Guid clientId)` method to `NetServer`.

### Changed

- Changed `ConcurrentBag` to `ConcurrentDictionary` for client handling.

## [2.0.0] - 2017-08-01

### Added

- Add netstandard1.3 support
- Add NetServerConfiguration class. Provide properties to configuration a NetServer.

### Changed

- NetServer socket management. Now using [SocketAsyncEventArgs](https://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs(v=vs.110).aspx) for scalability and performance.
- NetClient socket management. Now using [SocketAsyncEventArgs](https://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs(v=vs.110).aspx) for scalability and performance.

### Removed

- Remove .NET Framework 4.6 support
- Remove .NET Framework 4.6.1 support
- Remove NetDelayer system

## [1.1.7] - 2017-01-11

### Changed

- Fix `NetClient` critical issues

## [1.1.6] - 2017-01-05

### Added

- Add support for .NET Framework 4.5
- Add support for .NET Framework 4.5.1
- Add support for .NET Framework 4.6
- Add support for .NET Framework 4.6.1

## [1.1.5] - 2016-12-14

### Changed

- `NetClient` connect method returns a `bool`
- `NetServer<T>` client list is a readonly list
- `NetServer<T>` `OnClientConnected` and `OnClientDisconnected` methods takes `T` as parameter.

## [1.1.0] - 2016-10-10

### Added


### Changed


## [1.0.0] - 2016-11-03

### Added

- `NetServer<T>` fully managed server handling `T` connection where `T` is a `NetConnection`
- `NetClient` fully managed client.
- `NetPacket` packet management.
