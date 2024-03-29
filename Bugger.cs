// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RMX
{

	public class Testing {
		
		public static string Misc = "Misc";
		public static string GameCenter = "GameCenter";
		public static string Achievements = "Achievements";
		public static string Exceptions = "Exceptions";
		
		public static string Singletons = "Singletons";
		public static string Patches = "Patches";
		public static string Database = "Database";
		public static string EventCenter = "EventCenter";
	}


	public class Bugger : Singletons.ASingleton<Bugger>
	{
		public struct Log {
			public Log(string feature, string message) {
				this.feature = feature;
				this.message = message;
			}
			public string feature;
			public string message;

			public bool isEmpty {
				get {
					return this.message == "" || this.message == null;
				}
			}

			public bool isActive {
				get {
					return false;
				}
			}

			private string color {
				get {
					if (this.feature == Testing.Exceptions)
						return "red";
					else if (this.feature == Testing.GameCenter)
						return "yellow";
					else if (this.feature == Testing.Patches)
						return "green";
					else
						return "blue";
				}	
			}
			
			
		
			public override string ToString ()
			{
				string header = "<color=" + color + ">" + this.feature + ": </color>\n";
				var result = TextFormatter.Format( header + this.message);
				if (Singletons.SettingsInitialized && Singletons.Settings.PrintToScreen)
					Bugger.current.Queue.Add(result);
				return result;
			}
		}

		public List<string> Queue = new List<string>();
		private static List<Log> _lateLogs = new List<Log> ();
	
		bool _setupComplete = false;
		protected override bool SetupComplete {
			get {
				return _setupComplete;
			}
		}


		void Start() {
			if (!Singletons.SettingsInitialized)
				Debug.LogError ("Setting MUST be initialized before Bugger");
			_setupComplete = true;

			foreach (Log log in _lateLogs) {
				try {
					if (settings.IsDebugging(log.feature)) {
						var message = TextFormatter.Format( log.feature + ": _LATE_ " + log.message);
						Debug.Log(message);
					}
				} catch (Exception e) {
					Debug.LogWarning(e.Message);
				}
			}
			_lateLogs.Clear ();
			_lateLogs = null;

		}




		Log _log;
		public static string Last {
			get {
				if (IsInitialized)
					if (!current._log.isEmpty)
						return current._log.ToString();
					else
						throw new NullReferenceException ("_log should not be null!");
				else
					throw new Exception ("Bugger must be initialized before accessing Last log.");
			}
		}

		 static void AddLateLog(string feature, string message) {
			if (_lateLogs == null)
				throw new Exception ("Late Log Was Added too Late! - " + feature + "\n " + message);
			else		
				_lateLogs.Add (new Log (feature, message));

		}

		public static Log StartNewLog(string feature) {
			return StartNewLog (feature, "");
		}

		public static Log StartNewLog(string feature, string message) {
			if (IsInitialized) {
				return new Log (feature, message);
			} else
				throw new Exception ("Bugger must be initialized before StartNewLog(string feature, string message). Log is: \n" + feature + ": " + message);

		}
	


		public static bool WillLog(string feature, string message) {
			if (IsInitialized && Singletons.Settings != null) {
				if (Singletons.Settings.IsDebugging (feature)) {
					current._log = new Log (feature, message);
					return true;
				} else {
					return false;
				}
			} else 
				AddLateLog (feature, message);
			return false;
		}




		private bool timesUp {
			get{ 
				return settings != null && settings.PrintToScreen && Queue.Count > 0 && Time.fixedTime - _startedAt > settings.MaxDisplayTime;
			}
		}

		private int timeRemaining {
			get {
				return settings != null ? (int) (settings.MaxDisplayTime - (Time.fixedTime - _startedAt)) : 1;
			}
		}

		void Update() {
			if (timesUp) {
				Queue.RemoveAt(0);
				_startedAt = Time.fixedTime;
			}
		}
		private float _startedAt = 0;

		public void AddToQueue(string log) {
			if (Queue.Count == 0)
				_startedAt = Time.fixedTime;
			if (!Queue.Exists( val =>  {
				return val == log;
			}))
				Queue.Add (log);
		}

		void OnGUI () {
			if (settings != null && settings.PrintToScreen && Queue.Count > 0) {
				var text = timeRemaining + " – " + Queue[0];
				GUIStyle style = new GUIStyle ();
//				style.fontSize = 50;
				style.richText = true;
				style.wordWrap = true;
				style.alignment = TextAnchor.LowerLeft;
				style.padding.left = style.padding.right = style.padding.top = style.padding.bottom = 20;
//				style.border
				GUI.Label (new Rect (0, 0, Screen.width, Screen.height), text, style);
			}
		}


	}


}

