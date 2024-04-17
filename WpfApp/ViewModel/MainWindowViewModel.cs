using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Model;
using WpfApp.Common;
using WpfApp.Validator;

namespace WpfApp.ViewModel
{
    class MainWindowViewModel : ObservableObject
    {
        private LoginModel loginModel = new LoginModel();

        public MainWindowViewModel() 
        {
            loginModel.UserName = string.Empty;
        }

        [NotEmptyCheck]
        [UserNameExists]
        public string UserName
        { 
            get { return loginModel.UserName; }
            set { loginModel.UserName = value;  this.RaisePropertyChanged("UserName"); }
        }

        [NotEmptyCheck]
        public string Password
        {
            get { return loginModel.Password; }
            set { loginModel.Password = value; this.RaisePropertyChanged("Password"); }
        }

        public override bool IsValid
        {
            get
            {
                return loginModel.IsValid;
            }
            set
            {
                if (value == loginModel.IsValid)
                {
                    return;
                }
                loginModel.IsValid = value;
                this.RaisePropertyChanged("IsValid");
            }
        }

        private RelayCommand<object> openNewWindowCommand = null;
        public RelayCommand<object> OpenNewWindowCommand
        {
            get
            {
                if (openNewWindowCommand == null)
                {
                    openNewWindowCommand = new RelayCommand<object>((o) =>
                    {
                        WindowManager.Show("NewWindow", new NewWindowViewModel());
                        ToClose = true;
                    });
                }
                return openNewWindowCommand;
            }
        }

        private bool toClose = false;
        /// <summary>
        /// 是否要关闭窗口
        /// </summary>
        public bool ToClose
        {
            get
            {
                return toClose;
            }
            set
            {
                toClose = value;
                if (toClose)
                {
                    this.RaisePropertyChanged("ToClose");
                }
            }
        }
    }
}
