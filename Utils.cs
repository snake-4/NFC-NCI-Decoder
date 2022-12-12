using System;
using System.ComponentModel;

namespace NCIDecoder
{
    static class Utils
    {
        //Will throw an exception if the enum has no RFU
        public static TEnum NCIEnumMemberOrRFU<TEnum>(object val) where TEnum : Enum
        {
            object valCasted = Convert.ChangeType(val, Enum.GetUnderlyingType(typeof(TEnum)));
            if (Enum.IsDefined(typeof(TEnum), valCasted))
            {
                return (TEnum)valCasted;
            }

            Enum.TryParse(typeof(TEnum), "RFU", out object? RFUVal);
            if (RFUVal != null)
            {
                return (TEnum)RFUVal;
            }
            throw new InvalidEnumArgumentException($"The System.Enum type {typeof(TEnum).Name} has no RFU field.");
        }
    }
}
