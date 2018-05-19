using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Facile.ViewModels
{
	public class IncassiInfo : INotifyPropertyChanged, IEditableObject
	{
		private int _sca_id;
		private DateTime _sca_data;
		private double _sca_importo;
		private double _sca_incasso;
		private string _sca_fattura;
		private double _sca_tot_fat;
		private string _sca_desc;
		private int _sca_locked;

		public IncassiInfo()
		{
			sca_id = 0;

			sca_data = DateTime.Now;
			sca_importo = 0.0;
			sca_incasso = 0.0;
			sca_fattura = "";
			sca_tot_fat = 0.0;
			sca_desc = "";
			sca_locked = 0;
		}

		public int sca_id 
		{
			get { return _sca_id; }
			set
			{
				this._sca_id = value;
				RaisePropertyChanged("sca_id");
			}	
		}

		public DateTime sca_data
		{
			get { return _sca_data; }
			set
			{
				this._sca_data = value;
				RaisePropertyChanged("sca_data");
			}
		}

		public double sca_importo
		{
			get { return _sca_importo; }
			set
			{
				this._sca_importo = value;
				RaisePropertyChanged("sca_importo");
			}
		}

		public double sca_incasso
		{
			get { return _sca_incasso; }
			set
			{
				this._sca_incasso = value;
				RaisePropertyChanged("sca_incasso");
			}
		}

		public string sca_fattura
		{
			get { return _sca_fattura; }
			set
			{
				this._sca_fattura = value;
				RaisePropertyChanged("sca_fattura");
			}
		}

		public double sca_tot_fat
		{
			get { return _sca_tot_fat; }
			set
			{
				this._sca_tot_fat = value;
				RaisePropertyChanged("sca_tot_fat");
			}
		}

		public string sca_desc
		{
			get { return _sca_desc; }
			set
			{
				this._sca_desc = value;
				RaisePropertyChanged("sca_desc");
			}
		}

		public int sca_locked
		{
			get { return _sca_locked; }
			set
			{
				this._sca_locked = value;
				RaisePropertyChanged("sca_locked");
			}
		}


    	public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(String Name)
		{
			if (PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(Name));
		}

		private Dictionary<string, object> storedValues;


		public void BeginEdit()
		{
			this.storedValues = this.BackUp();
		}

		public void CancelEdit()
		{
			if (this.storedValues == null)
				return;

			foreach (var item in this.storedValues)
			{
				var itemProperties = this.GetType().GetTypeInfo().DeclaredProperties;
				var pDesc = itemProperties.FirstOrDefault(p => p.Name == item.Key);
				if (pDesc != null)
					pDesc.SetValue(this, item.Value);
			}
		}

		public void EndEdit()
		{
			if (this.storedValues != null)
			{
				this.storedValues.Clear();
				this.storedValues = null;
			}
			Debug.WriteLine("End Edit Called");
		}

		protected Dictionary<string, object> BackUp()
		{
			var dictionary = new Dictionary<string, object>();
			var itemProperties = this.GetType().GetTypeInfo().DeclaredProperties;
			foreach (var pDescriptor in itemProperties)
			{
				if (pDescriptor.CanWrite)
					dictionary.Add(pDescriptor.Name, pDescriptor.GetValue(this));
			}
			return dictionary;
		}
	}
}
