using System;
using System.Threading;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Services.Interface;
using Altinn.Platform.Profile.Models;
using Altinn.Platform.Register.Models;

namespace Altinn.Platform.Authorization.IntegrationTests.MockServices
{
    public class ProfileMock : IProfile
    {
        public Task<UserProfile> GetUserProfile(int userId, CancellationToken cancellationToken = default)
        {
            UserProfile userProfile = null;
            if (userId == 20010440)
            {
                userProfile = new UserProfile { Party = new Party { SSN = "13923949741" } };
            }
            else if (userId == 1337)
            {
                userProfile = new UserProfile { Party = new Party { SSN = "13371337133" } };
            }

            return Task.FromResult(userProfile);
        }

        public Task<UserProfile> GetUserProfileByPersonId(string personId, CancellationToken cancellationToken = default)
        {
            UserProfile userProfile = null;
            if (personId == "13923949741")
            {
                userProfile = new UserProfile { Party = new Party { SSN = "13923949741" } };
            }
            else if (personId == "13371337133")
            {
                userProfile = new UserProfile { Party = new Party { SSN = "13371337133" } };
            }
            else if (personId == "01039012345")
            {
                userProfile = new UserProfile { Party = new Party { SSN = "01039012345", PartyId = 1337 }, UserId = 1337 };
            }

            return Task.FromResult(userProfile);
        }
    }
}
