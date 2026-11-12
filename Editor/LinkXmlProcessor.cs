using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace com.ktgame.services.datetime.editor
{
	internal sealed class LinkXmlProcessor : IUnityLinkerProcessor
	{
		private const string PackageName = "com.ktgame.services.datetime";

		public int callbackOrder => 0;

		public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
		{
			var packageInfo = PackageInfo.FindForAssembly(typeof(LinkXmlProcessor).Assembly);
			var packageRoot = packageInfo != null ? packageInfo.resolvedPath : Path.GetFullPath($"Packages/{PackageName}");
			var linkXml = Path.Combine(packageRoot, "Runtime", "link.xml");

			if (!File.Exists(linkXml))
			{
				Debug.LogError($"[{PackageName}] link.xml not found: {linkXml}");
				return null;
			}

			return linkXml;
		}
	}
}
