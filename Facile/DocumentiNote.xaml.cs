using System;
using System.Collections.Generic;
using Facile.Interfaces;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DocumentiNote : ContentPage
	{
		private DocumentiEdit _parent;
		private bool change_event;
		private readonly SQLiteAsyncConnection dbcon_;

		public DocumentiNote(DocumentiEdit par)
		{
			_parent = par;
			change_event = true;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			InitializeComponent();
		}

		public void SetProtection()
		{
			m_note.IsEnabled  = _parent.doc.fat_editable;
			m_salva.IsEnabled = _parent.doc.fat_editable;
			m_salva.IsVisible = _parent.doc.fat_editable;
		}

		public void SetField()
		{
			change_event = false;
			m_note.Text = _parent.doc.fat_new_desc_varie != null ? _parent.doc.fat_new_desc_varie : "";
			if (_parent.doc.fat_new_desc_varie != null)
				m_note_title.Text = $"Note Varie ({_parent.doc.fat_new_desc_varie.Length}/512)";
			else
				m_note_title.Text = "Note Varie";
			change_event = true;
		}

		public void GetField()
		{
			if (m_note.Text.Trim().Length > 512)
				_parent.doc.fat_new_desc_varie = m_note.Text.Trim().Substring(0, 511);
			else
				_parent.doc.fat_new_desc_varie = m_note.Text.Trim();
		}

		void OnTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			if (!change_event) return;

			if (m_note.Text.Length > 512) m_note.Text = m_note.Text.Substring(0, 511);
			m_note_title.Text = $"Note Varie ({m_note.Text.Length}/512)";
		}

		async void OnSalvaClicked(object sender, System.EventArgs e)
		{
			if (!_parent.doc.fat_editable) return;
			busyIndicator.IsBusy = true;
			GetField();
			await dbcon_.UpdateAsync(_parent.doc);
			busyIndicator.IsBusy = false;
		}
	}
}
