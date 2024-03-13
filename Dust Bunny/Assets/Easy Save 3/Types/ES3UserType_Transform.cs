using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("position", "rotation", "localScale", "parent")]
	public class ES3UserType_Transform : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Transform() : base(typeof(UnityEngine.Transform)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Transform)obj;
			
			writer.WriteProperty("position", instance.position, ES3Type_Vector3.Instance);
			writer.WriteProperty("rotation", instance.rotation, ES3Type_Quaternion.Instance);
			writer.WriteProperty("localScale", instance.localScale, ES3Type_Vector3.Instance);
			writer.WritePropertyByRef("parent", instance.parent);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.Transform)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "position":
						instance.position = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "rotation":
						instance.rotation = reader.Read<UnityEngine.Quaternion>(ES3Type_Quaternion.Instance);
						break;
					case "localScale":
						instance.localScale = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "parent":
						instance.parent = reader.Read<UnityEngine.Transform>(ES3UserType_Transform.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_TransformArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TransformArray() : base(typeof(UnityEngine.Transform[]), ES3UserType_Transform.Instance)
		{
			Instance = this;
		}
	}
}