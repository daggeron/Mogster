using Sharlayan;
using Sharlayan.Core;
using Sharlayan.Utilities;
using System;
using System.Collections.Generic;


namespace Mogster.Core.Helpers
{
    public static class LINQExtension
    {
        public static IEnumerable<ActorItem> ToActor(this IEnumerable<IntPtr> pointers)
        {
            List<ActorItem> list = new List<ActorItem>();
            var sourceSize = MemoryHandler.Instance.Structures.ActorItem.SourceSize;
            

            foreach (var pointer in pointers)
            {
                byte[] source = MemoryHandler.Instance.GetByteArray(pointer, sourceSize);

                ActorItem entry = ActorItemResolver.ResolveActorFromBytes(source, true);
                list.Add(entry);
            }

            return list;
        }
    }
}
