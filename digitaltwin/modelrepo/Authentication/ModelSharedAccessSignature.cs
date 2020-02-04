// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Devices.Common.Authorization
{
    public class ModelSharedAccessSignature : SharedAccessSignature
    {
        internal string _repositoryId;

        private ModelSharedAccessSignature(
            string shareAccessSignatureName, 
            DateTime expiresOn, 
            string expiry, 
            string keyName, 
            string signature, 
            string encodedAudience, 
            string repositoryId) 
            : base(
                  shareAccessSignatureName, 
                  expiresOn, 
                  expiry, 
                  keyName, 
                  signature, 
                  encodedAudience)
        {
            _repositoryId = repositoryId;
        }

        public static ModelSharedAccessSignature ParseForModel(string shareAccessSignatureName, string rawToken)
        {
            if (string.IsNullOrWhiteSpace(shareAccessSignatureName))
            {
                throw new ArgumentNullException(nameof(shareAccessSignatureName));
            }

            if (string.IsNullOrWhiteSpace(rawToken))
            {
                throw new ArgumentNullException(nameof(rawToken));
            }

            IDictionary<string, string> parsedFields = ExtractFieldValues(rawToken);

            if (!parsedFields.TryGetValue(SharedAccessSignatureConstants.SignatureFieldName, out string signature))
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Missing field: {0}", SharedAccessSignatureConstants.SignatureFieldName));
            }

            if (!parsedFields.TryGetValue(ModelSharedAccessSignatureConstants.RepositoryIdFieldName, out string repositoryId))
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Missing field: {0}", ModelSharedAccessSignatureConstants.RepositoryIdFieldName));
            }

            if (!parsedFields.TryGetValue(SharedAccessSignatureConstants.ExpiryFieldName, out string expiry))
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Missing field: {0}", SharedAccessSignatureConstants.ExpiryFieldName));
            }

            // KeyName (skn) is optional .
            parsedFields.TryGetValue(SharedAccessSignatureConstants.KeyNameFieldName, out string keyName);

            if (!parsedFields.TryGetValue(SharedAccessSignatureConstants.AudienceFieldName, out string encodedAudience))
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Missing field: {0}", SharedAccessSignatureConstants.AudienceFieldName));
            }

            return new ModelSharedAccessSignature(
                shareAccessSignatureName,
                SharedAccessSignatureConstants.EpochTime + TimeSpan.FromSeconds(double.Parse(expiry, CultureInfo.InvariantCulture)),
                expiry, keyName, signature, repositoryId, encodedAudience);
        }
    }
}
