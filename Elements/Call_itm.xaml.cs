using ClassModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace PhoneBook_Kylosov.Elements
{
    /// <summary>
    /// Логика взаимодействия для Call_itm.xaml
    /// </summary>
    public partial class Call_itm : Page
    {
        Call call_loc;
        public Call_itm(Call _call)
        {
            InitializeComponent();
            call_loc = _call;
            if (_call.time_end != null)
            {
                User user_loc = MainWindow.connect.users.Find(x => x.id == _call.user_id);
                category_call_text.Content = user_loc.fio_user.ToString();

                string[] dateLoc1 = _call.time_start.ToString().Split(' ');
                string[] dateLoc2 = _call.time_end.ToString().Split(' ');

                string[] date1 = dateLoc1[0].Split('.');
                string[] date2 = dateLoc2[0].Split('.');

                DateTime dateStart = new DateTime(int.Parse(date1[2]), int.Parse(date1[1]), 
                                                  int.Parse(date1[0]), int.Parse(dateLoc1[1].Split(':')[0]), 
                                                   int.Parse(dateLoc1[1].Split(':')[1]), 0);

                DateTime dateFinish = new DateTime(int.Parse(date2[2]), int.Parse(date2[1]), 
                                                   int.Parse(date2[0]), int.Parse(dateLoc2[1].Split(':')[0]), 
                                                   int.Parse(dateLoc2[1].Split(':')[1]), 0);

                TimeSpan dateEnd = dateFinish.Subtract(dateStart);

                time_call_text.Content = "Продолжительность звонка: " + dateEnd.ToString();
                number_call_text.Content = "Номер телефона: " + user_loc.phone_num.ToString();

            }
            img_category_call.Source = (_call.category_call == 1) ? 
                    new BitmapImage(new Uri(@"/img/out.png", UriKind.RelativeOrAbsolute)) : new BitmapImage(new Uri(@"/img/in.png", UriKind.RelativeOrAbsolute));
            DoubleAnimation opgridAnimation = new DoubleAnimation();
            opgridAnimation.From = 0;
            opgridAnimation.To = 1;
            opgridAnimation.Duration = TimeSpan.FromSeconds(0.4);
            border.BeginAnimation(StackPanel.OpacityProperty, opgridAnimation);
        }

        private void Click_redact(object sender, RoutedEventArgs e)
        {
            MainWindow.main.Anim_move(MainWindow.main.scroll_main, MainWindow.main.frame_main, MainWindow.main.frame_main, new Pages.PagesUser.Call_win(call_loc));
        }

        private void Click_remove(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);
                string vs = $"DELETE FROM [calls] WHERE [Код] = " + call_loc.id.ToString() + "";
                var pc = MainWindow.connect.QueryAccess(vs);
                if (pc != null)
                {
                    MessageBox.Show("Успешное удаление звонка", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);
                    MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Pages.Main.page_main.calls);
                }
                else MessageBox.Show("Запрос на удаление звонка не был обработан", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
