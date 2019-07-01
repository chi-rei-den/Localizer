using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Localizer;

namespace LocalizerWPF
{
	public class Module : ExternalModule
	{
		private Thread _thread;

		public override void Run()
		{
			_thread = new Thread(
			() =>
			{
				var app = new App();
				app.InitializeComponent();
				app.Run();
			});
			_thread.SetApartmentState(ApartmentState.STA);
			_thread.IsBackground = true;
			_thread.Start();
		}
		public override void OnDispose()
		{
		}
	}
}
