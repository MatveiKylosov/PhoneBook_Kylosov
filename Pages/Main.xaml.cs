using ClassModule;
using PhoneBook_Kylosov.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace PhoneBook_Kylosov.Pages
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        public enum page_main
        {
            users, calls, filter ,none
        }

        public static page_main page_select;

        public Main()
        {
            InitializeComponent();

            page_select = page_main.none;
        }

        private void Click_Phone(object sender, RoutedEventArgs e)
        {
            Search.Visibility = Visibility.Hidden;
            scroll_main.Margin = new Thickness(0, 0, 0, 0);
            if (frame_main.Visibility == Visibility.Visible) 
                MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main);

            if (page_select != page_main.users)
            {
                page_select = page_main.users;
                DoubleAnimation opgridAnimation = new DoubleAnimation();
                opgridAnimation.From = 1;
                opgridAnimation.To = 0;
                opgridAnimation.Duration = TimeSpan.FromSeconds(0.2);
                opgridAnimation.Completed += delegate
                {
                    parrent.Children.Clear();
                    DoubleAnimation opgriAnimation = new DoubleAnimation();
                    opgriAnimation.From = 0;
                    opgriAnimation.To = 1;
                    opgriAnimation.Duration = TimeSpan.FromSeconds(0.2);
                    opgriAnimation.Completed += delegate
                    {
                        Dispatcher.InvokeAsync(async () =>
                        {
                            MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);
                            foreach (ClassModule.User user_itm in MainWindow.connect.users)
                            {
                                if (page_select == page_main.users)
                                {
                                    parrent.Children.Add(new Elements.User_itm(user_itm));
                                    await Task.Delay(90);
                                }
                            }
                            if (page_select == page_main.users)
                            {
                                var ff = new Pages.PagesUser.User_win(new ClassModule.User());
                                parrent.Children.Add(new Elements.Add_itm(ff));
                            }
                        });
                    };
                    parrent.BeginAnimation(StackPanel.OpacityProperty, opgriAnimation);
                };
                parrent.BeginAnimation(StackPanel.OpacityProperty, opgridAnimation);
            }
        }

        private void Click_History(object sender, RoutedEventArgs e)
        {            //Margin="240,0,0,0
            Search.Visibility= Visibility.Visible;
            scroll_main.Margin = new Thickness(240, 0, 0, 0);

            if (frame_main.Visibility == Visibility.Visible) 
                MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main);

            if (page_select != page_main.calls)
            {
                page_select = page_main.calls;

                DoubleAnimation opgridAnimation = new DoubleAnimation();
                opgridAnimation.From = 1;
                opgridAnimation.To = 0;
                opgridAnimation.Duration = TimeSpan.FromSeconds(0.2);
                opgridAnimation.Completed += delegate
                {
                    parrent.Children.Clear();
                    DoubleAnimation opgriAnimation = new DoubleAnimation();
                    opgriAnimation.From = 0;
                    opgriAnimation.To = 1;
                    opgriAnimation.Duration = TimeSpan.FromSeconds(0.2);
                    opgriAnimation.Completed += delegate
                    {
                        Dispatcher.InvokeAsync(async () =>
                        {
                            MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);
                            foreach (Call call_itm in MainWindow.connect.calls)
                            {
                                if (page_select == page_main.calls)
                                {
                                    parrent.Children.Add(new Elements.Call_itm(call_itm));
                                    await Task.Delay(90);
                                }
                            }
                            if (page_select == page_main.calls)
                            {
                                var ff = new Pages.PagesUser.Call_win(new Call());
                                parrent.Children.Add(new Elements.Add_itm(ff));
                            }
                        });
                    };
                    parrent.BeginAnimation(StackPanel.OpacityProperty, opgriAnimation);
                };
                parrent.BeginAnimation(StackPanel.OpacityProperty, opgridAnimation);
            }
        }

        public void Anim_move(Control control1, Control control2, Frame frame_main = null, Page pages = null, page_main page_restart = page_main.none)
        {
            if (page_restart != page_main.none)
            {
                if (page_restart == page_main.users)
                {
                    page_select = page_main.none;
                    Click_Phone(new object(), new RoutedEventArgs());
                }
                else if (page_restart == page_main.calls)
                {
                    page_select = page_main.none;
                    Click_History(new object(), new RoutedEventArgs());
                }
            }
            else
            {
                DoubleAnimation opgridAnimation = new DoubleAnimation();
                opgridAnimation.From = 1;
                opgridAnimation.To = 0;
                opgridAnimation.Duration = TimeSpan.FromSeconds(0.3);
                opgridAnimation.Completed += delegate
                {
                    if (pages != null)
                    {
                        frame_main.Navigate(pages);
                    }
                    control1.Visibility = Visibility.Hidden;
                    control2.Visibility = Visibility.Visible;
                    DoubleAnimation opgriAnimation = new DoubleAnimation();
                    opgriAnimation.From = 0;
                    opgriAnimation.To = 1;
                    opgriAnimation.Duration = TimeSpan.FromSeconds(0.4);
                    control2.BeginAnimation(ScrollViewer.OpacityProperty, opgriAnimation);
                };
                control1.BeginAnimation(ScrollViewer.OpacityProperty, opgridAnimation);
            }
        }
        
        private void Click_Search(object sender, MouseButtonEventArgs e)
        {
            page_select = page_main.filter;
            bool dateF = false;
            bool type = false;
            bool numberb = false;
            DateTime dateStart, dateFinish;

            if ((period_start.SelectedDate != null & period_end.SelectedDate == null) ||
               (period_start.SelectedDate == null & period_end.SelectedDate != null))
            {
                MessageBox.Show("укажите дату корректно");
                return;
            }
            
            //фильтр на даты
            if(period_start.SelectedDate != null & period_end.SelectedDate != null)
            {

                dateStart = (System.DateTime)period_start.SelectedDate;
                dateFinish = (System.DateTime)period_end.SelectedDate;
                TimeSpan dateEnd = dateFinish.Subtract(dateStart);

                if (dateEnd.ToString().Contains("-"))
                {
                    MessageBox.Show("Дата старта больше даты конца");
                    return;
                }
                dateF = true;
            }

            //фильтр на категорию звонка
            if(((TextBlock)call_category_text.SelectedItem).Text != "Неважно")
                type = true;

            //фильтр на номер телефона
            if (MainWindow.connect.ItsNumber(number.Text))
                numberb = true;


            if (!(dateF || type || numberb)) return;

            List<UIElement> temp = new List<UIElement>();

            foreach (UIElement child in parrent.Children)
            {
                if (child is Call_itm x)
                {
                    bool delete = false;

                    if (dateF)
                    {
                        DateTime ConvertToDateTime(string dateTimeStr)
                        {
                            string[] dateLoc = dateTimeStr.Split(' ');
                            string[] date = dateLoc[0].Split('.');
                            return new DateTime(int.Parse(date[2]), int.Parse(date[1]),
                                                int.Parse(date[0]), int.Parse(dateLoc[1].Split(':')[0]),
                                                int.Parse(dateLoc[1].Split(':')[1]), 0);
                        }

                        DateTime xStart = ConvertToDateTime(x.call_loc.time_start.ToString());
                        DateTime xEnd = ConvertToDateTime(x.call_loc.time_end.ToString());

                        DateTime fStart = ConvertToDateTime(period_start.ToString());
                        DateTime fEnd = ConvertToDateTime(period_end.ToString());

                        delete = fStart <= xStart && xEnd <= fEnd ? delete : true;
                    }

                    if (type)
                        delete = x.call_loc.category_call != (((TextBlock)call_category_text.SelectedItem).Text == "Входящий" ? 2 : 1) ? true : delete;

                    if (numberb)
                        delete = MainWindow.connect.users.Find(y => y.id == x.call_loc.user_id).phone_num != number.Text ? true : delete;

                    if (!delete) temp.Add(child);
                }
            }

            parrent.Children.Clear();

            foreach(UIElement child in temp)
                parrent.Children.Add(child);
        }

        private void Click_Reset(object sender, MouseButtonEventArgs e)
        {
            period_start.SelectedDate = period_end.SelectedDate =  null;
            number.Text = "";

            Click_History(null, null);
        }
    }
}
