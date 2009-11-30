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

namespace SevenZip
{
    public class SzExtract
    {
        public void Extract(
            FileStream inStream,
            ArchiveDatabaseEx db,
            uint fileIndex,
            out uint blockIndex,
            out byte[] outBuffer,
            out ulong outBufferSize,
            ref ulong offset,
            ref ulong outSizeProcessed)
        {
            //Console.WriteLine("Extract >>>");
            uint folderIndex = db.FileIndexToFolderIndexMap[fileIndex];
            offset = 0;
            outSizeProcessed = 0;
            outBuffer = null;
            if (folderIndex == UInt32.MaxValue)
            {
                //Console.WriteLine("folderindex is UInt32.MaxValue");
                blockIndex = folderIndex;
                outBuffer = null;
                outBufferSize = 0;
                return;
            }

            Folder folder = db.Database.Folders[folderIndex];
            ulong unPackSize = folder.GetUnPackSize();
            //#ifndef _LZMA_IN_CB
            ulong packSize = db.GetFolderFullPackSize(folderIndex);
            //Console.WriteLine("packsize: " + packSize + " unpacksize: " + unPackSize );
            //Byte *inBuffer = 0;
            //size_t processedSize;
            //#endif
            blockIndex = folderIndex;
            outBuffer = null;

            //Console.WriteLine("folderstreampos: " + db.GetFolderStreamPos(folderIndex, 0));
            inStream.Seek((long)db.GetFolderStreamPos(folderIndex, 0), SeekOrigin.Begin);
            //RINOK(inStream->Seek(inStream, SzArDbGetFolderStreamPos(db, folderIndex, 0)));

            //#ifndef _LZMA_IN_CB
            //if (packSize != 0)
            //{
            // inBuffer = (Byte *)allocTemp->Alloc((size_t)packSize);
            // if (inBuffer == 0)
            //   return SZE_OUTOFMEMORY;
            // }
            // res = inStream->Read(inStream, inBuffer, (size_t)packSize, &processedSize);
            // if (res == SZ_OK && processedSize != (size_t)packSize)
            //  res = SZE_FAIL;
            //#endif
            outBufferSize = unPackSize;
            if (unPackSize != 0)
            {
                outBuffer = new byte[unPackSize];
                //*outBuffer = (Byte *)allocMain->Alloc((size_t)unPackSize);
                //if (*outBuffer == 0)
                //res = SZE_OUTOFMEMORY;
            }
            ulong outRealSize;
            Compression.LZMA.Decoder decoder = new Compression.LZMA.Decoder();
            decoder.SetDecoderProperties(folder.Coders[0].Properties);
            Stream outStream = new MemoryStream(outBuffer);
            decoder.Code(inStream, outStream, (long)packSize, (long)unPackSize, null);
            //SzDecode(db.Database.PackSizes +
            // db.FolderStartPackStreamIndex[folderIndex], folder,
            //  //#ifdef _LZMA_IN_CB
            // inStream,
            // //#else
            // //inBuffer,
            //   //#endif
            //outBuffer, unPackSize, out outRealSize);
            outStream.Close();
            //Console.WriteLine( Encoding.UTF8.GetString( outBuffer, 0, (int)unPackSize ) );
            outRealSize = unPackSize;
            if (outRealSize == unPackSize)
            {
                //if (folde.UnPackCRCDefined)
                //{
                // if (!CrcVerifyDigest(folder->UnPackCRC, *outBuffer, (size_t)unPackSize))
                //  res = SZE_FAIL;
                //}
            }
            else
            {
                throw new Exception("Unpack size was different from packsize: " + unPackSize + " vs " + outRealSize);
                //res = SZE_FAIL;
            }
            UInt32 i;
            FileItem fileItem = db.Database.Files[fileIndex];
            offset = 0;
            for (i = db.FolderStartFileIndex[folderIndex]; i < fileIndex; i++)
            {
                offset += (UInt32)db.Database.Files[i].Size;
            }
            outSizeProcessed = fileItem.Size;
            if (offset + outSizeProcessed > outBufferSize)
            {
                throw new Exception("offset + outsizeprocessed > outbuffersize " + offset + " + " + outSizeProcessed + " > " + outBufferSize);
                // return SZE_FAIL;
            }
            //if (fileItem->IsFileCRCDefined)
            //{
            //   if (!CrcVerifyDigest(fileItem->FileCRC, *outBuffer + *offset, *outSizeProcessed))
            //      res = SZE_FAIL;
            //}
        }
    }
}

