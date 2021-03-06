using Nito.AsyncEx;
using System;

namespace BuddySDK
{
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public abstract partial class PlatformAccess
	{
		internal const string BuddyPushKey = "_bId";
		private int? _uiThreadId;
	 
		// device info
		//
		public abstract string Platform {get;}
		public abstract AsyncLazy<string> Model {get;}
		public abstract string DeviceUniqueId { get;}
		public abstract AsyncLazy<string> OSVersion { get;}
		public abstract bool   IsEmulator { get; }
		public abstract string ApplicationID {get;}
		public abstract string AppVersion {get;}

		public abstract ConnectivityLevel ConnectionType {get;}

		
		// TODO: Connection speed?

		private int _activity = 0;
		public bool ShowActivity {
			get {
				return _activity > 0;
			}
			set {
				SetActivityInternal (value);
			}
		}

		protected PlatformAccess() {

			InvokeOnUiThread (() => {
				_uiThreadId = DotNetDeltas.CurrentThreadId;
			});
		}

		public abstract bool SupportsFlags(BuddyClientFlags flags);

		protected virtual void OnShowActivity(bool show) {

		}

		private void SetActivityInternal(bool isActive) {
			bool wasActive = ShowActivity;

			if (isActive) {
				_activity++;
			} else if (_activity > 0) {
				_activity--;
			}

			if (ShowActivity != wasActive) {
				OnShowActivity (ShowActivity);
			}
		}

		private const string UserSettingExpireEncodeDelimiter = "\t";

		internal static string EncodeUserSetting(string value, DateTime? expires = default(DateTime?))
		{
			var dt = expires.GetValueOrDefault (new DateTime (0)); // TODO: why both default(DateTime?) & new DateTime (0)?

			return String.Format ("{0}{1}{2}", dt.Ticks, UserSettingExpireEncodeDelimiter, value);
		}

		internal static string DecodeUserSetting(string value)
		{
			if (string.IsNullOrEmpty (value)) {
				return null;
			}

			var tabIndex = value.IndexOf (UserSettingExpireEncodeDelimiter);

			if (tabIndex == -1)
			{
				return null;
			}

			var ticks = Int64.Parse (value.Substring (0, tabIndex));

			if (ticks > 0 && new DateTime(ticks) < DateTime.UtcNow) {
				return null;
			}

			return value.Substring (tabIndex + 1);
		}

		// settings
		public abstract string GetConfigSetting(string key);

		public abstract void SetUserSetting(string key, string value, DateTime? expires = null);
		public abstract string GetUserSetting(string key);

		public abstract void ClearUserSetting (string str);

		// Crypto
		public string SignString(string key, string stringToSign)
		{
			return DotNetDeltas.SignString(key, stringToSign);
		}

		// platform
		//

        public virtual bool IsUiThread
        {
            get
            {
                return DotNetDeltas.CurrentThreadId == _uiThreadId.GetValueOrDefault();
            }
        }

		protected abstract void InvokeOnUiThreadCore (Action a);

        public void InvokeOnUiThread(Action a)
        {
            if (IsUiThread)
            {
                a();
            }
            else
            {
                InvokeOnUiThreadCore(a);
            }
        }

		internal string PushToken { get; set; }

		public string GetPushToken()
		{
			if (PushToken == null)
			{
				PushToken = GetUserSetting("__PushToken");
			}

			return PushToken;
		}

		public event EventHandler PushTokenChanged;

		public void SetPushToken(string pushToken)
		{
			//because the MPNS channel may be updated before the initial call to POST /devices, this was getting nulled out non-deterministically from BuddyClient:240 before we could attach it to the device
			if (pushToken != null) {
				if (GetPushToken() != pushToken) {
					SetUserSetting ("__PushToken", pushToken);
					PushToken = null;
				}

				if (PushTokenChanged != null) {
					PushTokenChanged (this, EventArgs.Empty);
				}
			}
		}

		public class NotificationReceivedEventArgs
		{
			public string ID { get; set; }
		}

		internal event EventHandler<NotificationReceivedEventArgs> NotificationReceived;

		internal void OnNotificationReceived(string id)
		{
			if (NotificationReceived != null && !String.IsNullOrEmpty(id))
			{
				NotificationReceived(this, new NotificationReceivedEventArgs { ID = id });
			}
		}

		static PlatformAccess _current;

		public static PlatformAccess Current
		{
			get
			{
				if (_current == null)
				{
					// this is implemented in the derived per-platform files.
					_current = CreatePlatformAccess();
				}
				return _current;
			}
		}
	}
}