using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace InWorldz.Phlox.Util
{
    public class Clock
    {

#if __MonoCS__ 
        //Todo: Fill in implementation for linux/cross platform GetTickCount64 implementation
#else
        [DllImport("kernel32.dll")]
        static extern UInt64 GetTickCount64();
#endif

        public static UInt64 GetLongTickCount()
        {
#if __MonoCS__ 
                //Todo: Fill in implementation for linux/cross platform GetTickCount64 implementation
                return (UInt64)Environment.TickCount;
#else
            return GetTickCount64();
#endif
        }

        public static DateTime TickCountToDateTime(UInt64 tickCount, UInt64 currentTickCount)
        {
            //tick count should never be zero. if it is it's just uninitialized
            if (tickCount == 0)
            {
                return DateTime.Now;
            }

            try
            {
                Int64 diff = (Int64)tickCount - (Int64)currentTickCount;
                return DateTime.Now.AddMilliseconds((double)diff);
            }
            catch (ArgumentOutOfRangeException e)
            {

                throw new ArgumentOutOfRangeException(
                    String.Format("{0}: tickCount: {1}, currentTickCount: {2}", e.Message, tickCount, currentTickCount),
                    e);
            }
        }
    }
}
