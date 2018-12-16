using Ether.Network.Exceptions;
using Ether.Network.Tests.Contexts.NetConfig;
using System;
using Xunit;

namespace Ether.Network.Tests
{
    public class NetServerConfigurationTest : IDisposable
    {
        private readonly ConfigServer _server;

        public NetServerConfigurationTest()
        {
            this._server = new ConfigServer();
        }

        [Fact]
        public void StartServerWithoutConfiguration()
        {
            Exception ex = Assert.Throws<EtherConfigurationException>(() => this._server.Start());

            Assert.IsType<EtherConfigurationException>(ex);
        }

        [Fact]
        public void SetupServerConfigurationBeforeStart()
        {
            this._server.SetupConfiguration();
            this._server.Start();
        }

        [Fact]
        public void SetupServerConfigurationAfterStart()
        {
            this._server.SetupConfiguration();
            this._server.Start();

            Exception ex = Assert.Throws<EtherConfigurationException>(() => this._server.SetupConfiguration());

            Assert.IsType<EtherConfigurationException>(ex);
        }

        [Fact]
        public void SetupServerConfigurationAfterStartStop()
        {
            this._server.SetupConfiguration();
            this._server.Start();
            this._server.Stop();

            this._server.SetupConfiguration();
            this._server.Start();
            this._server.Stop();
        }

        public void Dispose()
        {
            this._server.Stop();
            this._server.Dispose();
        }
    }
}