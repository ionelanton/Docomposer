using System.Collections.Generic;
using System.Linq;

namespace Docomposer.Blazor.Data.DragAndDrop
{
    public static class ItemsListExtensions
    {
        public static List<ItemIncludedModel> Clone(this IEnumerable<ItemIncludedModel> items)
        {
            var clone = new List<ItemIncludedModel>();

            foreach (var item in items)
            {
                clone.Add(new ItemIncludedModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    PredecessorId = item.PredecessorId
                });
            }

            return clone;
        }

        public static bool Equal(this ItemIncludedModel item, ItemIncludedModel anotherItem)
        {
            return item.Id == anotherItem.Id && item.Name == anotherItem.Name &&
                   item.PredecessorId == anotherItem.PredecessorId;
        }

        public static void FixPredecessors(this List<ItemIncludedModel> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (i == 0)
                {
                    items[i].PredecessorId = 0;
                }
                else
                {
                    items[i].PredecessorId = items[i - 1].Id;
                }    
            }
        }
        
        public static List<ItemIncludedModel> GetItemsHavingPredecessorsChanged(this List<ItemIncludedModel> original, List<ItemIncludedModel> updated)
        {
            return updated.Where(item => !original.Exists(i => i.Equal(item))).ToList();
        }

        public static string ToStr(this List<ItemIncludedModel> items)
        {
            return items.Aggregate("", (current, item) => current + $"Id: {item.Id}, Name: {item.Name}, PredecessorId: {item.PredecessorId}\n");
        }
    }
}