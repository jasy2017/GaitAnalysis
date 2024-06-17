using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GaitAnalysis.ViewModel
{
    public class VMpageInfo : BaseViewModel
    {
        #region Variables
        public string _texto;
        #endregion
        #region Constructor
        public VMpageInfo(INavigation navigation)
        {
            Navigation = navigation;
        }
        #endregion
        #region Objetos
        public string Texto
        {
            get { return _texto; }
            set
            {
                SetValue(ref _texto, value);
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

