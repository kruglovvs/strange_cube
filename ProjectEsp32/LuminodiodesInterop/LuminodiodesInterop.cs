using System;
using System.Runtime.CompilerServices;

namespace LuminodiodesInterop
{
    public class LuminodiodesInterop
    {
        #region Stubs
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void OneWireSendLuminodiodes(byte[] image, UInt32 pinNumber);
        #endregion stubs
    }
}
