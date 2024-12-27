using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Text;

namespace kakarot
{
    internal class DiskUtils
    {
        private WinIOAPI.DISK_GEOMETRY geo;
        private const int FLOPPY_DISK = 8;
        private const int HARD_DISK = 12;
        private const int REMOVABLE_DISK = 11;
        private const string END_DELIMITER = "\0\0";

        [DllImport("fmifs.dll", CharSet = CharSet.Auto)]
        private static extern void FormatEx(
          string driveLetter,
          int mediaFlag,
          string fsType,
          string label,
          [MarshalAs(UnmanagedType.Bool)] bool quickFormat,
          int clusterSize,
          FormatCallBackDelegate callBackDelegate);

        [DllImport("kernel32.dll")]
        private static extern uint FormatMessage(
          uint dwFlags,
          nint lpSource,
          uint dwMessageId,
          uint dwLanguageId,
          StringBuilder lpBuffer,
          int nSize,
          nint Arguments);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHFileOperation([In] ref SHFILEOPSTRUCT lpFileOp);

        public static bool IsFloppy2DD(string driveLetter)
        {
            WinIOAPI winIoapi = new WinIOAPI();
            if (!new DriveInfo(driveLetter.Substring(0, 1)).DriveType.Equals(DriveType.Removable))
                return false;
            nint num1 = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WinIOAPI.DISK_GEOMETRY)));
            string filename = ConvPhysicalDrive(driveLetter);
            uint OutBufferSize = (uint)Marshal.SizeOf(typeof(WinIOAPI.DISK_GEOMETRY));
            uint BytesReturned = 0;
            SafeFileHandle file = winIoapi.CreateFile(filename, WinIOAPI.DesiredAccess.DEVICE_ACCESS, WinIOAPI.ShareMode.FILE_SHARE_READ, nint.Zero, WinIOAPI.CreationDisposition.OPEN_EXISTING, 0, nint.Zero);
            int num2 = winIoapi.DeviceIoControl(file, WinIOAPI.ControlCodes.IOCTL_DISK_GET_DRIVE_GEOMETRY, nint.Zero, 0U, num1, OutBufferSize, ref BytesReturned, nint.Zero) ? 1 : 0;
            file.Close();
            file.Dispose();
            return num2 != 0 && ((WinIOAPI.DISK_GEOMETRY)Marshal.PtrToStructure(num1, typeof(WinIOAPI.DISK_GEOMETRY))).MediaType == WinIOAPI.MEDIA_TYPE.F3_720_512;
        }

        /*  public bool FormatDisk(
           string driveLetter,
           DiskUtils.FormatType formatType,
           string VolumeLbl,
           bool QuickFormat = false,
           bool StartUpDisk = false,
           ProgressBar prgBar = null)
         {
             bool flag = true;
             switch (formatType)
             {
                 case DiskUtils.FormatType.MSX_DOS1:
                 case DiskUtils.FormatType.MSX_DOS2:
                     flag = this.FormatMSX(driveLetter, formatType, VolumeLbl, QuickFormat, StartUpDisk, prgBar);
                     break;
             }
             return flag;
         }

        private bool FormatMSX(
           string driveLetter,
           DiskUtils.FormatType formatType,
           string VolumeLbl,
           bool QuickFormat,
           bool SysCopy,
           ProgressBar prgBar = null)
         {
             VirtualDisk virtualDisk = new VirtualDisk();
             string startupPath = Application.StartupPath;
             string[] strArray = new string[2];
             if (!startupPath.EndsWith("\\"))
                 startupPath += "\\";
             if (!SysCopy)
             {
                 strArray[0] = startupPath + ConfigurationManager.AppSettings["DOS1dskFileName"];
                 strArray[1] = startupPath + ConfigurationManager.AppSettings["DOS2dskFileName"];
             }
             else
             {
                 strArray[0] = Settings.Default.DOS1ImageFile;
                 strArray[1] = Settings.Default.DOS2ImageFile;
             }
             string volumeLabeledVdisk = strArray[(int)(formatType - 1U)];
             if (!File.Exists(volumeLabeledVdisk))
             {
                 int num = (int)MessageBox.Show(string.Format(Resources.MSGB_FNNFD, (object)volumeLabeledVdisk), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                 return false;
             }
             if (formatType.Equals((object)DiskUtils.FormatType.MSX_DOS2) && !VolumeLbl.Equals(string.Empty))
             {
                 volumeLabeledVdisk = this.CreateVolumeLabeledVDisk(volumeLabeledVdisk, VolumeLbl);
                 if (volumeLabeledVdisk.Equals(string.Empty))
                 {
                     int num = (int)MessageBox.Show(string.Format(Resources.MSGB_VLFAL, (object)volumeLabeledVdisk), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                     return false;
                 }
             }
             int num1 = virtualDisk.VDiskToRDisk(volumeLabeledVdisk, driveLetter, prgBar, !QuickFormat) ? 1 : 0;
             if (!formatType.Equals((object)DiskUtils.FormatType.MSX_DOS2))
                 return num1 != 0;
             if (VolumeLbl.Equals(string.Empty))
                 return num1 != 0;
             File.Delete(volumeLabeledVdisk);
             return num1 != 0;
         }*/

        private string CreateVolumeLabeledVDisk(string DiskFileName, string VolumeLbl)
        {
            string volumeLabeledVdisk = Path.GetTempPath() + Path.GetFileName(DiskFileName);
            File.Copy(DiskFileName, volumeLabeledVdisk, true);
            DSKFATOperation dskfatOperation = new DSKFATOperation(volumeLabeledVdisk);
            dskfatOperation.WriteVolumeLabel(VolumeLbl);
            dskfatOperation.Dispose();
            return volumeLabeledVdisk;
        }

        private static int FormatCallBack(
          CallbackCommand callBackCommand,
          int subActionCommand,
          nint action)
        {
            switch (callBackCommand)
            {
                case CallbackCommand.PROGRESS:
                    Marshal.ReadInt32(action);
                    break;
                case CallbackCommand.DONE:
                    if (action != nint.Zero)
                    {
                        if (Marshal.ReadByte(action) == 1)
                        {
                            Console.WriteLine("フォーマット正常終了");
                            break;
                        }
                        Console.WriteLine("フォーマット失敗");
                        break;
                    }
                    break;
            }
            return 1;
        }

        public static bool PhysicalFormat(
          SafeFileHandle hdlFmt,
          WinIOAPI.MEDIA_TYPE media_type = WinIOAPI.MEDIA_TYPE.F3_720_512,
          uint StartHeadNumber = 0,
          uint EndHeadNumber = 1,
          uint StartCylinderNumber = 0,
          uint EndCylinderNumber = 79)
        {
            WinIOAPI winIoapi = new WinIOAPI();
            WinIOAPI.FORMAT_PARAMETERS structure = new WinIOAPI.FORMAT_PARAMETERS()
            {
                MediaType = media_type,
                StartHeadNumber = StartHeadNumber,
                EndHeadNumber = EndHeadNumber,
                StartCylinderNumber = StartCylinderNumber,
                EndCylinderNumber = EndCylinderNumber
            };
            uint cb1 = (uint)Marshal.SizeOf(typeof(WinIOAPI.FORMAT_PARAMETERS));
            nint num1 = Marshal.AllocCoTaskMem((int)cb1);
            nint ptr = num1;
            Marshal.StructureToPtr(structure, ptr, false);
            uint cb2 = (uint)Marshal.SizeOf(typeof(ushort));
            nint num2 = Marshal.AllocCoTaskMem((int)cb2);
            uint num3 = 0;
            SafeFileHandle hDevice = hdlFmt;
            nint lpInBuffer = num1;
            int BufferSize = (int)cb1;
            nint lpOutBuffer = num2;
            int OutBufferSize = (int)cb2;
            ref uint local = ref num3;
            nint zero = nint.Zero;
            return winIoapi.DeviceIoControl(hDevice, WinIOAPI.ControlCodes.IOCTL_DISK_FORMAT_TRACKS, lpInBuffer, (uint)BufferSize, lpOutBuffer, (uint)OutBufferSize, ref local, zero);
        }

        private bool FormatWIN(string driveLetter, FormatType formatType)
        {
            int mediaFlag = 0;
            FormatCallBackDelegate callBackDelegate = new FormatCallBackDelegate(FormatCallBack);
            if (geo.MediaType >= WinIOAPI.MEDIA_TYPE.F5_1Pt2_512 && geo.MediaType <= WinIOAPI.MEDIA_TYPE.F3_1Pt44_512)
                mediaFlag = 8;
            else if (geo.MediaType == WinIOAPI.MEDIA_TYPE.FixedMedia)
                mediaFlag = 12;
            else if (geo.MediaType == WinIOAPI.MEDIA_TYPE.RemovableMedia)
                mediaFlag = 11;
            FormatEx(driveLetter, mediaFlag, "FAT", "TEST_VOL", false, 0, callBackDelegate);
            return false;
        }

        public static string ConvPhysicalDrive(string driveLetter) => string.Format("\\\\.\\{0}", driveLetter.Replace("\\", ""));

        public static bool TrashFile(string FileName, nint hwndPtr)
        {
            SHFILEOPSTRUCT lpFileOp;
            lpFileOp.hwnd = hwndPtr;
            lpFileOp.wFunc = FileFuncFlags.FO_DELETE;
            lpFileOp.pFrom = FileName + "\0\0";
            lpFileOp.pTo = "\0\0";
            lpFileOp.fFlags = FILEOP_FLAGS.FOF_ALLOWUNDO;
            lpFileOp.fAnyOperationsAborted = true;
            lpFileOp.hNameMappings = nint.Zero;
            lpFileOp.lpszProgressTitle = null;
            int num1 = SHFileOperation(ref lpFileOp);
            bool flag;
            if (num1 == 0)
            {
                flag = true;
            }
            else
            {
                int num2 = (int)MessageBox.Show("TrashFile failed return code is " + num1.ToString("0000"), "CRITICAL", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                flag = false;
            }
            return flag;
        }

        private delegate int FormatCallBackDelegate(
          CallbackCommand callBackCommand,
          int subActionCommand,
          nint action);

        public enum FormatType : uint
        {
            MSX_DOS1 = 1,
            MSX_DOS2 = 2,
            WIN_DOS6 = 3,
        }

        private enum CallbackCommand
        {
            PROGRESS,
            DONEWITHSTRUCTURE,
            UNKNOWN2,
            UNKNOWN3,
            UNKNOWN4,
            UNKNOWN5,
            INSUFFICIENTRIGHTS,
            UNKNOWN7,
            DISKLOCKEDFORACCESS,
            UNKNOWN9,
            UNKNOWNA,
            DONE,
            UNKNOWNC,
            UNKNOWND,
            OUTPUT,
            STRUCTUREPROGRESS,
        }

        public enum FileFuncFlags : uint
        {
            FO_MOVE = 1,
            FO_COPY = 2,
            FO_DELETE = 3,
            FO_RENAME = 4,
        }

        [Flags]
        public enum FILEOP_FLAGS : ushort
        {
            FOF_MULTIDESTFILES = 1,
            FOF_CONFIRMMOUSE = 2,
            FOF_SILENT = 4,
            FOF_RENAMEONCOLLISION = 8,
            FOF_NOCONFIRMATION = 16, // 0x0010
            FOF_WANTMAPPINGHANDLE = 32, // 0x0020
            FOF_ALLOWUNDO = 64, // 0x0040
            FOF_FILESONLY = 128, // 0x0080
            FOF_SIMPLEPROGRESS = 256, // 0x0100
            FOF_NOCONFIRMMKDIR = 512, // 0x0200
            FOF_NOERRORUI = 1024, // 0x0400
            FOF_NOCOPYSECURITYATTRIBS = 2048, // 0x0800
            FOF_NORECURSION = 4096, // 0x1000
            FOF_NO_CONNECTED_ELEMENTS = 8192, // 0x2000
            FOF_WANTNUKEWARNING = 16384, // 0x4000
            FOF_NORECURSEREPARSE = 32768, // 0x8000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHFILEOPSTRUCT
        {
            public nint hwnd;
            public FileFuncFlags wFunc;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;
            public FILEOP_FLAGS fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public nint hNameMappings;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;
        }
    }
}
