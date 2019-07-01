using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer
{
	public abstract class ExternalModule : IDisposable
	{
		public abstract void Run();

		~ExternalModule()
		{
			Dispose();
		}

		public abstract void OnDispose();

		public void Dispose()
		{
			OnDispose();
		}
	}
}
