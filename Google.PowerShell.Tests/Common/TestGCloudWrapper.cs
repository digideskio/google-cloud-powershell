﻿using NUnit.Framework;
using Google.PowerShell.Common;
using Newtonsoft.Json.Linq;

namespace Google.PowerShell.Tests.Common
{
    [TestFixture]
    public class TestGCloudWrapper
    {
        /// <summary>
        /// Test that GetActiveConfig returns a valid config.
        /// </summary>
        [Test]
        public void TestGetAccessToken()
        {
            string config = GCloudWrapper.GetActiveConfig().Result;
            Assert.IsNotNull(config);

            JToken parsedConfigJson = JObject.Parse(config);
            Assert.IsNotNull(parsedConfigJson.SelectToken("sentinels.config_sentinel"),
                "Config returned by GetActiveConfig should have a sentinel file.");
            Assert.IsNotNull(parsedConfigJson.SelectToken("credential"),
                "Config returned by GetActiveConfig should have a credential property.");
            Assert.IsNotNull(parsedConfigJson.SelectToken("configuration.properties"),
                "Config returned by GetActiveConfig should have a configuration.");
        }
    }
}
