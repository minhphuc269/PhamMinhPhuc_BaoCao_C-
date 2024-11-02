using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Home
{
    public partial class Login : Form
    {
        // Định nghĩa thông tin đăng nhập
        private const string validUsername = "minhphuc"; // Tên đăng nhập hợp lệ
        private const string validPassword = "123456"; // Mật khẩu hợp lệ

        public Login()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*'; // Ẩn mật khẩu
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text; // Lấy tên đăng nhập từ TextBox
            string password = textBox2.Text; // Lấy mật khẩu từ TextBox

            // Kiểm tra thông tin đăng nhập
            if (username == validUsername && password == validPassword)
            {
                // Nếu thông tin hợp lệ, mở Form1
                Form1 frmForm1 = new Form1();
                frmForm1.Show();
                this.Hide(); // Ẩn form đăng nhập
            }
            else
            {
                // Nếu thông tin không hợp lệ, hiển thị thông báo lỗi
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Bạn có thể thêm mã xử lý sự kiện nếu cần
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Bạn có thể thêm mã xử lý sự kiện nếu cần
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
