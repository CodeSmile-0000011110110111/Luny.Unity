using Luny.Unity.Engine;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor_Editor = UnityEditor.Editor;

namespace Luny.UnityEditor
{
	[CustomEditor(typeof(Note))]
	public class NoteEditor : UnityEditor_Editor
	{
		private static String GetNoteTitle(HelpBoxMessageType messageType)
		{
			switch (messageType)
			{
				case HelpBoxMessageType.None:
				case HelpBoxMessageType.Info:
					return "Info";
				case HelpBoxMessageType.Warning:
					return "CAUTION";
				case HelpBoxMessageType.Error:
					return "STRENG VERBOTEN!";
				default:
					throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
			}
		}

		private static void SetHeaderProperties(HelpBox header, Note note)
		{
			header.messageType = note.Kind;
			header.text = GetNoteTitle(note.Kind);
			header.style.display = note.Kind != HelpBoxMessageType.None ? DisplayStyle.Flex : DisplayStyle.None;
		}

		public override VisualElement CreateInspectorGUI()
		{
			var note = (Note)target;
			var root = new VisualElement();
			root.style.paddingTop = 10;
			root.style.paddingBottom = 10;

			var header = new HelpBox(GetNoteTitle(note.Kind), note.Kind);
			header.style.unityFontStyleAndWeight = FontStyle.Bold;
			header.style.fontSize = 20;
			SetHeaderProperties(header, note);

			// Title text field
			// var titleField = new TextField();
			// titleField.bindingPath = nameof(Note.Title);

			// Note text field
			var textField = new TextField();
			textField.bindingPath = nameof(Note.Text);
			textField.multiline = true;
			//textField.style.marginTop = 5;
			textField.style.whiteSpace = WhiteSpace.Normal; // word wrap
			textField.style.flexGrow = 1f;
			// bigger font
			textField.RegisterCallback<AttachToPanelEvent>(evt => textField.style.fontSize = textField.resolvedStyle.fontSize);

			var typeField = new EnumField("Kind", note.Kind);
			typeField.bindingPath = nameof(Note.Kind);

			// titleField.value = note.Title;
			// titleField.RegisterValueChangedCallback(evt =>
			// {
			// 	note.Title = evt.newValue;
			// 	EditorUtility.SetDirty(note);
			// });
			textField.value = note.Text;
			textField.RegisterValueChangedCallback(evt =>
			{
				note.Text = evt.newValue;
				EditorUtility.SetDirty(note);
			});
			typeField.RegisterValueChangedCallback(evt =>
			{
				note.Kind = (HelpBoxMessageType)evt.newValue;
				SetHeaderProperties(header, note);
				EditorUtility.SetDirty(note);
			});

			root.Add(header);
			//root.Add(titleField);
			root.Add(textField);
			root.Add(typeField);

			return root;
		}
	}
}
