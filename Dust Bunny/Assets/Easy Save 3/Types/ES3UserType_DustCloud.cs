using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_amountOfDust")]
	public class ES3UserType_DustCloud : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_DustCloud() : base(typeof(DustCloud)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (DustCloud)obj;
			
			writer.WritePrivateField("_amountOfDust", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (DustCloud)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_amountOfDust":
					instance = (DustCloud)reader.SetPrivateField("_amountOfDust", reader.Read<System.Single>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_DustCloudArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DustCloudArray() : base(typeof(DustCloud[]), ES3UserType_DustCloud.Instance)
		{
			Instance = this;
		}
	}
}