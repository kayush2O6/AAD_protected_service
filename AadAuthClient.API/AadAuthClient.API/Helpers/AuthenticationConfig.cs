﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AadAuthClient.API
{
    public class AuthenticationConfig : IAuthenticationConfig
    {
        // public parameterless constructor is important
        public AuthenticationConfig() : this("AzureAD")
        {

        }

        public AuthenticationConfig(string section)
        {
            ConfigSection = section;
        }

        public string ConfigSection { get;  }

        /// <summary>
        /// instance of Azure AD, for example public Azure or a Sovereign cloud (Azure China, Germany, US government, etc ...)
        /// </summary>
        public string Instance { get; set; } // = "https://login.microsoftonline.com/{0}/v2.0";

        /// <summary>
        /// The Tenant is:
        /// - either the tenant ID of the Azure AD tenant in which this application is registered (a guid)
        /// or a domain name associated with the tenant
        /// - or 'organizations' (for a multi-tenant application)
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// Guid used by the application to uniquely identify itself to Azure AD
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// URL of the authority
        /// </summary>
        public string Authority
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture, Instance, Tenant);
                //return $"https://login.microsoftonline.com/{Tenant}/v2.0";
            }
        }

        /// <summary>
        /// Client secret (application password)
        /// </summary>
        /// <remarks>Daemon applications can authenticate with AAD through two mechanisms: ClientSecret
        /// (which is a kind of application password: this property)
        /// or a certificate previously shared with AzureAD during the application registration 
        /// (and identified by the CertificateName property belows)
        /// <remarks> 
        public string ClientSecret { get; set; }

        /// <summary>
        /// Name of a certificate in the user certificate store
        /// </summary>
        /// <remarks>Daemon applications can authenticate with AAD through two mechanisms: ClientSecret
        /// (which is a kind of application password: the property above)
        /// or a certificate previously shared with AzureAD during the application registration 
        /// (and identified by this CertificateName property)
        /// <remarks> 
        public string CertificateName { get; set; }

        /// <summary>
        /// Web Api base URL
        /// </summary>
        public string AADAuthBaseAddress { get; set; }

        /// <summary>
        /// Web Api scope. With client credentials flows, the scopes is ALWAYS of the shape "resource/.default"
        /// </summary>
        public string AADAuthScope { get; set; }

        /// <summary>
        /// Reads the configuration from a json file
        /// </summary>
        /// <param name="path">Path to the configuration json file</param>
        /// <returns>AuthenticationConfig read from the json file</returns>
        public AuthenticationConfig ReadFromJsonFile(string path)
        {
            IConfiguration Configuration;


            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory().ToString())
            .AddJsonFile(path);

            Configuration = builder.Build();
            // Get will Recusively bind the keys.
            Configuration = Configuration.GetSection(ConfigSection);
            return Configuration.Get<AuthenticationConfig>();
        }
    }
}
