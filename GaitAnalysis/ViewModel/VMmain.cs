using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GaitAnalysis.ViewModel
{
    internal class VMmain:BaseViewModel
    {
        #region Variables
        public string _texto;
        #endregion
        #region Constructor
        public VMmain(INavigation navigation)
        {
            Navigation = navigation;
        }
        #endregion
        #region Objetos
        public string Texto
        {
            get { return _texto; }
            set
            {   SetValue(ref _texto, value);
                OnPropertyChanged();
            }
        }
        #endregion
        #region Procesos
        public async Task ProcessAsync()
        {
        }
        public void ProcessSimple() 
        { 
        
        }
        #endregion
        #region Comandos
        public ICommand ProcessAsyncComand => new Command(async () => await ProcessAsync());
        public ICommand ProcessSimpleComand => new Command(ProcessSimple);
        #endregion
    }
}
