//using FJanter.TakuLib.DiskAccessLib;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace TakuLib.DiskAccessLib
{
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public sealed class WinIOAPI : IDisposable
    {
        private const uint FORMAT_MESSAGE_FROM_SYSTEM = 4096;
        private const string API_ERR_NUM_FORM = "0000000";
        private static string _errorMessage = "";
        private static uint _APIErrorCode = 0;
        private const int ERROR_INVALID_HANDLE = 6;
        public const uint ERROR_CRC = 23;
        public const uint ERROR_SEEK = 25;
        public const uint ERROR_SECTOR_NOT_FOUND = 27;
        public const uint INVALID_SET_FILE_POINTER = 4294967295;

        [DllImport("kernel32.dll")]
        private static extern uint FormatMessage(
          uint dwFlags,
          IntPtr lpSource,
          uint dwMessageId,
          uint dwLanguageId,
          StringBuilder lpBuffer,
          int nSize,
          IntPtr Arguments);

        public void Dispose()
        {
        }

        public static string MediaName(WinIOAPI.MEDIA_TYPE g) => new string[26]
        {
      "Format is unknown",
      "5.25\", 1.2MB,  512 bytes/sector",
      "3.5\",  1.44MB, 512 bytes/sector",
      "3.5\",  2.88MB, 512 bytes/sector",
      "3.5\",  20.8MB, 512 bytes/sector",
      "3.5\",  720KB,  512 bytes/sector",
      "5.25\", 360KB,  512 bytes/sector",
      "5.25\", 320KB,  512 bytes/sector",
      "5.25\", 320KB,  1024 bytes/sector",
      "5.25\", 180KB,  512 bytes/sector",
      "5.25\", 160KB,  512 bytes/sector",
      "Removable media other than floppy",
      "Fixed hard disk media",
      "3.5\", 120M Floppy",
      "3.5\",  640KB,  512 bytes/sector",
      "5.25\",  640KB,  512 bytes/sector",
      "5.25\",  720KB,  512 bytes/sector",
      "3.5\" ,  1.2Mb,  512 bytes/sector",
      "3.5\" ,  1.23Mb, 1024 bytes/sector",
      "5.25\",  1.23MB, 1024 bytes/sector",
      "3.5\" MO 128Mb   512 bytes/sector",
      "3.5\" MO 230Mb   512 bytes/sector",
      "8\",     256KB,  128 bytes/sector",
      "3.5\",   200M Floppy (HiFD)",
      "3.5\",   240Mb Floppy (HiFD)",
      "3.5\",   32Mb Floppy"
        }[(int)g];

        public uint APIErrorCode => WinIOAPI._APIErrorCode;

        public string APIErrorMessage
        {
            get
            {
                StringBuilder lpBuffer = new StringBuilder((int)byte.MaxValue);
                int num = (int)WinIOAPI.FormatMessage(4096U, IntPtr.Zero, this.APIErrorCode, 0U, lpBuffer, lpBuffer.Capacity, IntPtr.Zero);
                WinIOAPI._errorMessage = this.APIErrorCode.ToString("0000000") + ":" + lpBuffer.ToString();
                return WinIOAPI._errorMessage;
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "CreateFile", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern SafeFileHandle _CreateFile(
          [MarshalAs(UnmanagedType.LPTStr)] string filename,
          [MarshalAs(UnmanagedType.U4)] WinIOAPI.DesiredAccess access,
          [MarshalAs(UnmanagedType.U4)] WinIOAPI.ShareMode share,
          IntPtr securityAttributes,
          [MarshalAs(UnmanagedType.U4)] WinIOAPI.CreationDisposition creationDisposition,
          [MarshalAs(UnmanagedType.U4)] WinIOAPI.FlagsAndAttributes flagsAndAttributes,
          IntPtr templateFile);

        public SafeFileHandle CreateFile(
          string filename,
          WinIOAPI.DesiredAccess access,
          WinIOAPI.ShareMode share,
          IntPtr securityAttribute,
          WinIOAPI.CreationDisposition creationDisposition,
          WinIOAPI.FlagsAndAttributes flagsAndAttributes,
          IntPtr templateFile)
        {
            SafeFileHandle file = WinIOAPI._CreateFile(filename, access, share, securityAttribute, creationDisposition, flagsAndAttributes, templateFile);
            return !file.IsInvalid ? file : throw new WinIOAPI.APICallException("CreateFile 呼び出しでエラーが発生しました。" + this.APIErrorMessage);
        }

        [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _DeviceIoControl(
          SafeFileHandle hDevice,
          [MarshalAs(UnmanagedType.U4)] WinIOAPI.ControlCodes dwIOControlCode,
          IntPtr lpInBuffer,
          uint nInBufferSize,
          IntPtr lpOutBuffer,
          uint nOutBufferSize,
          out uint lpBytesReturned,
          IntPtr lpOverlapped);

        public bool DeviceIoControl(
          SafeFileHandle hDevice,
          WinIOAPI.ControlCodes ControlCode,
          IntPtr lpInBuffer,
          uint BufferSize,
          IntPtr lpOutBuffer,
          uint OutBufferSize,
          ref uint BytesReturned,
          IntPtr lpOverlapped)
        {
            int num = WinIOAPI._DeviceIoControl(hDevice, ControlCode, lpInBuffer, BufferSize, lpOutBuffer, OutBufferSize, out BytesReturned, lpOverlapped) ? 1 : 0;
            if (num != 0)
                return num != 0;
            WinIOAPI._APIErrorCode = (uint)Marshal.GetLastWin32Error();
            return num != 0;
        }

        [DllImport("kernel32.dll", EntryPoint = "WriteFile", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _WriteFile(
          SafeFileHandle hFile,
          byte[] aBuffer,
          uint numerOfByteToWrite,
          ref uint numberOfByteWrittten,
          IntPtr Overlapped);

        public bool WriteFile(
          SafeFileHandle hFile,
          byte[] aBuffer,
          uint numberOfByteToWrite,
          ref uint numberOfByteWritten,
          IntPtr Overlapped)
        {
            int num = WinIOAPI._WriteFile(hFile, aBuffer, numberOfByteToWrite, ref numberOfByteWritten, Overlapped) ? 1 : 0;
            if (num != 0)
                return num != 0;
            WinIOAPI._APIErrorCode = (uint)Marshal.GetLastWin32Error();
            return num != 0;
        }

        [DllImport("kernel32.dll", EntryPoint = "ReadFile", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _ReadFile(
          SafeFileHandle hFile,
          byte[] aBuffer,
          uint numerOfByteToRead,
          ref uint numberOfByteRead,
          IntPtr Overlapped);

        public bool ReadFile(
          SafeFileHandle hFile,
          byte[] aBuffer,
          uint numberOfByteToRead,
          ref uint numberOfByteRead,
          IntPtr Overlapped)
        {
            int num = WinIOAPI._ReadFile(hFile, aBuffer, numberOfByteToRead, ref numberOfByteRead, Overlapped) ? 1 : 0;
            if (num != 0)
                return num != 0;
            WinIOAPI._APIErrorCode = (uint)Marshal.GetLastWin32Error();
            return num != 0;
        }

        [DllImport("kernel32.dll", EntryPoint = "SetFilePointer", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern uint _SetFilePointer(
          SafeFileHandle hFile,
          uint distanceToMove,
          UIntPtr distanceToMoveHigh,
          WinIOAPI.MoveMethod method);

        public uint SetFilePointer(
          SafeFileHandle hFile,
          uint distanceToMove,
          UIntPtr distanceToMoveHigh,
          WinIOAPI.MoveMethod method)
        {
            int num = (int)WinIOAPI._SetFilePointer(hFile, distanceToMove, distanceToMoveHigh, method);
            if (num != -1)
                return (uint)num;
            WinIOAPI._APIErrorCode = (uint)Marshal.GetLastWin32Error();
            return (uint)num;
        }

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr SHGetFileInfo(
          string pszPath,
          uint dwAttribute,
          ref WinIOAPI.SHFILEINFO psfi,
          uint cbSizeFileInfo,
          uint uFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetShortPathNameW(
          [MarshalAs(UnmanagedType.LPTStr)] string lpszeLongPath,
          [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszShortPath,
          uint cBuffer);

        public string GetShortPath(string Path)
        {
            StringBuilder lpszShortPath = new StringBuilder(1024);
            if (WinIOAPI.GetShortPathNameW(Path, lpszShortPath, (uint)lpszShortPath.Capacity) == 0U)
            {
                WinIOAPI._APIErrorCode = (uint)Marshal.GetLastWin32Error();
                if (WinIOAPI._APIErrorCode != 0U)
                    throw new WinIOAPI.APICallException(this.APIErrorMessage);
                lpszShortPath.Append(Path);
            }
            return lpszShortPath.ToString();
        }

        public bool GetDriveGeometry(string DriveName, ref WinIOAPI.DISK_GEOMETRY outGEOMETRY)
        {
            uint BytesReturned = 0;
            IntPtr num = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WinIOAPI.DISK_GEOMETRY)));
            string filename = DiskUtils.ConvPhysicalDrive(DriveName);
            uint OutBufferSize = (uint)Marshal.SizeOf(typeof(WinIOAPI.DISK_GEOMETRY));
            using (SafeFileHandle file = this.CreateFile(filename, WinIOAPI.DesiredAccess.DEVICE_ACCESS, WinIOAPI.ShareMode.FILE_SHARE_READ, IntPtr.Zero, WinIOAPI.CreationDisposition.OPEN_EXISTING, (WinIOAPI.FlagsAndAttributes)0, IntPtr.Zero))
            {
                if (!this.DeviceIoControl(file, WinIOAPI.ControlCodes.IOCTL_DISK_GET_DRIVE_GEOMETRY, IntPtr.Zero, 0U, num, OutBufferSize, ref BytesReturned, IntPtr.Zero))
                {
                    WinIOAPI._APIErrorCode = (uint)Marshal.GetLastWin32Error();
                    if (WinIOAPI._APIErrorCode != 0U)
                        throw new WinIOAPI.APICallException(this.APIErrorMessage);
                }
                outGEOMETRY = (WinIOAPI.DISK_GEOMETRY)Marshal.PtrToStructure(num, typeof(WinIOAPI.DISK_GEOMETRY));
                return true;
            }
        }

        public class APICallException : Exception
        {
            public APICallException()
            {
            }

            public APICallException(string message)
              : base(message)
            {
            }

            public APICallException(string message, Exception innerException)
              : base(message, innerException)
            {
            }

            protected APICallException(SerializationInfo info, StreamingContext context)
              : base(info, context)
            {
            }
        }

        public enum MEDIA_TYPE : uint
        {
            Unknown,
            F5_1Pt2_512,
            F3_1Pt44_512,
            F3_2Pt88_512,
            F3_20Pt8_512,
            F3_720_512,
            F5_360_512,
            F5_320_512,
            F5_320_1024,
            F5_180_512,
            F5_160_512,
            RemovableMedia,
            FixedMedia,
            F3_120M_512,
            F3_640_512,
            F5_640_512,
            F5_720_512,
            F3_1Pt2_512,
            F3_1Pt23_1024,
            F5_1Pt23_1024,
            F3_128Mb_512,
            F3_230Mb_512,
            F8_256_128,
            F3_200Mb_512,
            F3_240M_512,
            F3_32M_512,
        }

        public struct DISK_GEOMETRY
        {
            public ulong Cylinders;
            public WinIOAPI.MEDIA_TYPE MediaType;
            public uint TracksPerCylinder;
            public uint SectorsPerTrack;
            public uint BytesPerSector;
        }

        public struct FORMAT_PARAMETERS
        {
            public WinIOAPI.MEDIA_TYPE MediaType;
            public uint StartCylinderNumber;
            public uint EndCylinderNumber;
            public uint StartHeadNumber;
            public uint EndHeadNumber;
        }

        public struct DISK_GEOMETRY_EX
        {
            public WinIOAPI.DISK_GEOMETRY Geometry;
            public ulong DiskSize;
            public byte data;
        }

        public enum ControlCodes
        {
            IOCTL_DISK_GET_DRIVE_GEOMETRY = 458752, // 0x00070000
            IOCTL_DISK_VERIFY = 458772, // 0x00070014
            IOCTL_DISK_PERFORMANCE = 458784, // 0x00070020
            IOCTL_DISK_GET_DRIVE_GEOMETRY_EX = 458912, // 0x000700A0
            IOCTL_DISK_GET_PARTITION_INFO = 475140, // 0x00074004
            IOCTL_DISK_GET_DRIVE_LAYOUT = 475148, // 0x0007400C
            IOCTL_DISK_SET_PARTITION_INFO = 507912, // 0x0007C008
            IOCTL_DISK_SET_DRIVE_LAYOUT = 507920, // 0x0007C010
            IOCTL_DISK_FORMAT_TRACKS = 507928, // 0x0007C018
            IOCTL_DISK_REASSIGN_BLOCKS = 507932, // 0x0007C01C
            FSCTL_REQUEST_OPLOCK_LEVEL_1 = 589824, // 0x00090000
            FSCTL_REQUEST_OPLOCK_LEVEL_2 = 589828, // 0x00090004
            FSCTL_REQUEST_BATCH_OPLOCK = 589832, // 0x00090008
            FSCTL_OPLOCK_BREAK_ACKNOWLEDGE = 589836, // 0x0009000C
            FSCTL_OPBATCH_ACK_CLOSE_PENDING = 589840, // 0x00090010
            FSCTL_OPLOCK_BREAK_NOTIFY = 589844, // 0x00090014
            FSCTL_LOCK_VOLUME = 589848, // 0x00090018
            FSCTL_UNLOCK_VOLUME = 589852, // 0x0009001C
            FSCTL_DISMOUNT_VOLUME = 589856, // 0x00090020
            FSCTL_GET_COMPRESSION = 589884, // 0x0009003C
            FSCTL_SET_COMPRESSION = 589888, // 0x00090040
            FSCTL_OPLOCK_BREAK_ACK_NO_2 = 589904, // 0x00090050
            FSCTL_QUERY_FAT_BPB = 589912, // 0x00090058
            FSCTL_REQUEST_FILTER_OPLOCK = 589916, // 0x0009005C
            FSCTL_ALLOW_EXTENDED_DASD_IO = 589955, // 0x00090083
            FSCTL_SET_REPARSE_POINT = 589988, // 0x000900A4
            FSCTL_GET_REPARSE_POINT = 589992, // 0x000900A8
            FSCTL_DELETE_REPARSE_POINT = 589996, // 0x000900AC
            FSCTL_ENUM_USN_DATA = 590003, // 0x000900B3
            FSCTL_READ_USN_JOURNAL = 590011, // 0x000900BB
            FSCTL_SET_SPARSE = 590020, // 0x000900C4
            FSCTL_CREATE_USN_JOURNAL = 590055, // 0x000900E7
            FSCTL_QUERY_USN_JOURNAL = 590068, // 0x000900F4
            FSCTL_DELETE_USN_JOURNAL = 590072, // 0x000900F8
            FSCTL_MARK_HANDLE = 590076, // 0x000900FC
            FSCTL_RECALL_FILE = 590103, // 0x00090117
            FSCTL_QUERY_ALLOCATED_RANGES = 606415, // 0x000940CF
            FSCTL_SET_ZERO_DATA = 622792, // 0x000980C8
            IOCTL_STORAGE_GET_MEDIA_TYPES = 2952192, // 0x002D0C00
            IOCTL_STORAGE_CHECK_VERIFY = 2967552, // 0x002D4800
            IOCTL_STORAGE_MEDIA_REMOVAL = 2967556, // 0x002D4804
            IOCTL_STORAGE_EJECT_MEDIA = 2967560, // 0x002D4808
            IOCTL_STORAGE_LOAD_MEDIA = 2967564, // 0x002D480C
            IOCTL_VOLUME_ONLINE = 5685256, // 0x0056C008
            IOCTL_VOLUME_OFFLINE = 5685260, // 0x0056C00C
        }

        public enum DesiredAccess : uint
        {
            DEVICE_ACCESS = 0,
            GENERIC_WRITE = 1073741824, // 0x40000000
            GENERIC_READ = 2147483648, // 0x80000000
        }

        public enum ShareMode : uint
        {
            FILE_SHARE_READ = 1,
            FILE_SHARE_WRITE = 2,
            FILE_SHARE_DELETE = 4,
        }

        public enum CreationDisposition : uint
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXISTING = 5,
        }

        public enum FlagsAndAttributes : uint
        {
            FILE_ATTRIBUTE_READONLY = 1,
            FILE_ATTRIBUTE_HIDDEN = 2,
            FILE_ATTRIBUTE_SYSTEM = 4,
            FILE_ATTRIBUTE_ARCHIVE = 32, // 0x00000020
            FILE_ATTRIBUTE_NORMAL = 128, // 0x00000080
            FILE_ATTRIBUTE_TEMPORARY = 256, // 0x00000100
            FILE_ATTRIBUTE_OFFLINE = 4096, // 0x00001000
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 8192, // 0x00002000
            FILE_ATTRIBUTE_ENCRYPTED = 16384, // 0x00004000
            FILE_FLAG_NO_BUFFERING = 536870912, // 0x20000000
        }

        public enum MoveMethod : uint
        {
            FILE_BEGIN,
            FILE_CURRENT,
            FILE_END,
        }

        public enum SHGFI
        {
            LARGEICON = 0,
            SMALLICON = 1,
            ICON = 256, // 0x00000100
        }

        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
    }
}
