using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GaitAnalysis.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation;
        private ImageSource foto;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _title;
        private bool _isBusy;
        private bool _isLoading;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void SetValue<T>(ref T backingFieled, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingFieled, value))
            {
                return;
            }
            backingFieled = value;
            OnPropertyChanged(propertyName);
        }
        public void OnpropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        public ImageSource Foto
        {
            get { return foto; }
            set
            {
                foto = value;
                OnpropertyChanged();
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
            }
        }
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                SetProperty(ref _isBusy, value);//método auxiliar que encapsula la lógica de establecer el
                                                //valor de una propiedad y notificar el cambio si el valor realmente ha cambiado
            }
        }      
        public bool IsLoading
        {
            get { return _isLoading;}
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));//método que notifica a los enlaces de datos que el
                                                     //valor de una propiedad ha cambiado
            }
        }

        public async Task DisplayAlert(string title, string message, string cancel)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }   
    }
}
