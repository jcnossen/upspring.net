using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ce.util.vfs
{
	public class DirectoryReader : IArchive
	{
		string dir;

		class Item : VFSItem {
			public string realpath;
		}
		Item[] items;

		public DirectoryReader(string dir, string virtualPath, bool recurseSubDirs)
		{
			this.dir = dir;

			if (!Directory.Exists(dir)) {
				this.items = new Item[] {};
				return;
			}

			var files = Directory.GetFiles(dir, "*", recurseSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

			if (virtualPath == null)
				virtualPath = "";
			else if (!virtualPath.EndsWith("/"))
				virtualPath += "/";

			List<Item> items = new List<Item>();
			
			foreach (string orgFile in files) {
				FileInfo info = new FileInfo(orgFile);
				string file = orgFile.Replace('\\', '/');

				// Ignore hidden files
				if ((info.Attributes & FileAttributes.Hidden) != 0)
					continue;

				if (file.Split('/').Any(s => s.StartsWith(".")))
					continue;

				string subPath = orgFile.Replace(dir, "");
				string r = subPath.Replace('\\','/').TrimStart('/');

				items.Add(new Item()
				{
					name = virtualPath + r,
					realpath = orgFile,
					size = info.Length
				});
			}
			this.items = items.ToArray();
		}

		#region IArchive Members

		public VFSItem[] GetItems()
		{
			return Array.ConvertAll(items, i => (VFSItem)i);
		}

		public System.IO.Stream ReadFile(VFSItem fileItem)
		{
			return File.OpenRead(((Item)fileItem).realpath);
		}

		public string PhysicalPathDescription(VFSItem item)
		{
			return ((Item)item).realpath;
		}

		#endregion
	}
}
