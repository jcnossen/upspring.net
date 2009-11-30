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
    public class ArchiveDatabaseEx
    {
        public ArchiveDatabase Database = new ArchiveDatabase();
        public InArchiveInfo ArchiveInfo = new InArchiveInfo();
        public uint[] FolderStartPackStreamIndex;
        public ulong[] PackStreamStartPositions;
        public uint[] FolderStartFileIndex;
        public uint[] FileIndexToFolderIndexMap;
        //void SzArDbExInit(CArchiveDatabaseEx *db);
        //void SzArDbExFree(CArchiveDatabaseEx *db, void (*freeFunc)(void *));

        public void Fill()
        {
            uint startPos = 0;
            ulong startPosSize = 0;
            uint i;
            uint folderIndex = 0;
            uint indexInFolder = 0;
            FolderStartPackStreamIndex = new uint[Database.NumFolders];
            //RINOK(MySzInAlloc((void **)&db->FolderStartPackStreamIndex, db->Database.NumFolders * sizeof(UInt32), allocFunc));
            for (i = 0; i < Database.NumFolders; i++)
            {
                FolderStartPackStreamIndex[i] = startPos;
                startPos += Database.Folders[i].NumPackStreams;
            }

            PackStreamStartPositions = new ulong[Database.NumPackStreams];
            //RINOK(MySzInAlloc((void **)&db->PackStreamStartPositions, db->Database.NumPackStreams * sizeof(CFileSize), allocFunc));

            for (i = 0; i < Database.NumPackStreams; i++)
            {
                PackStreamStartPositions[i] = startPosSize;
                startPosSize += Database.PackSizes[i];
            }

            FolderStartFileIndex = new uint[Database.NumFolders];
            //RINOK(MySzInAlloc((void **)&db->FolderStartFileIndex, db->Database.NumFolders * sizeof(UInt32), allocFunc));
            FileIndexToFolderIndexMap = new uint[Database.NumFiles];
            //RINOK(MySzInAlloc((void **)&db->FileIndexToFolderIndexMap, db->Database.NumFiles * sizeof(UInt32), allocFunc));

            //Console.WriteLine("db.database.numfiles: " + Database.NumFiles);
            //Console.WriteLine("db.database.numfolders: " + Database.NumFolders);
            for (i = 0; i < Database.NumFiles; i++)
            {
                FileItem file = Database.Files[i];
                bool emptyStream = !file.HasStream;
                if (emptyStream && indexInFolder == 0)
                {
                    FileIndexToFolderIndexMap[i] = UInt32.MaxValue; // ???
                    continue;
                }
                if (indexInFolder == 0)
                {
                    /*
                    v3.13 incorrectly worked with empty folders
                    v4.07: Loop for skipping empty folders
                    */
                    while (true)
                    {
                        if (folderIndex >= Database.NumFolders)
                        {
                            throw new Exception("folderindex " + folderIndex + " higher than db.database.numfolders " + Database.NumFolders);
                        }
                        FolderStartFileIndex[folderIndex] = i;
                        if (Database.Folders[folderIndex].NumUnPackStreams != 0)
                            break;
                        folderIndex++;
                    }
                }
                FileIndexToFolderIndexMap[i] = folderIndex;
                if (emptyStream)
                    continue;
                indexInFolder++;
                if (indexInFolder >= Database.Folders[folderIndex].NumUnPackStreams)
                {
                    folderIndex++;
                    indexInFolder = 0;
                }
            }
        }

        public ulong GetFolderStreamPos( uint folderIndex, uint indexInFolder)
        {
            return ArchiveInfo.DataStartPosition +
            //return 20 +
              PackStreamStartPositions[FolderStartPackStreamIndex[folderIndex] + indexInFolder];
        }

        public ulong GetFolderFullPackSize( uint folderIndex)
        {
            uint packStreamIndex = FolderStartPackStreamIndex[folderIndex];
            Folder folder = Database.Folders[ folderIndex ];
            ulong size = 0;
            uint i;
            for (i = 0; i < folder.NumPackStreams; i++)
                size += Database.PackSizes[packStreamIndex + i];
            return size;
        }
    }

}
