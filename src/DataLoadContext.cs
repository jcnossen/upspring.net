using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ce.util.vfs;

namespace ce.engine
{
	/// <summary>
	/// Serves as a container for global manager objects
	/// </summary>
	public class DataLoadContext
	{
		// TODO: Make these properties?
		public FileSystem FileSystem;
		public TextureManager TextureManager;

		public static DataLoadContext CreateNew()
		{
				var dlc = new DataLoadContext();

				dlc.FileSystem = new FileSystem();
				dlc.TextureManager = new TextureManager(dlc.FileSystem);

				return dlc;
		}
	}
}
