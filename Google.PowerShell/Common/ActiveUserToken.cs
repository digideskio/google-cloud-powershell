﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Google.PowerShell.Common
{
    /// <summary>
    /// OAuth 2.0 model for a successful access token response as specified in 
    /// http://tools.ietf.org/html/rfc6749#section-5.1.
    /// </summary>
    public class ActiveUserToken
    {
        /// <summary>
        /// The user that this token corresponds to.
        /// </summary>
        public string User { get; private set; }

        /// <summary>The access token issued by the authorization server.</summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// The date and time that this token was issued.
        /// This is set in UTC time.
        /// It should be set by the CLIENT after the token was received from the server.
        /// </summary>
        public DateTime ExpiredTime { get; internal set; }

        /// <summary>
        /// Returns <c>true</c> if the token is expired or it's going to be expired in the next minute.
        /// </summary>
        public bool IsExpired
        {
            get
            {
                if (AccessToken == null)
                {
                    return true;
                }

                if (ExpiredTime == DateTime.MaxValue)
                {
                    return false;
                }

                return ExpiredTime.AddSeconds(-60) <= DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Construct a new token by parsing activeConfigJson and get the credential.
        /// </summary>
        public ActiveUserToken(JToken userCredentialJson, string user)
        {
            User = user;
            JToken accessTokenJson = userCredentialJson.SelectToken("access_token");
            JToken tokenExpiryJson = userCredentialJson.SelectToken("token_expiry");

            if (accessTokenJson == null || accessTokenJson.Type != JTokenType.String)
            {
                throw new InvalidDataException("Credential JSON should contain access token key.");
            }

            AccessToken = accessTokenJson.Value<string>();

            // Service account credentials do not expire.
            if (tokenExpiryJson == null || tokenExpiryJson.Type == JTokenType.Null)
            {
                ExpiredTime = DateTime.MaxValue;
            }
            else
            {
                // The expiry time will be in UTC.
                DateTime parsedExpiredDate;
                if (!DateTime.TryParse(tokenExpiryJson.Value<string>(), out parsedExpiredDate))
                {
                    throw new InvalidDataException("Credential JSON contains an invalid token_expiry.");
                }

                ExpiredTime = parsedExpiredDate.ToUniversalTime();
            }
        }
    }
}

