using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Mogster.Core.Helpers
{
    //Idea from: https://stackoverflow.com/questions/7299097/dynamically-replace-the-contents-of-a-c-sharp-method
    internal sealed class Injection
    {
        internal static BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;

        public static void PatchMethod(Type replaceType, String repaceMethod, Type injectType, String injectMethod)
        {
            MethodInfo methodToReplace = replaceType.GetMethod(repaceMethod, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo methodToInject = injectType.GetMethod(injectMethod, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            PatchMethod(methodToReplace, methodToInject);
        }

        public static void PatchMethod(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);

            unsafe
            {
                if (IntPtr.Size == 4)
                {
                    int* inj = (int*)methodToInject.MethodHandle.Value.ToPointer() + 2;
                    int* tar = (int*)methodToReplace.MethodHandle.Value.ToPointer() + 2;
                    *tar = *inj;
                }
                else
                {
                    long* inj = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
                    *tar = *inj;
                }
            }
        }

        private void BlankVoid() { }
        private Boolean ReturnTrue() { return true; }
        private Boolean ReturnFalse() { return false; }

        public static void InvokeMethod(object obj, String methodName, object[] objects)
        {
            obj.GetType().GetMethod(methodName, bindingAttr).Invoke(obj, objects);
        }

        public static void SetField(object obj, String field, object value)
        {
            obj.GetType().GetField(field, bindingAttr).SetValue(obj, value);
        }
    }
}
