using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_currentPatrolPoint", "_position", "_positionHasValue", "_isFacingRight")]
	public class ES3UserType_EnemyMovement : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_EnemyMovement() : base(typeof(EnemyMovement)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (EnemyMovement)obj;
			
			writer.WritePrivateField("_currentPatrolPoint", instance);
			writer.WritePrivateField("_position", instance);
			writer.WritePrivateField("_positionHasValue", instance);
			writer.WritePrivateField("_isFacingRight", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (EnemyMovement)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_currentPatrolPoint":
					instance = (EnemyMovement)reader.SetPrivateField("_currentPatrolPoint", reader.Read<System.Int32>(), instance);
					break;
					case "_position":
					instance = (EnemyMovement)reader.SetPrivateField("_position", reader.Read<UnityEngine.Vector3>(), instance);
					break;
					case "_positionHasValue":
					instance = (EnemyMovement)reader.SetPrivateField("_positionHasValue", reader.Read<System.Boolean>(), instance);
					break;
					case "_isFacingRight":
					instance = (EnemyMovement)reader.SetPrivateField("_isFacingRight", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_EnemyMovementArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_EnemyMovementArray() : base(typeof(EnemyMovement[]), ES3UserType_EnemyMovement.Instance)
		{
			Instance = this;
		}
	}
}