using System;
using System.Text;

namespace Mogster.Core.IPC.NativeWebSocket
{
    static class HelperExtensions
    {
        /// <summary>
        /// Converts the specified binary data to a string data using the specified encoding.
        /// </summary>
        /// <param name="segment">Binary data.</param>
        /// <param name="e">Encoding.</param>
        /// <returns>Text data.</returns>
        public static string ToString(this ArraySegment<byte> segment, Encoding e)
        {
            var str = e.GetString(segment.Array, segment.Offset, segment.Count);
            return str;
        }
    }
}
