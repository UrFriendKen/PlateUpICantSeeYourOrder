using Kitchen;
using MessagePack;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenICantSeeYourOrder
{
    public class ItemCollectionExpanderView : UpdatableObjectView<ItemCollectionExpanderView.ViewData>
    {
        public class UpdateView : IncrementalViewSystemBase<ViewData>
        {
            EntityQuery Views;

            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(typeof(CLinkedView), typeof(CInterfaceOf), typeof(CDisplayedItem), typeof(CPosition));
            }

            protected override void OnUpdate()
            {
                using NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                using NativeArray<CInterfaceOf> interfaceOfs = Views.ToComponentDataArray<CInterfaceOf>(Allocator.Temp);
                for (int i = 0; i < views.Length; i++)
                {
                    CInterfaceOf interfaceOf = interfaceOfs[i];
                    SendUpdate(views[i], new ViewData()
                    {
                        IsExpanded = Require(interfaceOf.Entity, out CAssignedTable assignedTable) && Has<CExpandOrders>(assignedTable.Table)
                    });
                }
            }
        }

        public ItemCollectionView ItemCollectionView;

        public Vector3 DefaultScale = Vector3.one;

        static FieldInfo f_DrawnItems = typeof(ItemCollectionView).GetField("DrawnItems", BindingFlags.NonPublic | BindingFlags.Instance);

        [MessagePackObject(false)]
        public class ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public bool IsExpanded;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<ItemCollectionExpanderView>();

            public bool IsChangedFrom(ViewData check)
            {
                return IsExpanded != check.IsExpanded;
            }
        }

        bool ShouldExpand = false;

        bool IsExpanded = false;

        ItemCollectionView.DrawnItem[] _drawnItems;

        void Update()
        {
            if (ItemCollectionView == null)
                return;
            if (_drawnItems == null)
            {
                ItemCollectionView.DrawnItem[] drawnItems = (ItemCollectionView.DrawnItem[])f_DrawnItems?.GetValue(ItemCollectionView);
                if (drawnItems == null)
                    return;
                _drawnItems = drawnItems;
            }
            float interpotationRatio = Mathf.Clamp01(Time.deltaTime / Main.PrefManager.Get<float>(Main.TRANSITION_TIME_ID));
            Vector3 targetScale = Main.PrefManager.Get<float>(ShouldExpand? Main.EXPANDED_SIZE_ID : Main.MINIMIZED_SIZE_ID) * DefaultScale;
            for (int i = 0; i < _drawnItems.Length; i++)
            {
                GameObject obj = _drawnItems[i].Object;
                if (obj == null)
                    continue;
                if (_drawnItems[i].IsComplete)
                {
                    obj.transform.localScale = DefaultScale;
                    continue;
                }
                obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, targetScale, interpotationRatio);
            }
        }

        protected override void UpdateData(ViewData data)
        {
            ShouldExpand = data.IsExpanded;
        }
    }
}
