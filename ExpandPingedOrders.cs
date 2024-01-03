using Kitchen;
using KitchenMods;
using System.Runtime.InteropServices;
using Unity.Entities;

namespace KitchenICantSeeYourOrder
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct CExpandOrders : IComponentData, IModComponent { }

    [UpdateBefore(typeof(MakePing))]
    public class ExpandPingedOrders : ItemInteractionSystem, IModSystem
    {
        protected override InteractionType RequiredType => InteractionType.Notify;

        Entity _tableSetEntity;

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require(data.Target, out CApplianceTable table) || table.IsWaitingTable)
                return false;
            if (!Require(data.Target, out CPartOfTableSet partOfTableSet) || partOfTableSet.TableSet == default)
                return false;
            if (!Has<CTableSet>(partOfTableSet.TableSet))
                return false;
            if (Has<CExpandOrders>(partOfTableSet.TableSet))
                return false;
            _tableSetEntity = partOfTableSet.TableSet;
            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            Set<CExpandOrders>(_tableSetEntity);
        }
    }
}
