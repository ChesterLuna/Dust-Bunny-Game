using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_state")]
	public class ES3UserType_Checkpoint : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Checkpoint() : base(typeof(Checkpoint)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Checkpoint)obj;
			
			writer.WritePrivateField("_state", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Checkpoint)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_state":
					instance = (Checkpoint)reader.SetPrivateField("_state", reader.Read<Checkpoint.CheckpointState>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_CheckpointArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CheckpointArray() : base(typeof(Checkpoint[]), ES3UserType_Checkpoint.Instance)
		{
			Instance = this;
		}
	}
}