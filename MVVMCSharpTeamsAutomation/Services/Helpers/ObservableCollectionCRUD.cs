using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVVMCSharpTeamsAutomation.Services.Helpers
{
    /// <summary>
    /// Class for simplifying the process of operating on program's lists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ObservableCollectionCRUD<T>
    {
        /// <summary>
        /// Adds a new item to the target list.
        /// </summary>
        /// <param name="newItem">Item to add.</param>
        /// <param name="targetList">List to which new item is added.</param>
        public static void AddToCollection(T newItem, ObservableCollection<T> targetList)
        {
            targetList.Add(newItem);
        }

        /// <summary>
        /// Updates the target item with properties of new item.
        /// Method omits read-only properties and Id fields of model objects.
        /// </summary>
        /// <param name="newItem">Item to copy properties from.</param>
        /// <param name="targetItem">Item in a collection which is updated.</param>
        public static void UpdateCollection(T newItem, T targetItem)
        {
            foreach (PropertyInfo propertyInfo in targetItem.GetType().GetProperties())
            {
                if(propertyInfo.Name != "Id" && propertyInfo.CanWrite)
                {
                    var valueToUpdate = propertyInfo.GetValue(newItem, null);
                    propertyInfo.SetValue(targetItem, valueToUpdate);
                }
            }
        }

        /// <summary>
        /// Deletes item in a collection by index.
        /// </summary>
        /// <param name="targetList">Collection from which item should be deleted</param>
        /// <param name="targetIndex">Index of an item to delete.</param>
        public static void DeleteFromCollection(ObservableCollection<T> targetList, int targetIndex)
        {
            targetList.RemoveAt(targetIndex);
        }
    }
}
