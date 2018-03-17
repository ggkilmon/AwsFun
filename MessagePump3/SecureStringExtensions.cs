using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AWSFun
{
    public static class SecureStringExtensions
    {
        public static SecureString ToSecureString(this String str)
        {
            SecureString sString = new SecureString();
            foreach (Char c in str)
            {
                sString.AppendChar(c);
            }

            return sString;
        }

        public static string ToUnsecureString(this SecureString str)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(str);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
