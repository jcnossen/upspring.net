using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace ce.util.vfs
{
	public class VFSItem
	{
		public long size;
		public string name;
		public int archiveIndex;
	}

	public interface IArchive
	{
		VFSItem[] GetItems();
		Stream ReadFile(VFSItem fileItem);
		string PhysicalPathDescription(VFSItem item);
	}

	public interface IArchiveType
	{
		string[] GetExtensions();
		IArchive OpenArchive(string filename);
	}

	public class FileSystem
	{
		List<IArchive> archives = new List<IArchive>();
		List<IArchiveType> types = new List<IArchiveType>();
		Dictionary<string, IArchiveType> extMap = new Dictionary<string, IArchiveType>();
		Dictionary<string, File> files = new Dictionary<string, File>();
		Directory rootDir;

		public class Directory
		{
			public Directory(Directory parent)
			{
				this.parent = parent;
			}
			public string Path
			{
				get { return (parent != null ? parent.Path : "") + name + "/"; }
			}
			public Directory parent;
			public string name;
			public List<File> files = new List<File>();
			public List<Directory> subdirs = new List<Directory>();
			public override string ToString() { return name; }
		}

		public class File
		{
			public VFSItem item;
			public IArchive archive;
			public string Name { get { return item.name; } }
			public long Size { get { return item.size; } }
		}

		public Directory RootDirectory
		{
			get { return rootDir; }
		}

		public FileSystem()
		{
			AddArchiveType(new SevenZipReader());
			rootDir = new Directory(null);
		}

		public Stream OpenRead(string name)
		{
			File f;
			name = name.ToLower();
			if (files.TryGetValue(name, out f))
			{
				if (f.archive == null)
					return new FileStream(name, FileMode.Open, FileAccess.Read);

				Trace.WriteLine("Reading " + name);
				return f.archive.ReadFile(f.item);
			}
			throw new FileNotFoundException("File not found in VFS", name);
		}

		public void AddArchiveType(IArchiveType type)
		{
			types.Add(type);

			foreach (string ext in type.GetExtensions())
				extMap[ext] = type;
		}

		public void AddDirectory(string dir)
		{
			AddArchive(new DirectoryReader(dir, null, true));
		}
		public void AddDirectory(string dir, bool subdirs)
		{
			AddArchive(new DirectoryReader(dir, null, subdirs));
		}


		/// <summary>
		/// Add files from a directory under a virtual path:
		/// IE:
		///  With file "/a/file"
		///  AddDirectory("/a", "b") adds file as "b/file" to the file system
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="virtualPath"></param>
		public void AddDirectory(string dir, string virtualPath)
		{
			AddArchive(new DirectoryReader(dir, virtualPath, true));
		}

		public void AddArchive(string filename)
		{
			string ext = Path.GetExtension(filename);
			ext = ext.TrimStart('.');			

			if (!extMap.ContainsKey(ext))
				throw new Exception("Unknown archive type: " + ext);

			IArchiveType type = extMap[ext];

			AddArchive(type.OpenArchive(filename));
		}

		public void AddArchive(IArchive ar)
		{
			foreach (VFSItem i in ar.GetItems()) {
	//			Trace.WriteLine("FS: Added file: " + i.name);

				string filename = i.name.ToLower();
				string[] dirs = filename.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				Directory dir = RegisterDirectory (dirs.Take(dirs.Length-1).ToArray());

				File file = new File() { item = i, archive = ar };

				files[filename] = file;
				dir.files.Add(file);
			}
		}

		private Directory RegisterDirectory(string[] dir)
		{
			int x = 0;
			Directory d = rootDir;
			while (x < dir.Length) {
				Directory subdir = d.subdirs.Find(u => u.name == dir[x]);
				if (subdir == null) {
					subdir = new Directory(d) { name = dir[x] };
					d.subdirs.Add(subdir);
				}
				d = subdir;
				x++;
			}
			return d;
		}

		public Directory GetDirectory(string dir)
		{
			string[] parts = dir.Split('/');
			Directory d = rootDir;
			for (int x = 0; x < parts.Length && d != null; x++ )
				d = d.subdirs.Find(u => u.name == parts[x]);
			return d;
		}

		public byte[] ReadAllBytes(string filename)
		{
			using (Stream s = OpenRead(filename)) {
				long l = s.Length;
				byte[] data = new byte[l];
				s.Read(data, 0, (int)l);
				return data;
			}
		}

		public string ReadAllText(string filename)
		{
			byte[] data = ReadAllBytes(filename);
			return Encoding.UTF8.GetString(data);
		}

		public bool FileExists(string file)
		{
			return files.ContainsKey(file);
		}

		/// <summary>
		/// Returns a physical path, or a description of a physical location of the file
		/// </summary>
		public string PhysicalPath(string filename)
		{
			filename = filename.ToLower();
			
			File file;
			if (files.TryGetValue(filename, out file))
				return file.archive.PhysicalPathDescription(file.item);

			throw new FileNotFoundException("File " + filename + " not found");
		}
	}
}
