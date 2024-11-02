using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Home
{
    public partial class Quanlinhanvien : Form
    {
        private List<NhanVien> danhSachNhanVien = new List<NhanVien>();
        private string connectionString = "Server=MINHPHUC;Database=QLNhanVien;User Id=sa;Password=sa;";

        public Quanlinhanvien()
        {
            InitializeComponent();

            LoadData();
            // Connect checkbox event handler
            this.checkBoxLuong.CheckedChanged += new System.EventHandler(this.checkBoxLuong_CheckedChanged);
        }



        private void LoadData()
        {
            danhSachNhanVien.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM NhanVien", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        NhanVien nv = new NhanVien
                        {
                            MaNV = reader["MaNV"].ToString(),
                            TenNV = reader["TenNV"].ToString(),
                            Tuoi = reader["Tuoi"] != DBNull.Value ? Convert.ToInt32(reader["Tuoi"]) : 0,
                            GioiTinh = reader["GioiTinh"].ToString(),
                            NgayBatDau = reader["NgayBatDau"] != DBNull.Value ? Convert.ToDateTime(reader["NgayBatDau"]) : DateTime.MinValue,
                            ChucVu = reader["ChucVu"].ToString(),
                            Luong = reader["Luong"] != DBNull.Value ? Convert.ToDecimal(reader["Luong"]) : 0
                        };
                        danhSachNhanVien.Add(nv);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nạp dữ liệu: " + ex.Message);
            }
            CapNhatDanhSach();
        }

        private void CapNhatDanhSach()
        {
            dataGridView1.Rows.Clear();
            foreach (var nv in danhSachNhanVien)
            {
                dataGridView1.Rows.Add(nv.MaNV, nv.TenNV, nv.Tuoi, nv.GioiTinh, nv.NgayBatDau.ToShortDateString(), nv.ChucVu, nv.Luong);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (ValidateInput(out int tuoi, out string gioiTinh))
            {
                // Kiểm tra xem mã nhân viên đã tồn tại chưa
                if (IsMaNVExist(txtMa.Text))
                {
                    MessageBox.Show("Nhân viên với mã " + txtMa.Text + " đã tồn tại!");
                    return; // Ngừng thực hiện thêm mới
                }

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("INSERT INTO NhanVien (MaNV, TenNV, Tuoi, GioiTinh, NgayBatDau, ChucVu, Luong) VALUES (@MaNV, @TenNV, @Tuoi, @GioiTinh, @NgayBatDau, @ChucVu, @Luong)", connection);
                        command.Parameters.AddWithValue("@MaNV", txtMa.Text);
                        command.Parameters.AddWithValue("@TenNV", txtTen.Text);
                        command.Parameters.AddWithValue("@Tuoi", tuoi);
                        command.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                        command.Parameters.AddWithValue("@NgayBatDau", dateTimePickerNgayBatDau.Value);
                        command.Parameters.AddWithValue("@ChucVu", comboBoxChucVu.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@Luong", decimal.TryParse(txtLuong.Text, out decimal luong) ? luong : (object)DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                    ResetInputFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm nhân viên: " + ex.Message);
                }
                finally
                {
                    LoadData();
                }
            }
        }

        private bool IsMaNVExist(string maNV)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM NhanVien WHERE MaNV = @MaNV", connection);
                command.Parameters.AddWithValue("@MaNV", maNV);
                int count = (int)command.ExecuteScalar();
                return count > 0; // Trả về true nếu mã nhân viên đã tồn tại
            }
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        string maNV = row.Cells[0].Value?.ToString();
                        if (!string.IsNullOrEmpty(maNV))
                        {
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                SqlCommand command = new SqlCommand("DELETE FROM NhanVien WHERE MaNV = @MaNV", connection);
                                command.Parameters.AddWithValue("@MaNV", maNV);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa nhân viên: " + ex.Message);
                }
                finally
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhân viên để xóa!");
            }
        }


        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];
                string maNV = row.Cells[0].Value?.ToString();

                if (string.IsNullOrEmpty(maNV))
                {
                    MessageBox.Show("Không có mã nhân viên để sửa!");
                    return;
                }

                string gioiTinh = radioButtonNam.Checked ? "Nam" : "Nữ";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("UPDATE NhanVien SET TenNV = @TenNV, Tuoi = @Tuoi, GioiTinh = @GioiTinh, NgayBatDau = @NgayBatDau, ChucVu = @ChucVu, Luong = @Luong WHERE MaNV = @MaNV", connection);
                        command.Parameters.AddWithValue("@MaNV", maNV);
                        command.Parameters.AddWithValue("@TenNV", txtTen.Text);
                        command.Parameters.AddWithValue("@Tuoi", int.TryParse(txtTuoi.Text, out int tuoi) ? tuoi : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                        command.Parameters.AddWithValue("@NgayBatDau", dateTimePickerNgayBatDau.Value);
                        command.Parameters.AddWithValue("@ChucVu", comboBoxChucVu.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@Luong", decimal.TryParse(txtLuong.Text, out decimal luong) ? luong : (object)DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                    ResetInputFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sửa nhân viên: " + ex.Message);
                }
                finally
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhân viên để sửa!");
            }
        }


        private bool ValidateInput(out int tuoi, out string gioiTinh)
        {
            if (string.IsNullOrWhiteSpace(txtMa.Text) || string.IsNullOrWhiteSpace(txtTen.Text) || string.IsNullOrWhiteSpace(txtTuoi.Text) || comboBoxChucVu.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                tuoi = 0;
                gioiTinh = string.Empty;
                return false;
            }

            if (!int.TryParse(txtTuoi.Text, out tuoi))
            {
                MessageBox.Show("Tuổi không hợp lệ!");
                gioiTinh = string.Empty;
                return false;
            }

            gioiTinh = radioButtonNam.Checked ? "Nam" : "Nữ";
            return true;
        }

        private void ResetInputFields()
        {
            txtMa.Clear();
            txtTen.Clear();
            txtTuoi.Clear();
            radioButtonNam.Checked = true;
            radioButtonNu.Checked = false;
            comboBoxChucVu.SelectedIndex = -1;
            txtLuong.Clear();
            checkBoxLuong.Checked = false;
            dateTimePickerNgayBatDau.Value = DateTime.Now;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];
                txtMa.Text = row.Cells[0].Value.ToString();
                txtTen.Text = row.Cells[1].Value.ToString();
                txtTuoi.Text = row.Cells[2].Value.ToString();
                radioButtonNam.Checked = row.Cells[3].Value.ToString() == "Nam";
                radioButtonNu.Checked = row.Cells[3].Value.ToString() == "Nữ";
                dateTimePickerNgayBatDau.Value = Convert.ToDateTime(row.Cells[4].Value);
                comboBoxChucVu.SelectedItem = row.Cells[5].Value.ToString();
                txtLuong.Text = row.Cells[6].Value.ToString();
            }
            else
            {
                ResetInputFields();
            }
        }

        private void checkBoxLuong_CheckedChanged(object sender, EventArgs e)
        {
            txtLuong.Enabled = checkBoxLuong.Checked; // Enable txtLuong if checkbox is checked
            if (!checkBoxLuong.Checked)
            {
                txtLuong.Clear(); // Clear txtLuong if checkbox is unchecked
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txtTuoi_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class NhanVien
    {
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public int Tuoi { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgayBatDau { get; set; }
        public string ChucVu { get; set; }
        public decimal Luong { get; set; }
    }
}
