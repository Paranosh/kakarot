using kakarot.Properties;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Text;

namespace kakarot
{
    internal class VirtualDisk
    {
        private const byte MSX_CRC_ERROR = 5;
        private const byte MSX_SEK_ERROR = 7;
        private const byte MSX_RNT_ERROR = 8;
        private const byte MSX_OTH_ERROR = 13;
        private string _ErrorDetail;

        public string ErrorDetail => _ErrorDetail;

        public bool WriteToDSKFile(
          string Drive,
          string DSKFileName,
          bool ErrorEmu,
          TextBox tLog,
          ProgressBar pgBar,
          MediaType media = MediaType.DetectType)
        {
            try
            {
                uint num1 = 0;
                using (WinIOAPI winIoapi = new WinIOAPI())
                {
                    WinIOAPI.DISK_GEOMETRY outGEOMETRY = new WinIOAPI.DISK_GEOMETRY();
                    switch (media)
                    {
                        case MediaType.DetectType:
                            if (!winIoapi.GetDriveGeometry(Drive, ref outGEOMETRY))
                            {
                                int num2 = (int)MessageBox.Show(MyResources.MSGB_FLDIF + Environment.NewLine + winIoapi.APIErrorMessage, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                return false;
                            }
                            break;
                        case MediaType.Type1DD:
                        case MediaType.Type2DD:
                            outGEOMETRY.BytesPerSector = 512U;
                            outGEOMETRY.SectorsPerTrack = 9U;
                            outGEOMETRY.Cylinders = 2UL;
                            outGEOMETRY.TracksPerCylinder = 80U;
                            break;
                    }
                    string filename = DiskUtils.ConvPhysicalDrive(Drive);
                    uint num3 = media != MediaType.Type1DD ? (uint)(outGEOMETRY.TracksPerCylinder * outGEOMETRY.Cylinders * outGEOMETRY.SectorsPerTrack * outGEOMETRY.BytesPerSector) : outGEOMETRY.TracksPerCylinder * outGEOMETRY.SectorsPerTrack * outGEOMETRY.BytesPerSector;
                    if (tLog != null)
                    {
                        tLog.Text = "";
                        TextBox textBox1 = tLog;
                        textBox1.Text = textBox1.Text + "Copying from Drive " + Drive + Environment.NewLine;
                        TextBox textBox2 = tLog;
                        textBox2.Text = textBox2.Text + "Media type is " + outGEOMETRY.MediaType.ToString() + Environment.NewLine;
                        TextBox textBox3 = tLog;
                        textBox3.Text = textBox3.Text + "Media size = " + num3.ToString("#,0") + "bytes" + Environment.NewLine;
                    }
                    using (SafeFileHandle file = winIoapi.CreateFile(filename, WinIOAPI.DesiredAccess.GENERIC_READ, WinIOAPI.ShareMode.FILE_SHARE_READ | WinIOAPI.ShareMode.FILE_SHARE_WRITE, nint.Zero, WinIOAPI.CreationDisposition.OPEN_EXISTING, WinIOAPI.FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL, nint.Zero))
                    {
                        uint length = !ErrorEmu ? outGEOMETRY.BytesPerSector * outGEOMETRY.SectorsPerTrack : outGEOMETRY.BytesPerSector;
                        byte[] numArray = new byte[(int)length];
                        uint numberOfByteRead = 0;
                        using (FileStream output = new FileStream(DSKFileName, FileMode.Create))
                        {
                            using (BinaryWriter binaryWriter = new BinaryWriter(output))
                            {
                                pgBar.Minimum = 0;
                                pgBar.Maximum = (int)num3;
                                pgBar.Value = 0;
                                do
                                {
                                    if (!winIoapi.ReadFile(file, numArray, length, ref numberOfByteRead, nint.Zero))
                                    {
                                        uint num4 = winIoapi.SetFilePointer(file, 0U, nuint.Zero, WinIOAPI.MoveMethod.FILE_CURRENT);
                                        TextBox textBox4 = tLog;
                                        textBox4.Text = textBox4.Text + "Disk read error at sector: " + (num4 / outGEOMETRY.BytesPerSector).ToString("x4") + "]" + winIoapi.APIErrorMessage;
                                        tLog.Refresh();
                                        if (ErrorEmu)
                                        {
                                            Encoding.ASCII.GetBytes("_ErrSec_").CopyTo(numArray, 0);
                                            numArray[8] = 0;
                                            switch (winIoapi.APIErrorCode)
                                            {
                                                case 23:
                                                    numArray[8] |= 5;
                                                    break;
                                                case 25:
                                                    numArray[8] |= 7;
                                                    break;
                                                default:
                                                    numArray[8] |= 13;
                                                    break;
                                            }
                                            numberOfByteRead = length;
                                            uint num5 = winIoapi.SetFilePointer(file, length, nuint.Zero, WinIOAPI.MoveMethod.FILE_CURRENT);
                                            if (num5 == uint.MaxValue)
                                            {
                                                TextBox textBox5 = tLog;
                                                textBox5.Text = textBox5.Text + "File seek error at Sector: " + (num5 / outGEOMETRY.BytesPerSector).ToString("x4") + "]" + winIoapi.APIErrorMessage + Environment.NewLine;
                                                tLog.Refresh();
                                                goto label_29;
                                            }
                                        }
                                        else
                                            goto label_29;
                                    }
                                    if (numberOfByteRead == 0U)
                                    {
                                        TextBox textBox = tLog;
                                        textBox.Text = textBox.Text + "Read finished !" + Environment.NewLine;
                                        goto label_29;
                                    }
                                    else
                                    {
                                        binaryWriter.Write(numArray);
                                        num1 += numberOfByteRead;
                                        pgBar.Value = (int)num1;
                                        Application.DoEvents();
                                    }
                                }
                                while (num1 < num3);
                                TextBox textBox6 = tLog;
                                textBox6.Text = textBox6.Text + "Write finished !" + Environment.NewLine;
                            }
                        }
                    label_29:
                        return true;
                    }
                }
            }
            catch (IOException ex)
            {
                int num = (int)MessageBox.Show(MyResources.MSGB_NRWRT, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
            catch (WinIOAPI.APICallException ex)
            {
                int num = (int)MessageBox.Show(MyResources.MSGB_APIER + Environment.NewLine + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
        }

        private bool WriteToSAVFile(string Drive, string SAVFileName)
        {
            try
            {
                uint BytesReturned = 0;
                WinIOAPI winIoapi = new WinIOAPI();
                WinIOAPI.DISK_GEOMETRY diskGeometry = new WinIOAPI.DISK_GEOMETRY();
                nint num1 = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WinIOAPI.DISK_GEOMETRY)));
                string filename = string.Format("\\\\.\\{0}", Drive);
                uint OutBufferSize = (uint)Marshal.SizeOf(typeof(WinIOAPI.DISK_GEOMETRY));
                SafeFileHandle file1 = winIoapi.CreateFile(filename, WinIOAPI.DesiredAccess.DEVICE_ACCESS, WinIOAPI.ShareMode.FILE_SHARE_READ, nint.Zero, WinIOAPI.CreationDisposition.OPEN_EXISTING, 0, nint.Zero);
                if (!winIoapi.DeviceIoControl(file1, WinIOAPI.ControlCodes.IOCTL_DISK_GET_DRIVE_GEOMETRY, nint.Zero, 0U, num1, OutBufferSize, ref BytesReturned, nint.Zero))
                {
                    int num2 = (int)MessageBox.Show(MyResources.MSGB_FLDIF, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    file1.Close();
                    return false;
                }
                WinIOAPI.DISK_GEOMETRY structure = (WinIOAPI.DISK_GEOMETRY)Marshal.PtrToStructure(num1, typeof(WinIOAPI.DISK_GEOMETRY));
                file1.Close();
                SafeFileHandle file2 = winIoapi.CreateFile(filename, WinIOAPI.DesiredAccess.GENERIC_READ, WinIOAPI.ShareMode.FILE_SHARE_READ | WinIOAPI.ShareMode.FILE_SHARE_WRITE, nint.Zero, WinIOAPI.CreationDisposition.OPEN_EXISTING, WinIOAPI.FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL, nint.Zero);
                byte[] numArray = new byte[(int)structure.BytesPerSector];
                uint numberOfByteRead = 0;
                using (FileStream output = new FileStream(SAVFileName, FileMode.Create))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(output))
                    {
                        uint num3 = 0;
                        uint num4 = 0;
                        uint num5 = 0;
                        uint num6 = 0;
                        while (winIoapi.ReadFile(file2, numArray, structure.BytesPerSector, ref numberOfByteRead, nint.Zero))
                        {
                            if (numberOfByteRead != 0U)
                            {
                                binaryWriter.Write(numArray);
                                ++num5;
                                if (num5 >= structure.SectorsPerTrack)
                                {
                                    num5 = 0U;
                                    ++num4;
                                }
                                if (num4 >= structure.Cylinders)
                                {
                                    num4 = 0U;
                                    ++num3;
                                }
                                ++num6;
                                uint distanceToMove = structure.BytesPerSector * num6;
                                if (winIoapi.SetFilePointer(file2, distanceToMove, nuint.Zero, WinIOAPI.MoveMethod.FILE_BEGIN) == uint.MaxValue)
                                    break;
                            }
                            else
                                break;
                        }
                    }
                }
                file2.Close();
                return true;
            }
            catch (IOException ex)
            {
                int num = (int)MessageBox.Show(MyResources.MSGB_NRWRT, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
            catch (WinIOAPI.APICallException ex)
            {
                int num = (int)MessageBox.Show(MyResources.MSGB_APIER + Environment.NewLine + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
        }

        public bool VDiskToRDisk(
          string vdFileName,
          string driveLetter,
          ProgressBar prgBar = null,
          bool FormatDisk = false)
        {
            bool rdisk = true;
            try
            {
                if (prgBar != null)
                {
                    prgBar.Value = 0;
                    prgBar.Minimum = 0;
                    prgBar.Maximum = 100;
                }
                string filename = DiskUtils.ConvPhysicalDrive(driveLetter);
                using (WinIOAPI winIoapi = new WinIOAPI())
                {
                    using (SafeFileHandle file = winIoapi.CreateFile(filename, WinIOAPI.DesiredAccess.GENERIC_WRITE | WinIOAPI.DesiredAccess.GENERIC_READ, WinIOAPI.ShareMode.FILE_SHARE_WRITE, nint.Zero, WinIOAPI.CreationDisposition.OPEN_EXISTING, WinIOAPI.FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL | WinIOAPI.FlagsAndAttributes.FILE_FLAG_NO_BUFFERING, nint.Zero))
                    {
                        uint BytesReturned = 0;
                        if (!winIoapi.DeviceIoControl(file, WinIOAPI.ControlCodes.FSCTL_LOCK_VOLUME, nint.Zero, 0U, nint.Zero, 0U, ref BytesReturned, nint.Zero))
                        {
                            int num = (int)MessageBox.Show(MyResources.MSGB_FDFLK, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            return false;
                        }
                        if (FormatDisk)
                        {
                            if (!DiskUtils.PhysicalFormat(file))
                                return false;
                            if (prgBar != null)
                                prgBar.Value = 50;
                        }
                        ResultOfVCopy resultOfVcopy = PhysicalCopyVDSK2RDSK(vdFileName, driveLetter, file, prgBar);
                        switch (resultOfVcopy)
                        {
                            case ResultOfVCopy.COPY_SUCCESSFULL:
                                int num1 = (int)MessageBox.Show(MyResources.MSGB_EJFDD, "INFO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                if (!winIoapi.DeviceIoControl(file, WinIOAPI.ControlCodes.FSCTL_UNLOCK_VOLUME, nint.Zero, 0U, nint.Zero, 0U, ref BytesReturned, nint.Zero))
                                {
                                    int num2 = (int)MessageBox.Show(MyResources.MSGB_FDFUL, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                    return false;
                                }
                                goto label_26;
                            case ResultOfVCopy.EXCEPTION_IO_ERROR:
                            case ResultOfVCopy.EXCEPTION_API_CALL:
                            case ResultOfVCopy.EXCEPTION_OTHER_ERR:
                            case ResultOfVCopy.ERR_VDSK_NOT_FOUND:
                            case ResultOfVCopy.ERR_VDSK_READ_ERROR:
                            case ResultOfVCopy.ERR_DISK_NOT_FOUND:
                            case ResultOfVCopy.ERR_DISK_SET_F_PTR:
                                int num3 = (int)MessageBox.Show(MyResources.MSGB_ERRWR + "(" + resultOfVcopy.ToString() + ")", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                break;
                            case ResultOfVCopy.ERR_DISK_WRITE_ERR:
                                int num4 = (int)MessageBox.Show(MyResources.MSGB_WPCTD + Environment.NewLine + _ErrorDetail, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                break;
                        }
                        return false;
                    }
                }
            }
            catch (IOException ex)
            {
                int num = (int)MessageBox.Show(MyResources.MSGB_IOERR + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                rdisk = false;
            }
            catch (WinIOAPI.APICallException ex)
            {
                int num = (int)MessageBox.Show(MyResources.MSGB_APIER + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                rdisk = false;
            }
        label_26:
            return rdisk;
        }

        public ResultOfVCopy PhysicalCopyVDSK2RDSK(
          string VDiskFile,
          string DestDrive,
          SafeFileHandle hdlDsk,
          ProgressBar prgBar)
        {
            WinIOAPI winIoapi = new WinIOAPI();
            ResultOfVCopy resultOfVcopy = ResultOfVCopy.COPY_SUCCESSFULL;
            if (!File.Exists(VDiskFile))
            {
                int num = (int)MessageBox.Show(string.Format(MyResources.MSGB_FNAFD, VDiskFile), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return ResultOfVCopy.ERR_VDSK_NOT_FOUND;
            }
            if (prgBar != null && prgBar.Value.Equals(0))
            {
                prgBar.Minimum = 0;
                prgBar.Maximum = 100;
            }
            using (FileStream fileStream = new FileStream(VDiskFile, FileMode.Open, FileAccess.Read))
            {
                uint distanceToMove = 0;
                int num1 = 0;
                long length1 = new FileInfo(VDiskFile).Length;
                byte[] numArray1 = new byte[length1];
                if (prgBar != null)
                    num1 = (int)(length1 / (prgBar.Maximum - prgBar.Value));
                if (winIoapi.SetFilePointer(hdlDsk, distanceToMove, nuint.Zero, WinIOAPI.MoveMethod.FILE_BEGIN) == uint.MaxValue)
                    return ResultOfVCopy.ERR_DISK_SET_F_PTR;
                try
                {
                    WinIOAPI.DISK_GEOMETRY outGEOMETRY = new WinIOAPI.DISK_GEOMETRY();
                    if (!winIoapi.GetDriveGeometry(DestDrive, ref outGEOMETRY))
                        return ResultOfVCopy.EXCEPTION_API_CALL;
                    int length2 = (int)outGEOMETRY.SectorsPerTrack * (int)outGEOMETRY.BytesPerSector;
                    byte[] numArray2 = new byte[length2];
                    int num2 = fileStream.Read(numArray1, 0, (int)length1);
                    uint numberOfByteWritten = 0;
                    int num3 = (int)length1;
                    uint sourceIndex = 0;
                    if (num2 != length1)
                        return ResultOfVCopy.ERR_VDSK_READ_ERROR;
                    int num4 = num1;
                    while (num3 > 0)
                    {
                        Array.Copy(numArray1, sourceIndex, numArray2, 0L, length2);
                        if (!winIoapi.WriteFile(hdlDsk, numArray2, (uint)length2, ref numberOfByteWritten, nint.Zero))
                        {
                            _ErrorDetail = winIoapi.APIErrorMessage;
                            resultOfVcopy = ResultOfVCopy.ERR_DISK_WRITE_ERR;
                            break;
                        }
                        Application.DoEvents();
                        distanceToMove += (uint)length2;
                        if (winIoapi.SetFilePointer(hdlDsk, distanceToMove, nuint.Zero, WinIOAPI.MoveMethod.FILE_BEGIN) == uint.MaxValue)
                        {
                            resultOfVcopy = ResultOfVCopy.ERR_DISK_SET_F_PTR;
                            break;
                        }
                        num3 -= (int)numberOfByteWritten;
                        sourceIndex += numberOfByteWritten;
                        if (prgBar != null && sourceIndex > num4)
                        {
                            num4 += num1;
                            ++prgBar.Value;
                        }
                    }
                    if (prgBar != null)
                        prgBar.Value = 100;
                }
                catch (IOException ex)
                {
                    int num5 = (int)MessageBox.Show(MyResources.MSGB_IOERR + Environment.NewLine + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    resultOfVcopy = ResultOfVCopy.EXCEPTION_IO_ERROR;
                }
                catch (WinIOAPI.APICallException ex)
                {
                    int num6 = (int)MessageBox.Show(MyResources.MSGB_APIER + Environment.NewLine + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    resultOfVcopy = ResultOfVCopy.EXCEPTION_API_CALL;
                }
                catch (Exception ex)
                {
                    int num7 = (int)MessageBox.Show(MyResources.MSGB_FTLER + Environment.NewLine + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    resultOfVcopy = ResultOfVCopy.EXCEPTION_OTHER_ERR;
                }
            }
            return resultOfVcopy;
        }

        public enum DiskType
        {
            DSKImage,
            SAVImage,
        }

        public enum MediaType
        {
            DetectType,
            Type1DD,
            Type2DD,
        }

        public enum ResultOfVCopy : uint
        {
            COPY_SUCCESSFULL = 0,
            EXCEPTION_IO_ERROR = 225, // 0x000000E1
            EXCEPTION_API_CALL = 226, // 0x000000E2
            EXCEPTION_OTHER_ERR = 227, // 0x000000E3
            ERR_VDSK_NOT_FOUND = 241, // 0x000000F1
            ERR_VDSK_READ_ERROR = 242, // 0x000000F2
            ERR_DISK_NOT_FOUND = 243, // 0x000000F3
            ERR_DISK_SET_F_PTR = 244, // 0x000000F4
            ERR_DISK_WRITE_ERR = 245, // 0x000000F5
        }
    }
}
