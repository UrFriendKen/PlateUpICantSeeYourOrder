using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace KitchenICantSeeYourOrder
{
    public class ClearExpandedOrders : GameSystemBase, IModSystem
    {
        EntityQuery TableSets;

        protected override void Initialise()
        {
            base.Initialise();
            TableSets = GetEntityQuery(new QueryHelper()
                .All(typeof(CExpandOrders), typeof(CTableSet)));
            RequireForUpdate(TableSets);
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = TableSets.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                bool shouldRemove = true;
                if (RequireBuffer(entity, out DynamicBuffer<CTableSetParts> tableSetParts))
                {
                    for (int i = 0; i < tableSetParts.Length; i++)
                    {
                        if (!Has<CBeingLookedAt>(tableSetParts[i].Entity))
                            continue;
                        shouldRemove = false;
                        break;
                    }
                }
                if (!shouldRemove)
                    continue;
                EntityManager.RemoveComponent<CExpandOrders>(entity);
            }

        }
    }
}
