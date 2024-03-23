using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_isOn")]
	public class ES3UserType_Switch : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Switch() : base(typeof(Switch)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Switch)obj;
			
			writer.WritePrivateField("_isOn", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Switch)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_isOn":
					instance = (Switch)reader.SetPrivateField("_isOn", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_SwitchArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SwitchArray() : base(typeof(Switch[]), ES3UserType_Switch.Instance)
		{
			Instance = this;
		}
	}
}