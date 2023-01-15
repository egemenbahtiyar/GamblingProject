using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GamblingProject.Services;
using Microsoft.AspNetCore.Mvc;
using Moralis;
using Moralis.AuthApi.Models;
using Moralis.Network;
using Moralis.Web3Api.Models;

namespace GamblingProject.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> RequestMessage(string address = "0x35ba4825204dcE15C7147eA89b31178a00750f81",
            ChainNetworkType network = 0, ChainList chainId = ChainList.mumbai)
        {
            var req = new ChallengeRequestDto
                {
                    // The Ethereum address performing the signing conformant to capitalization encoded
                    // checksum specified in EIP-55 where applicable.
                    Address = address,
                    // The EIP-155 Chain ID to which the session is bound, and the network where Contract
                    // Accounts MUST be resolved.
                    ChainId = (long) chainId,
                    // The RFC 3986 authority that is requesting the signing
                    Domain = "defi.finance",
                    // The ISO 8601 datetime string that, if present, indicates when the signed
                    // authentication message is no longer valid.
                    ExpirationTime = DateTime.UtcNow.AddMinutes(60),
                    // The ISO 8601 datetime string that, if present, indicates when the signed
                    // authentication message will become valid.
                    NotBefore = DateTime.UtcNow,
                    // A list of information or references to information the user wishes to have resolved
                    // as part of authentication by the relying party. They are expressed as RFC 3986 URIs
                    // separated by "\n- " where \n is the byte 0x0a.
                    Resources = new[] {"https://www.1155project.com"},
                    // Time is seconds at which point this request becomes invalid.
                    Timeout = 120,
                    // A human-readable ASCII assertion that the user will sign, and it must not
                    // contain '\n' (the byte 0x0a).
                    Statement = "Please confirm",
                    // An RFC 3986 URI referring to the resource that is the subject of the signing
                    // (as in the subject of a claim).
                    Uri = "https://defi.finance",
                };

                var resp = await MoralisClient.AuthenticationApi.AuthEndpoint.Challenge(req,network);

                return new CreatedAtRouteResult(nameof(RequestMessage), resp);
        }
    }
}