﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Okta.Sdk.Configuration;
using Okta.Sdk.Internal;
using Xunit;

namespace Okta.Sdk.IntegrationTests
{
    public class DefaultDataStoreShould
    {
        [Fact]
        public async Task NotSendCloseConnectionHeaderWhenUsingTheDefaultClient()
        {
            var testOktaClient = TestClient.Create();
            var configuration = testOktaClient.Configuration;
            var httpClient = DefaultHttpClient.Create(configuration.ConnectionTimeout, configuration.Proxy,
                NullLogger.Instance);

            var requestExecutor = new DefaultRequestExecutor(configuration, httpClient, NullLogger.Instance);
            var resourceFactory = new ResourceFactory(testOktaClient, NullLogger.Instance);
            var dataStore = new DefaultDataStore(requestExecutor, new DefaultSerializer(), resourceFactory, NullLogger.Instance);

            var response = await dataStore.GetAsync<Resource>(
                new HttpRequest
            {
                // Endpoint that returns a chunked response
                Uri = $"/api/v1/meta/schemas/user/default",
            }, null, default(CancellationToken));

            var connectionHeader = response.Headers.FirstOrDefault(x => x.Key.Equals("connection", StringComparison.OrdinalIgnoreCase));
            connectionHeader.Value.Any(s => s.Equals("close", StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }
    }
}
