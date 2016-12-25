using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostDistributer
{
    public class SingleInstanceChecker : IDisposable
    {
        private const uint ERROR_SUCCESS = 0;
        private const uint ERROR_ALREADY_EXISTS = 183;

        [DllImport("Kernel32.dll")]
        private extern static IntPtr CreateMutex(
            IntPtr lpMutexAttributes,
            [MarshalAs(UnmanagedType.Bool)]
            bool bInitialOwner,
            [MarshalAs(UnmanagedType.LPStr)]
            string lpName);

        [DllImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool CloseHandle(IntPtr hObject);

        [DllImport("Kernel32.dll")]
        private extern static uint GetLastError();

        [DllImport("Kernel32.dll")]
        private extern static void SetLastError(uint dwErrCode);

        // Implementation

        private IntPtr _hMutex;

        public SingleInstanceChecker()
        {
            _hMutex = IntPtr.Zero;
        }

        public void Dispose()
        {
            Close();
        }

        public RegisterInstanceResult RegisterInstance(string guid)
        {
            if (_hMutex != IntPtr.Zero)
                return RegisterInstanceResult.CallCloseBeforeRegisteringAgain;

            SetLastError(ERROR_SUCCESS);
            _hMutex = CreateMutex(IntPtr.Zero, false, string.Format(@"Global\{0}", guid));
            var errorCode = GetLastError();
            if (_hMutex == IntPtr.Zero)
                return RegisterInstanceResult.UnknownError;

            if (errorCode == ERROR_ALREADY_EXISTS)
            {
                Close();
                return RegisterInstanceResult.InstanceAlreadyExist;
            }

            return RegisterInstanceResult.Succeeded;
        }

        public void Close()
        {
            if (_hMutex != IntPtr.Zero)
            {
                CloseHandle(_hMutex);
                _hMutex = IntPtr.Zero;
            }
        }
    }

    public enum RegisterInstanceResult
    {
        Succeeded,
        InstanceAlreadyExist,
        CallCloseBeforeRegisteringAgain,
        UnknownError
    }
}
