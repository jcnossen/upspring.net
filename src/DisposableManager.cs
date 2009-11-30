using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ce.engine
{
	public abstract class GfxDisposable : IDisposable
	{
		public GfxDisposable() {
			GfxDisposableManager.Add(this);
		}

		public abstract void GfxDispose();

		public void Dispose()
		{
			Dispose(true);
		}

		public void Dispose(bool remove)
		{
			GfxDispose();
			if (remove)
				GfxDisposableManager.Remove(this);

			GC.SuppressFinalize(this);
		}
	}

	public static class GfxDisposableManager
	{
		static HashSet<GfxDisposable> disposables = new HashSet<GfxDisposable>();

		public static void DisposeAll()
		{
			foreach (GfxDisposable d in disposables.ToArray())
				d.Dispose(false);
			disposables = new HashSet<GfxDisposable>();
		}

		public static void Add(GfxDisposable d)
		{
			disposables.Add(d);
		}

		public static void Remove(GfxDisposable d)
		{
			disposables.Remove(d);
		}
	}
}
