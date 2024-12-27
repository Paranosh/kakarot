using kakarot.Properties;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;


namespace kakarot
{
    public abstract class VFATOperation : IDisposable
    {
        private const int DIR_NAME_LEN = 11;
        private const int DIR_NMFL_LEN = 8;
        private const int DIR_NMDR_LEN = 3;
        private const int DIR_ATTR_LEN = 1;
        private const int DIR_NTRS_LEN = 1;
        private const int DIR_CTTT_LEN = 1;
        private const int DIR_CRTM_LEN = 2;
        private const int DIR_CRDT_LEN = 2;
        private const int DIR_LSAC_LEN = 2;
        private const int DIR_FSHI_LEN = 2;
        private const int DIR_WRTM_LEN = 2;
        private const int DIR_WRDT_LEN = 2;
        private const int DIR_FSLO_LEN = 2;
        private const int DIR_FSSZ_LEN = 4;
        protected byte[] FAT;
        protected DIR_BYTE[] RootDir_byte;
        protected DIR_BYTE[] SubDir_byte;
        protected DIR_BYTE[] CurDir_byte;
        public DIR[] RootDir;
        public DIR[] SubDir;
        public DIR[] CurDir;
        protected int FATStartSector;
        protected int SectorSize;
        protected int NumOfFAT;
        protected int FATSectors;
        protected int ClusterSize;
        protected int RootDirStartSector;
        protected int RootDirSectors;
        protected int RootDirEntryCount;
        protected int DirectorySize;
        protected int SectorPerCluster;
        protected int DataStartSector;
        protected int DataSectors;
        protected int FATSize;
        protected int FATEntry;
        protected const byte DIRE_NO_ENTRY = 0;
        protected const byte DIRE_DELETED = 229;
        protected uint CLUSNO_BAD_FAT12 = 4087;
        protected uint CLUSNO_EOC_FAT12 = 4095;
        protected byte DOT = 46;
        protected FileStream fs;
        private const uint CURRENT_DIR_ROOT = 0;
        private const string CURRENT_DIRECTORY = ".          ";
        private const string PARENT_DIRECTORY = "..         ";
        private char[] Invalid_chars = new char[8]
        {
      '\\',
      '/',
      ':',
      '*',
      '?',
      '<',
      '>',
      '|'
        };
        private uint _CurrDirCluster;
        protected FormatType _DiskFormatType;

        public bool IsError { get; set; }

        public uint CurrDirCluster => _CurrDirCluster;

        public uint DiskFreeSpace => (uint)((ulong)(SectorSize * SectorPerCluster) * GetFreeClusterCount());

        public abstract uint DiskSpace { get; }

        public EnumFATType FATType { get; set; }

        public FormatType DiskFormatType => _DiskFormatType;

        protected FilePathType CheckAttribute(string pathFile)
        {
            if (File.Exists(pathFile))
                return FilePathType.File;
            if (!Directory.Exists(pathFile))
                return FilePathType.NotFound;
            return pathFile.EndsWith(":\\") || pathFile.EndsWith(":") ? FilePathType.Drive : FilePathType.Folder;
        }

        public VDiskStatus VDiskReadStatus { get; protected set; }

