using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ce.util.vfs
{
	/// <summary>
	/// Exposes a single physical file to the virtual file system
	/// </summary>
	public class SingleFile : IArchive
	{
		string filePath, vfsName;

		public SingleFile(string filePath, string vfsName)
		{
			this.filePath = filePath;
			this.vfsName = vfsName;
		}

		#region IArchive Members

		public VFSItem[] GetItems()
		{
			return new VFSItem[] {
				new VFSItem() { archiveIndex =0, name=vfsName, size= new FileInfo(filePath).Length }
			};
		}

		public System.IO.Stream ReadFile(VFSItem fileItem)
		{
			return File.OpenRead(filePath);
		}

		public string PhysicalPathDescription(VFSItem item)
		{
			return filePath;
		}

		#endregion
	}
}
