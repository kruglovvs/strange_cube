using System.Runtime.CompilerServices;

namespace NF.OneWire
{
    public class YF923
    {
        #region Stubs
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void SendImage(byte[] image, uint pinNumber);
        #endregion stubs
    }
}
