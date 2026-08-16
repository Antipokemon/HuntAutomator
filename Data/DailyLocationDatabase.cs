using System.Numerics;

namespace HuntAutomator.Data;

// Optional fast-path coordinates. Unknown marks fall back to a full map patrol.
// These can be expanded without changing the engine.
internal static class DailyLocationDatabase
{
    private static readonly Dictionary<uint, Vector2> Positions = new()
    {
        // Shadowbringers examples / frequently selected bill targets
        [8498] = new(19.0f, 9.0f), [8502] = new(28.0f, 23.2f), [8503] = new(14.0f, 16.5f),
        [8517] = new(31.9f, 18.9f), [8518] = new(36.4f, 28.7f), [8520] = new(17.0f, 18.0f),
        [8544] = new(11.4f, 30.4f), [8545] = new(19.1f, 20.9f), [8547] = new(30.4f, 12.3f),
        [8569] = new(18.0f, 31.0f), [8574] = new(31.0f, 14.3f), [8575] = new(19.9f, 16.3f),
        [8596] = new(8.8f, 35.6f), [8597] = new(27.3f, 25.6f), [8598] = new(25.1f, 14.2f),
        [8618] = new(28.6f, 6.2f), [8619] = new(28.2f, 18.3f), [8621] = new(22.6f, 31.7f),

        // Endwalker
        [10668] = new(28.8f, 8.8f), [10669] = new(31.0f, 25.5f), [10670] = new(15.0f, 6.5f),
        [10697] = new(19.0f, 23.9f), [10698] = new(13.8f, 18.5f), [10699] = new(19.2f, 32.6f),
        [10648] = new(18.8f, 9.8f), [10649] = new(25.5f, 17.5f), [10650] = new(15.5f, 19.5f),
        [10458] = new(23.9f, 20.0f), [10459] = new(23.7f, 20.3f), [10460] = new(8.6f, 35.5f),
        [10590] = new(25.7f, 33.9f), [10591] = new(16.5f, 29.9f), [10592] = new(22.6f, 20.0f),
        [10419] = new(30.1f, 25.9f), [10420] = new(19.3f, 11.8f), [10421] = new(34.8f, 28.8f),

        // Dawntrail
        [13079] = new(32.0f, 13.4f), [13090] = new(22.5f, 16.9f), [13083] = new(22.5f, 11.8f),
        [12946] = new(19.5f, 23.8f), [12935] = new(14.0f, 19.3f), [12930] = new(10.2f, 9.5f),
        [12957] = new(21.1f, 5.6f), [12966] = new(7.4f, 24.4f), [12969] = new(16.4f, 30.9f),
        [12990] = new(14.7f, 9.4f), [12989] = new(27.6f, 13.1f), [12975] = new(11.4f, 17.1f),
        [13115] = new(9.6f, 19.5f), [13101] = new(33.4f, 27.7f), [13103] = new(22.5f, 16.7f),
        [13121] = new(33.1f, 34.4f), [13137] = new(12.0f, 18.7f), [13133] = new(30.5f, 17.1f),
    };

    // Extra community-known spawn clusters. The first entry in Positions remains the
    // preferred fast path; these are searched before falling back to a full-map patrol.
    private static readonly Dictionary<uint, Vector2[]> AdditionalPositions = new()
    {
        // Alpaca groups in Urqopacha. Hunt Buddy lists more than one cluster, and the
        // old (32.0, 13.4) point can leave navigation at the edge of the northern group.
        [13079] = [new(32.0f, 14.9f), new(12.5f, 8.8f)],
    };

    public static IReadOnlyList<Vector2> GetPositions(uint mobId)
    {
        if (!Positions.TryGetValue(mobId, out var primary))
            return Array.Empty<Vector2>();

        if (!AdditionalPositions.TryGetValue(mobId, out var additional))
            return new[] { primary };

        var result = new Vector2[additional.Length + 1];
        result[0] = primary;
        additional.CopyTo(result, 1);
        return result;
    }
}
