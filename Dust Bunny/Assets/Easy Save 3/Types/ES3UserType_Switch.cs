using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_targetGameObjects", "_targets", "_isOn", "_onSprite", "_offSprite")]
	public class ES3UserType_Switch : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Switch() : base(typeof(Switch)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Switch)obj;
			
			writer.WritePrivateField("_targetGameObjects", instance);
			writer.WritePrivateField("_targets", instance);
			writer.WritePrivateField("_isOn", instance);
			writer.WritePrivateFieldByRef("_onSprite", instance);
			writer.WritePrivateFieldByRef("_offSprite", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Switch)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_targetGameObjects":
					instance = (Switch)reader.SetPrivateField("_targetGameObjects", reader.Read<UnityEngine.GameObject[]>(), instance);
					break;
					case "_targets":
					instance = (Switch)reader.SetPrivateField("_targets", reader.Read<System.Collections.Generic.List<ISwitchable>>(), instance);
					break;
					case "_isOn":
					instance = (Switch)reader.SetPrivateField("_isOn", reader.Read<System.Boolean>(), instance);
					break;
					case "_onSprite":
					instance = (Switch)reader.SetPrivateField("_onSprite", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "_offSprite":
					instance = (Switch)reader.SetPrivateField("_offSprite", reader.Read<UnityEngine.Sprite>(), instance);
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