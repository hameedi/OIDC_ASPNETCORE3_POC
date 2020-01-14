using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IDS4.IDP
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser {
                    SubjectId = "abc1234",
                    Username = "Furqan",
                    Password = "password",
                    Claims = new List<Claim> {
                        new Claim("given_name", "Furqan"),
                        new Claim("family_name", "Hameedi"),
                        new Claim("address", "Address 1"),
                    }
                },
                new TestUser {
                    SubjectId = "abc123456",
                    Username = "Ghulam",
                    Password = "password",
                    Claims = new List<Claim> {
                        new Claim("given_name", "Ghulam"),
                        new Claim("family_name", "Abbas"),
                        new Claim("address", "address 2"),
                    }
                }

            };
        }


        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
            };
        }


        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client {
                    ClientName = "IDS4ApiClient",
                    ClientId = "IDS4ApiClient",
                    AllowedGrantTypes= GrantTypes.Hybrid,
                    RedirectUris = new List<String> {
                        "https://localhost:44335/signin-oidc"
                    },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address
                    },
                    ClientSecrets = {
                        new Secret("IDS4ApiClientsecret".Sha256())
                    },
                    // AlwaysIncludeUserClaimsInIdToken = true
                    PostLogoutRedirectUris = {
                        "https://localhost:44335/signout-callback-oidc"
                    },
                }
            };
        }
    }

}
