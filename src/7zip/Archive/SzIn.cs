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
using System.IO;

// packinfo is informationg on the packed file, eg compressed size
// unpackinfo is information on the unpacked file, eg real size

namespace SevenZip
{
    public class InArchiveInfo
    {
        public ulong StartPositionAfterHeader;
        public ulong DataStartPosition;
    }

    public class SzIn
    {
        byte[] k7zSignature = new byte[] { (byte)'7', (byte)'z', 0xBC, 0xAF, 0x27, 0x1C };
        const int k7zSignatureSize = 6;
        const int k7zMajorVersion = 0;
        const int k7zStartHeaderSize = 0x20; // 32 or 20  ???
        byte[] kUtf8Limits = new byte[]{ 0xC0, 0xE0, 0xF0, 0xF8, 0xFC };
        enum EIdEnum
        {
            k7zIdEnd,

            k7zIdHeader,

            k7zIdArchiveProperties,

            k7zIdAdditionalStreamsInfo,
            k7zIdMainStreamsInfo,
            k7zIdFilesInfo,

            k7zIdPackInfo,
            k7zIdUnPackInfo,
            k7zIdSubStreamsInfo,

            k7zIdSize,
            k7zIdCRC,

            k7zIdFolder,

            k7zIdCodersUnPackSize,
            k7zIdNumUnPackStream,

            k7zIdEmptyStream,
            k7zIdEmptyFile,
            k7zIdAnti,

            k7zIdName,
            k7zIdCreationTime,
            k7zIdLastAccessTime,
            k7zIdLastWriteTime,
            k7zIdWinAttributes,
            k7zIdComment,

            k7zIdEncodedHeader,

            k7zIdStartPos
        };

        void CheckSignature(byte[] signature)
        {
            for (int i = 0; i < k7zSignatureSize; i++)
            {
                if (signature[i] != k7zSignature[i])
                {
                    throw new Exception("Invalid signature, not 7zip");
                }
            }
        }

        void szSkeepDataSize( SzData sd, UInt64 size)
        {
            //if( size > data.GetUpperBound(0) + 1)
            //{
             //   throw new Exception("szSkeepDataSize: end of file error");
            //}
            sd.Offset += size;
        }

        void szSkeepData(SzData sd)
        {
            UInt64 size = szReadNumber( sd );
            szSkeepDataSize(sd, size);
        }

        void CheckMajorVersion(byte majorversion)
        {
            if (majorversion != k7zMajorVersion)
            {
                throw new Exception("Invalid major version: " + majorversion.ToString());
            }
        }

        byte ReadByte()
        {
            bytesread++;
            inStream.Read(buffer, 0, 1);
            return buffer[0];
        }

        UInt32 ReadUInt32()
        {
            bytesread+=4;
            inStream.Read(buffer, 0, 4);
            return BitConverter.ToUInt32( buffer, 0 );
        }

