using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_collider", "_state", "_animator", "_particleSystemObject")]
	public class ES3UserType_Checkpoint : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Checkpoint() : base(typeof(Checkpoint)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Checkpoint)obj;
			
			writer.WritePrivateFieldByRef("_collider", instance);
			writer.WritePrivateField("_state", instance);
			writer.WritePrivateFieldByRef("_animator", instance);
			writer.WritePrivateFieldByRef("_particleSystemObject", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Checkpoint)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_collider":
					instance = (Checkpoint)reader.SetPrivateField("_collider", reader.Read<UnityEngine.Collider2D>(), instance);
					break;
					case "_state":
					instance = (Checkpoint)reader.SetPrivateField("_state", reader.Read<Checkpoint.CheckpointState>(), instance);
					break;
					case "_animator":
					instance = (Checkpoint)reader.SetPrivateField("_animator", reader.Read<UnityEngine.Animator>(), instance);
					break;
					case "_particleSystemObject":
					instance = (Checkpoint)reader.SetPrivateField("_particleSystemObject", reader.Read<UnityEngine.GameObject>(), instance);
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