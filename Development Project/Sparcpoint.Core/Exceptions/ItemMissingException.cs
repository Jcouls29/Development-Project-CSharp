using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Exceptions
{
    public class ItemMissingException : Exception
    {
        public string ItemType { get; }
        public string ItemId { get; }

        public ItemMissingException(string itemType, string itemId)
            :base(FormatMessage(itemType, itemId))
        {
            ItemId = itemId;
            ItemType = itemType;
        }

        public ItemMissingException(string itemType, string itemId, Exception innerException)
            : base(FormatMessage(itemType, itemId), innerException)
        {
            ItemId = itemId;
            ItemType = itemType;
        }

        public ItemMissingException(string itemType, string itemId, string message)
            : base(FormatMessage(itemType, itemId, message))
        {
            ItemId = itemId;
            ItemType = itemType;
        }

        public ItemMissingException(string itemType, string itemId, string message, Exception innerException)
            : base(FormatMessage(itemType, itemId, message), innerException)
        {
            ItemId = itemId;
            ItemType = itemType;
        }

        private static string FormatMessage(string itemType, string itemId, string message = "")
        {
            return $"{itemType} with Id {itemId} is expected but missing. {message}".Trim();
        }
    }
}
