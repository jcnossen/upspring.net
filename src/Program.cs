using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;

using Tao.DevIl;

using ce.engine;

using ce.util.vfs;

namespace upspring
{
	static class Program
	{
		public static string BinariesPath;
		public static string DataPath;

		public static FileSystem FileSystem;
		public static DataLoadContext DataLoadContext;

		static MainForm mainForm;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			BinariesPath = Directory.GetCurrentDirectory();
			DataPath = new DirectoryInfo (BinariesPath).Parent.FullName + "/data/";
			
			Il.ilInit();
			Ilu.iluInit();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			mainForm = new MainForm();

			DataLoadContext = ce.engine.DataLoadContext.CreateNew();
			FileSystem = DataLoadContext.FileSystem;
			FileSystem.AddDirectory(DataPath + "textures", "textures");

			Application.Run(mainForm);
			GfxDisposableManager.DisposeAll();

			Il.ilShutDown();
		}
	}
}
