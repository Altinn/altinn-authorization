using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Exceptions;
using Altinn.Platform.Authorization.Services.Interfaces;
using Altinn.Platform.Register.Models;

using Newtonsoft.Json;

namespace Altinn.Platform.Events.Tests.Mocks
{
    public class RegisterServiceMock : IRegisterService
    {
        private readonly int _partiesCollection;

        public RegisterServiceMock(int partiesCollection = 1)
        {
            _partiesCollection = partiesCollection;
        }

        public async Task<Party> GetParty(int partyId)
        {
            Party party = null;
            string partyPath = GetPartyPath(partyId);
            if (File.Exists(partyPath))
            {
                string content = File.ReadAllText(partyPath);
                party = JsonConvert.DeserializeObject<Party>(content);
            }

            return await Task.FromResult(party);
        }

        public async Task<Party> PartyLookup(string orgNo, string person)
        {
            string eventsPath = Path.Combine(GetPartiesPath(), $@"{_partiesCollection}.json");
            Party party = null;

            if (File.Exists(eventsPath))
            {
                string content = File.ReadAllText(eventsPath);
                List<Party> parties = JsonConvert.DeserializeObject<List<Party>>(content);

                if (!string.IsNullOrEmpty(orgNo))
                {
                    party = parties.Where(p => p.OrgNumber != null && p.OrgNumber.Equals(orgNo)).FirstOrDefault();
                }
                else
                {
                    party = parties.Where(p => p.SSN != null && p.SSN.Equals(person)).FirstOrDefault();
                }
            }

            return party != null
                ? party
                : throw await PlatformHttpException.CreateAsync(new HttpResponseMessage
                { Content = new StringContent(string.Empty), StatusCode = System.Net.HttpStatusCode.NotFound });
        }

        private static string GetPartiesPath()
        {
            string unitTestFolder = Path.GetDirectoryName(new Uri(typeof(RegisterServiceMock).Assembly.Location).LocalPath);
            return Path.Combine(unitTestFolder, "..", "..", "..", "Data", "Register");
        }

        private static string GetPartyPath(int partyId)
        {
            string unitTestFolder = Path.GetDirectoryName(new Uri(typeof(RegisterServiceMock).Assembly.Location).LocalPath);
            return Path.Combine(unitTestFolder, "..", "..", "..", "Data", "Register", partyId.ToString() + ".json");
        }
    }
}
