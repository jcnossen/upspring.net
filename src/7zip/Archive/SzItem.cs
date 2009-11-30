// Copyright 7Zip and Hugh Perkins 2006
// hughperkins@gmail.com http://manageddreams.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

// ported to c# by Hugh Perkins 2006

using System;
using System.Collections.Generic;
using System.Text;

namespace SevenZip
{
    public class Folder
    {
        public CoderInfo[] Coders;
        public BindPair[] BindPairs;
        public ulong[] UnPackSizes;
        public uint[] PackStreams;
        public bool UnPackCRCDefined;
        public uint UnPackCRC;
        public uint NumCoders;
        public uint NumBindPairs;
        public uint NumPackStreams;
        public uint NumUnPackStreams;
        public Folder()
        {
        }
        public override string ToString()
        {
            return "Folder: numCoders: " + NumCoders + 
                " NumBindPairs: " + NumBindPairs + 
                " NumPackStreams: " + NumPackStreams + 
                " PackStreams size: " + ( PackStreams.GetUpperBound(0) + 1 ).ToString() + " " +
                " NumUnPackStreams: " + NumUnPackStreams;
        }
        public int FindBindPairForInStream(uint inStreamIndex)
        {
            for (uint i = 0; i < NumBindPairs; i++)
            {
                if (BindPairs[i].InIndex == inStreamIndex)
                {
                    return Convert.ToInt32( i );
                }
            }
            return -1;
        }
        int FindBindPairForOutStream( uint outStreamIndex )
        {
            for (uint i = 0; i < NumBindPairs; i++)
            {
                if( BindPairs[i].OutIndex == outStreamIndex)
                {
                    return Convert.ToInt32( i );
                }
            }
            return -1;
        }
        public uint GetNumOutStreams()
        {
            uint result = 0;
            for (uint i = 0; i < NumCoders; i++)
                result += Coders[i].NumOutStreams;
            return result;
        }
        public ulong GetUnPackSize()
        {
            int i = (int)GetNumOutStreams();
            if( i == 0 )
                return 0;
            for( i--; i >= 0; i-- )
            {
                if( FindBindPairForOutStream( (uint)i ) < 0 )
                {
                    return UnPackSizes[i];
                }
            }
            //throw new Exception("GetUnPackSize: shouldnt get here");
            /* throw 1; */ 
            return 0;
        }
    }

    public class CoderInfo
    {
        public MethodID MethodID = new MethodID();
        public uint NumInStreams;
        public uint NumOutStreams;
        public byte[] Properties;
        public CoderInfo()
        {
        }
        public override string ToString()
        {
            return "CoderInfo: NumInStreams: " + NumInStreams + " NumOutStreams: " + NumOutStreams + " properties size: " + Properties.GetUpperBound(0) + 1;
        }
    }

    public class MethodID
    {
        public byte IDSize;
        public byte[] ID;
        public override string ToString()
        {
            return "MethodID: " + IDSize + " bytes";
        }
    }

    public class BindPair
    {
        public uint InIndex;
        public uint OutIndex;
        public override string ToString()
        {
            return "BindPair: " + InIndex + " -> " + OutIndex;
        }
    }
    public class FileItem
    {
      public ulong Size;
      public uint FileCRC;
      public string Name;

      public bool IsFileCRCDefined;
      public bool HasStream;
      public bool IsDirectory;
      public bool IsAnti;
        public override string ToString()
        {
            return "FileItem: " + Name + " Size: " + Size + " HasStream: " + HasStream + " IsDirectory: " + IsDirectory +
                " IsAnti: " + IsAnti;
        }
    }

    public class ArchiveDatabase
    {
      public uint NumPackStreams;
      public ulong[] PackSizes;
      public bool[]PackCRCsDefined;
      public uint[] PackCRCs;
      public uint NumFolders;
      public Folder[] Folders;
      public uint NumFiles;
      public FileItem[] Files;
    //void SzArchiveDatabaseInit(CArchiveDatabase *db);
    //void SzArchiveDatabaseFree(CArchiveDatabase *db, void (*freeFunc)(void *));
    }
}
