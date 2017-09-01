﻿//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.Xml;
#if !(ANDROID || IOS) && !NETFX_CORE && !NETSTANDARD2_0
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ComponentModel;
#endif


namespace Csla.DataPortalClient
{
    // This is constants for GZip message encoding policy.
    static class GZipMessageEncodingPolicyConstants
    {
        public const string GZipEncodingName = "GZipEncoding";
        public const string GZipEncodingNamespace = "http://schemas.microsoft.com/ws/06/2004/mspolicy/netgzip1";
        public const string GZipEncodingPrefix = "gzip";
    }

#if !(ANDROID || IOS) && !NETFX_CORE && !NETSTANDARD2_0

  //This is the binding element that, when plugged into a custom binding, will enable the GZip encoder
  public sealed class GZipMessageEncodingBindingElement
                        : MessageEncodingBindingElement //BindingElement
                        , IPolicyExportExtension
    {

        //We will use an inner binding element to store information required for the inner encoder
        MessageEncodingBindingElement innerBindingElement;

        //By default, use the default text encoder as the inner encoder
        public GZipMessageEncodingBindingElement()
            : this(new TextMessageEncodingBindingElement()) { }

        public GZipMessageEncodingBindingElement(MessageEncodingBindingElement messageEncoderBindingElement)
        {
            this.innerBindingElement = messageEncoderBindingElement;
        }

        public MessageEncodingBindingElement InnerMessageEncodingBindingElement
        {
            get { return innerBindingElement; }
            set { innerBindingElement = value; }
        }

        //Main entry point into the encoder binding element. Called by WCF to get the factory that will create the
        //message encoder
        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new GZipMessageEncoderFactory(innerBindingElement.CreateMessageEncoderFactory());
        }

        public override MessageVersion MessageVersion
        {
            get { return innerBindingElement.MessageVersion; }
            set { innerBindingElement.MessageVersion = value; }
        }

        public override BindingElement Clone()
        {
            return new GZipMessageEncodingBindingElement(this.innerBindingElement);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return innerBindingElement.GetProperty<T>(context);
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext policyContext)
        {
            if (policyContext == null)
            {
                throw new ArgumentNullException("policyContext");
            }
            XmlDocument document = new XmlDocument();
            policyContext.GetBindingAssertions().Add(document.CreateElement(
                GZipMessageEncodingPolicyConstants.GZipEncodingPrefix,
                GZipMessageEncodingPolicyConstants.GZipEncodingName,
                GZipMessageEncodingPolicyConstants.GZipEncodingNamespace));
        }
    }

    //This class is necessary to be able to plug in the GZip encoder binding element through
    //a configuration file
    public class GZipMessageEncodingElement : BindingElementExtensionElement
    {
        public GZipMessageEncodingElement()
        {
            Properties.Add(new ConfigurationProperty("MaxArrayLength", typeof(string), "2147483647"));
            Properties.Add(new ConfigurationProperty("MaxBytesPerRead", typeof(string), "2147483647"));
            Properties.Add(new ConfigurationProperty("MaxStringContentLength", typeof(string), "2147483647"));
            Properties.Add(new ConfigurationProperty("MaxDepth", typeof(string), "2147483647"));
            Properties.Add(new ConfigurationProperty("MaxNameTableCharCount", typeof(string), "2147483647"));
        }

        //Called by the WCF to discover the type of binding element this config section enables
        public override Type BindingElementType
        {
            get { return typeof(GZipMessageEncodingBindingElement); }
        }

        //The only property we need to configure for our binding element is the type of
        //inner encoder to use. Here, we support text and binary.
        [ConfigurationProperty("innerMessageEncoding", DefaultValue = "textMessageEncoding")]
        public string InnerMessageEncoding
        {
            get { return (string)base["innerMessageEncoding"]; }
            set { base["innerMessageEncoding"] = value; }
        }

        [Category("ReaderQuotas")]
        [Description("MaxArrayLength")]
        public string MaxArrayLength
        {
            get { return (string)this["MaxArrayLength"]; }
            set { this["MaxArrayLength"] = value; }
        }

        [Category("ReaderQuotas")]
        [Description("MaxBytesPerRead")]
        public string MaxBytesPerRead
        {
            get { return (string)this["MaxBytesPerRead"]; }
            set { this["MaxBytesPerRead"] = value; }
        }

        [Category("ReaderQuotas")]
        [Description("MaxStringContentLength")]
        public string MaxStringContentLength
        {
            get { return (string)this["MaxStringContentLength"]; }
            set { this["MaxStringContentLength"] = value; }
        }

        [Category("ReaderQuotas")]
        [Description("MaxDepth")]
        public string MaxDepth
        {
            get { return (string)this["MaxDepth"]; }
            set { this["MaxDepth"] = value; }
        }

        [Category("ReaderQuotas")]
        [Description("MaxNameTableCharCount")]
        public string MaxNameTableCharCount
        {
            get { return (string)this["MaxNameTableCharCount"]; }
            set { this["MaxNameTableCharCount"] = value; }
        }

        //Called by the WCF to apply the configuration settings (the property above) to the binding element
        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            GZipMessageEncodingBindingElement binding = (GZipMessageEncodingBindingElement)bindingElement;
            PropertyInformationCollection propertyInfo = this.ElementInformation.Properties;
            if (propertyInfo["innerMessageEncoding"].ValueOrigin != PropertyValueOrigin.Default)
            {
                switch (this.InnerMessageEncoding)
                {
                    case "textMessageEncoding":
                        TextMessageEncodingBindingElement txtMsgEnco = new TextMessageEncodingBindingElement();
                        txtMsgEnco.ReaderQuotas = new XmlDictionaryReaderQuotas()
                        {
                            MaxArrayLength = Convert.ToInt32(this.MaxArrayLength),
                            MaxBytesPerRead = Convert.ToInt32(this.MaxBytesPerRead),
                            MaxDepth = Convert.ToInt32(this.MaxDepth),
                            MaxNameTableCharCount = Convert.ToInt32(this.MaxNameTableCharCount),
                            MaxStringContentLength = Convert.ToInt32(this.MaxStringContentLength)
                        };
                        binding.InnerMessageEncodingBindingElement = txtMsgEnco;
                        break;
                    case "binaryMessageEncoding":
                        BinaryMessageEncodingBindingElement binMsgEnco = new BinaryMessageEncodingBindingElement();
                        binMsgEnco.ReaderQuotas = new XmlDictionaryReaderQuotas()
                        {
                            MaxArrayLength = Convert.ToInt32(this.MaxArrayLength),
                            MaxBytesPerRead = Convert.ToInt32(this.MaxBytesPerRead),
                            MaxDepth = Convert.ToInt32(this.MaxDepth),
                            MaxNameTableCharCount = Convert.ToInt32(this.MaxNameTableCharCount),
                            MaxStringContentLength = Convert.ToInt32(this.MaxStringContentLength)
                        };
                        binding.InnerMessageEncodingBindingElement = binMsgEnco;
                        break;
                }
            }
        }

        //Called by the WCF to create the binding element
        protected override BindingElement CreateBindingElement()
        {
            GZipMessageEncodingBindingElement bindingElement = new GZipMessageEncodingBindingElement();
            this.ApplyConfiguration(bindingElement);
            return bindingElement;
        }
    }

#endif
}