        ulong Readulong()
        {
            bytesread += 8;
            inStream.Read(buffer, 0, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }

        byte szReadByte(SzData sd)
        {
            byte result = sd.Data[sd.Offset];
            sd.Offset++;
            return result;
        }

        uint szReadUInt32(SzData sd)
        {
            uint value = 0;
            for ( int i = 0; i < 4; i++)
            {
                byte b;
                b = szReadByte( sd );
                value |= ((UInt32)(b) << (8 * i));
            }
            return value;
        }

        ulong szReadNumber( SzData sd )
        {
            //Console.WriteLine("SzReadNumber offset " + offset );
            byte firstByte;
            byte mask = 0x80;
            int i;
            //firstByte = data[offset];
            firstByte = szReadByte(sd);
            //RINOK(SzReadByte(sd, &firstByte));
            //offset++;
            ulong value = 0;
            for (i = 0; i < 8; i++)
            {
                //Console.WriteLine("             offset " + offset + " i: " + i);
                byte b;
                if ((firstByte & mask) == 0)
                {
                    ulong highPart = (ulong)( firstByte & (mask - 1) );
                    value += (highPart << (8 * i));
                    return value;
                }
                //b = data[offset];
                //offset++;
                b = szReadByte(sd);
                //RINOK(SzReadByte(sd, &b));
                value |= ((ulong)b << (8 * i));
                mask >>= 1;
            }
            return value;
        }

        byte[] szReadBytes(SzData sd, ulong size)
        {
            byte[] bytes = new byte[size];
            for ( ulong i = 0; i < size; i++)
            {
                bytes[i] = szReadByte( sd );
            }
            return bytes;
        }

        EIdEnum szReadID( SzData sd )
        {
            return (EIdEnum)szReadNumber( sd );
        }

        ulong szReadSize(SzData sd)
        {
            return szReadNumber( sd );
        }

        uint szReadNumber32(SzData sd)
        {
            return (uint)szReadNumber( sd );
        }

        void szReadSwitch(SzData sd)
        {
            byte external = szReadByte( sd );
            if (external != 0)
            {
                throw new Exception("szReadSwitch, external should be zero");
            }
        }

        bool[] szReadBoolVector( SzData sd, ulong numItems )
        {
          byte b = 0;
          byte mask = 0;
          
          bool[]v = new bool[numItems];
          for( ulong i = 0; i < numItems; i++)
          {
            if (mask == 0)
            {
              b = szReadByte( sd );
              mask = 0x80;
            }
            if ((b & mask) != 0)
            {
                v[i] = true;
            }
            else
            {
                v[i] = false;
            }
            //(*v)[i] = (Byte)(((b & mask) != 0) ? 1 : 0);
            mask >>= 1;
          }
          return v;
        }

        bool[] szReadBoolVector2(SzData sd, ulong numItems)
        {
            byte allAreDefined = szReadByte( sd );
            if (allAreDefined == 0)
            {
                return szReadBoolVector(sd, numItems);
            }
            bool[] v = new bool[numItems];
            for (ulong i = 0; i < numItems; i++)
            {
                v[i] = true;
            }
            return v;
        }

        void szReadHashDigests(SzData sd, ulong numItems, out bool[]digestsDefined, out uint[] digests )
        {
            digestsDefined = szReadBoolVector2(sd, numItems);
            digests = new uint[numItems];
            for( ulong i = 0; i < numItems; i++)
            {
                if( digestsDefined[i] )
                {
                    digests[i] = szReadUInt32( sd );
                }
            }
        }

        void szWaitAttribute(SzData sd, EIdEnum attribute)
        {
            while (true)
            {
                EIdEnum type = szReadID( sd );
                // Console.WriteLine("WaitAttribute, found " + type);
                if (type == attribute)
                {
                    return;
                }
                if (type == EIdEnum.k7zIdEnd)
                {
                    throw new Exception("szWaitAttribute: attribute " + attribute + " not found.");
                }
                szSkeepData(sd);
            }
        }

        void szReadPackInfo(SzData sd, 
            out ulong dataOffset, out uint numPackStreams, out ulong[] packSizes,
            out bool[]packCRCsDefined, out uint[]packCRCs )
        {
            //Console.WriteLine("ReadPackInfo >>>");

            packCRCsDefined = null;
            packCRCs = null;

            dataOffset = szReadSize(sd);
            // Console.WriteLine("szREadPackInfo, dataOffset = " + dataOffset);
            numPackStreams = szReadNumber32(sd);

            szWaitAttribute(sd, EIdEnum.k7zIdSize);
            packSizes = new ulong[ numPackStreams ];
            for (ulong i = 0; i < numPackStreams; i++)
            {
                packSizes[i] = szReadSize(sd);
                //Console.WriteLine("Packsize[" + i + "] : " + packSizes[i]);
            }

            while( true )
            {
                EIdEnum type = szReadID( sd );
                //Console.WriteLine("ReadPackInfo 2, got type: " + type);
                if (type == EIdEnum.k7zIdEnd)
                    break;
                if (type == EIdEnum.k7zIdCRC)
                {
                    szReadHashDigests(sd, numPackStreams, out packCRCsDefined, out packCRCs );
                    continue;
                }
                szSkeepData(sd);
            }
            
            if ( packCRCsDefined == null )
            {
                packCRCsDefined = new bool[numPackStreams];
                packCRCs = new uint[ numPackStreams ];
                for ( uint i = 0; i < numPackStreams; i++)
                {
                    packCRCsDefined[i] = false;
                    packCRCs[i] = 0;
                }
            }

            //Console.WriteLine("ReadPackInfo <<<");
        }

        Folder szGetNextFolderItem(SzData sd)
        {
            Folder folder = new Folder();
            //Console.WriteLine("szGetNextFolderItem >>>");
            uint numCoders = szReadNumber32(sd);
            folder.NumCoders = numCoders;

            //Console.WriteLine("numcoders: " + numCoders);
            folder.Coders = new CoderInfo[numCoders];

            uint numInStreams = 0;
            uint numOutStreams = 0;

            for (uint i = 0; i < numCoders; i++)
            {
                CoderInfo coder = new CoderInfo();
                folder.Coders[i] = coder;
                byte mainByte = szReadByte(sd);
                coder.MethodID.IDSize = (byte)(mainByte & 0xF);
                coder.MethodID.ID = szReadBytes(sd, coder.MethodID.IDSize);
                if ((mainByte & 0x10) != 0)
                {
                    coder.NumInStreams = szReadNumber32(sd);
                    coder.NumOutStreams = szReadNumber32(sd);
                }
                else
                {
                    coder.NumInStreams = 1;
                    coder.NumOutStreams = 1;
                }
                if ((mainByte & 0x20) != 0)
                {
                    UInt64 propertiesSize = szReadNumber(sd);
                    coder.Properties = szReadBytes(sd, propertiesSize);
                }
                while ((mainByte & 0x80) != 0)
                {
                    mainByte = szReadByte(sd);
                    szSkeepDataSize(sd, (ulong)(mainByte & 0xF));
                    //RINOK(SzSkeepDataSize(sd, (mainByte & 0xF)));
                    if ((mainByte & 0x10) != 0)
                    {
                        //UInt32 n;
                        szReadNumber32(sd);
                        szReadNumber32(sd);
                    }
                    if ((mainByte & 0x20) != 0)
                    {
                        UInt64 propertiesSize = szReadNumber(sd);
                        szSkeepDataSize(sd, propertiesSize);
                    }
                }
                numInStreams += coder.NumInStreams;
                numOutStreams += coder.NumOutStreams;
                //Console.WriteLine(coder);
            }

            uint numBindPairs = numOutStreams - 1;
            folder.NumBindPairs = numBindPairs;

            //Console.WriteLine("numbindpairs: " + numBindPairs);
            folder.BindPairs = new BindPair[numBindPairs];

            for (uint i = 0; i < numBindPairs; i++)
            {
                BindPair bindpair = new BindPair();
                folder.BindPairs[i] = bindpair;
                //CBindPair *bindPair = folder->BindPairs + i;;
                bindpair.InIndex = szReadNumber32(sd);
                bindpair.OutIndex = szReadNumber32(sd);
                //Console.WriteLine("Bind pair: " + i + " " + bindpair.ToString());
            }

            uint numPackedStreams = numInStreams - numBindPairs;

            folder.NumPackStreams = numPackedStreams;
            folder.PackStreams = new uint[numPackedStreams];

            if (numPackedStreams == 1)
            {
                UInt32 pi = 0;
                for (uint j = 0; j < numInStreams; j++)
                {
                    if (folder.FindBindPairForInStream(j) < 0)
                    {
                        folder.PackStreams[pi++] = j;
                        break;
                    }
                }
            }
            else
            {
                for (uint i = 0; i < numPackedStreams; i++)
                {
                    folder.PackStreams[i] = szReadNumber32(sd);
                }
            }

            //Console.WriteLine("szGetNextFolderItem <<<");
            return folder;
        }

        void szReadUnPackInfo(SzData sd, out uint numFolders, out Folder[] folders )
        {
            //Console.WriteLine("szReadUnPackInfo >>>");
            szWaitAttribute( sd, EIdEnum.k7zIdFolder );
            numFolders = szReadNumber32( sd );
            //Console.WriteLine("numfolders: " + numFolders);
            szReadSwitch(sd);
            
            folders = new Folder[ numFolders ];

            for ( uint i = 0; i < numFolders; i++)
            {
                folders[i] = szGetNextFolderItem(sd);
                //Console.WriteLine(folders[i]);
            }
            
            szWaitAttribute( sd, EIdEnum.k7zIdCodersUnPackSize );

            for ( uint i = 0; i < numFolders; i++)
            {
                Folder folder = folders[i];
                uint numOutStreams = folder.GetNumOutStreams();

                folder.UnPackSizes = new ulong[numOutStreams];

                for ( uint j = 0; j < numOutStreams; j++)
                {
                    folder.UnPackSizes[j] = szReadSize(sd);
                    //Console.WriteLine("unpacksize: " + folder.UnPackSizes[j]);
                }
            }
            
            while( true )
            {
                EIdEnum type = szReadID( sd );
                //Console.WriteLine( "got type: " + type );
                if (type == EIdEnum.k7zIdEnd)
                {
                    //Console.WriteLine("szReadUnPackInfo <<<");
                    return;
                }
                if (type == EIdEnum.k7zIdCRC)
                {
                    bool[] crcsDefined;
                    uint[] crcs;
                    szReadHashDigests( sd, numFolders, out crcsDefined, out crcs );
                    for ( uint i = 0; i < numFolders; i++)
                    {
                        Folder folder = folders[i];
                        folder.UnPackCRCDefined = crcsDefined[i];
                        folder.UnPackCRC = crcs[i];
                    }
                    continue;
                }
                szSkeepData(sd);
            }
            //Console.WriteLine("szReadUnPackInfo <<<");
            //return;
        }

        void szReadSubStreamsInfo( SzData sd, Folder[] folders,
            out uint numUnPackStreams, out ulong[] unPackSizes, out bool[] digestsDefined,
            out uint[] digests )
        {
            uint numDigests = 0;
            uint numFolders = Convert.ToUInt32( folders.GetUpperBound(0) + 1 );

            for ( uint i = 0; i < numFolders; i++)
            {
                folders[i].NumUnPackStreams = 1;
            }

            numUnPackStreams = numFolders;

            EIdEnum type = EIdEnum.k7zIdEnd;
            while( true )
            {
                type = szReadID(sd);
                //Console.WriteLine("ReadSubStreamsInfo, got type: " + type);
                if (type == EIdEnum.k7zIdNumUnPackStream)
                {
                    numUnPackStreams = 0;
                    for ( uint i = 0; i < numFolders; i++)
                    {
                        uint numStreams = szReadNumber32(sd);
                        folders[i].NumUnPackStreams = numStreams;
                        numUnPackStreams += numStreams;
                    }
                    continue;
                }
                if (type == EIdEnum.k7zIdCRC || type == EIdEnum.k7zIdSize)
                    break;
                if (type == EIdEnum.k7zIdEnd)
                    break;
                szSkeepData(sd );
            }

            unPackSizes = null;
            digestsDefined = null;
            digests = null;
            if (numUnPackStreams > 0)
            {
                unPackSizes = new ulong[numUnPackStreams];
                digestsDefined = new bool[numUnPackStreams];
                digests = new uint[numUnPackStreams];
                //*unPackSizes = (CFileSize*)allocTemp->Alloc((size_t) * numUnPackStreams * sizeof(CFileSize));
                //RINOM(*unPackSizes);
                //*digestsDefined = (Byte*)allocTemp->Alloc((size_t) * numUnPackStreams * sizeof(Byte));
                //RINOM(*digestsDefined);
                //*digests = (UInt32*)allocTemp->Alloc((size_t) * numUnPackStreams * sizeof(UInt32));
                //RINOM(*digests);
            }

            uint si = 0;
            for ( uint i = 0; i < numFolders; i++)
            {
                //
                //v3.13 incorrectly worked with empty folders
                //v4.07: we check that folder is empty
                //
                ulong sum = 0;
                uint numSubstreams = folders[i].NumUnPackStreams;
                if (numSubstreams == 0)
                    continue;
                if (type == EIdEnum.k7zIdSize)
                {
                    for ( uint j = 1; j < numSubstreams; j++)
                    {
                        ulong size = szReadSize( sd );
                        unPackSizes[si] = size;
                        sum += size;
                        si++;
                    }
                }
                unPackSizes[ si ] = folders[i].GetUnPackSize() - sum;
                //Console.Write("unpacksize[" + si + "] : " + unPackSizes[si]);
                si++;
                //(*unPackSizes)[si++] = SzFolderGetUnPackSize(folders + i) - sum;
            }
            if (type == EIdEnum.k7zIdSize)
            {
                type = szReadID( sd );
                //Console.WriteLine("ReadSubStreamsInfo: 3, got type " + type);
            }

            for ( uint i = 0; i < numUnPackStreams; i++)
            {
                digestsDefined[i] = false;
                digests[i] = 0;
            }

            for ( uint i = 0; i < numFolders; i++)
            {
                uint numSubstreams = folders[i].NumUnPackStreams;
                if (numSubstreams != 1 || !folders[i].UnPackCRCDefined)
                {
                    numDigests += numSubstreams;
                }
            }

            si = 0;
            while( true )
            {
                if (type == EIdEnum.k7zIdCRC)
                {
                    int digestIndex = 0;
                    bool[] digestsDefined2;
                    uint[] digests2;
                    szReadHashDigests(sd, numDigests, out digestsDefined2, out digests2);
                    for ( uint i = 0; i < numFolders; i++)
                    {
                        Folder folder = folders[i];
                        uint numSubstreams = folder.NumUnPackStreams;
                        if (numSubstreams == 1 && folder.UnPackCRCDefined)
                        {
                            digestsDefined[si] = true;
                            digests[si] = folder.UnPackCRC;
                            si++;
                        }
                        else
                        {
                            for (uint j = 0; j < numSubstreams; j++, digestIndex++)
                            {
                                digestsDefined[si] = digestsDefined2[digestIndex];
                                digests[si] = digests2[digestIndex];
                                si++;
                            }
                        }
                    }
                }
                else if (type == EIdEnum.k7zIdEnd)
                    return;
                else
                {
                    szSkeepData( sd );
                }
                type = szReadID( sd );
            }
        }

        void szReadStreamsInfo(SzData sd, out ulong dataOffset,
            ArchiveDatabase db,
            out uint numUnPackStreams, out ulong[] unPackSizes,
            out bool[]packCRCsDefined, out uint[] packCRCs)
        {
            //Console.WriteLine("szReadStreamsInfo >>>");
            numUnPackStreams = 0;
            unPackSizes = null;
            packCRCsDefined = null;
            packCRCs = null;

            dataOffset = 0;

            while (true)
            {
                EIdEnum type = szReadID(sd );
                //Console.WriteLine( "ReadStreamsInfox, got type: " +  type);
                switch (type)
                {
                    case EIdEnum.k7zIdEnd:
                        return;

                    case EIdEnum.k7zIdPackInfo:
                        //Console.WriteLine("calling szReadPackInfo...");
                        szReadPackInfo(sd, out dataOffset, out db.NumPackStreams,
                            out db.PackSizes, out db.PackCRCsDefined, out db.PackCRCs);
                        //Console.WriteLine("szReadStreamsInfo, crc info: ");
                        break;

                    case EIdEnum.k7zIdUnPackInfo:
                        szReadUnPackInfo(sd,out db.NumFolders, out db.Folders );
                        break;

                    case EIdEnum.k7zIdSubStreamsInfo:
                        szReadSubStreamsInfo(sd, db.Folders,
                            out numUnPackStreams, out unPackSizes, out packCRCsDefined, out packCRCs );
                            //RINOK(SzReadSubStreamsInfo(sd, db->NumFolders, db->Folders,
                            //    numUnPackStreams, unPackSizes, digestsDefined, digests, allocTemp));
                        break;

                    default:
                        throw new Exception("Unexpected type in szReadStreamsInfo " + type);
                }
            }
            //Console.WriteLine("szReadStreamsInfo <<<");
        }

        void szReadFileNames(SzData sd, uint numFiles, FileItem[] files)
        {
            //Console.WriteLine("szReadFileNames >>> numfiles: " + numFiles);
            uint i;
            for (i = 0; i < numFiles; i++)
            {
                uint len = 0;
                ulong pos = sd.Offset;
                FileItem file = files[i];
                while (pos + 2 <= sd.Length )
                {
                    int numAdds;
                    uint value = (uint)(sd.Data[ pos] | (((uint)sd.Data[ pos + 1]) << 8));
                    pos += 2;
                    len++;
                    if (value == 0)
                        break;
                    if (value < 0x80)
                        continue;
                    if (value >= 0xD800 && value < 0xE000)
                    {
                        uint c2;
                        if (value >= 0xDC00)
                        {
                            throw new Exception("Unexpected value in szReadFileNames: " + value);
                            //return SZE_ARCHIVE_ERROR;
                        }
                        if (pos + 2 > sd.Length )
                        {
                            throw new Exception("End of file error in szReadFileNames");
                            //return SZE_ARCHIVE_ERROR;
                        }
                        c2 = (uint)(sd.Data[pos ] | (((uint)sd.Data[pos + 1]) << 8));
                        pos += 2;
                        if (c2 < 0xDC00 || c2 >= 0xE000)
                        {
                            throw new Exception("Unexpected c2 value in szReadFileNames: " + c2);
                            //return SZE_ARCHIVE_ERROR;
                        }
                        value = ((value - 0xD800) << 10) | (c2 - 0xDC00);
                    }
                    for (numAdds = 1; numAdds < 5; numAdds++)
                        if (value < (((uint)1) << (numAdds * 5 + 6)))
                            break;
                    len += (uint)numAdds;
                }

                char[] namechararray = new char[len];
                //RINOK(MySzInAlloc((void **)&file->Name, (size_t)len * sizeof(char), allocFunc));
                len = 0;
                while ( sd.Length - sd.Offset >= 2 )
                {
                    int numAdds;
                    uint value = (uint)(sd.Data[sd.Offset] | (((uint)sd.Data[sd.Offset + 1]) << 8));
                    szSkeepDataSize(sd, 2);
                    if (value < 0x80)
                    {
                        namechararray[len++] = (char)value;
                        if (value == 0)
                            break;
                        continue;
                    }
                    if (value >= 0xD800 && value < 0xE000)
                    {
                        uint c2 = (uint)(sd.Data[sd.Offset] | (((uint)sd.Data[sd.Offset + 1]) << 8));
                        szSkeepDataSize(sd, 2);
                        value = ((value - 0xD800) << 10) | (c2 - 0xDC00);
                    }
                    for (numAdds = 1; numAdds < 5; numAdds++)
                        if (value < (((uint)1) << (numAdds * 5 + 6)))
                            break;
                    namechararray[len++] = (char)(kUtf8Limits[numAdds - 1] + (value >> (6 * numAdds)));
                    do
                    {
                        numAdds--;
                        namechararray[len++] = (char)(0x80 + ((value >> (6 * numAdds)) & 0x3F));
                    }
                    while (numAdds > 0);

                    len += (uint)numAdds;
                }
                file.Name = new String( namechararray );
                //Console.WriteLine("Filename: " + file.Name);
            }
        }

        void SzReadHeader2(SzData sd, ArchiveDatabaseEx db,
            out ulong[] unPackSizes, out bool[] digestsDefined, out uint[] digests,
            out bool[] emptyStreamVector, out bool[] emptyFileVector)
        {
            // Console.WriteLine("SzReadHeader2 >>>");

            EIdEnum type;
            uint numUnPackStreams = 0;
            uint numFiles = 0;
            FileItem[] files = null;
            uint numEmptyStreams = 0;

            unPackSizes = null;
            digestsDefined = null;
            digests = null;
            emptyStreamVector = null;
            emptyFileVector = null;

            type = szReadID(sd);
            //Console.WriteLine("SzReadHeader2， got type: " + type);

            if (type == EIdEnum.k7zIdArchiveProperties)
            {
                //SzReadArchiveProperties( sd );
                throw new Exception("Need to write szReadArchiveProperties, please contact 7zip port developer");
                //type = szReadID(sd);
            }

            if (type == EIdEnum.k7zIdMainStreamsInfo)
            {
                szReadStreamsInfo(sd, out db.ArchiveInfo.DataStartPosition,
                    db.Database, out numUnPackStreams,
                    out unPackSizes,
                    out digestsDefined, out digests);
                db.ArchiveInfo.DataStartPosition += db.ArchiveInfo.StartPositionAfterHeader;
                type = szReadID(sd);
            }
            if (type == EIdEnum.k7zIdEnd)
            {
                return;
            }
            if (type != EIdEnum.k7zIdFilesInfo)
            {
                throw new Exception("Error, unexpected type: " + type);
            }
            numFiles = szReadNumber32(sd);
            //Console.WriteLine("SzReadHeader2 Number files: " + numFiles);
            db.Database.NumFiles = numFiles;

            files = new FileItem[numFiles];
            //RINOK(MySzInAlloc((void **)&files, (size_t)numFiles * sizeof(CFileItem), allocMain->Alloc));

            db.Database.Files = files;
            for (uint i = 0; i < numFiles; i++)
            {
                db.Database.Files[i] = new FileItem();
                // SzFileInit(files + i);
            }

            while (true)
            {
                ulong size;
                type = szReadID(sd); // note to self: maybe need to read type as UInt64???
                if (type == EIdEnum.k7zIdEnd)
                {
                    break;
                }

                size = szReadNumber(sd);

                if ((UInt64)((int)type) != (UInt64)type) // note to self: maybe need to read type as UInt64???
                {
                    szSkeepDataSize(sd, size);
                }
                else
                {
                    switch (type)
                    {
                        case EIdEnum.k7zIdName:
                            {
                                szReadSwitch(sd);
                                szReadFileNames(sd, numFiles, files);
                                //RINOK(SzReadFileNames(sd, numFiles, files, allocMain->Alloc))
                                break;
                            }
                        case EIdEnum.k7zIdEmptyStream:
                            {
                                emptyStreamVector = szReadBoolVector(sd, numFiles);
                                //RINOK(SzReadBoolVector(sd, numFiles, emptyStreamVector, allocTemp->Alloc));
                                numEmptyStreams = 0;
                                for (uint i = 0; i < numFiles; i++)
                                    if (emptyStreamVector[i])
                                        numEmptyStreams++;
                                break;
                            }

                        case EIdEnum.k7zIdEmptyFile:
                            {
                                emptyFileVector = szReadBoolVector(sd, numEmptyStreams);
                                //RINOK(SzReadBoolVector(sd, numEmptyStreams, emptyFileVector, allocTemp->Alloc));
                                break;
                            }
                        default:
                            {
                                szSkeepDataSize(sd, size);
                                // RINOK(SzSkeepDataSize(sd, size));
                                break;
                            }
                    }
                }
                uint emptyFileIndex = 0;
                uint sizeIndex = 0;
                for (uint i = 0; i < numFiles; i++)
                {
                    FileItem file = files[i];
                    file.IsAnti = false;
                    if (emptyStreamVector == null)
                    {
                        file.HasStream = true;
                    }
                    else
                    {
                        file.HasStream = !emptyStreamVector[i];
                        //file.HasStream = (Byte)((*emptyStreamVector)[i] ? 0 : 1);
                    }
                    if (file.HasStream)
                    {
                        file.IsDirectory = false;
                        file.Size = unPackSizes[sizeIndex];
                        file.FileCRC = digests[sizeIndex];
                        file.IsFileCRCDefined = digestsDefined[sizeIndex];
                        sizeIndex++;
                    }
                    else
                    {
                        if (emptyFileVector == null)
                        {
                            file.IsDirectory = true;
                        }
                        else
                        {
                            file.IsDirectory = !emptyFileVector[emptyFileIndex];
                            //file.IsDirectory = (Byte)((*emptyFileVector)[emptyFileIndex] ? false : true);
                        }
                        emptyFileIndex++;
                        file.Size = 0;
                        file.IsFileCRCDefined = false;
                    }
                }
            }
            db.Fill();
            //foreach (FileItem file in files)
            //{
             //   Console.WriteLine(file);
            //}
            //Console.WriteLine("SzReadHeader <<<");
        }

        void SzReadHeader(
            SzData sd,
            ArchiveDatabaseEx db )
        {
            ulong[] unPackSizes;
            bool[] digestsDefined;
            uint[]digests;
            bool[] emptyStreamVector;
            bool[] emptyFileVector;
            SzReadHeader2(sd, db,
                out unPackSizes, out digestsDefined, out digests,
                out emptyStreamVector, out emptyFileVector );
        }

        void SzReadAndDecodePackedStreams2(
            FileStream inStream,
            SzData sd,
            out byte[] outBuffer,
            ulong baseOffset,
            ArchiveDatabase db,
            out ulong[] unPackSizes,
            out bool[] digestsDefined,
            out uint[] digests
            )
        {

            uint numUnPackStreams = 0;
            ulong dataStartPos = 0;
            Folder folder;
            ulong unPackSize;
            //ulong outRealSize;
            ulong packSize = 0;
            //uint i = 0;

            outBuffer = null;

            szReadStreamsInfo(sd, out dataStartPos, db, out numUnPackStreams, out unPackSizes,
                out digestsDefined, out digests);
            //RINOK(SzReadStreamsInfo(sd, &dataStartPos, db,
            //&numUnPackStreams,  unPackSizes, digestsDefined, digests, 
            //allocTemp->Alloc, allocTemp));

            dataStartPos += baseOffset;
            if (db.NumFolders != 1)
            {
                throw new Exception("db.NumFolders value unexpected: " + db.NumFolders);
            }

            folder = db.Folders[0];
            unPackSize = folder.GetUnPackSize();

            //Console.WriteLine("datastartpos : " + dataStartPos);
            inStream.Seek((long)dataStartPos, SeekOrigin.Begin);

            //#ifndef _LZMA_IN_CB
            for (uint i = 0; i < db.NumPackStreams; i++)
              packSize += db.PackSizes[i];
          //Console.WriteLine("packsize: " + packSize);

            //RINOK(MySzInAlloc((void **)inBuffer, (size_t)packSize, allocTemp->Alloc));

            //RINOK(SafeReadDirect(inStream, *inBuffer, (size_t)packSize));
            //#endif

            outBuffer = new byte[unPackSize];
            //if (!SzByteBufferCreate(outBuffer, (size_t)unPackSize, allocTemp->Alloc))
             //   return SZE_OUTOFMEMORY;

            byte[]inbuffer = new byte[ packSize ];
            inStream.Read( inbuffer, 0, (int)packSize );

            //Console.WriteLine("NumCoders: " + ( folder.Coders.GetUpperBound(0) + 1 ));
            //Console.WriteLine("Coder: " + folder.Coders[0]);
            //Console.WriteLine("Coder num properties: " + folder.Coders[0].Properties.GetUpperBound(0) + 1);
            //DumpBytes(folder.Coders[0].Properties, folder.Coders[0].Properties.GetUpperBound(0) + 1);

            inStream.Seek((long)dataStartPos, SeekOrigin.Begin);
            //byte[] properties = new byte[5];
            //inStream.Read(properties, 0, 5);
            Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
            decoder.SetDecoderProperties(folder.Coders[0].Properties);
            MemoryStream outStream = new MemoryStream(outBuffer);
            decoder.Code(inStream, outStream, (long)( packSize - 5 ), (long)unPackSize, null);
            outStream.Close();
            //DumpBytes(outBuffer, (int)unPackSize);
            //Console.WriteLine(outBuffer.GetUpperBound(0) + 1);
            //for (ulong i = 0; i < unPackSize; i++)
            //{
            //    Console.WriteLine( (char)outBuffer[i]);
            //}
            //SzDecode(db.PackSizes, folder,
                //#ifdef _LZMA_IN_CB
              //      inStream,
                //#else
                //*inBuffer, 
                //#endif
                //    outBuffer, unPackSize,
                  //  out outRealSize);
            //if (outRealSize != (uint)unPackSize)
            //{
             //   throw new Exception("OutRealSize != unPackSize " + outRealSize + " vs " + unPackSize);
            //}
            //if (folder.UnPackCRCDefined)
            //{
                // note to self: to do
                //if (!CrcVerifyDigest(folder->UnPackCRC, outBuffer->Items, (size_t)unPackSize))
                //{
                //  return SZE_FAIL;
                //}
            //}
        }

        void szReadAndDecodePackedStreams(
            FileStream inStream, SzData sd,
                out byte[] outBuffer, ulong baseOffset)
        {
            ArchiveDatabase db = new ArchiveDatabase();
            ulong[] unPackSizes;
            bool[] digestsDefined;
            uint[] digests;
            SzReadAndDecodePackedStreams2(inStream, sd,
                out outBuffer, baseOffset, db, out unPackSizes, out digestsDefined, out digests);
        }

        FileStream inStream;
        byte[] buffer = new byte[4096];
        int bytesread = 0;

        void szArchiveOpen2(FileStream inStream, out ArchiveDatabaseEx db)
        {
            this.inStream = inStream;
            db = new ArchiveDatabaseEx();
            byte[] signature = new byte[k7zSignatureSize];

            inStream.Read(signature, 0, k7zSignatureSize);
            bytesread += k7zSignatureSize;
            CheckSignature(signature);

            byte MajorVersion = ReadByte();
            CheckMajorVersion(MajorVersion);

            byte version = ReadByte();

            UInt32 crc = ReadUInt32();
            ulong nextHeaderOffset = Readulong();
            ulong nextHeaderSize = Readulong();
            UInt32 nextHeaderCRC = ReadUInt32();

            //Console.WriteLine("bytesread: " + bytesread);
            //Console.WriteLine("k7zStartHeaderSize: " + k7zStartHeaderSize);

            //Console.WriteLine("headeroffset: " + nextHeaderOffset);
            //Console.WriteLine("nextHeaderSize: " + nextHeaderSize);

            /* note to self: to do
            CrcInit(&crc);
            RINOK(SafeReadDirectUInt64(inStream, &nextHeaderOffset));
            CrcUpdateUInt64(&crc, nextHeaderOffset);
            RINOK(SafeReadDirectUInt64(inStream, &nextHeaderSize));
            CrcUpdateUInt64(&crc, nextHeaderSize);
            RINOK(SafeReadDirectUInt32(inStream, &nextHeaderCRC));
            CrcUpdateUInt32(&crc, nextHeaderCRC);
             */

            uint pos = k7zStartHeaderSize;
            db.ArchiveInfo.StartPositionAfterHeader = pos;

            //if (CrcGetDigest(&crc) != crcFromArchive)
            //return SZE_ARCHIVE_ERROR;

            if (nextHeaderSize == 0)
                return;

            inStream.Seek((long)(pos + nextHeaderOffset), SeekOrigin.Begin);
            byte[] thisheader = new byte[nextHeaderSize];
            inStream.Read(thisheader, 0, (int)nextHeaderSize);

            // note to self: to do
            // CrcVerifyDigest(nextHeaderCRC, buffer.Items, (UInt32)nextHeaderSize);

            SzData sd = new SzData();
            sd.Data = thisheader;
            sd.Length = nextHeaderSize;
            while (true)
            {
                //ulong thisheaderoffset = 0;
                // Console.WriteLine((int)EIdEnum.k7zIdHeader);
                //Console.WriteLine((int)EIdEnum.k7zIdEnd);
                EIdEnum type = szReadID(sd);
                //Console.WriteLine("szArchiveOpen2, got type: " + type);
                if (type == EIdEnum.k7zIdHeader)
                {
                    SzReadHeader(sd, db);
                    break;
                }
                if (type != EIdEnum.k7zIdEncodedHeader)
                {
                    throw new Exception("Error: invalid type number read: " + type);
                    //break;
                }
                byte[] outBuffer;
                //CSzByteBuffer outBuffer;
                szReadAndDecodePackedStreams(inStream, sd, out outBuffer,
                    db.ArchiveInfo.StartPositionAfterHeader);
                //res = SzReadAndDecodePackedStreams(inStream, &sd, &outBuffer, 
                //   db->ArchiveInfo.StartPositionAfterHeader, 
                //  allocTemp);
                thisheader = outBuffer;
                sd.Data = outBuffer;
                sd.Length = Convert.ToUInt64(outBuffer.GetLongLength(0) );
                sd.Offset = 0;
                //Console.WriteLine(sd.Length);
                //buffer.Items = outBuffer.Items;
                //buffer.Capacity = outBuffer.Capacity;
            }
        }

        public void szArchiveOpen(
            FileStream inStream,
            out ArchiveDatabaseEx db)
        {
            szArchiveOpen2(inStream, out db );
        }
    }
}
