using System.Windows;

namespace Genguid.FactoryObservers
{
	/// <summary>
	/// A factory observer for writing generated GUIDs to the Windows clipboard.
	/// </summary>
	internal class ClipboardWriter : IGuidFactoryObserver
	{
		/// <summary>
		/// Notifies the clipboard writer of a newly generated GUID.
		/// </summary>
		/// <param name="packet">
		/// A packet containing the information to append to the log.
		/// </param>
		public void NotifyOfGeneratedGuid(GuidPacket packet)
		{
			Clipboard.SetText(packet.FormattedValue);
		}
	}
}
