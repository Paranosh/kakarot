using System.Runtime.InteropServices;

namespace kakarot
{
    internal class DSKFATOperation : VFATOperation
    {
        private const int BPB_SIZE = 512;
        private BPB bpb_data;

        public DSKFATOperation(string Target)
          : base(Target)
        {
        }

        public override uint DiskSpace => (uint)(BitConverter.ToUInt16(bpb_data.TotalSectorCnt16, 0) * (ulong)SectorSize);

        public void ReadBPB()
        {
            byte[] numArray = new byte[512];
            fs.Read(numArray, 0, 512);
            nint num = Marshal.AllocHGlobal(512);
            Marshal.Copy(numArray, 0, num, 512);
            bpb_data = (BPB)Marshal.PtrToStructure(num, typeof(BPB));
            Marshal.FreeHGlobal(num);
        }

        private void WriteBPB()
        {
            byte[] numArray = new byte[512];
            nint num = Marshal.AllocHGlobal(512);
            Marshal.StructureToPtr(bpb_data, num, true);
            Marshal.Copy(num, numArray, 0, 512);
            fs.Seek(0L, SeekOrigin.Begin);
            fs.Write(numArray, 0, 512);
        }

        protected override void Initialize()
        {
            ReadBPB();
            FATStartSector = BitConverter.ToInt16(bpb_data.RsvdSecCount, 0);
            SectorSize = BitConverter.ToInt16(bpb_data.BytePerSector, 0);
            NumOfFAT = bpb_data.NumberOfFATs[0];
            FATSectors = BitConverter.ToInt16(bpb_data.FATSize16, 0) * NumOfFAT;
            RootDirStartSector = FATStartSector + FATSectors;
            RootDirEntryCount = BitConverter.ToInt16(bpb_data.RootEntryCnt, 0);
            DirectorySize = Marshal.SizeOf(typeof(DIR_BYTE));
            RootDirSectors = (DirectorySize * RootDirEntryCount + SectorSize - 1) / SectorSize;
            DataStartSector = RootDirStartSector + RootDirSectors;
            DataSectors = BitConverter.ToInt16(bpb_data.TotalSectorCnt16, 0) - DataStartSector;
            SectorPerCluster = bpb_data.SecPerCluster[0];
            ClusterSize = SectorPerCluster * SectorSize;
            FATSize = FATSectors * SectorSize;
            FATEntry = DataSectors * SectorSize / ClusterSize + 1;
            if (BitConverter.ToUInt32(bpb_data.TotalSectorCnt32, 0) == 1598836566U)
                _DiskFormatType = FormatType.MSX_DOS2;
            else
                _DiskFormatType = FormatType.MSX_DOS1;
            DetectFileSystem();
        }

        private void DetectFileSystem()
        {
            int num = DataSectors / bpb_data.SecPerCluster[0];
            if (num <= 4085)
                FATType = EnumFATType.FAT12;
            else if (num >= 4086 && num <= 65526)
            {
                FATType = EnumFATType.FAT16;
            }
            else
            {
                if (num < 65527)
                    return;
                FATType = EnumFATType.FAT32;
            }
        }

        protected override void ReadSector(int secNo, byte[] buf, int ptr)
        {
            try
            {
                uint uint16 = BitConverter.ToUInt16(bpb_data.TotalSectorCnt16, 0);
                byte[] buffer = new byte[SectorSize];
                if (secNo > uint16)
                    throw new VFATOperationException("Sector Number is out of range.");
                fs.Seek(secNo * SectorSize, SeekOrigin.Begin);
                if (fs.Read(buffer, 0, SectorSize) < SectorSize)
                    throw new VFATOperationException("Read size is less than sector size.");
                buffer.CopyTo(buf, ptr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void WriteSector(int secNo, byte[] buf, int ptr)
        {
            try
            {
                uint uint16 = BitConverter.ToUInt16(bpb_data.TotalSectorCnt16, 0);
                if (secNo > uint16)
                    throw new VFATOperationException("Sector Number is out of range.");
                fs.Seek(SectorSize * secNo, SeekOrigin.Begin);
                fs.Write(buf, ptr, SectorSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public struct BPB
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] JmpBoot;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] OEMName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] BytePerSector;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] SecPerCluster;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] RsvdSecCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] NumberOfFATs;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] RootEntryCnt;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] TotalSectorCnt16;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] Media;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] FATSize16;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] SectorPerTrack;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] NumberOfHeads;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] HiddenSectorCnt;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] TotalSectorCnt32;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] DriveNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] Reserved1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public byte[] BootSignature;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] VolumeSerialID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public byte[] VolumeLabel;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] FileSystemType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 448)]
            public byte[] BootStrapCode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] BootSign;
        }
    }
}
