//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Xml;
using System.Collections.Generic;
#if !(ANDROID || IOS) && !NETFX_CORE && !NETSTANDARD2_0
using System.ServiceModel.Description;
#endif


namespace Csla.DataPortalClient
{
#if !(ANDROID || IOS) && !NETFX_CORE && !NETSTANDARD2_0
  public class GZipMessageEncodingBindingElementImporter : IPolicyImportExtension
    {
        public GZipMessageEncodingBindingElementImporter()
        {
        }

        void IPolicyImportExtension.ImportPolicy(MetadataImporter importer, PolicyConversionContext context)
        {
            if (importer == null)
            {
                throw new ArgumentNullException("importer");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ICollection<XmlElement> assertions = context.GetBindingAssertions();
            foreach (XmlElement assertion in assertions)
            {
                if ((assertion.NamespaceURI == GZipMessageEncodingPolicyConstants.GZipEncodingNamespace) &&
                    (assertion.LocalName == GZipMessageEncodingPolicyConstants.GZipEncodingName)
                    )
                {
                    assertions.Remove(assertion);
                    context.BindingElements.Add(new GZipMessageEncodingBindingElement());
                    break;
                }
            }
        }
    }
#endif
}

