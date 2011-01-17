﻿namespace Raven.Tests.Silverlight.UnitTestProvider.Example
{
	using System;
	using System.Diagnostics;
	using System.Windows;
	using System.Windows.Browser;
	using Microsoft.Silverlight.Testing;
	using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

	public partial class App : Application
	{
		public App()
		{
			Startup += Application_Startup;
			UnhandledException += Application_UnhandledException;

			InitializeComponent();
		}

		void Application_Startup(object sender, StartupEventArgs e)
		{
			UnitTestProviders.Providers.Clear();
			UnitTestSystem.RegisterUnitTestProvider(new RavenCustomProvider());

			RootVisual = UnitTestSystem.CreateTestPage();
		}


		void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// If the app is running outside of the debugger then report the exception using
			// the browser's exception mechanism. On IE this will display it a yellow alert 
			// icon in the status bar and Firefox will display a script error.
			if (!Debugger.IsAttached)
			{
				// NOTE: This will allow the application to continue running after an exception has been thrown
				// but not handled. 
				// For production applications this error handling should be replaced with something that will 
				// report the error to the website and stop the application.
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
			}
		}

		void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch (Exception)
			{
			}
		}
	}
}