
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace LaWare.Utility.Build
{
	public class FilesTaskBase : Task
	{

		[Required]
		public ITaskItem[] InputFiles { get; set; }

		[Output]
		public ITaskItem[] OutputFiles { get; set; }

		public override bool Execute()
		{
			// ReSharper disable once CoVariantArrayConversion
			OutputFiles = InputFiles.Select(item => new TaskItem(item.ItemSpec)).ToArray();
			return true;
		}
	}
	public class RegexUpdateFile : Task
	{
		[Required]
		public ITaskItem[] Files { get; set; }

		[Required]
		public string Regex { get; set; }

		[Required]
		public string ReplacementText { get; set; }

		public override bool Execute()
		{
			try
			{
				var regex = new Regex(Regex);
				var files = Files;
				foreach (var t in files)
				{
					var metadata = t.GetMetadata("FullPath");
					if (!File.Exists(metadata)) continue;
					var text = File.ReadAllText(metadata);
					text = regex.Replace(text, ReplacementText);
					File.WriteAllText(metadata, text);
					Log.LogMessage("RegexUpdateFile: " + metadata);
				}
				return true;
			}
			catch (Exception exception)
			{
				Log.LogErrorFromException(exception);
				return false;
			}
		}
	}
}
