using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Constants;
using Altinn.Platform.Authorization.IntegrationTests.Data;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Models.AccessManagement;
using Altinn.Platform.Authorization.Services.Interface;
using Microsoft.VisualBasic;

namespace Altinn.Platform.Authorization.IntegrationTests.MockServices;

public class AccessManagementWrapperMock : IAccessManagementWrapper
{
    public Task<IEnumerable<DelegationChange>> GetAllDelegationChanges(DelegationChangeInput input)
    {
        var data = new List<Action<DelegationChangeInput, List<DelegationChange>>>()
        {
            ConditionalAdd(
                DelegationChangesTestData.Default(DelegationChangesTestData.WithAltinnAppID("org1/app1"), DelegationChangesTestData.WithOfferedByPartyID(50001337), DelegationChangesTestData.WithCoveredByUserID(20001337)),
                WithDefaultCondition("org1/app1", 50001335, 20001337),
                WithDefaultCondition("org1/app1", 50001337, 20001337)), // UserDelegation MainUnit Permit
            ConditionalAdd(
                DelegationChangesTestData.Default(DelegationChangesTestData.WithAltinnAppID("skd/taxreport"), DelegationChangesTestData.WithOfferedByPartyID(1000), DelegationChangesTestData.WithCoveredByUserID(20001337)),
                WithDefaultCondition("skd/taxreport", 1000, 20001337)),
            ConditionalAdd(
                DelegationChangesTestData.Default(DelegationChangesTestData.WithAltinnAppID("org1/app1"), DelegationChangesTestData.WithOfferedByPartyID(50001337), DelegationChangesTestData.WithCoveredByUserID(20001338), DelegationChangesTestData.WithDelegationChangeType(DelegationChangeType.RevokeLast)),
                WithDefaultCondition("org1/app1", 50001337, 20001338)),
            ConditionalAdd(
                DelegationChangesTestData.Default(DelegationChangesTestData.WithAltinnAppID("org1/app1"), DelegationChangesTestData.WithOfferedByPartyID(50001337), DelegationChangesTestData.WithCoveredByPartyID(50001336)),
                WithDefaultCondition("org1/app1", 50001337, 50001336),
                WithDefaultCondition("org1/app1", 50001337, 20001336),
                WithDefaultCondition("org1/app1", 50001335, 20001336)),
            ConditionalAdd(
                DelegationChangesTestData.Default(DelegationChangesTestData.WithAltinnAppID("skd/taxreport"), DelegationChangesTestData.WithOfferedByPartyID(50001337), DelegationChangesTestData.WithCoveredByUserID(20001336), DelegationChangesTestData.WithPerformedByUserID(20001337)),
                WithDefaultCondition("skd/taxreport", 50001337, 20001336)),
            ConditionalAdd(
                DelegationChangesTestData.Default(DelegationChangesTestData.WithAltinnAppID("skd/taxreport"), DelegationChangesTestData.WithOfferedByPartyID(50001337), DelegationChangesTestData.WithCoveredByUserID(20001335), DelegationChangesTestData.WithPerformedByUserID(20001337)),
                WithDefaultCondition("skd/taxreport", 50001337, 20001335)),
            ConditionalAdd(
                DelegationChangesTestData.Default(DelegationChangesTestData.WithAltinnAppID("skd/taxreport"), DelegationChangesTestData.WithOfferedByPartyID(50001336), DelegationChangesTestData.WithCoveredByUserID(20001335), DelegationChangesTestData.WithPerformedByUserID(20001337)),
                WithDefaultCondition("skd/taxreport", 50001337, 50001336)),
            ConditionalAdd(
                DelegationChangesTestData.Default(DelegationChangesTestData.WithAltinnAppID("skd/taxreport"), DelegationChangesTestData.WithOfferedByPartyID(50001338), DelegationChangesTestData.WithCoveredByUserID(50001339), DelegationChangesTestData.WithPerformedByUserID(20001338)),
                WithDefaultCondition("skd/taxreport", 50001338, 50001339)),
            ConditionalAdd(
                DelegationChangesTestData.Default(DelegationChangesTestData.WithAltinnAppID("skd/taxreport"), DelegationChangesTestData.WithOfferedByPartyID(50001338), DelegationChangesTestData.WithCoveredByUserID(50001340), DelegationChangesTestData.WithPerformedByUserID(20001338)),
                WithDefaultCondition("skd/taxreport", 50001338, 50001340)),
            ConditionalAdd(
                DelegationChangesTestData.Default(DelegationChangesTestData.WithAltinnAppID("skd/taxreport"), DelegationChangesTestData.WithOfferedByPartyID(50001339), DelegationChangesTestData.WithCoveredByUserID(20001336), DelegationChangesTestData.WithPerformedByUserID(20001339)),
                WithDefaultCondition("skd/taxreport", 50001339, 20001336)),
        };

        var result = new List<DelegationChange>();
        foreach (var item in data)
        {
            item(input, result);
        }

        return Task.FromResult(result as IEnumerable<DelegationChange>);
    }

    public static Func<DelegationChangeInput, bool> WithDefaultCondition(string appID, int partyID, int userID) => delegation =>
        IfAltinnAppID(appID)(delegation) && IfOfferedPartyID(partyID)(delegation) && IfCoveredByUserID(userID)(delegation);

    public static Func<DelegationChangeInput, bool> IfAltinnAppID(string appID) => delegation =>
    {
        var app = Strings.Join(
            new string[]
            {
                delegation.Resource.FirstOrDefault(resource => resource.Id == AltinnXacmlConstants.MatchAttributeIdentifiers.OrgAttribute)?.Value,
                delegation.Resource.FirstOrDefault(resource => resource.Id == AltinnXacmlConstants.MatchAttributeIdentifiers.AppAttribute)?.Value
            },
            "/");

        return app == appID;
    };

    public static Func<DelegationChangeInput, bool> IfCoveredByUserID(int userID) => delegation =>
        delegation.Subject.Value == userID.ToString();

    public static Func<DelegationChangeInput, bool> IfOfferedPartyID(int partyID) => delegation =>
        delegation.Party.Value == partyID.ToString();

    public static Action<DelegationChangeInput, List<DelegationChange>> ConditionalAdd(DelegationChange data, params Func<DelegationChangeInput, bool>[] actions)
    {
        return (input, result) =>
        {
            if (actions.Any(condtion => condtion(input)))
            {
                result.Add(data);
            }
        };
    }

    public async Task<IEnumerable<DelegationChange>> GetAllDelegationChanges(params Action<DelegationChangeInput>[] actions)
    {
        var result = new DelegationChangeInput();
        foreach (var action in actions)
        {
            action(result);
        }

        return await GetAllDelegationChanges(result);
    }
}