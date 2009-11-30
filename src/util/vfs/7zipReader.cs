using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ce.util.vfs
{
	using SevenZip;

	class SevenZipReader : IArchiveType
	{
		#region IArchiveType Members

		public string[] GetExtensions()
		{
			return new string[] { "7z", "sd7" };
		}

		class Archive : IArchive
		{
			public ArchiveDatabaseEx db;
			public string filename;

			public VFSItem[] GetItems()
			{
				List<VFSItem> items = new List<VFSItem>(db.Database.Files.Length);
				for (int i=0;i<db.Database.Files.Length;i++) {
					FileItem item = db.Database.Files[i];
					if (item.IsDirectory)
						continue;

					items.Add(new VFSItem() {
						archiveIndex = i,
						name = item.Name.TrimEnd('\0').Replace('\\', '/'),
						size = (int)item.Size
					});
				}
				return items.ToArray();
			}

			public Stream ReadFile(VFSItem item)
			{
				FileItem file = db.Database.Files[item.archiveIndex];
				using (FileStream fs = File.OpenRead(filename)) {
					uint blockindex;
					byte[] outbuffer;
					ulong outbuffersize;
					ulong offset = 0;
					ulong outsizeprocessed = 0;

					new SevenZip.SzExtract().Extract(fs, db,(uint) item.archiveIndex, out blockindex,
							out outbuffer, out outbuffersize, ref offset, ref outsizeprocessed);

					return new MemoryStream(outbuffer);
				}
			}

			public string PhysicalPathDescription(VFSItem item)
			{
				return String.Format("7zip:{0}/{1}", filename, item.name);
			}
		}

		public IArchive OpenArchive(string filename)
		{
			FileStream inStream = File.OpenRead(filename);

			ArchiveDatabaseEx db;
			new SzIn().szArchiveOpen(inStream, out db);

			return new Archive() { db = db, filename = filename };
		}

		#endregion
	}
}
