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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhoneBook_Kylosov.Pages.PagesUser
{
    /// <summary>
    /// Логика взаимодействия для User_win.xaml
    /// </summary>
    public partial class User_win : Page
    {
        ClassModule.User user_loc;

        public User_win(User _user)
        {
            InitializeComponent();

            user_loc = _user;
            if (_user.fio_user != null)
            {
                fio_user.Text = _user.fio_user;
                phone_user.Text = _user.phone_num;
                addrec_user.Text = _user.passport_data;
            }
        }

        bool SaveUser(int id = -1)
        {
            string query = id == -1 ?
                $"UPDATE [users] SET [phone_num] = '{phone_user.Text}', [FIO_user] = '{fio_user.Text}', [pasport_data] = '{addrec_user.Text}' WHERE [Код] = {user_loc.id}" :
                $"INSERT INTO [users]([Код], [phone_num], [FIO_user], [pasport_data]) VALUES ({id.ToString()}, '{phone_user.Text}', '{fio_user.Text}', '{addrec_user.Text}')";

            var pc = MainWindow.connect.QueryAccess(query);
            MessageBox.Show(ClassConnection.Connection.err);
            if (pc == null) return false;

            MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);
            return true;
        }

        private void Click_User_Redact(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.connect.ItsOnlyFIO(fio_user.Text))
            {
                MessageBox.Show("Вы не правильно написали ФИО");
                return;
            }
            if (!MainWindow.connect.ItsNumber(phone_user.Text))
            {
                MessageBox.Show("Вы не правильно написали номер телефона");
                return;
            }
            if (addrec_user.Text.Trim() == "")
            {
                MessageBox.Show("Вы не правильно написали номер паспорта");
                return;
            }

            bool user = false;

            if (user_loc.fio_user == null)
            {
                user = SaveUser(MainWindow.connect.SetLastId(ClassConnection.Connection.tabels.users));
                MessageBox.Show($"Запрос на добавление клиента {(user ? "" : "не")} был обработан", 
                                user ? "Успешно" : "Ошибка",
                                MessageBoxButton.OK,
                                user ? MessageBoxImage.Information : MessageBoxImage.Warning);
            }
            else
            {
                user = SaveUser();
                MessageBox.Show($"Запрос на изменение клиента {(user ? "" : "не")} был обработан",
                                user ? "Успешно" : "Ошибка",
                                MessageBoxButton.OK,
                                user ? MessageBoxImage.Information : MessageBoxImage.Warning);
            }

            if(user) 
                MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page_main.users);
        }

        private void Click_Cancel_User_Redact(object sender, RoutedEventArgs e)
        {
            MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main);
        }

        private void Click_Remove_User_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);

                Call userFind = MainWindow.connect.calls.Find(x => x.user_id == user_loc.id);
                if (userFind != null)
                {
                    var click = MessageBox.Show("У данного клиента есть звонки, все равно удалить его?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (click == MessageBoxResult.No) return;
                }

                var pc = MainWindow.connect.QueryAccess($"DELETE FROM [calls] WHERE [user_id] = '{user_loc.id.ToString()}'");

                var pc1 = MainWindow.connect.QueryAccess($"DELETE FROM [users] WHERE [Код] = " + user_loc.id.ToString());

                MessageBox.Show($"Запрос на удаление клиента {(pc != null && pc1 != null ? "" : "не")} был обработан",
                                pc != null && pc1 != null ? "Успешно" : "Ошибка",
                                MessageBoxButton.OK,
                                pc != null && pc1 != null ? MessageBoxImage.Information : MessageBoxImage.Warning);

                if (pc != null && pc1 != null)
                {
                    MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);
                    MainWindow.main.Anim_move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page_main.users);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
