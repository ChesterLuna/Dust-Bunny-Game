using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_bunnySize", "_dust", "IsFacingRight")]
	public class ES3UserType_PlayerController : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PlayerController() : base(typeof(PlayerController)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (PlayerController)obj;
			
			writer.WritePrivateField("_bunnySize", instance);
			writer.WritePrivateField("_dust", instance);
			writer.WriteProperty("IsFacingRight", instance.IsFacingRight, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (PlayerController)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_bunnySize":
					instance = (PlayerController)reader.SetPrivateField("_bunnySize", reader.Read<System.Int32>(), instance);
					break;
					case "_dust":
					instance = (PlayerController)reader.SetPrivateField("_dust", reader.Read<System.Single>(), instance);
					break;
					case "IsFacingRight":
						instance.IsFacingRight = reader.Read<System.Boolean>(ES3Type_bool.Instance);
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