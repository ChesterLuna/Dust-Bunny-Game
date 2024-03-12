using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_isFacingRight")]
	public class ES3UserType_RidableEnemy : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_RidableEnemy() : base(typeof(RidableEnemy)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (RidableEnemy)obj;
			
			writer.WritePrivateField("_isFacingRight", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (RidableEnemy)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_isFacingRight":
					instance = (RidableEnemy)reader.SetPrivateField("_isFacingRight", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_RidableEnemyArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_RidableEnemyArray() : base(typeof(RidableEnemy[]), ES3UserType_RidableEnemy.Instance)
		{
			Instance = this;
		}
	}
}