using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_currentDust", "Stats")]
	public class ES3UserType_PlayerController : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PlayerController() : base(typeof(PlayerController)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (PlayerController)obj;
			
			writer.WritePrivateField("_currentDust", instance);
			writer.WritePrivatePropertyByRef("Stats", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (PlayerController)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_currentDust":
					instance = (PlayerController)reader.SetPrivateField("_currentDust", reader.Read<System.Single>(), instance);
					break;
					case "Stats":
					instance = (PlayerController)reader.SetPrivateProperty("Stats", reader.Read<PlayerStats>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_PlayerControllerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PlayerControllerArray() : base(typeof(PlayerController[]), ES3UserType_PlayerController.Instance)
		{
			Instance = this;
		}
	}
}