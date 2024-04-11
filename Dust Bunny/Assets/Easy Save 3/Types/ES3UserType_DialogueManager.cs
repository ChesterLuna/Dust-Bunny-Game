using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_isFinishedDialogue")]
	public class ES3UserType_DialogueManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_DialogueManager() : base(typeof(DialogueManager)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (DialogueManager)obj;
			
			writer.WritePrivateField("_isFinishedDialogue", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (DialogueManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_isFinishedDialogue":
					instance = (DialogueManager)reader.SetPrivateField("_isFinishedDialogue", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_DialogueManagerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DialogueManagerArray() : base(typeof(DialogueManager[]), ES3UserType_DialogueManager.Instance)
		{
			Instance = this;
		}
	}
}