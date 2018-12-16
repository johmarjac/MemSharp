using System;
using Ether.Network.Exceptions;
using Ether.Network.Tests.Contexts.NetConfig;
using Xunit;

namespace Ether.Network.Tests
{
    public class NetClientConfigurationTest : IDisposable
    {
        private readonly ConfigServer _server;
        private readonly ConfigClient _client;

        public NetClientConfigurationTest()
        {
            this._server = new ConfigServer();
            this._client = new ConfigClient();
        }

        [Fact]
        public void StartClientWihtoutConfiguration()
        {
            this._server.SetupConfiguration();
            this._server.Start();

            Exception ex = Assert.Throws<EtherConfigurationException>(() => this._client.Connect());
            Assert.IsType<EtherConfigurationException>(ex);

            this._client.Disconnect();
        }

        [Fact]
        public void StartClientWithConfiguration()
        {
            this._server.SetupConfiguration();
            this._server.Start();

            this._client.SetupConfiguration();
            this._client.Connect();
            this._client.Disconnect();
        }

        [Fact]
        public void SetupClientConfigurationAfterConnected()
        {
            this._server.SetupConfiguration();
            this._server.Start();

            this._client.SetupConfiguration();
            this._client.Connect();

            Exception ex = Assert.Throws<EtherConfigurationException>(() => this._client.SetupConfiguration());
            Assert.IsType<EtherConfigurationException>(ex);

            this._client.Disconnect();
        }

        public void Dispose()
        {
            this._server.Stop();
            this._server.Dispose();
            this._client.Dispose();
        }
    }
}