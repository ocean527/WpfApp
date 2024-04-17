using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp.Common;

namespace WpfApp.ViewModel
{
    class NewWindowViewModel : ObservableObject
    {
        public NewWindowViewModel() 
        { 
        
        }

        private UIElement pageContent;
        public UIElement PageContent
        {
            get { return pageContent; }
            set { pageContent = value; this.RaisePropertyChanged("PageContent"); }
        }
        public Dictionary<string, UIElement> PageDict { get; set; } = new Dictionary<string, UIElement>();

        private RelayCommand<object> openPageCommand = null;
        public RelayCommand<object> OpenPageCommand
        {
            get
            {
                if (openPageCommand == null)
                {
                    openPageCommand = new RelayCommand<object>((o) =>
                    {
                        //反射创建
                        Type type = Assembly.GetExecutingAssembly().GetType("WpfApp.Pages." + o.ToString());
                        //避免重复创建UIElement实例
                        if (!PageDict.ContainsKey(o.ToString()))
                        {
                            PageDict.Add(o.ToString(), (UIElement)Activator.CreateInstance(type));
                        }
                        //如果页面是Page
                        PageContent = new Frame() { Content = PageDict[o.ToString()] };
                        //如果页面是UserControl
                        //PageContent = PageDict[o.ToString()];
                    });
                }
                return openPageCommand;
            }
        }
    }
}