        public VFATOperation(string Target)
        {
            if (CheckAttribute(Target) != FilePathType.File)
                return;
            try
            {
                fs = new FileStream(Target, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (IOException ex)
            {
                VDiskReadStatus = VDiskStatus.CANT_USEFILE;
                fs.Close();
                return;
            }
            Initialize();
            ReadFAT();
            ReadRootDirectoryEntry();
        }

        protected abstract void Initialize();

        protected void ReadFAT()
        {
            FAT = new byte[FATSectors * SectorSize];
            for (int index = 0; index < FATSectors; ++index)
                ReadSector(FATStartSector + index, FAT, SectorSize * index);
        }

        protected void WriteFAT()
        {
            for (int index = 0; index < FATSectors; ++index)
                WriteSector(FATStartSector + index, FAT, SectorSize * index);
        }

        public void ReadRootDirectoryEntry()
        {
            GetRootDirectory();
            _CurrDirCluster = 0U;
            Array.Resize(ref CurDir_byte, RootDir_byte.Length);
            Array.Resize(ref CurDir, RootDir.Length);
            CurDir_byte = new DIR_BYTE[RootDir_byte.Length];
            CurDir = new DIR[RootDir.Length];
            Array.Copy(RootDir_byte, CurDir_byte, RootDir_byte.Length);
            Array.Copy(RootDir, CurDir, RootDir.Length);
        }

        protected int RootEntryCount => RootDirEntryCount;

        private bool GetRootDirectory()
        {
            bool rootDirectory = true;
            int num1 = Marshal.SizeOf(typeof(DIR_BYTE));
            int length = RootEntryCount * num1;
            int num2 = length / SectorSize;
            byte[] numArray = new byte[length];
            RootDir_byte = new DIR_BYTE[RootEntryCount];
            RootDir = new DIR[RootEntryCount];
            for (int index = 0; index < num2; ++index)
                ReadSector(RootDirStartSector + index, numArray, index * SectorSize);
            for (int index = 0; index < RootEntryCount; ++index)
            {
                nint num3 = Marshal.AllocHGlobal(num1);
                Marshal.Copy(numArray, index * num1, num3, num1);
                RootDir_byte[index] = (DIR_BYTE)Marshal.PtrToStructure(num3, typeof(DIR_BYTE));
                Marshal.FreeHGlobal(num3);
                if (!CheckFileName(RootDir_byte[index].Name))
                {
                    rootDirectory = false;
                    break;
                }
            }
            if (!rootDirectory)
                return rootDirectory;
            DIRMemoryToStucture(RootDir_byte, ref RootDir);
            return rootDirectory;
        }

        protected bool CheckFileName(byte[] filename)
        {
            bool flag = true;
            for (int index1 = 0; index1 < Invalid_chars.Length; ++index1)
            {
                for (int index2 = 0; index2 < filename.Length; ++index2)
                {
                    if (filename[index2] == Invalid_chars[index1])
                    {
                        flag = false;
                        break;
                    }
                }
                if (!flag)
                    break;
            }
            return flag;
        }

        public void GetSelectedDirectory(uint ClusterNo)
        {
            if (ClusterNo == 0U)
            {
                GetRootDirectory();
                CurDir_byte = new DIR_BYTE[RootDir_byte.Length];
                CurDir = new DIR[RootDir.Length];
                Array.Copy(RootDir_byte, CurDir_byte, RootDir_byte.Length);
                Array.Copy(RootDir, CurDir, RootDir.Length);
            }
            else
            {
                GetSubDirectory(ClusterNo);
                CurDir_byte = new DIR_BYTE[SubDir_byte.Length];
                CurDir = new DIR[SubDir.Length];
                Array.Copy(SubDir_byte, CurDir_byte, SubDir_byte.Length);
                Array.Copy(SubDir, CurDir, SubDir.Length);
            }
            _CurrDirCluster = ClusterNo;
        }

        private void GetSubDirectory(uint ClusterNo)
        {
            uint ClusterNo1 = ClusterNo;
            int num1 = 0;
            int length = ClusterSize / DirectorySize;
            byte[] numArray = new byte[ClusterSize];
            _CurrDirCluster = ClusterNo;
            SubDir_byte = new DIR_BYTE[length];
            SubDir = new DIR[length];
            do
            {
                if (ReadCluster(ClusterNo1, numArray))
                {
                    for (int index = num1; index < length + num1; ++index)
                    {
                        nint num2 = Marshal.AllocHGlobal(DirectorySize);
                        Marshal.Copy(numArray, (index - num1) * DirectorySize, num2, DirectorySize);
                        SubDir_byte[index] = (DIR_BYTE)Marshal.PtrToStructure(num2, typeof(DIR_BYTE));
                        Marshal.FreeHGlobal(num2);
                    }
                    DIRMemoryToStucture(SubDir_byte, ref SubDir);
                }
                ClusterNo1 = GetFAT(ClusterNo1);
                if ((int)ClusterNo1 != (int)CLUSNO_EOC_FAT12)
                {
                    num1 += length;
                    Array.Resize(ref SubDir_byte, length + num1);
                    Array.Resize(ref SubDir, length + num1);
                }
            }
            while ((int)ClusterNo1 != (int)CLUSNO_EOC_FAT12);
        }

        protected void WriteDirectoryEntry(DIR_BYTE[] dir_ent)
        {
            int num1 = !dir_ent[0].Name[0].Equals(DOT) ? RootDirEntryCount : dir_ent.Length;
            int length = num1 * DirectorySize;
            byte[] destBytes = new byte[length];
            for (int idx = 0; idx < num1; ++idx)
                CopyStructureToByte(dir_ent[idx], idx, ref destBytes);
            if (dir_ent[0].Name[0].Equals(DOT))
            {
                uint ClusterNo = BitConverter.ToUInt16(dir_ent[0].FstClusLO, 0);
                uint freeCluster = GetFreeCluster();
                byte[] numArray = new byte[ClusterSize];
                int int32 = Convert.ToInt32(Math.Ceiling((decimal)(length / ClusterSize)));
                for (int index = 0; index < int32; ++index)
                {
                    Array.Clear(numArray, 0, numArray.Length);
                    Array.Copy(destBytes, index * ClusterSize, numArray, 0, ClusterSize);
                    WriteCluster(ClusterNo, numArray);
                    if (index < int32 - 1)
                    {
                        SetFAT(ClusterNo, freeCluster);
                        ClusterNo = freeCluster;
                        freeCluster = GetFreeCluster();
                    }
                    else
                        SetFAT(ClusterNo, CLUSNO_EOC_FAT12);
                    if (freeCluster >= CLUSNO_EOC_FAT12)
                    {
                        int num2 = (int)MessageBox.Show(MyResources.MSGB_ODSPC, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        break;
                    }
                }
            }
            else
            {
                int num3 = length / SectorSize;
                for (int index = 0; index < num3; ++index)
                    WriteSector(RootDirStartSector + index, destBytes, index * SectorSize);
            }
        }

        public bool ReadFileFromVDisk(DIR entry, string WinPath, FileMode mode)
        {
            bool flag = false;
            if ((entry.Attribute & 16) > 0)
                flag = ExtractFolderFromVDisk(entry, WinPath, mode);
            else if ((entry.Attribute & 32) > 0 || entry.Attribute == 0)
                flag = ExtractFileFromVDisk(entry, WinPath, mode);
            return flag;
        }

        private bool ExtractFileFromVDisk(DIR entry, string WinPath, FileMode mode)
        {
            try
            {
                Encoding.GetEncoding("Shift_JIS");
                string path = Path.Combine(WinPath, entry.Name);
                uint ClusterNo = entry.FirstClusterNo;
                uint num = 0;
                uint size = entry.Size;
                byte[] buf = new byte[ClusterSize];
                byte[] buffer = new byte[(int)size];
                while (true)
                {
                    ReadCluster(ClusterNo, buf);
                    ClusterNo = GetFAT(ClusterNo);
                    if ((int)ClusterNo != (int)CLUSNO_EOC_FAT12)
                    {
                        buf.CopyTo(buffer, num * ClusterSize);
                        ++num;
                    }
                    else
                        break;
                }
                for (int index = 0; index < ClusterSize && num * ClusterSize + index <= size - 1U; ++index)
                    buffer[num * ClusterSize + index] = buf[index];
                using (FileStream fileStream = new FileStream(path, mode, FileAccess.Write))
                    fileStream.Write(buffer, 0, (int)entry.Size);
                return true;
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
        }

        private bool ExtractFolderFromVDisk(DIR entry, string WinPath, FileMode mode)
        {
            string str = Path.Combine(WinPath, entry.Name);
            try
            {
                Directory.CreateDirectory(str);
                GetSubDirectory(entry.FirstClusterNo);
                DIR[] destinationArray = new DIR[SubDir.Length];
                Array.Copy(SubDir, destinationArray, SubDir.Length);
                for (int index = 2; index < SubDir.Length; ++index)
                {
                    if (destinationArray[index].Attribute == 16)
                    {
                        bool folderFromVdisk = ExtractFolderFromVDisk(destinationArray[index], str, mode);
                        if (!folderFromVdisk)
                            return folderFromVdisk;
                    }
                    else if (destinationArray[index].Attribute == 32)
                    {
                        bool fileFromVdisk = ExtractFileFromVDisk(destinationArray[index], str, mode);
                        if (!fileFromVdisk)
                            return fileFromVdisk;
                    }
                }
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                int num = (int)MessageBox.Show(MyResources.MSGB_NWPM, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
            catch (IOException ex)
            {
                int num = (int)MessageBox.Show(MyResources.MSGB_FXSTA, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
        }

        public bool WriteFileToVDisk(string FullPath)
        {
            bool vdisk = true;
            uint currentDirCluster = GetCurrentDirCluster();
            DIR_BYTE directory = GetDirectory(FullPath);
            if (directory.Name[0] == 0)
            {
                vdisk = false;
            }
            else
            {
                int emptyEntry = GetEmptyEntry(directory, ref CurDir_byte);
                switch (emptyEntry)
                {
                    case -2:
                    case -1:
                        vdisk = false;
                        break;
                    default:
                        if (DiskFreeSpace < BitConverter.ToInt16(directory.FileSize, 0))
                        {
                            vdisk = false;
                            break;
                        }
                        if ((directory.Attr[0] & 16) != 0)
                        {
                            uint freeCluster = GetFreeCluster();
                            if (freeCluster < CLUSNO_BAD_FAT12)
                            {
                                directory.FstClusLO = BitConverter.GetBytes((ushort)freeCluster);
                                WriteSubDirectory(FullPath, currentDirCluster, freeCluster);
                            }
                            else
                                vdisk = false;
                        }
                        else if ((directory.Attr[0] & 32) != 0)
                        {
                            if (new FileInfo(FullPath).Length > 0L)
                            {
                                uint freeCluster = GetFreeCluster();
                                directory.FstClusLO = BitConverter.GetBytes((ushort)freeCluster);
                                WriteFile(FullPath, freeCluster);
                            }
                            else
                                directory.FstClusLO[0] = 0;
                        }
                        CurDir_byte[emptyEntry] = directory;
                        break;
                }
            }
            WriteDirectoryEntry(CurDir_byte);
            WriteFAT();
            DIRMemoryToStucture(CurDir_byte, ref CurDir);
            return vdisk;
        }

        private bool WriteFile(string WinPathFile, uint StartCluster)
        {
            byte[] numArray = new byte[ClusterSize];
            uint ClusterNo = StartCluster;
            using (FileStream fileStream = new FileStream(WinPathFile, FileMode.Open, FileAccess.Read))
            {
                long length = fileStream.Length;
                long num1 = 0;
                uint clusnoEocFaT12 = CLUSNO_EOC_FAT12;
                while (true)
                {
                    Array.Clear(numArray, 0, ClusterSize);
                    if (!ClusterNo.Equals(0U))
                    {
                        long num2 = fileStream.Read(numArray, 0, ClusterSize);
                        num1 += num2;
                        if (num2 != 0L)
                        {
                            WriteCluster(ClusterNo, numArray);
                            SetFAT(ClusterNo, CLUSNO_EOC_FAT12);
                            if (num2.Equals(ClusterSize))
                            {
                                uint freeCluster = GetFreeCluster();
                                SetFAT(ClusterNo, freeCluster);
                                ClusterNo = freeCluster;
                            }
                            else
                                goto label_8;
                        }
                        else
                            goto label_5;
                    }
                    else
                        break;
                }
                return false;
            label_5:
                SetFAT(ClusterNo, CLUSNO_EOC_FAT12);
                goto label_12;
            label_8:
                SetFAT(ClusterNo, CLUSNO_EOC_FAT12);
            }
        label_12:
            return true;
        }

        public bool WriteSubDirectory(string WinPathDirectory, uint ParentCluster, uint StartCluster)
        {
            bool flag = true;
            Encoding encoding = Encoding.GetEncoding("Shift_JIS");
            DateTime now = DateTime.Now;
            string[] files = Directory.GetFiles(WinPathDirectory, "*", SearchOption.TopDirectoryOnly);
            int num1 = files.Count();
            string[] directories = Directory.GetDirectories(WinPathDirectory, "*", SearchOption.TopDirectoryOnly);
            int length1 = directories.Count();
            int length2 = num1 + length1;
            string[] destinationArray = new string[length2];
            if (files.Count() > 0)
                Array.Copy(files, destinationArray, num1);
            if (directories.Count() > 0)
                Array.Copy(directories, 0, destinationArray, num1, length1);
            int num2 = ClusterSize / DirectorySize;
            DIR_BYTE[] dir = new DIR_BYTE[((length2 + 2) / num2 + 1) * num2];
            byte[] numArray = new byte[ClusterSize];
            DIR_BYTEsInitialize(ref dir);
            dir[0].Name = encoding.GetBytes(".          ");
            dir[0].FileSize = BitConverter.GetBytes(0);
            dir[0].FstClusLO = BitConverter.GetBytes((ushort)StartCluster);
            dir[0].CrtDate = BitConverter.GetBytes(GetDOSDate(now));
            dir[0].CrtTime = BitConverter.GetBytes(GetDOSTime(now));
            dir[0].Attr[0] = 16;
            dir[1].Name = encoding.GetBytes("..         ");
            dir[1].FileSize = BitConverter.GetBytes(0);
            dir[1].FstClusLO = BitConverter.GetBytes((ushort)ParentCluster);
            dir[1].CrtDate = BitConverter.GetBytes(GetDOSDate(now));
            dir[1].CrtTime = BitConverter.GetBytes(GetDOSTime(now));
            dir[1].Attr[0] = 16;
            SetFAT(StartCluster, CLUSNO_EOC_FAT12);
            for (int index = 2; index < length2 + 2; ++index)
            {
                dir[index] = GetDirectory(destinationArray[index - 2]);
                uint freeCluster = GetFreeCluster();
                if (freeCluster < CLUSNO_BAD_FAT12)
                {
                    dir[index].FstClusLO = BitConverter.GetBytes((ushort)freeCluster);
                    SetFAT(freeCluster, CLUSNO_EOC_FAT12);
                    if ((dir[index].Attr[0] & 16) != 0)
                        WriteSubDirectory(destinationArray[index - 2], StartCluster, freeCluster);
                    else if ((dir[index].Attr[0] & 32) != 0)
                        WriteFile(destinationArray[index - 2], freeCluster);
                }
                else
                    flag = false;
            }
            if (flag)
            {
                WriteDirectoryEntry(dir);
                WriteFAT();
            }
            return flag;
        }

        public bool DeleteVDiskDirFile(DIR dir)
        {
            int index = Array.IndexOf(CurDir, dir);
            if ((CurDir_byte[index].Attr[0] & 5) > 0)
                return false;
            int num = DeleteFiles(CurDir_byte[index], CurDir_byte) ? 1 : 0;
            DIRMemoryToStucture(CurDir_byte, ref CurDir);
            return num != 0;
        }

        private bool DeleteFiles(DIR_BYTE dir_b, DIR_BYTE[] edit_dir)
        {
            bool flag1 = false;
            int index1 = Array.IndexOf(edit_dir, dir_b);
            bool flag2;
            if (index1 >= 0)
            {
                uint uint16 = BitConverter.ToUInt16(dir_b.FstClusLO, 0);
                edit_dir[index1].Name[0] = 229;
                if ((dir_b.Attr[0] & 16) > 0)
                {
                    DIR_BYTE[] array = new DIR_BYTE[1];
                    GetSubDirectory(uint16);
                    Array.Resize(ref array, SubDir_byte.Length);
                    Array.Copy(SubDir_byte, array, SubDir_byte.Length);
                    int directoryCount = GetDirectoryCount(SubDir_byte);
                    for (int index2 = 2; index2 < directoryCount; ++index2)
                        flag1 = DeleteFiles(array[index2], array);
                }
                ClearFATChain(uint16);
                WriteFAT();
                WriteDirectoryEntry(edit_dir);
                flag2 = true;
            }
            else
                flag2 = false;
            return flag2;
        }

        private int GetDirectoryCount(DIR_BYTE[] dirs)
        {
            int directoryCount = 0;
            for (int index = 0; index < dirs.Length; ++index)
            {
                if (dirs[index].Name[0] == 0)
                {
                    directoryCount = index;
                    break;
                }
            }
            return directoryCount;
        }

        private DIR_BYTE GetDirectory(string WinPathFile)
        {
            DIRAttribute Attr;
            if (File.Exists(WinPathFile))
                Attr = DIRAttribute.ATTR_ARCHIVE;
            else if (Directory.Exists(WinPathFile))
            {
                Attr = DIRAttribute.ATTR_DIRECTORY;
            }
            else
            {
                int num = (int)MessageBox.Show(MyResources.MSGB_NFCFF, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return new DIR_BYTE();
            }
            FileInfo fileInfo = new FileInfo(WinPathFile);
            long FileSize = 0;
            if (fileInfo.Attributes != FileAttributes.Directory)
                FileSize = fileInfo.Length;
            return CreateNewDIR_BYTE(WinPathFile, Attr, fileInfo.CreationTime, fileInfo.LastWriteTime, FileSize);
        }

        public uint CreateNewEmptyFolder(string Name)
        {
            DateTime now = DateTime.Now;
            DateTime updateTimeStamp = now;
            DIR_BYTE newDirByte = CreateNewDIR_BYTE(Name, DIRAttribute.ATTR_DIRECTORY, now, updateTimeStamp, 0L);
            CurDir_byte[GetEmptyEntry(newDirByte, ref CurDir_byte)] = newDirByte;
            return 0;
        }

        private DIR_BYTE CreateNewDIR_BYTE(
          string Name,
          DIRAttribute Attr,
          DateTime createTimeStamp,
          DateTime updateTimeStamp,
          long FileSize)
        {
            Encoding encoding = Encoding.GetEncoding("Shift_JIS");
            DIR_BYTE dir = new DIR_BYTE();
            DIR_BYTEInitialize(ref dir);
            string upper = new WinIOAPI().GetShortPath(Name).ToUpper();
            string withoutExtension = Path.GetFileNameWithoutExtension(Path.GetFileName(upper));
            string s = Path.GetExtension(Path.GetFileName(upper)).Replace(".", "").PadRight(3);
            byte[] bytes1 = encoding.GetBytes(withoutExtension);
            if (bytes1[0] == 229)
                bytes1[0] = 5;
            byte[] bytes2 = encoding.GetBytes(s);
            for (int index = 0; index < 11; ++index)
                dir.Name[index] = 32;
            Array.Copy(bytes1, dir.Name, bytes1.Length);
            Array.Copy(bytes2, 0, dir.Name, 8, 3);
            ushort dosDate = GetDOSDate(updateTimeStamp);
            ushort dosTime = GetDOSTime(updateTimeStamp);
            dir.WrtDate = BitConverter.GetBytes(dosDate);
            dir.WrtTime = BitConverter.GetBytes(dosTime);
            dir.FileSize = Attr != DIRAttribute.ATTR_DIRECTORY ? BitConverter.GetBytes(FileSize) : BitConverter.GetBytes(0);
            dir.Attr[0] = (byte)Attr;
            return dir;
        }

        private int GetEmptyEntry(DIR_BYTE new_dir, ref DIR_BYTE[] dirs)
        {
            int emptyEntry = -2;
            for (int index = 0; index < dirs.Length; ++index)
            {
                if (dirs[index].Name[0] == 0 || dirs[index].Name[0] == 229)
                {
                    emptyEntry = index;
                    break;
                }
                if (dirs[index].Name.SequenceEqual(new_dir.Name) && dirs[index].Attr[0] == new_dir.Attr[0])
                {
                    emptyEntry = (uint)((ulong)(SectorSize * SectorPerCluster) * (CountClusters(BitConverter.ToUInt16(dirs[index].FstClusLO, 0)) + GetFreeClusterCount())) >= BitConverter.ToInt16(new_dir.FileSize, 0) ? index : -1;
                    break;
                }
            }
            return emptyEntry;
        }

        public bool FileExists(string WinPathFile)
        {
            byte[] array = Enumerable.Repeat((byte)32, 11).ToArray();
            bool flag = false;
            Encoding encoding = Encoding.GetEncoding("Shift_JIS");
            FileInfo fileInfo = new FileInfo(WinPathFile);
            WinIOAPI winIoapi = new WinIOAPI();
            int length = CurDir_byte.Length;
            string upper = winIoapi.GetShortPath(WinPathFile).ToUpper();
            string withoutExtension = Path.GetFileNameWithoutExtension(upper);
            string s = Path.GetExtension(upper).Replace(".", "");
            encoding.GetBytes(withoutExtension).CopyTo(array, 0);
            if (s.Length > 0)
                encoding.GetBytes(s).CopyTo(array, 8);
            for (int index = 0; index < length; ++index)
            {
                if (CurDir_byte[index].Name.SequenceEqual(array))
                {
                    flag = true;
                    break;
                }
            }
            winIoapi.Dispose();
            return flag;
        }

        private uint CountClusters(uint ClusterNo)
        {
            uint num = 0;
            uint ClusterNo1 = ClusterNo;
            do
            {
                ++num;
                ClusterNo1 = GetFAT(ClusterNo1);
            }
            while (ClusterNo1 < CLUSNO_BAD_FAT12);
            return num;
        }

        private uint GetFreeClusterCount()
        {
            uint freeClusterCount = 0;
            for (uint ClusterNo = 2; ClusterNo <= FATEntry; ++ClusterNo)
            {
                if (GetFAT(ClusterNo) == 0U)
                    ++freeClusterCount;
            }
            return freeClusterCount;
        }

        private uint GetFreeCluster()
        {
            uint freeCluster = CLUSNO_EOC_FAT12;
            for (uint ClusterNo = 2; ClusterNo <= FATEntry; ++ClusterNo)
            {
                if (GetFAT(ClusterNo) == 0U)
                {
                    freeCluster = ClusterNo;
                    break;
                }
            }
            return freeCluster;
        }

        protected void DIRMemoryToStucture(
          DIR_BYTE[] from_byte,
          ref DIR[] to_strct)
        {
            for (int index = 0; index < from_byte.Length; ++index)
            {
                if (from_byte[index].Name[0] != 0 && from_byte[index].Name[0] != 229)
                {
                    to_strct[index].UsingFlag = true;
                    Encoding encoding = Encoding.GetEncoding("Shift_JIS");
                    to_strct[index].Attribute = from_byte[index].Attr[0];
                    if (from_byte[index].Name[0] == 5)
                        from_byte[index].Name[0] = 229;
                    string str1 = encoding.GetString(from_byte[index].Name);
                    if ((to_strct[index].Attribute & 8) != 0)
                    {
                        to_strct[index].Name = str1;
                    }
                    else
                    {
                        string str2 = encoding.GetString(from_byte[index].Name, 0, 8).Trim(' ');
                        string str3 = encoding.GetString(from_byte[index].Name, 8, 3).Trim(' ');
                        string str4 = !str3.Equals("") ? str2 + "." + str3 : str2;
                        to_strct[index].Name = str4;
                    }
                    to_strct[index].WriteDateTime = GetDateTime(from_byte[index].WrtDate, from_byte[index].WrtTime);
                    to_strct[index].FirstClusterNo = BitConverter.ToUInt16(from_byte[index].FstClusLO, 0);
                    to_strct[index].Size = BitConverter.ToUInt32(from_byte[index].FileSize, 0);
                }
                else
                    to_strct[index].UsingFlag = false;
            }
        }

        private DateTime GetDateTime(byte[] date, byte[] time)
        {
            int uint16_1 = BitConverter.ToUInt16(date, 0);
            ushort uint16_2 = BitConverter.ToUInt16(time, 0);
            DateTime result;
            return DateTime.TryParse(string.Format("{0}/{1}/{2} {3}:{4}:{5}", (uint16_1 >> 9) + 1980, (uint16_1 & 480) >> 5, uint16_1 & 31, uint16_2 >> 11, (uint16_2 & 2016) >> 5, (uint16_2 & 31) * 2), out result) ? result : DateTime.MinValue;
        }

        private ushort GetDOSDate(DateTime dtTimeStamp) => (ushort)((ushort)((ushort)((dtTimeStamp.Year - 1980 & sbyte.MaxValue) << 9) + (uint)(ushort)((dtTimeStamp.Month & 15) << 5)) + (uint)(ushort)(dtTimeStamp.Day & 31));

        private ushort GetDOSTime(DateTime dtTimeStamp) => (ushort)((ushort)((ushort)((dtTimeStamp.Hour & 31) << 11) + (uint)(ushort)((dtTimeStamp.Minute & 63) << 5)) + (uint)(ushort)(dtTimeStamp.Second >> 1 & 31));

        private void SetFAT(uint ClusterNo, uint value)
        {
            uint num = 0;
            if (NumOfFAT > 1)
                num = (uint)(FATSectors * SectorSize / NumOfFAT);
            switch (FATType)
            {
                case EnumFATType.FAT12:
                    ClusterNo &= 4095U;
                    if ((ClusterNo & 1U) > 0U)
                    {
                        uint index = (uint)((int)(ClusterNo / 2U) * 3 + 1);
                        FAT[(int)index] = (byte)((byte)(value & 15U) << 4 | FAT[(int)index] & 15);
                        FAT[(int)index + 1] = (byte)(value >> 4);
                        if (NumOfFAT <= 1)
                            break;
                        FAT[(int)index + (int)num] = FAT[(int)index];
                        FAT[(int)index + 1 + (int)num] = FAT[(int)index + 1];
                        break;
                    }
                    uint index1 = ClusterNo / 2U * 3U;
                    FAT[(int)index1] = (byte)(value & byte.MaxValue);
                    FAT[(int)index1 + 1] = (byte)(FAT[(int)index1 + 1] & 240 | (byte)(value >> 8) & 15);
                    if (NumOfFAT <= 1)
                        break;
                    FAT[(int)index1 + (int)num] = FAT[(int)index1];
                    FAT[(int)index1 + 1 + (int)num] = FAT[(int)index1 + 1];
                    break;
            }
        }

        private uint GetFAT(uint ClusterNo)
        {
            uint fat = 0;
            switch (FATType)
            {
                case EnumFATType.FAT12:
                    ClusterNo &= 4095U;
                    if ((ClusterNo & 1U) > 0U)
                    {
                        uint index = (uint)((int)(ClusterNo / 2U) * 3 + 1);
                        fat = (uint)FAT[(int)index + 1] << 4 | (uint)(FAT[(int)index] >> 4 & 15);
                        break;
                    }
                    uint index1 = ClusterNo / 2U * 3U;
                    fat = (FAT[(int)index1 + 1] & 15U) << 8 | FAT[(int)index1];
                    break;
            }
            return fat;
        }

        public bool ReadCluster(uint ClusterNo, byte[] buf)
        {
            try
            {
                for (uint index = 0; index < SectorPerCluster; ++index)
                    ReadSector((int)(DataStartSector + (ClusterNo - 2U) * SectorPerCluster + index), buf, (int)(index * SectorSize));
                return true;
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
        }

        public bool WriteCluster(uint ClusterNo, byte[] buf)
        {
            try
            {
                for (uint index = 0; index < SectorPerCluster; ++index)
                    WriteSector((int)(DataStartSector + (ClusterNo - 2U) * SectorPerCluster + index), buf, (int)(index * SectorSize));
                return true;
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
        }

        protected abstract void ReadSector(int secNo, byte[] buf, int ptr);

        protected abstract void WriteSector(int secNo, byte[] buf, int ptr);

        private void ClearFATChain(uint ClusterNo)
        {
            uint fat;
            do
            {
                fat = GetFAT(ClusterNo);
                SetFAT(ClusterNo, 0U);
                ClusterNo = fat;
            }
            while (fat < CLUSNO_BAD_FAT12);
        }

        public uint GetParentDirCluster() => GetDirNameCluster("..         ".Substring(0, 8));

        public uint GetCurrentDirCluster() => GetDirNameCluster(".          ".Substring(0, 8));

        private uint GetDirNameCluster(string DirName)
        {
            uint dirNameCluster = 0;
            for (int index = 0; index < CurDir.Count(); ++index)
            {
                if (CurDir[index].Name != null && CurDir[index].Name.Equals(DirName))
                {
                    dirNameCluster = CurDir[index].FirstClusterNo;
                    break;
                }
            }
            return dirNameCluster;
        }

        private void CopyStructureToByte(DIR_BYTE dir, int idx, ref byte[] destBytes)
        {
            Array.Copy(dir.Name, 0, destBytes, idx * DirectorySize, 11);
            Array.Copy(dir.Attr, 0, destBytes, idx * DirectorySize + 11, 1);
            Array.Copy(dir.NTRes, 0, destBytes, idx * DirectorySize + 11 + 1, 1);
            Array.Copy(dir.CrtTimeTenth, 0, destBytes, idx * DirectorySize + 11 + 1 + 1, 1);
            Array.Copy(dir.CrtTime, 0, destBytes, idx * DirectorySize + 11 + 1 + 1 + 1, 2);
            Array.Copy(dir.CrtDate, 0, destBytes, idx * DirectorySize + 11 + 1 + 1 + 1 + 2, 2);
            Array.Copy(dir.LstAccDate, 0, destBytes, idx * DirectorySize + 11 + 1 + 1 + 1 + 2 + 2, 2);
            Array.Copy(dir.FstClusHI, 0, destBytes, idx * DirectorySize + 11 + 1 + 1 + 1 + 2 + 2 + 2, 2);
            Array.Copy(dir.WrtTime, 0, destBytes, idx * DirectorySize + 11 + 1 + 1 + 1 + 2 + 2 + 2 + 2, 2);
            Array.Copy(dir.WrtDate, 0, destBytes, idx * DirectorySize + 11 + 1 + 1 + 1 + 2 + 2 + 2 + 2 + 2, 2);
            Array.Copy(dir.FstClusLO, 0, destBytes, idx * DirectorySize + 11 + 1 + 1 + 1 + 2 + 2 + 2 + 2 + 2 + 2, 2);
            Array.Copy(dir.FileSize, 0, destBytes, idx * DirectorySize + 11 + 1 + 1 + 1 + 2 + 2 + 2 + 2 + 2 + 2 + 2, 4);
        }

        private void DIR_BYTEsInitialize(ref DIR_BYTE[] dir)
        {
            for (int index = 0; index < dir.Length; ++index)
                DIR_BYTEInitialize(ref dir[index]);
        }

        private void DIR_BYTEInitialize(ref DIR_BYTE dir)
        {
            dir.Name = new byte[11];
            Array.Clear(dir.Name, 0, 11);
            dir.Attr = new byte[1];
            Array.Clear(dir.Attr, 0, 1);
            dir.NTRes = new byte[1];
            Array.Clear(dir.NTRes, 0, 1);
            dir.CrtTimeTenth = new byte[1];
            Array.Clear(dir.CrtTimeTenth, 0, 1);
            dir.CrtTime = new byte[2];
            Array.Clear(dir.CrtTime, 0, 2);
            dir.CrtDate = new byte[2];
            Array.Clear(dir.CrtDate, 0, 2);
            dir.LstAccDate = new byte[2];
            Array.Clear(dir.LstAccDate, 0, 2);
            dir.FstClusHI = new byte[2];
            Array.Clear(dir.FstClusHI, 0, 2);
            dir.WrtTime = new byte[2];
            Array.Clear(dir.WrtTime, 0, 2);
            dir.WrtDate = new byte[2];
            Array.Clear(dir.WrtDate, 0, 2);
            dir.FstClusLO = new byte[2];
            Array.Clear(dir.FstClusLO, 0, 2);
            dir.FileSize = new byte[4];
            Array.Clear(dir.FileSize, 0, 4);
        }

        public bool Rename(string NewName, uint index)
        {
            Encoding encoding = Encoding.GetEncoding("Shift_JIS");
            CurDir[(int)index].Name = NewName;
            CurDir_byte[(int)index].Name = encoding.GetBytes(NewName);
            return true;
        }

        public bool WriteVolumeLabel(string name)
        {
            bool flag = false;
            if (DiskFormatType.Equals(FormatType.MSX_DOS2))
            {
                name = name.PadRight(11);
                RootDir_byte[0].Name = Encoding.GetEncoding("Shift_JIS").GetBytes(name);
                RootDir_byte[0].Attr[0] = 8;
                RootDir_byte[0].FileSize[0] = 0;
                RootDir_byte[0].FstClusHI[0] = 0;
                RootDir_byte[0].FstClusLO[0] = 0;
                WriteDirectoryEntry(RootDir_byte);
                WriteFAT();
                flag = true;
            }
            return flag;
        }

        public void Dispose()
        {
            if (fs == null)
                return;
            fs.Close();
            fs.Dispose();
        }

        ~VFATOperation() => Dispose();

        public enum FormatType
        {
            MSX_DOS1 = 1,
            MSX_DOS2 = 2,
            DOS6_WIN = 3,
        }

        public enum EnumFATType
        {
            UNKNOWN = -1, // 0xFFFFFFFF
            FAT12 = 12, // 0x0000000C
            FAT16 = 16, // 0x00000010
            FAT32 = 32, // 0x00000020
        }

        protected enum FilePathType
        {
            NotFound = -2, // 0xFFFFFFFE
            Folder = 21, // 0x00000015
            File = 31, // 0x0000001F
            Drive = 41, // 0x00000029
        }

        private enum EntryEnum
        {
            No_Entry = -2, // 0xFFFFFFFE
            No_DiskSpace = -1, // 0xFFFFFFFF
            Succeed = 0,
        }

        public enum DIRAttribute
        {
            ATTR_MSXDOS_FILE = 0,
            ATTR_READ_ONLY = 1,
            ATTR_HIDDEN = 2,
            ATTR_SYSTEM = 4,
            ATTR_VOLUME_ID = 8,
            ATTR_LONG_FILE_NAME = 15, // 0x0000000F
            ATTR_DIRECTORY = 16, // 0x00000010
            ATTR_ARCHIVE = 32, // 0x00000020
        }

        public struct DIR
        {
            public bool UsingFlag;
            public string Name;
            public byte Attribute;
            public byte NTRes;
            public DateTime CreateDateTime;
            public DateTime LastAccessDate;
            public DateTime WriteDateTime;
            public uint Size;
            public uint FirstClusterNo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct DIR_BYTE
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] Name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] Attr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] NTRes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] CrtTimeTenth;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] CrtTime;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] CrtDate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] LstAccDate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] FstClusHI;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] WrtTime;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] WrtDate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] FstClusLO;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] FileSize;
        }

        public class VFATOperationException : Exception
        {
            public VFATOperationException()
            {
            }

            public VFATOperationException(string message)
              : base(message)
            {
            }

            public VFATOperationException(string message, Exception innerException)
              : base(message, innerException)
            {
            }

            protected VFATOperationException(SerializationInfo info, StreamingContext context)
              : base(info, context)
            {
            }
        }

        public enum VDiskStatus
        {
            READY_TO_USE,
            CANT_USEFILE,
        }
    }
}
