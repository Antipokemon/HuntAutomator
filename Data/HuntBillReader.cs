using System.Globalization;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HuntAutomator.Models;
using Lumina.Excel.Sheets;

namespace HuntAutomator.Data;

internal static class HuntBillReader
{
    public static unsafe List<HuntTarget> ReadCurrentBills(Configuration cfg)
    {
        var result = new List<HuntTarget>();
        var mh = MobHunt.Instance();
        if (mh == null) return result;

        var orderSheet = Service.DataManager.GetSubrowExcelSheet<MobHuntOrder>();
        var typeSheet = Service.DataManager.Excel.GetSheet<MobHuntOrderType>();
        if (orderSheet is null || typeSheet is null) return result;

        for (byte bill = 0; bill < MobHunt.MaxMarkIndex; bill++)
        {
            if (!mh->IsMarkBillObtained(bill)) continue;

            MobHuntOrderType type;
            try { type = typeSheet.GetRow(bill); }
            catch { continue; }

            // Mark indexes are not ordered like the sheet (ARR elite was inserted after
            // Heavensward). Let the client resolve the currently obtained order row.
            var rowId = mh->GetObtainedHuntOrderRowId(bill);
            if (rowId <= 0) continue;

            IEnumerable<MobHuntOrder> rows;
            try { rows = orderSheet[(uint)rowId]; }
            catch { continue; }

            foreach (var order in rows)
            {
                if (order.Target.ValueNullable is not { } target || target.Map.ValueNullable is not { } map || map.TerritoryType.ValueNullable is not { } territory)
                    continue;

                var expansion = territory.ExVersion.RowId;
                if (!ExpansionEnabled(expansion, cfg)) continue;

                var elite = type.Type == 2;
                var kind = elite ? HuntKind.BRank : HuntKind.Daily;
                if (kind == HuntKind.Daily && !cfg.RunDailyMarks) continue;
                if (kind == HuntKind.BRank && !cfg.RunWeeklyBRanks) continue;

                var mobId = target.Name.RowId;
                var currentKills = elite ? 0 : mh->GetKillCount(bill, (byte)order.SubrowId);
                result.Add(new HuntTarget(
                    mobId,
                    CultureInfo.InvariantCulture.TextInfo.ToTitleCase(target.Name.Value.Singular.ToString()),
                    territory.RowId,
                    target.Map.RowId,
                    expansion,
                    kind,
                    bill,
                    (byte)order.SubrowId,
                    elite ? 1 : order.NeededKills,
                    currentKills,
                    DailyLocationDatabase.GetPositions(mobId)));
            }
        }

        return result
            .GroupBy(x => (x.MobId, x.BillNumber, x.MobIndex))
            .Select(x => x.First())
            .Where(x => x.Kind != HuntKind.Daily || x.RemainingKills > 0)
            .ToList();
    }

    public static unsafe int GetKillCount(HuntTarget target)
    {
        var mh = MobHunt.Instance();
        return mh == null ? target.CurrentKills : mh->GetKillCount(target.BillNumber, target.MobIndex);
    }

    private static bool ExpansionEnabled(uint id, Configuration c) => id switch
    {
        3 => c.EnableShadowbringers,
        4 => c.EnableEndwalker,
        5 => c.EnableDawntrail,
        _ => false,
    };
}
